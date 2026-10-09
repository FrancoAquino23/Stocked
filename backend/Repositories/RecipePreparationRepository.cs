using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class RecipePreparationRepository
{
    private readonly StockedDbContext _dbContext;

    public RecipePreparationRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Buscar una preparación por su ID
    public Task<RecipePreparation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.RecipePreparations
            .Include(preparation => preparation.Recipe)
                .ThenInclude(recipe => recipe.RecipeIngredients)
                    .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .FirstOrDefaultAsync(preparation => preparation.Id == id, cancellationToken);
    }

    // Preparaciones sin terminar (Pendiente o En curso)
    public Task<List<RecipePreparation>> GetUnfinishedAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.RecipePreparations
            .Include(preparation => preparation.Recipe)
                .ThenInclude(recipe => recipe.RecipeIngredients)
                    .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .Where(preparation => preparation.FinishedAt == null)
            .ToListAsync(cancellationToken);
    }

    // Preparaciones terminadas dentro de una ventana de fechas, alimentan "consumido"
    public Task<List<RecipePreparation>> GetFinishedWithinWindowAsync(
        DateTime windowStart, DateTime windowEnd, CancellationToken cancellationToken = default)
    {
        return _dbContext.RecipePreparations
            .Include(preparation => preparation.Recipe)
                .ThenInclude(recipe => recipe.RecipeIngredients)
                    .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .Where(preparation => preparation.FinishedAt != null
                && preparation.FinishedAt >= windowStart && preparation.FinishedAt <= windowEnd)
            .ToListAsync(cancellationToken);
    }

    // Declarar una preparación nueva
    public async Task<RecipePreparation> AddAsync(RecipePreparation preparation, CancellationToken cancellationToken = default)
    {
        _dbContext.RecipePreparations.Add(preparation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return preparation;
    }

    // Guardar los cambios de una preparación ya existente
    public Task UpdateAsync(RecipePreparation preparation, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
