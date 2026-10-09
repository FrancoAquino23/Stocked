using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Pantry;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/pantry-adjustments")]
public class PantryAdjustmentController : ControllerBase
{
    private readonly PantryService _pantryService;

    public PantryAdjustmentController(PantryService pantryService)
    {
        _pantryService = pantryService;
    }

    // Reportar que un ingrediente se echó a perder sin haberse usado en ninguna receta
    [HttpPost]
    public async Task<ActionResult<PantryAdjustmentDto>> Report([FromBody] ReportPantryAdjustmentRequestDto request)
    {
        var adjustment = await _pantryService.ReportAdjustmentAsync(
            request.ExternalId, request.Quantity, request.Unit, request.Reason);

        return Ok(MapToDto(adjustment));
    }

    private static PantryAdjustmentDto MapToDto(PantryAdjustment adjustment)
    {
        return new PantryAdjustmentDto
        {
            Id = adjustment.Id,
            ExternalId = adjustment.Ingredient.ExternalId,
            IngredientName = adjustment.Ingredient.Name,
            Quantity = adjustment.Quantity,
            Unit = adjustment.Unit,
            Reason = adjustment.Reason,
            CreatedAt = adjustment.CreatedAt
        };
    }
}
