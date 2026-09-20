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

    // Etiquetas (Tipo de platillo)
    public string[] Tags { get; set; } = [];

    public DateTime CachedAt { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
