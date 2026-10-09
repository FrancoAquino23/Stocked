namespace Stocked.Api.Services.Inventory;

// Modelo: Familia de unidades convertibles entre sí
public enum UnitFamily
{
    Volume,
    Weight
}

// Convierte cantidades entre unidades de la misma familia
public static class UnitConversionService
{
    // Abreviaturas y palabras completas que Spoonacular realmente usa
    private static readonly Dictionary<string, (UnitFamily Family, decimal FactorToBaseUnit)> Units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["ml"] = (UnitFamily.Volume, 1m),
            ["milliliter"] = (UnitFamily.Volume, 1m),
            ["milliliters"] = (UnitFamily.Volume, 1m),
            ["l"] = (UnitFamily.Volume, 1000m),
            ["liter"] = (UnitFamily.Volume, 1000m),
            ["liters"] = (UnitFamily.Volume, 1000m),
            ["cup"] = (UnitFamily.Volume, 240m),
            ["cups"] = (UnitFamily.Volume, 240m),
            ["tablespoon"] = (UnitFamily.Volume, 15m),
            ["tablespoons"] = (UnitFamily.Volume, 15m),
            ["teaspoon"] = (UnitFamily.Volume, 5m),
            ["teaspoons"] = (UnitFamily.Volume, 5m),
            ["g"] = (UnitFamily.Weight, 1m),
            ["gram"] = (UnitFamily.Weight, 1m),
            ["grams"] = (UnitFamily.Weight, 1m),
            ["kg"] = (UnitFamily.Weight, 1000m),
            ["kilogram"] = (UnitFamily.Weight, 1000m),
            ["kilograms"] = (UnitFamily.Weight, 1000m),
            ["oz"] = (UnitFamily.Weight, 28.35m),
            ["ounce"] = (UnitFamily.Weight, 28.35m),
            ["ounces"] = (UnitFamily.Weight, 28.35m),
            ["lb"] = (UnitFamily.Weight, 453.6m),
            ["pound"] = (UnitFamily.Weight, 453.6m),
            ["pounds"] = (UnitFamily.Weight, 453.6m)
        };

    // Convertir una cantidad de una unidad a otra
    public static decimal? TryConvert(decimal quantity, string fromUnit, string toUnit)
    {
        if (string.Equals(fromUnit, toUnit, StringComparison.OrdinalIgnoreCase))
        {
            return quantity;
        }

        if (!Units.TryGetValue(fromUnit, out var from) || !Units.TryGetValue(toUnit, out var to))
        {
            return null;
        }

        if (from.Family != to.Family)
        {
            return null;
        }

        return quantity * from.FactorToBaseUnit / to.FactorToBaseUnit;
    }
}
