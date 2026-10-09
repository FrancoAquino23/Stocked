using System.Text.Json;
using Stocked.Api.Models;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Inventory;
using Stocked.Api.Services.Spoonacular;

namespace Stocked.Api.Services.Recipes;

public class RecipeService
{
    private const int MaxRecentRecipes = 20;

    private readonly SpoonacularClient _spoonacularClient;
    private readonly RecipeRepository _recipeRepository;
    private readonly IngredientRepository _ingredientRepository;
    private readonly FavoriteRecipeRepository _favoriteRecipeRepository;
    private readonly PantryRepository _pantryRepository;

    public RecipeService(
        SpoonacularClient spoonacularClient,
        RecipeRepository recipeRepository,
        IngredientRepository ingredientRepository,
        FavoriteRecipeRepository favoriteRecipeRepository,
        PantryRepository pantryRepository)
    {
        _spoonacularClient = spoonacularClient;
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _favoriteRecipeRepository = favoriteRecipeRepository;
        _pantryRepository = pantryRepository;
    }

    // Servir la receta ya guardada en memoria o guardarla por primera vez, marcándola como vista
    public async Task<Recipe> GetOrFetchRecipeAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var cachedRecipe = await _recipeRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (cachedRecipe is not null)
        {
            cachedRecipe.LastViewedAt = DateTime.UtcNow;
            await _recipeRepository.UpdateAsync(cachedRecipe, cancellationToken);
            return cachedRecipe;
        }

        var detail = await _spoonacularClient.GetRecipeInformationAsync(externalId, cancellationToken);
        var recipe = await MapToRecipeAsync(detail, cancellationToken);
        recipe.LastViewedAt = DateTime.UtcNow;

        return await _recipeRepository.AddAsync(recipe, cancellationToken);
    }

    // Conocer si una receta está marcada como favorita
    public async Task<bool> IsFavoriteAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRecipeRepository.GetByRecipeIdAsync(recipeId, cancellationToken);
        return favorite is not null;
    }

    // Marcar una receta como favorita
    public async Task AddFavoriteAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var recipe = await GetOrFetchRecipeAsync(externalId, cancellationToken);
        var existingFavorite = await _favoriteRecipeRepository.GetByRecipeIdAsync(recipe.Id, cancellationToken);
        if (existingFavorite is null)
        {
            await _favoriteRecipeRepository.AddAsync(recipe.Id, cancellationToken);
        }
    }

    // Quitar una receta de favoritas
    public async Task RemoveFavoriteAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (recipe is null)
        {
            return;
        }

        var favorite = await _favoriteRecipeRepository.GetByRecipeIdAsync(recipe.Id, cancellationToken);
        if (favorite is not null)
        {
            await _favoriteRecipeRepository.RemoveAsync(favorite, cancellationToken);
        }
    }

    // Listar las recetas marcadas como favoritas
    public Task<List<Recipe>> GetFavoriteRecipesAsync(CancellationToken cancellationToken = default)
    {
        return _favoriteRecipeRepository.GetAllRecipesAsync(cancellationToken);
    }

    // Listar las recetas vistas recientemente
    public Task<List<Recipe>> GetRecentRecipesAsync(CancellationToken cancellationToken = default)
    {
        return _recipeRepository.GetRecentAsync(MaxRecentRecipes, cancellationToken);
    }

    // Conocer para cada receta, si se puede cocinar con lo que hay en la despensa
    public async Task<Dictionary<int, RecipeMatch>> GetMatchesAsync(
        IEnumerable<Recipe> recipes, CancellationToken cancellationToken = default)
    {
        var pantryItems = await _pantryRepository.GetAllAsync(cancellationToken);
        var pantryIngredientIds = pantryItems.Select(pantryItem => pantryItem.IngredientId).ToHashSet();

        return recipes.ToDictionary(
            recipe => recipe.Id,
            recipe => RecipeMatchCalculator.Calculate(recipe, pantryIngredientIds));
    }

    // Armar la receta en memoria a partir del detalle de Spoonacular
    private async Task<Recipe> MapToRecipeAsync(SpoonacularRecipeDetail detail, CancellationToken cancellationToken)
    {
        var recipeIngredients = new List<RecipeIngredient>();
        foreach (var ingredientDetail in detail.ExtendedIngredients)
        {
            var ingredient = await ResolveIngredientAsync(ingredientDetail, cancellationToken);
            recipeIngredients.Add(new RecipeIngredient
            {
                Ingredient = ingredient,
                Quantity = ingredientDetail.Amount,
                Unit = ingredientDetail.Unit
            });
        }

        return new Recipe
        {
            ExternalId = detail.Id,
            Name = detail.Title,
            Summary = detail.Summary,
            ImageUrl = detail.Image ?? string.Empty,
            Instructions = SerializeSteps(detail.AnalyzedInstructions),
            DishTypes = [.. detail.DishTypes],
            Diets = [.. detail.Diets],
            Cuisines = [.. detail.Cuisines],
            CachedAt = DateTime.UtcNow,
            RecipeIngredients = recipeIngredients
        };
    }

    // Reutilizar el ingrediente ya guardado en memoria o guardarlo por primera vez
    private async Task<Ingredient> ResolveIngredientAsync(SpoonacularIngredientDetail ingredientDetail, CancellationToken cancellationToken)
    {
        var existingIngredient = await _ingredientRepository.GetByExternalIdAsync(ingredientDetail.Id, cancellationToken);
        if (existingIngredient is not null)
        {
            return existingIngredient;
        }

        return new Ingredient
        {
            ExternalId = ingredientDetail.Id,
            Name = ingredientDetail.Name,
            ImageUrl = SpoonacularClient.BuildIngredientImageUrl(ingredientDetail.Image)
        };
    }

    // Serializar los pasos de preparación
    private static string SerializeSteps(List<SpoonacularInstructionGroup> analyzedInstructions)
    {
        var steps = analyzedInstructions
            .FirstOrDefault()?.Steps
            .Select(step => new StoredRecipeStep { Number = step.Number, Step = step.Step })
            ?? [];

        return JsonSerializer.Serialize(steps);
    }

    // Leer los pasos de preparación guardados
    public static List<StoredRecipeStep> ParseSteps(string instructionsJson)
    {
        return JsonSerializer.Deserialize<List<StoredRecipeStep>>(instructionsJson) ?? [];
    }
}
