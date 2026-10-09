using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Calendar;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/meal-plan-entries")]
public class MealPlanController : ControllerBase
{
    private readonly MealPlanService _mealPlanService;

    public MealPlanController(MealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    // Listar el calendario completo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MealPlanEntryDto>>> GetAll()
    {
        var entries = await _mealPlanService.GetAllAsync();
        return Ok(entries.Select(MapToDto));
    }

    // Agendar una receta en el calendario
    [HttpPost]
    public async Task<ActionResult<MealPlanEntryDto>> Add([FromBody] AddMealPlanEntryRequestDto request)
    {
        var entry = await _mealPlanService.AddAsync(request.ExternalId, request.Date, request.MealSlot);
        return Ok(MapToDto(entry));
    }

    // Mover una entrada agendada a otro día
    [HttpPut("{id:int}")]
    public async Task<ActionResult<MealPlanEntryDto>> Update(int id, [FromBody] UpdateMealPlanEntryRequestDto request)
    {
        var entry = await _mealPlanService.UpdateAsync(id, request.Date, request.MealSlot);
        if (entry is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(entry));
    }

    // Quitar una entrada del calendario
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remove(int id)
    {
        await _mealPlanService.RemoveAsync(id);
        return NoContent();
    }

    private static MealPlanEntryDto MapToDto(MealPlanEntry entry)
    {
        return new MealPlanEntryDto
        {
            Id = entry.Id,
            ExternalId = entry.Recipe.ExternalId,
            RecipeName = entry.Recipe.Name,
            Date = entry.Date,
            MealSlot = entry.MealSlot
        };
    }
}
