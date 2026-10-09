using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Services.Inventory;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/shopping-list")]
public class ShoppingListController : ControllerBase
{
    private readonly ShoppingListService _shoppingListService;

    public ShoppingListController(ShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    // Calcular la lista de faltantes de la semana que arranca
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingListItemDto>>> GetShoppingList([FromQuery] DateOnly weekStart)
    {
        var entries = await _shoppingListService.CalculateAsync(weekStart);

        var items = entries.Select(entry => new ShoppingListItemDto
        {
            IngredientName = entry.IngredientName,
            Uncertain = entry.Uncertain
        });

        return Ok(items);
    }
}
