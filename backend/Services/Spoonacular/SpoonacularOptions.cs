namespace Stocked.Api.Services.Spoonacular;

// Configuración de acceso a la API de Spoonacular
public class SpoonacularOptions
{
    public const string SectionName = "Spoonacular";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.spoonacular.com";
}
