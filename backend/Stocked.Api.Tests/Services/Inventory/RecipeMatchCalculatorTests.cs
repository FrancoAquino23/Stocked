using Stocked.Api.Models;
using Stocked.Api.Services.Inventory;

namespace Stocked.Api.Tests.Services.Inventory;

public class RecipeMatchCalculatorTests
{
    // Si la despensa tiene todos los ingredientes de la receta, se puede cocinar
    [Fact]
    public void Calculate_TodosLosIngredientesEnDespensa_PuedeHacerla()
    {
        var receta = CrearReceta(ingredienteIds: [1, 2, 3]);
        var despensa = new HashSet<int> { 1, 2, 3, 4 };

        var resultado = RecipeMatchCalculator.Calculate(receta, despensa);

        Assert.True(resultado.CanMake);
        Assert.Equal(0, resultado.MissingIngredientsCount);
    }

    // Si faltan algunos ingredientes, no se puede cocinar y se cuenta cuántos faltan
    [Fact]
    public void Calculate_FaltanAlgunosIngredientes_NoPuedeHacerlaYCuentaCuantosFaltan()
    {
        var receta = CrearReceta(ingredienteIds: [1, 2, 3]);
        var despensa = new HashSet<int> { 1 };

        var resultado = RecipeMatchCalculator.Calculate(receta, despensa);

        Assert.False(resultado.CanMake);
        Assert.Equal(2, resultado.MissingIngredientsCount);
    }

    // Con la despensa vacía, faltan todos los ingredientes de la receta
    [Fact]
    public void Calculate_DespensaVacia_FaltanTodos()
    {
        var receta = CrearReceta(ingredienteIds: [1, 2]);
        var despensa = new HashSet<int>();

        var resultado = RecipeMatchCalculator.Calculate(receta, despensa);

        Assert.False(resultado.CanMake);
        Assert.Equal(2, resultado.MissingIngredientsCount);
    }

    private static Recipe CrearReceta(int[] ingredienteIds)
    {
        return new Recipe
        {
            RecipeIngredients = ingredienteIds
                .Select(id => new RecipeIngredient { IngredientId = id, Quantity = 1, Unit = "g" })
                .ToList()
        };
    }
}
