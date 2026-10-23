using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Models;
using Stocked.Api.Services.Inventory;
using Stocked.Api.Services.Recipes;
using Stocked.Api.Services.Spoonacular;

namespace Stocked.Api.Controllers;

[Authorize]
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

    // Buscar recetas por texto libre, con filtros opcionales de cocina/dieta/tipo de comida
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RecipeSearchResultDto>>> Search(
        [FromQuery] string? query, [FromQuery] string? cuisine, [FromQuery] string? diet, [FromQuery] string? type)
    {
        if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(cuisine)
            && string.IsNullOrWhiteSpace(diet) && string.IsNullOrWhiteSpace(type))
        {
            return BadRequest("Se requiere 'query' o al menos un filtro (cuisine/diet/type).");
        }

        var results = await _spoonacularClient.SearchRecipesAsync(query ?? string.Empty, cuisine, diet, type);

        var searchResults = results.Results.Select(result => new RecipeSearchResultDto
        {
            ExternalId = result.Id,
            Name = result.Title,
            ImageUrl = result.Image ?? string.Empty,
            ReadyInMinutes = result.ReadyInMinutes,
            DishTypes = result.DishTypes?.ToArray() ?? [],
            Cuisines = result.Cuisines?.ToArray() ?? []
        });

        return Ok(searchResults);
    }

    // Recetas al azar para no arrancar la pantalla de búsqueda vacía
    [AllowAnonymous]
    [HttpGet("random")]
    public async Task<ActionResult<IEnumerable<RecipeSearchResultDto>>> GetRandom()
    {
        // Se piden algunas de más porque se descartan las que no traen imagen
        var results = await _spoonacularClient.GetRandomRecipesAsync(15);

        var randomResults = results.Recipes
            .Where(result => !string.IsNullOrWhiteSpace(result.Image))
            .Take(10)
            .Select(result => new RecipeSearchResultDto
            {
                ExternalId = result.Id,
                Name = result.Title,
                ImageUrl = result.Image ?? string.Empty,
                ReadyInMinutes = result.ReadyInMinutes,
                DishTypes = result.DishTypes?.ToArray() ?? [],
                Cuisines = result.Cuisines?.ToArray() ?? []
            });

        return Ok(randomResults);
    }

    // Ver el detalle completo de una receta
    [HttpGet("{externalId:int}")]
    public async Task<ActionResult<RecipeDetailDto>> GetDetail(int externalId)
    {
        var recipe = await _recipeService.GetOrFetchRecipeAsync(externalId);
        var isFavorite = await _recipeService.IsFavoriteAsync(this.GetUserId(), recipe.Id);

        return Ok(MapToDetailDto(recipe, isFavorite));
    }

    // Marcar una receta como favorita
    [HttpPost("{externalId:int}/favorite")]
    public async Task<IActionResult> AddFavorite(int externalId)
    {
        await _recipeService.AddFavoriteAsync(this.GetUserId(), externalId);
        return NoContent();
    }

    // Quitar una receta de favoritas
    [HttpDelete("{externalId:int}/favorite")]
    public async Task<IActionResult> RemoveFavorite(int externalId)
    {
        await _recipeService.RemoveFavoriteAsync(this.GetUserId(), externalId);
        return NoContent();
    }

    // Listar las recetas favoritas, con indicador de si se pueden cocinar con la despensa actual
    [HttpGet("favorites")]
    public async Task<ActionResult<IEnumerable<RecipeWithMatchDto>>> GetFavorites()
    {
        var userId = this.GetUserId();
        var recipes = await _recipeService.GetFavoriteRecipesAsync(userId);
        var matches = await _recipeService.GetMatchesAsync(userId, recipes);

        return Ok(recipes.Select(recipe => MapToMatchDto(recipe, matches[recipe.Id])));
    }

    // Listar las recetas vistas recientemente, con indicador de si se pueden cocinar
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<RecipeWithMatchDto>>> GetRecent()
    {
        var userId = this.GetUserId();
        var recipes = await _recipeService.GetRecentRecipesAsync();
        var matches = await _recipeService.GetMatchesAsync(userId, recipes);

        return Ok(recipes.Select(recipe => MapToMatchDto(recipe, matches[recipe.Id])));
    }

    private static RecipeWithMatchDto MapToMatchDto(Recipe recipe, RecipeMatch match)
    {
        return new RecipeWithMatchDto
        {
            ExternalId = recipe.ExternalId,
            Name = recipe.Name,
            ImageUrl = recipe.ImageUrl,
            CanMake = match.CanMake,
            MissingIngredientsCount = match.MissingIngredientsCount,
            ReadyInMinutes = recipe.ReadyInMinutes,
            DishTypes = recipe.DishTypes,
            Diets = recipe.Diets,
            Cuisines = recipe.Cuisines
        };
    }

    private static RecipeDetailDto MapToDetailDto(Recipe recipe, bool isFavorite)
    {
        return new RecipeDetailDto
        {
            ExternalId = recipe.ExternalId,
            Name = recipe.Name,
            Summary = recipe.Summary,
            ImageUrl = recipe.ImageUrl,
            ReadyInMinutes = recipe.ReadyInMinutes,
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
