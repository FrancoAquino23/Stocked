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

    // Listar el calendario de un usuario
    public Task<List<MealPlanEntry>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.MealPlanEntries
            .Include(entry => entry.Recipe)
            .Where(entry => entry.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    // Buscar una entrada del calendario por su ID, dentro del calendario de un usuario
    public Task<MealPlanEntry?> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.MealPlanEntries
            .Include(entry => entry.Recipe)
            .FirstOrDefaultAsync(entry => entry.UserId == userId && entry.Id == id, cancellationToken);
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
