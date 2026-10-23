using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Spoonacular;

// Forma de un resultado de búsqueda
public class SpoonacularRecipeSummary
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("readyInMinutes")]
    public int? ReadyInMinutes { get; set; }

    [JsonPropertyName("dishTypes")]
    public List<string>? DishTypes { get; set; }

    [JsonPropertyName("cuisines")]
    public List<string>? Cuisines { get; set; }
}
