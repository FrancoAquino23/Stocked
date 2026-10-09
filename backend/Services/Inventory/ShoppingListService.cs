using Stocked.Api.Models;
using Stocked.Api.Repositories;

namespace Stocked.Api.Services.Inventory;

// Arma los datos reales y delega el cálculo en sí a ShoppingListCalculator
public class ShoppingListService
{
    private readonly PantryRepository _pantryRepository;
    private readonly RecipePreparationRepository _recipePreparationRepository;
    private readonly PantryAdjustmentRepository _pantryAdjustmentRepository;

    public ShoppingListService(
        PantryRepository pantryRepository,
        RecipePreparationRepository recipePreparationRepository,
        PantryAdjustmentRepository pantryAdjustmentRepository)
    {
        _pantryRepository = pantryRepository;
        _recipePreparationRepository = recipePreparationRepository;
        _pantryAdjustmentRepository = pantryAdjustmentRepository;
    }

    // Calcular la lista de faltantes de la semana
    public async Task<List<ShoppingListEntry>> CalculateAsync(DateOnly weekStart, CancellationToken cancellationToken = default)
    {
        var windowStart = DateTime.SpecifyKind(weekStart.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var windowEnd = DateTime.SpecifyKind(weekStart.AddDays(7).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var pantryItems = await _pantryRepository.GetAllAsync(cancellationToken);
        var finishedPreparations = await _recipePreparationRepository
            .GetFinishedWithinWindowAsync(windowStart, windowEnd, cancellationToken);
        var unfinishedPreparations = await _recipePreparationRepository.GetUnfinishedAsync(cancellationToken);
        var adjustments = await _pantryAdjustmentRepository
            .GetWithinWindowAsync(windowStart, windowEnd, cancellationToken);

        var consumed = ToIngredientAmounts(finishedPreparations);
        var required = ToIngredientAmounts(unfinishedPreparations);
        var lost = adjustments.Select(adjustment =>
            new IngredientAmount(adjustment.IngredientId, adjustment.Ingredient.Name, adjustment.Quantity, adjustment.Unit));

        return ShoppingListCalculator.Calculate(pantryItems, consumed, lost, required);
    }

    // Una fila por cada ingrediente de cada receta preparada, multiplicada por cuántas veces se hizo
    private static IEnumerable<IngredientAmount> ToIngredientAmounts(IEnumerable<RecipePreparation> preparations)
    {
        return preparations.SelectMany(preparation => preparation.Recipe.RecipeIngredients.Select(recipeIngredient =>
            new IngredientAmount(
                recipeIngredient.IngredientId,
                recipeIngredient.Ingredient.Name,
                recipeIngredient.Quantity * preparation.Multiplier,
                recipeIngredient.Unit)));
    }
}
