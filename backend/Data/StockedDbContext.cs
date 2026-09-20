using Microsoft.EntityFrameworkCore;
using Stocked.Api.Models;

namespace Stocked.Api.Data;

public class StockedDbContext : DbContext
{
    public StockedDbContext(DbContextOptions<StockedDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();
    public DbSet<PantryItem> PantryItems => Set<PantryItem>();
    public DbSet<MealPlanEntry> MealPlanEntries => Set<MealPlanEntry>();
    public DbSet<RecipePreparation> RecipePreparations => Set<RecipePreparation>();

    // Configurar índices, tipos de columna y relaciones del modelo de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Ingredient>()
            .HasIndex(ingredient => ingredient.ExternalId)
            .IsUnique();

        modelBuilder.Entity<Recipe>()
            .HasIndex(recipe => recipe.ExternalId)
            .IsUnique();

        // Procedimiento de preparación (Pasos)
        modelBuilder.Entity<Recipe>()
            .Property(recipe => recipe.Instructions)
            .HasColumnType("jsonb");

        // Borrar el plan agendado no debe bloquearse ni borrar la preparación
        modelBuilder.Entity<RecipePreparation>()
            .HasOne(preparation => preparation.MealPlanEntry)
            .WithMany()
            .HasForeignKey(preparation => preparation.MealPlanEntryId)
            .OnDelete(DeleteBehavior.SetNull);

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
