using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class IngredientRepository
{
    private readonly StockedDbContext _dbContext;

    public IngredientRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Buscar un ingrediente ya cacheado por su ID de Spoonacular
    public Task<Ingredient?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Ingredients
            .FirstOrDefaultAsync(ingredient => ingredient.ExternalId == externalId, cancellationToken);
    }

    // Guardar un ingrediente nuevo por sí solo
    public async Task<Ingredient> AddAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ingredient;
    }
}
