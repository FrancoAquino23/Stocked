namespace Stocked.Api.DTOs;

// DTO: Ingrediente en la lista de compras calculada
public class ShoppingListItemDto
{
    public string IngredientName { get; set; } = string.Empty;

    public bool Uncertain { get; set; }
}
