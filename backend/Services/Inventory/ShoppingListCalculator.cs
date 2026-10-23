using Stocked.Api.Models;

namespace Stocked.Api.Services.Inventory;

// Calcula qué ingredientes faltan comprar
public static class ShoppingListCalculator
{
    // Comparar lo requerido contra lo disponible
    public static List<ShoppingListEntry> Calculate(
        IEnumerable<PantryItem> pantryItems,
        IEnumerable<IngredientAmount> consumed,
        IEnumerable<IngredientAmount> lost,
        IEnumerable<IngredientAmount> required)
    {
        var pantryByIngredient = pantryItems.ToDictionary(pantryItem => pantryItem.IngredientId);
        var results = new List<ShoppingListEntry>();

        foreach (var group in required.GroupBy(amount => amount.IngredientId))
        {
            var ingredientName = group.First().IngredientName;

            var totalRequired = SumToCommonUnit(group, out var unit);
            if (totalRequired is null || unit is null)
            {
                results.Add(new ShoppingListEntry(ingredientName, Uncertain: true));
                continue;
            }

            if (!pantryByIngredient.TryGetValue(group.Key, out var pantryItem) ||
                pantryItem.UsualQuantity is null || pantryItem.UsualUnit is null)
            {
                results.Add(new ShoppingListEntry(ingredientName, Uncertain: true));
                continue;
            }

            var usualConverted = UnitConversionService.TryConvert(pantryItem.UsualQuantity.Value, pantryItem.UsualUnit, unit);
            var consumedTotal = SumConvertedTo(consumed.Where(amount => amount.IngredientId == group.Key), unit);
            var lostTotal = SumConvertedTo(lost.Where(amount => amount.IngredientId == group.Key), unit);

            if (usualConverted is null || consumedTotal is null || lostTotal is null)
            {
                results.Add(new ShoppingListEntry(ingredientName, Uncertain: true));
                continue;
            }

            var available = usualConverted.Value - consumedTotal.Value - lostTotal.Value;
            var missing = totalRequired.Value - available;

            if (missing > 0)
            {
                results.Add(new ShoppingListEntry(ingredientName, Uncertain: false));
            }
        }

        return results;
    }

    // Calcular cuántas veces (de las solicitadas) alcanza la despensa para una sola receta
    public static (int AffordableMultiplier, List<string> Uncertain) CalculateAffordability(
        IEnumerable<PantryItem> pantryItems,
        IEnumerable<IngredientAmount> consumed,
        IEnumerable<IngredientAmount> committed,
        IEnumerable<IngredientAmount> perUnitRequired,
        int requestedMultiplier)
    {
        var pantryByIngredient = pantryItems.ToDictionary(pantryItem => pantryItem.IngredientId);
        var uncertain = new List<string>();
        var maxAffordable = requestedMultiplier;

        foreach (var ingredient in perUnitRequired)
        {
            if (!pantryByIngredient.TryGetValue(ingredient.IngredientId, out var pantryItem) ||
                pantryItem.UsualQuantity is null || pantryItem.UsualUnit is null)
            {
                uncertain.Add(ingredient.IngredientName);
                continue;
            }

            var usualConverted = UnitConversionService.TryConvert(pantryItem.UsualQuantity.Value, pantryItem.UsualUnit, ingredient.Unit);
            var consumedTotal = SumConvertedTo(consumed.Where(amount => amount.IngredientId == ingredient.IngredientId), ingredient.Unit);
            var committedTotal = SumConvertedTo(committed.Where(amount => amount.IngredientId == ingredient.IngredientId), ingredient.Unit);

            if (usualConverted is null || consumedTotal is null || committedTotal is null)
            {
                uncertain.Add(ingredient.IngredientName);
                continue;
            }

            var available = usualConverted.Value - consumedTotal.Value - committedTotal.Value;
            var affordableForThisIngredient = available <= 0 || ingredient.Quantity <= 0
                ? 0
                : (int)Math.Floor(available / ingredient.Quantity);

            maxAffordable = Math.Min(maxAffordable, Math.Max(0, affordableForThisIngredient));
        }

        return (maxAffordable, uncertain);
    }

    // Sumar a una sola unidad
    private static decimal? SumToCommonUnit(IEnumerable<IngredientAmount> amounts, out string? commonUnit)
    {
        var list = amounts.ToList();
        commonUnit = list[0].Unit;
        return SumConvertedTo(list, commonUnit);
    }

    // Sumar cantidades ya convertidas a la unidad objetivo
    private static decimal? SumConvertedTo(IEnumerable<IngredientAmount> amounts, string targetUnit)
    {
        decimal total = 0;
        foreach (var amount in amounts)
        {
            var converted = UnitConversionService.TryConvert(amount.Quantity, amount.Unit, targetUnit);
            if (converted is null)
            {
                return null;
            }
            total += converted.Value;
        }

        return total;
    }
}
