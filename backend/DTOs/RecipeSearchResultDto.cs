namespace Stocked.Api.DTOs;

// DTO: Resultado de búsqueda de receta
public class RecipeSearchResultDto
{
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int? ReadyInMinutes { get; set; }
    public string[] DishTypes { get; set; } = [];
    public string[] Cuisines { get; set; } = [];
}
