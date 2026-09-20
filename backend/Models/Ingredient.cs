namespace Stocked.Api.Models;

// Modelo: Ingrediente API
public class Ingredient
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
