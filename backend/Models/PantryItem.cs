namespace Stocked.Api.Models;

// Modelo: Ingrediente
public class PantryItem
{
    public int Id { get; set; }

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    // Cantidad de ingrediente que el usuario suele comprar del producto
    public decimal? UsualQuantity { get; set; }
    public string? UsualUnit { get; set; }
}
