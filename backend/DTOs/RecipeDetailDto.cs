namespace Stocked.Api.DTOs;

// DTO: Detalle completo de una receta
public class RecipeDetailDto
{
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string[] DishTypes { get; set; } = [];
    public string[] Diets { get; set; } = [];
    public string[] Cuisines { get; set; } = [];
    public bool IsFavorite { get; set; }
    public List<RecipeStepDto> Steps { get; set; } = [];
    public List<RecipeIngredientDto> Ingredients { get; set; } = [];
}
