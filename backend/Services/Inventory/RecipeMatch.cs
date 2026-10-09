namespace Stocked.Api.Services.Inventory;

// Resultado de comparar los ingredientes de una receta contra la despensa
public record RecipeMatch(bool CanMake, int MissingIngredientsCount);
