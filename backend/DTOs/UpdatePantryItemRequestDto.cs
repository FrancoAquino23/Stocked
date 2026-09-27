namespace Stocked.Api.DTOs;

// DTO: Petición para actualizar la cantidad habitual de compra de un ingrediente
public class UpdatePantryItemRequestDto
{
    public decimal? UsualQuantity { get; set; }
    public string? UsualUnit { get; set; }
}
