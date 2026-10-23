using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stocked.Api.DTOs;
using Stocked.Api.Services.Auth;

namespace Stocked.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // Registrar un usuario nuevo
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (result is null)
        {
            return Conflict("Ya existe una cuenta con ese correo.");
        }

        return Ok(MapToDto(result));
    }

    // Iniciar sesión
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        if (result is null)
        {
            return Unauthorized("Correo o contraseña incorrectos.");
        }

        return Ok(MapToDto(result));
    }

    // Guardar el día/hora de despensa
    [Authorize]
    [HttpPut("pantry-schedule")]
    public async Task<ActionResult<AuthResponseDto>> UpdatePantrySchedule([FromBody] UpdatePantryScheduleRequestDto request)
    {
        var result = await _authService.UpdatePantryScheduleAsync(this.GetUserId(), request.DayOfWeek, request.Time);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(result));
    }

    // Convertir el resultado del servicio en el contrato público de la API
    private static AuthResponseDto MapToDto(AuthResult result)
    {
        return new AuthResponseDto
        {
            Token = result.Token,
            Username = result.Username,
            Email = result.Email,
            PantryDayOfWeek = result.PantryDayOfWeek,
            PantryTime = result.PantryTime
        };
    }
}
