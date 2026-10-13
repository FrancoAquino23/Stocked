using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Stocked.Api.Controllers;

public static class ControllerBaseExtensions
{
    // Leer el ID del usuario autenticado desde el JWT de la petición
    public static int GetUserId(this ControllerBase controller)
    {
        var claim = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(claim!);
    }
}
