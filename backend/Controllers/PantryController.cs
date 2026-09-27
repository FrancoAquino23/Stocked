using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Pantry;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/pantry-items")]
public class PantryController : ControllerBase
{
    private readonly PantryService _pantryService;

    public PantryController(PantryService pantryService)
    {
        _pantryService = pantryService;
    }

    // Listar la despensa completa
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PantryItemDto>>> GetAll()
    {
        var pantryItems = await _pantryService.GetAllPantryItemsAsync();
        return Ok(pantryItems.Select(MapToDto));
    }

    // Agregar un ingrediente a la despensa
    [HttpPost]
    public async Task<ActionResult<PantryItemDto>> Add([FromBody] AddPantryItemRequestDto request)
    {
        var pantryItem = await _pantryService.AddPantryItemAsync(request.ExternalId);
        return Ok(MapToDto(pantryItem));
    }

    // Capturar/actualizar la cantidad habitual de compra de un ingrediente ya en la despensa
    [HttpPut("{externalId:int}")]
    public async Task<ActionResult<PantryItemDto>> Update(int externalId, [FromBody] UpdatePantryItemRequestDto request)
    {
        var pantryItem = await _pantryService.UpdatePantryItemAsync(externalId, request.UsualQuantity, request.UsualUnit);
        if (pantryItem is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(pantryItem));
    }

    // Quitar un ingrediente de la despensa
    [HttpDelete("{externalId:int}")]
    public async Task<IActionResult> Remove(int externalId)
    {
        await _pantryService.RemovePantryItemAsync(externalId);
        return NoContent();
    }

    private static PantryItemDto MapToDto(PantryItem pantryItem)
    {
        return new PantryItemDto
        {
            ExternalId = pantryItem.Ingredient.ExternalId,
            Name = pantryItem.Ingredient.Name,
            ImageUrl = pantryItem.Ingredient.ImageUrl,
            UsualQuantity = pantryItem.UsualQuantity,
            UsualUnit = pantryItem.UsualUnit
        };
    }
}
