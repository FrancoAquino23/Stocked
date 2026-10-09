using Stocked.Api.Models;

namespace Stocked.Api.DTOs;

// DTO: Ajuste de despensa ya registrado
public class PantryAdjustmentDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public PantryAdjustmentReason Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
