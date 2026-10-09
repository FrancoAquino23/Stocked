using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class RecipeRepository
{
    private readonly StockedDbContext _dbContext;

    public RecipeRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Buscar una receta ya cacheada por su ID de "Spoonacular" (Con sus ingredientes incluidos)
    public Task<Recipe?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Recipes
            .Include(recipe => recipe.RecipeIngredients)
                .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .FirstOrDefaultAsync(recipe => recipe.ExternalId == externalId, cancellationToken);
    }

    // Guardar una receta nueva
    public async Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return recipe;
    }

    // Listar las recetas vistas recientemente
    public Task<List<Recipe>> GetRecentAsync(int maxResults, CancellationToken cancellationToken = default)
    {
        return _dbContext.Recipes
            .Include(recipe => recipe.RecipeIngredients)
                .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .Where(recipe => recipe.LastViewedAt != null)
            .OrderByDescending(recipe => recipe.LastViewedAt)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
