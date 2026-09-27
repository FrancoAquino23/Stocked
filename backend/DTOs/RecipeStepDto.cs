namespace Stocked.Api.DTOs;

// DTO: Paso de preparación de una receta
public class RecipeStepDto
{
    public int Number { get; set; }
    public string Description { get; set; } = string.Empty;
}
