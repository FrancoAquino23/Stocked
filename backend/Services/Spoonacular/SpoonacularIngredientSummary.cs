using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Spoonacular;

// Ingrediente de Spoonacular
public class SpoonacularIngredientSummary
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Nombre del archivo de imagen (No URL)
    [JsonPropertyName("image")]
    public string? Image { get; set; }
}
