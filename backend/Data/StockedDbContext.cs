using Microsoft.EntityFrameworkCore;
using Stocked.Api.Models;

namespace Stocked.Api.Data;

public class StockedDbContext : DbContext
{
    public StockedDbContext(DbContextOptions<StockedDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    // Configurar restricciones e índices del modelo de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        // TEMP: Verificar la conectividad Postgres <-> API <-> Angular
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Email = "prueba@stocked.dev",
            DisplayName = "Usuario de Prueba",
            CreatedAt = new DateTime(2026, 9, 20, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
