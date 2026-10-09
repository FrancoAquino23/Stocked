namespace Stocked.Api.Services.Inventory;

// Cantidad de un ingrediente de una receta, multiplicada por cuántas veces se prepara
public record IngredientAmount(int IngredientId, string IngredientName, decimal Quantity, string Unit);
