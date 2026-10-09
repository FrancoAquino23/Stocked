namespace Stocked.Api.DTOs;

// DTO: Preparación de una receta (declarada, en curso o terminada)
public class RecipePreparationDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int Multiplier { get; set; }
    public string Status { get; set; } = string.Empty;
}
