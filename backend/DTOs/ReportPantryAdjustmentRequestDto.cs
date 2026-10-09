using Stocked.Api.Models;

namespace Stocked.Api.DTOs;

// DTO: Petición para reportar un ajuste de despensa
public class ReportPantryAdjustmentRequestDto
{
    public int ExternalId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public PantryAdjustmentReason Reason { get; set; }
}
