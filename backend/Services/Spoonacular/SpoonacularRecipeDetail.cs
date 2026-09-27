using System.Text.Json.Serialization;

namespace Stocked.Api.Services.Spoonacular;

// Forma del detalle de una receta
public class SpoonacularRecipeDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("dishTypes")]
    public List<string> DishTypes { get; set; } = [];

    [JsonPropertyName("diets")]
    public List<string> Diets { get; set; } = [];

    [JsonPropertyName("cuisines")]
    public List<string> Cuisines { get; set; } = [];

    [JsonPropertyName("extendedIngredients")]
    public List<SpoonacularIngredientDetail> ExtendedIngredients { get; set; } = [];

    [JsonPropertyName("analyzedInstructions")]
    public List<SpoonacularInstructionGroup> AnalyzedInstructions { get; set; } = [];
}

// Ingrediente dentro del detalle de una receta
public class SpoonacularIngredientDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Nombre del archivo de imagen (no URL)
    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;
}

// Grupo de pasos de preparación
public class SpoonacularInstructionGroup
{
    [JsonPropertyName("steps")]
    public List<SpoonacularInstructionStep> Steps { get; set; } = [];
}

// Paso individual de preparación
public class SpoonacularInstructionStep
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("step")]
    public string Step { get; set; } = string.Empty;
}
