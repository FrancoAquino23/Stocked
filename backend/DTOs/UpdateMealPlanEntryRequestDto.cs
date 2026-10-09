using Stocked.Api.Models;

namespace Stocked.Api.DTOs;

// DTO: Petición para mover una entrada del calendario a otro día
public class UpdateMealPlanEntryRequestDto
{
    public DateOnly Date { get; set; }
    public MealSlot MealSlot { get; set; }
}
