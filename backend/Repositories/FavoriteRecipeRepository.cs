using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class FavoriteRecipeRepository
{
    private readonly StockedDbContext _dbContext;

    public FavoriteRecipeRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Buscar si una receta ya está marcada como favorita
    public Task<FavoriteRecipe?> GetByRecipeIdAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        return _dbContext.FavoriteRecipes
            .FirstOrDefaultAsync(favorite => favorite.RecipeId == recipeId, cancellationToken);
    }

    // Obtener las recetas marcadas como favoritas
    public Task<List<Recipe>> GetAllRecipesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.FavoriteRecipes
            .Include(favorite => favorite.Recipe)
            .Select(favorite => favorite.Recipe)
            .ToListAsync(cancellationToken);
    }

    // Marcar una receta como favorita
    public async Task AddAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        _dbContext.FavoriteRecipes.Add(new FavoriteRecipe
        {
            RecipeId = recipeId,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Quitar una receta de favoritas
    public async Task RemoveAsync(FavoriteRecipe favorite, CancellationToken cancellationToken = default)
    {
        _dbContext.FavoriteRecipes.Remove(favorite);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
