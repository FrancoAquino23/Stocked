using System.ComponentModel.DataAnnotations;

namespace Stocked.Api.DTOs;

// DTO: Petición para actualizar la cantidad habitual de compra de un ingrediente
public class UpdatePantryItemRequestDto
{
    [Range(0.01, double.MaxValue)]
    public decimal? UsualQuantity { get; set; }

    public string? UsualUnit { get; set; }
}
