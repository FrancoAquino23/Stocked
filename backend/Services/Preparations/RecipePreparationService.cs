using Stocked.Api.Models;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Inventory;
using Stocked.Api.Services.Recipes;

namespace Stocked.Api.Services.Preparations;

public class RecipePreparationService
{
    private readonly RecipeService _recipeService;
    private readonly RecipePreparationRepository _recipePreparationRepository;
    private readonly PantryRepository _pantryRepository;

    public RecipePreparationService(
        RecipeService recipeService, RecipePreparationRepository recipePreparationRepository, PantryRepository pantryRepository)
    {
        _recipeService = recipeService;
        _recipePreparationRepository = recipePreparationRepository;
        _pantryRepository = pantryRepository;
    }

    // Declarar que se va a preparar una receta X veces
    public async Task<RecipePreparation> DeclareAsync(
        int userId, int externalId, int multiplier, int? mealPlanEntryId, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId, cancellationToken);
        var preparation = new RecipePreparation
        {
            UserId = userId,
            RecipeId = recipe.Id,
            Multiplier = multiplier,
            MealPlanEntryId = mealPlanEntryId
        };

        return await _recipePreparationRepository.AddAsync(preparation, cancellationToken);
    }

    // Marcar que ya se empezó a cocinar, pasa de Pendiente a En curso
    public async Task<RecipePreparation?> StartAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        var preparation = await _recipePreparationRepository.GetByIdAsync(userId, id, cancellationToken);
        if (preparation is null)
        {
            return null;
        }

        preparation.StartedAt = DateTime.UtcNow;
        await _recipePreparationRepository.UpdateAsync(preparation, cancellationToken);

        return preparation;
    }

    // Marcar que ya se terminó y descontar sus ingredientes del inventario de la semana
    public async Task<RecipePreparation?> FinishAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        var preparation = await _recipePreparationRepository.GetByIdAsync(userId, id, cancellationToken);
        if (preparation is null)
        {
            return null;
        }

        preparation.FinishedAt = DateTime.UtcNow;
        await _recipePreparationRepository.UpdateAsync(preparation, cancellationToken);

        return preparation;
    }

    // Calcular para cuántas de las veces solicitadas alcanza la despensa actual
    public async Task<(int Affordable, bool FullyCovered, List<string> Uncertain)> CheckAffordabilityAsync(
        int userId, int externalId, int multiplier, DateOnly weekStart, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId, cancellationToken);
        var windowStart = DateTime.SpecifyKind(weekStart.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var windowEnd = DateTime.SpecifyKind(weekStart.AddDays(7).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var pantryItems = await _pantryRepository.GetAllAsync(userId, cancellationToken);
        var finished = await _recipePreparationRepository.GetFinishedWithinWindowAsync(userId, windowStart, windowEnd, cancellationToken);
        var unfinished = await _recipePreparationRepository.GetUnfinishedAsync(userId, cancellationToken);

        var consumed = ShoppingListService.ToIngredientAmounts(finished);
        var committed = ShoppingListService.ToIngredientAmounts(unfinished);
        var perUnitRequired = recipe.RecipeIngredients.Select(recipeIngredient =>
            new IngredientAmount(recipeIngredient.IngredientId, recipeIngredient.Ingredient.Name, recipeIngredient.Quantity, recipeIngredient.Unit));

        var (affordable, uncertain) = ShoppingListCalculator.CalculateAffordability(
            pantryItems, consumed, committed, perUnitRequired, multiplier);

        // Solo se reporta "alcanza completo" si ningún ingrediente quedó incierto
        var fullyCovered = affordable >= multiplier && uncertain.Count == 0;
        return (affordable, fullyCovered, uncertain);
    }
}
