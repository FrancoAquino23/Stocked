namespace Stocked.Api.DTOs;

// DTO: Fila de la despensa del usuario
public class PantryItemDto
{
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? UsualQuantity { get; set; }
    public string? UsualUnit { get; set; }
}
