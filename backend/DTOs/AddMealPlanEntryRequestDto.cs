using Stocked.Api.Models;

namespace Stocked.Api.DTOs;

// DTO: Petición para agendar una receta en el calendario
public class AddMealPlanEntryRequestDto
{
    public int ExternalId { get; set; }
    public DateOnly Date { get; set; }
    public MealSlot MealSlot { get; set; }
}
