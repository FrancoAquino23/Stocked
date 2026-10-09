using Stocked.Api.Models;

namespace Stocked.Api.Services.Inventory;

// Comparar los ingredientes de una receta contra la despensa (ignora cantidad)
public static class RecipeMatchCalculator
{
    public static RecipeMatch Calculate(Recipe recipe, IReadOnlySet<int> pantryIngredientIds)
    {
        var missingCount = recipe.RecipeIngredients
            .Count(recipeIngredient => !pantryIngredientIds.Contains(recipeIngredient.IngredientId));

        return new RecipeMatch(CanMake: missingCount == 0, MissingIngredientsCount: missingCount);
    }
}
