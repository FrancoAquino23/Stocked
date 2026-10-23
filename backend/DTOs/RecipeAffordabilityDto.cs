namespace Stocked.Api.DTOs;

// DTO: Qué tanto alcanza la despensa para preparar una receta X veces
public class RecipeAffordabilityDto
{
    public int RequestedMultiplier { get; set; }
    public int AffordableMultiplier { get; set; }
    public bool FullyCovered { get; set; }
    public List<string> UncertainIngredients { get; set; } = [];
}
