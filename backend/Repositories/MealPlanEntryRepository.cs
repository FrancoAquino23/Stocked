using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class MealPlanEntryRepository
{
    private readonly StockedDbContext _dbContext;

    public MealPlanEntryRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Listar el calendario completo
    public Task<List<MealPlanEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.MealPlanEntries
            .Include(entry => entry.Recipe)
            .ToListAsync(cancellationToken);
    }

    // Buscar una entrada del calendario por su ID
    public Task<MealPlanEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.MealPlanEntries
            .Include(entry => entry.Recipe)
            .FirstOrDefaultAsync(entry => entry.Id == id, cancellationToken);
    }

    // Agendar una receta nueva en el calendario
    public async Task<MealPlanEntry> AddAsync(MealPlanEntry entry, CancellationToken cancellationToken = default)
    {
        _dbContext.MealPlanEntries.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entry;
    }

    // Guardar los cambios de una entrada ya existente en el calendario
    public Task UpdateAsync(MealPlanEntry entry, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Quitar una entrada del calendario
    public async Task RemoveAsync(MealPlanEntry entry, CancellationToken cancellationToken = default)
    {
        _dbContext.MealPlanEntries.Remove(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
