using System.ComponentModel.DataAnnotations;

namespace Stocked.Api.DTOs;

// DTO: Petición para declarar que se va a preparar una receta
public class DeclareRecipePreparationRequestDto
{
    public int ExternalId { get; set; }

    [Range(1, int.MaxValue)]
    public int Multiplier { get; set; }

    public int? MealPlanEntryId { get; set; }
}
