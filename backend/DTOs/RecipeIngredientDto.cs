namespace Stocked.Api.DTOs;

// DTO: Ingrediente requerido por una receta
public class RecipeIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
