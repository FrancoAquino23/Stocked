using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Recipes;

// Forma en la que los pasos de preparación se guardan
public class StoredRecipeStep
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("step")]
    public string Step { get; set; } = string.Empty;
}
