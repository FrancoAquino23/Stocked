using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;
using Stocked.Api.DTOs;

namespace Stocked.Api.Controllers;

// TEMP: Verificar la conectividad Postgres <-> API <-> Angular
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly StockedDbContext _dbContext;

    public UsersController(StockedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Devolver todos los usuarios registrados
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _dbContext.Users
            .OrderBy(user => user.Id)
            .Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                CreatedAt = user.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }
}
