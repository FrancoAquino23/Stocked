namespace Stocked.Api.Models;

// Modelo: Receta favorita
public class FavoriteRecipe
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
