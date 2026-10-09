using Stocked.Api.Models;

namespace Stocked.Api.DTOs;

// DTO: Entrada del calendario semanal
public class MealPlanEntryDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public MealSlot MealSlot { get; set; }
}
