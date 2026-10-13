namespace Stocked.Api.Models;

// Modelo: Plan de comida
public class MealPlanEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateOnly Date { get; set; }
    public MealSlot MealSlot { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
