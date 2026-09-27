using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Spoonacular;

// Forma de la respuesta de búsqueda de recetas de Spoonacular
public class SpoonacularSearchResponse
{
    [JsonPropertyName("results")]
    public List<SpoonacularRecipeSummary> Results { get; set; } = [];

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }
}
