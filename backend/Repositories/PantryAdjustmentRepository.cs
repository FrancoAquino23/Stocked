using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class PantryAdjustmentRepository
{
    private readonly StockedDbContext _dbContext;

    public PantryAdjustmentRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Listar los ajustes dentro de una ventana de fechas
    public Task<List<PantryAdjustment>> GetWithinWindowAsync(
        DateTime windowStart, DateTime windowEnd, CancellationToken cancellationToken = default)
    {
        return _dbContext.PantryAdjustments
            .Include(adjustment => adjustment.Ingredient)
            .Where(adjustment => adjustment.CreatedAt >= windowStart && adjustment.CreatedAt <= windowEnd)
            .ToListAsync(cancellationToken);
    }

    // Registrar un ajuste nuevo
    public async Task<PantryAdjustment> AddAsync(PantryAdjustment adjustment, CancellationToken cancellationToken = default)
    {
        _dbContext.PantryAdjustments.Add(adjustment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return adjustment;
    }
}
