using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Preparations;

namespace Stocked.Api.Controllers;

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
            request.ExternalId, request.Multiplier, request.MealPlanEntryId);

        return Ok(MapToDto(preparation));
    }

    // Marcar que ya se empezó a cocinar
    [HttpPut("{id:int}/start")]
    public async Task<ActionResult<RecipePreparationDto>> Start(int id)
    {
        var preparation = await _recipePreparationService.StartAsync(id);
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
        var preparation = await _recipePreparationService.FinishAsync(id);
        if (preparation is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(preparation));
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
            return "Terminada";
        }

        return preparation.StartedAt is not null ? "EnCurso" : "Pendiente";
    }
}
