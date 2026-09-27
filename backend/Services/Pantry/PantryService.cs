using Stocked.Api.Models;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Spoonacular;

namespace Stocked.Api.Services.Pantry;

public class PantryService
{
    private readonly SpoonacularClient _spoonacularClient;
    private readonly IngredientRepository _ingredientRepository;
    private readonly PantryRepository _pantryRepository;

    public PantryService(
        SpoonacularClient spoonacularClient,
        IngredientRepository ingredientRepository,
        PantryRepository pantryRepository)
    {
        _spoonacularClient = spoonacularClient;
        _ingredientRepository = ingredientRepository;
        _pantryRepository = pantryRepository;
    }

    // Listar toda la despensa
    public Task<List<PantryItem>> GetAllPantryItemsAsync(CancellationToken cancellationToken = default)
    {
        return _pantryRepository.GetAllAsync(cancellationToken);
    }

    // Agregar un ingrediente a la despensa
    public async Task<PantryItem> AddPantryItemAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var existingPantryItem = await _pantryRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (existingPantryItem is not null)
        {
            return existingPantryItem;
        }

        var ingredient = await GetOrFetchIngredientAsync(externalId, cancellationToken);
        var pantryItem = new PantryItem { IngredientId = ingredient.Id, Ingredient = ingredient };

        return await _pantryRepository.AddAsync(pantryItem, cancellationToken);
    }

    // Actualizar la cantidad habitual de compra
    public async Task<PantryItem?> UpdatePantryItemAsync(
        int externalId, decimal? usualQuantity, string? usualUnit, CancellationToken cancellationToken = default)
    {
        var pantryItem = await _pantryRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (pantryItem is null)
        {
            return null;
        }

        pantryItem.UsualQuantity = usualQuantity;
        pantryItem.UsualUnit = usualUnit;
        await _pantryRepository.UpdateAsync(pantryItem, cancellationToken);

        return pantryItem;
    }

    // Quitar un ingrediente de la despensa
    public async Task RemovePantryItemAsync(int externalId, CancellationToken cancellationToken = default)
    {
        var pantryItem = await _pantryRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (pantryItem is not null)
        {
            await _pantryRepository.RemoveAsync(pantryItem, cancellationToken);
        }
    }

    // Reutilizar el ingrediente ya guardado en memoria o guardarlo por primera vez
    private async Task<Ingredient> GetOrFetchIngredientAsync(int externalId, CancellationToken cancellationToken)
    {
        var cachedIngredient = await _ingredientRepository.GetByExternalIdAsync(externalId, cancellationToken);
        if (cachedIngredient is not null)
        {
            return cachedIngredient;
        }

        var detail = await _spoonacularClient.GetIngredientInformationAsync(externalId, cancellationToken);
        var ingredient = new Ingredient
        {
            ExternalId = detail.Id,
            Name = detail.Name,
            ImageUrl = SpoonacularClient.BuildIngredientImageUrl(detail.Image)
        };

        return await _ingredientRepository.AddAsync(ingredient, cancellationToken);
    }
}
