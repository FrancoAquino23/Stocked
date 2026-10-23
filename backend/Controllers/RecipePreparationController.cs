using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Preparations;

namespace Stocked.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/recipe-preparations")]
public class RecipePreparationController : ControllerBase
{
    private readonly RecipePreparationService _recipePreparationService;

    public RecipePreparationController(RecipePreparationService recipePreparationService)
    {
        _recipePreparationService = recipePreparationService;
    }

    // Declarar que se va a preparar una receta X veces
    [HttpPost]
    public async Task<ActionResult<RecipePreparationDto>> Declare([FromBody] DeclareRecipePreparationRequestDto request)
    {
        var preparation = await _recipePreparationService.DeclareAsync(
            this.GetUserId(), request.ExternalId, request.Multiplier, request.MealPlanEntryId);

        return Ok(MapToDto(preparation));
    }

    // Marcar que ya se empezó a cocinar
    [HttpPut("{id:int}/start")]
    public async Task<ActionResult<RecipePreparationDto>> Start(int id)
    {
        var preparation = await _recipePreparationService.StartAsync(this.GetUserId(), id);
        if (preparation is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(preparation));
    }

    // Marcar que ya se terminó y descontar sus ingredientes del inventario de la semana
    [HttpPut("{id:int}/finish")]
    public async Task<ActionResult<RecipePreparationDto>> Finish(int id)
    {
        var preparation = await _recipePreparationService.FinishAsync(this.GetUserId(), id);
        if (preparation is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(preparation));
    }

    // Calcular para cuántas de las veces solicitadas alcanza la despensa actual
    [HttpGet("affordability")]
    public async Task<ActionResult<RecipeAffordabilityDto>> CheckAffordability(
        [FromQuery] int externalId, [FromQuery] int multiplier, [FromQuery] DateOnly weekStart)
    {
        var (affordable, fullyCovered, uncertain) = await _recipePreparationService
            .CheckAffordabilityAsync(this.GetUserId(), externalId, multiplier, weekStart);

        return Ok(new RecipeAffordabilityDto
        {
            RequestedMultiplier = multiplier,
            AffordableMultiplier = affordable,
            FullyCovered = fullyCovered,
            UncertainIngredients = uncertain
        });
    }

    private static RecipePreparationDto MapToDto(RecipePreparation preparation)
    {
        return new RecipePreparationDto
        {
            Id = preparation.Id,
            ExternalId = preparation.Recipe.ExternalId,
            RecipeName = preparation.Recipe.Name,
            Multiplier = preparation.Multiplier,
            Status = DescribeStatus(preparation)
        };
    }

    private static string DescribeStatus(RecipePreparation preparation)
    {
        if (preparation.FinishedAt is not null)
        {
            return "Done";
        }

        return preparation.StartedAt is not null ? "InProgress" : "Pending";
    }
}
