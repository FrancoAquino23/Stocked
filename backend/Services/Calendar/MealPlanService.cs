using Stocked.Api.Models;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Recipes;

namespace Stocked.Api.Services.Calendar;

public class MealPlanService
{
    private readonly RecipeService _recipeService;
    private readonly MealPlanEntryRepository _mealPlanEntryRepository;

    public MealPlanService(RecipeService recipeService, MealPlanEntryRepository mealPlanEntryRepository)
    {
        _recipeService = recipeService;
        _mealPlanEntryRepository = mealPlanEntryRepository;
    }

    // Listar el calendario de un usuario
    public Task<List<MealPlanEntry>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _mealPlanEntryRepository.GetAllAsync(userId, cancellationToken);
    }

    // Agendar una receta en el calendario
    public async Task<MealPlanEntry> AddAsync(
        int userId, int externalId, DateOnly date, MealSlot mealSlot, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId, cancellationToken);
        var entry = new MealPlanEntry { UserId = userId, RecipeId = recipe.Id, Date = date, MealSlot = mealSlot };

        return await _mealPlanEntryRepository.AddAsync(entry, cancellationToken);
    }

    // Mover una entrada agendada a otro día
    public async Task<MealPlanEntry?> UpdateAsync(
        int userId, int id, DateOnly date, MealSlot mealSlot, CancellationToken cancellationToken = default)
    {
        var entry = await _mealPlanEntryRepository.GetByIdAsync(userId, id, cancellationToken);
        if (entry is null)
        {
            return null;
        }

        entry.Date = date;
        entry.MealSlot = mealSlot;
        await _mealPlanEntryRepository.UpdateAsync(entry, cancellationToken);

        return entry;
    }

    // Quitar una entrada del calendario
    public async Task RemoveAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        var entry = await _mealPlanEntryRepository.GetByIdAsync(userId, id, cancellationToken);
        if (entry is not null)
        {
            await _mealPlanEntryRepository.RemoveAsync(entry, cancellationToken);
        }
    }
}
