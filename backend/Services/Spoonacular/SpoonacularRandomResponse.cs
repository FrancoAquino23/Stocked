using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Spoonacular;

// Forma de la respuesta de recetas al azar de Spoonacular
public class SpoonacularRandomResponse
{
    [JsonPropertyName("recipes")]
    public List<SpoonacularRecipeSummary> Recipes { get; set; } = [];
}
