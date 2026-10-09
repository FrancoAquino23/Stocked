namespace Stocked.Api.Models;

// Modelo: Receta
public class Recipe
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // Procedimiento (Pasos numerados)
    public string Instructions { get; set; } = string.Empty;

    // Tipo de comida
    public string[] DishTypes { get; set; } = [];

    // Tipo de dieta
    public string[] Diets { get; set; } = [];

    // Tipo de cocina
    public string[] Cuisines { get; set; } = [];

    public DateTime CachedAt { get; set; }

    // Última vez que se consultó el detalle, alimenta la pestaña "Recientes"
    public DateTime? LastViewedAt { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
