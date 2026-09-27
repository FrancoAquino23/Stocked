using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Services.Spoonacular;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientController : ControllerBase
{
    private readonly SpoonacularClient _spoonacularClient;

    public IngredientController(SpoonacularClient spoonacularClient)
    {
        _spoonacularClient = spoonacularClient;
    }

    // Autocompletar ingredientes por texto libre
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<IngredientSearchResultDto>>> Search([FromQuery] string query)
    {
        var results = await _spoonacularClient.SearchIngredientsAsync(query);

        var searchResults = results.Select(result => new IngredientSearchResultDto
        {
            ExternalId = result.Id,
            Name = result.Name,
            ImageUrl = SpoonacularClient.BuildIngredientImageUrl(result.Image)
        });

        return Ok(searchResults);
    }
}
