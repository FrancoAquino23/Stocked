using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.Models;

namespace Stocked.Api.Repositories;

public class UserRepository
{
    private readonly StockedDbContext _dbContext;

    public UserRepository(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Buscar un usuario por su correo 
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    // Registrar un usuario nuevo
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
