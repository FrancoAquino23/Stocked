using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class PantryRepository
{
    private readonly StockedDbContext _dbContext;

    public PantryRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Listar toda la despensa
    public Task<List<PantryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.PantryItems
            .Include(pantryItem => pantryItem.Ingredient)
            .ToListAsync(cancellationToken);
    }

    // Buscar la fila de despensa de un ingrediente por su ID
    public Task<PantryItem?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return _dbContext.PantryItems
            .Include(pantryItem => pantryItem.Ingredient)
            .FirstOrDefaultAsync(pantryItem => pantryItem.Ingredient.ExternalId == externalId, cancellationToken);
    }

    // Guardar un ítem nuevo en la despensa
    public async Task<PantryItem> AddAsync(PantryItem pantryItem, CancellationToken cancellationToken = default)
    {
        _dbContext.PantryItems.Add(pantryItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return pantryItem;
    }

    // Guardar los cambios de un ítem ya existente en la despensa
    public Task UpdateAsync(PantryItem pantryItem, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Quitar un ítem de la despensa
    public async Task RemoveAsync(PantryItem pantryItem, CancellationToken cancellationToken = default)
    {
        _dbContext.PantryItems.Remove(pantryItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
