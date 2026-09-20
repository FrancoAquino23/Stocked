namespace Stocked.Api.Models;

// Modelo: Preparación de receta
public class RecipePreparation
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    // Receta agendada en el calendario
    public int? MealPlanEntryId { get; set; }
    public MealPlanEntry? MealPlanEntry { get; set; }

    // Cantidad de veces que se prepara una receta
    public int Multiplier { get; set; }

    public DateTime StartedAt { get; set; }

    // Estado: "Pendiente" / "En curso" / "Terminada"
    public DateTime? FinishedAt { get; set; }
}
