namespace Stocked.Api.Models;

// Modelo: Ajuste de despensa
public class PantryAdjustment
{
    public int Id { get; set; }

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public PantryAdjustmentReason Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}
