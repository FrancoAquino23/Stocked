using Stocked.Api.Models;
using Stocked.Api.Services.Inventory;

namespace Stocked.Api.Tests.Services.Inventory;

public class ShoppingListCalculatorTests
{
    private const int LecheId = 1;
    private const int AjoId = 2;

    // Si la despensa alcanza para lo requerido, el ingrediente no debe aparecer en la lista
    [Fact]
    public void Calculate_AlcanzaConLoDisponible_NoApareceEnLaLista()
    {
        var despensa = new[] { CrearPantryItem(LecheId, 3, "l") };
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], requerido);

        Assert.Empty(resultado);
    }

    // Si no alcanza, debe aparecer en la lista con el cálculo confiable, no como incierto
    [Fact]
    public void Calculate_NoAlcanza_ApareceSinIncertidumbre()
    {
        var despensa = new[] { CrearPantryItem(LecheId, 200, "ml") };
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.Equal("Leche", item.IngredientName);
        Assert.False(item.Uncertain);
    }

    // Sin cantidad habitual capturada no hay con qué comparar, debe aparecer como incierto
    [Fact]
    public void Calculate_SinCantidadHabitualCapturada_ApareceComoIncierto()
    {
        var despensa = new[] { CrearPantryItem(LecheId, usualQuantity: null, usualUnit: null) };
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.True(item.Uncertain);
    }

    // Un ingrediente que ni siquiera está en la despensa también debe aparecer como incierto
    [Fact]
    public void Calculate_IngredienteNoEstaEnLaDespensa_ApareceComoIncierto()
    {
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(pantryItems: [], consumed: [], lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.True(item.Uncertain);
    }

    // Unidades que no son convertibles entre sí deben marcar el ingrediente como incierto
    [Fact]
    public void Calculate_UnidadDeLaDespensaNoCoincideConLaDeLaReceta_ApareceComoIncierto()
    {
        var despensa = new[] { CrearPantryItem(AjoId, 2, "cabeza") };
        var requerido = new[] { new IngredientAmount(AjoId, "Ajo", 3, "clove") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.True(item.Uncertain);
    }

    // El mismo ingrediente pedido por dos recetas en familias de unidades distintas no se puede sumar con confianza
    [Fact]
    public void Calculate_MismoIngredienteEnDosRecetasConFamiliasDistintas_ApareceComoIncierto()
    {
        var despensa = new[] { CrearPantryItem(AjoId, 10, "clove") };
        var requerido = new[]
        {
            new IngredientAmount(AjoId, "Ajo", 3, "clove"),
            new IngredientAmount(AjoId, "Ajo", 15, "g")
        };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.True(item.Uncertain);
    }

    // Lo ya consumido esta semana debe restarse de lo disponible antes de calcular si falta
    [Fact]
    public void Calculate_LoYaConsumidoReduceLoDisponible()
    {
        var despensa = new[] { CrearPantryItem(LecheId, 1, "l") };
        var consumido = new[] { new IngredientAmount(LecheId, "Leche", 700, "ml") };
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumido, lost: [], requerido);

        var item = Assert.Single(resultado);
        Assert.False(item.Uncertain);
    }

    // Lo reportado como perdido (merma) debe restarse de lo disponible igual que lo consumido
    [Fact]
    public void Calculate_LoReportadoComoPerdidoReduceLoDisponible()
    {
        var despensa = new[] { CrearPantryItem(LecheId, 1, "l") };
        var perdido = new[] { new IngredientAmount(LecheId, "Leche", 700, "ml") };
        var requerido = new[] { new IngredientAmount(LecheId, "Leche", 400, "ml") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], perdido, requerido);

        var item = Assert.Single(resultado);
        Assert.False(item.Uncertain);
    }

    // Sin nada requerido, la lista debe salir vacía
    [Fact]
    public void Calculate_SinNadaRequerido_ListaVacia()
    {
        var despensa = new[] { CrearPantryItem(LecheId, 1, "l") };

        var resultado = ShoppingListCalculator.Calculate(despensa, consumed: [], lost: [], required: []);

        Assert.Empty(resultado);
    }

    private static Models.PantryItem CrearPantryItem(int ingredientId, decimal? usualQuantity, string? usualUnit)
    {
        return new Models.PantryItem { IngredientId = ingredientId, UsualQuantity = usualQuantity, UsualUnit = usualUnit };
    }
}
