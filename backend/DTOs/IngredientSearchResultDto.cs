namespace Stocked.Api.DTOs;

// DTO: Resultado de autocompletado de ingredientes
public class IngredientSearchResultDto
{
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
