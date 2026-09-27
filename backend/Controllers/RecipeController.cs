using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Recipes;
using Stocked.Api.Services.Spoonacular;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController : ControllerBase
{
    private readonly SpoonacularClient _spoonacularClient;
    private readonly RecipeService _recipeService;

    public RecipeController(SpoonacularClient spoonacularClient, RecipeService recipeService)
    {
        _spoonacularClient = spoonacularClient;
        _recipeService = recipeService;
    }

    // Buscar recetas por texto libre
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RecipeSearchResultDto>>> Search([FromQuery] string query)
    {
        var results = await _spoonacularClient.SearchRecipesAsync(query);

        var searchResults = results.Results.Select(result => new RecipeSearchResultDto
        {
            ExternalId = result.Id,
            Name = result.Title,
            ImageUrl = result.Image ?? string.Empty
        });

        return Ok(searchResults);
    }

    // Ver el detalle completo de una receta
    [HttpGet("{externalId:int}")]
    public async Task<ActionResult<RecipeDetailDto>> GetDetail(int externalId)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId);
        var isFavorite = await _recipeService.IsFavoriteAsync(recipe.Id);

        return Ok(MapToDetailDto(recipe, isFavorite));
    }

    // Marcar una receta como favorita
    [HttpPost("{externalId:int}/favorite")]
    public async Task<IActionResult> AddFavorite(int externalId)
    {
        await _recipeService.AddFavoriteAsync(externalId);
        return NoContent();
    }

    // Quitar una receta de favoritas
    [HttpDelete("{externalId:int}/favorite")]
    public async Task<IActionResult> RemoveFavorite(int externalId)
    {
        await _recipeService.RemoveFavoriteAsync(externalId);
        return NoContent();
    }

    // Listar las recetas favoritas
    [HttpGet("favorites")]
    public async Task<ActionResult<IEnumerable<RecipeSearchResultDto>>> GetFavorites()
    {
        var recipes = await _recipeService.GetFavoriteRecipesAsync();

        var favorites = recipes.Select(recipe => new RecipeSearchResultDto
        {
            ExternalId = recipe.ExternalId,
            Name = recipe.Name,
            ImageUrl = recipe.ImageUrl
        });

        return Ok(favorites);
    }

    private static RecipeDetailDto MapToDetailDto(Recipe recipe, bool isFavorite)
    {
        return new RecipeDetailDto
        {
            ExternalId = recipe.ExternalId,
            Name = recipe.Name,
            Summary = recipe.Summary,
            ImageUrl = recipe.ImageUrl,
            DishTypes = recipe.DishTypes,
            Diets = recipe.Diets,
            Cuisines = recipe.Cuisines,
            IsFavorite = isFavorite,
            Steps = [.. RecipeService.ParseSteps(recipe.Instructions)
                .Select(step => new RecipeStepDto { Number = step.Number, Description = step.Step })],
            Ingredients = [.. recipe.RecipeIngredients.Select(recipeIngredient => new RecipeIngredientDto
            {
                Name = recipeIngredient.Ingredient.Name,
                ImageUrl = recipeIngredient.Ingredient.ImageUrl,
                Quantity = recipeIngredient.Quantity,
                Unit = recipeIngredient.Unit
            })]
        };
    }
}
