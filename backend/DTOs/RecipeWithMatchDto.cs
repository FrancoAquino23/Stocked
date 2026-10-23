namespace Stocked.Api.DTOs;

// DTO: Receta con indicador de si se puede cocinar con la despensa actual
public class RecipeWithMatchDto
{
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool CanMake { get; set; }
    public int MissingIngredientsCount { get; set; }
    public int? ReadyInMinutes { get; set; }
    public string[] DishTypes { get; set; } = [];
    public string[] Diets { get; set; } = [];
    public string[] Cuisines { get; set; } = [];
}
