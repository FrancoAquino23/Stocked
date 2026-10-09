using Stocked.Api.Models;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Recipes;

namespace Stocked.Api.Services.Preparations;

public class RecipePreparationService
{
    private readonly RecipeService _recipeService;
    private readonly RecipePreparationRepository _recipePreparationRepository;

    public RecipePreparationService(RecipeService recipeService, RecipePreparationRepository recipePreparationRepository)
    {
        _recipeService = recipeService;
        _recipePreparationRepository = recipePreparationRepository;
    }

    // Declarar que se va a preparar una receta X veces
    public async Task<RecipePreparation> DeclareAsync(
        int externalId, int multiplier, int? mealPlanEntryId, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId, cancellationToken);
        var preparation = new RecipePreparation
        {
            RecipeId = recipe.Id,
            Multiplier = multiplier,
            MealPlanEntryId = mealPlanEntryId
        };

        return await _recipePreparationRepository.AddAsync(preparation, cancellationToken);
    }

    // Marcar que ya se empezó a cocinar, pasa de Pendiente a En curso
    public async Task<RecipePreparation?> StartAsync(int id, CancellationToken cancellationToken = default)
    {
        var preparation = await _recipePreparationRepository.GetByIdAsync(id, cancellationToken);
        if (preparation is null)
        {
            return null;
        }

        preparation.StartedAt = DateTime.UtcNow;
        await _recipePreparationRepository.UpdateAsync(preparation, cancellationToken);

        return preparation;
    }

    // Marcar que ya se terminó y descontar sus ingredientes del inventario de la semana
    public async Task<RecipePreparation?> FinishAsync(int id, CancellationToken cancellationToken = default)
    {
        var preparation = await _recipePreparationRepository.GetByIdAsync(id, cancellationToken);
        if (preparation is null)
        {
            return null;
        }

        preparation.FinishedAt = DateTime.UtcNow;
        await _recipePreparationRepository.UpdateAsync(preparation, cancellationToken);

        return preparation;
    }
}
