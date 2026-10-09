using Stocked.Api.Services.Inventory;

namespace Stocked.Api.Tests.Services.Inventory;

public class UnitConversionServiceTests
{
    // Misma unidad no hace falta convertir nada
    [Fact]
    public void TryConvert_MismaUnidad_RegresaLaMismaCantidad()
    {
        var resultado = UnitConversionService.TryConvert(5, "g", "g");

        Assert.Equal(5, resultado);
    }

    // Litros a mililitros
    [Fact]
    public void TryConvert_LitrosAMililitros_Convierte()
    {
        var resultado = UnitConversionService.TryConvert(3, "l", "ml");

        Assert.Equal(3000, resultado);
    }

    // Mililitros a litros
    [Fact]
    public void TryConvert_MililitrosALitros_Convierte()
    {
        var resultado = UnitConversionService.TryConvert(400, "ml", "l");

        Assert.Equal(0.4m, resultado);
    }

    // Kilogramos a gramos
    [Fact]
    public void TryConvert_KilogramosAGramos_Convierte()
    {
        var resultado = UnitConversionService.TryConvert(2, "kg", "g");

        Assert.Equal(2000, resultado);
    }

    // Onzas a libras
    [Fact]
    public void TryConvert_OnzasALibras_Convierte()
    {
        var resultado = UnitConversionService.TryConvert(16, "oz", "lb");

        Assert.NotNull(resultado);
        Assert.Equal(1m, resultado.Value, precision: 2);
    }

    // Spoonacular regresa palabras completas en inglés
    [Fact]
    public void TryConvert_PalabrasCompletasDeSpoonacular_Convierte()
    {
        var resultadoOnzasALibras = UnitConversionService.TryConvert(16, "ounces", "pounds");
        var resultadoTazasAMililitros = UnitConversionService.TryConvert(1, "cups", "ml");

        Assert.NotNull(resultadoOnzasALibras);
        Assert.Equal(1m, resultadoOnzasALibras.Value, precision: 2);
        Assert.Equal(240, resultadoTazasAMililitros);
    }

    // Volumen y peso no son convertibles entre sí
    [Fact]
    public void TryConvert_FamiliasDistintas_RegresaNull()
    {
        var resultado = UnitConversionService.TryConvert(400, "ml", "g");

        Assert.Null(resultado);
    }

    // Una unidad de conteo no registrada nunca se convierte a peso o volumen
    [Fact]
    public void TryConvert_UnidadDeConteoDesconocida_RegresaNull()
    {
        var resultado = UnitConversionService.TryConvert(2, "clove", "g");

        Assert.Null(resultado);
    }

    // Dos unidades de conteo iguales sí se comparan directo
    [Fact]
    public void TryConvert_UnidadesDeConteoIguales_RegresaLaCantidad()
    {
        var resultado = UnitConversionService.TryConvert(3, "clove", "clove");

        Assert.Equal(3, resultado);
    }

    // Dos unidades de conteo distintas nunca se consideran equivalentes
    [Fact]
    public void TryConvert_UnidadesDeConteoDistintas_RegresaNull()
    {
        var resultado = UnitConversionService.TryConvert(1, "clove", "piece");

        Assert.Null(resultado);
    }
}
