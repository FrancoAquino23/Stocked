using System.Net.Http.Json;

namespace Stocked.Api.Services.Spoonacular;

// Cliente HTTP de la API externa de Spoonacular
public class SpoonacularClient
{
    private readonly HttpClient _httpClient;

    public SpoonacularClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Buscar recetas por texto libre
    public Task<SpoonacularSearchResponse> SearchRecipesAsync(string query, CancellationToken cancellationToken = default)
    {
        var path = $"/recipes/complexSearch?query={Uri.EscapeDataString(query)}";
        return GetAsync<SpoonacularSearchResponse>(path, cancellationToken);
    }

    // Obtener el detalle completo de una receta (Ingredientes y pasos incluidos)
    public Task<SpoonacularRecipeDetail> GetRecipeInformationAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var path = $"/recipes/{externalId}/information";
        return GetAsync<SpoonacularRecipeDetail>(path, cancellationToken);
    }

    // Autocompletar ingredientes por texto libre
    public Task<List<SpoonacularIngredientSummary>> SearchIngredientsAsync(string query, CancellationToken cancellationToken = default)
    {
        var path = $"/food/ingredients/autocomplete?query={Uri.EscapeDataString(query)}&number=10&metaInformation=true";
        return GetAsync<List<SpoonacularIngredientSummary>>(path, cancellationToken);
    }

    // Obtener el detalle de un ingrediente (Nombre e imagen)
    public Task<SpoonacularIngredientSummary> GetIngredientInformationAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var path = $"/food/ingredients/{externalId}/information?amount=1";
        return GetAsync<SpoonacularIngredientSummary>(path, cancellationToken);
    }

    // Armar la URL completa de la imagen de un ingrediente
    public static string BuildIngredientImageUrl(string? imageFileName)
    {
        return string.IsNullOrEmpty(imageFileName)
            ? string.Empty
            : $"https://img.spoonacular.com/ingredients_100x100/{imageFileName}";
    }

    // Hacer la petición GET
    private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken) where T : new()
    {
        var response = await _httpClient.GetAsync(path, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Spoonacular respondió {(int)response.StatusCode} en '{path}': {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        return result ?? new T();
    }
}
