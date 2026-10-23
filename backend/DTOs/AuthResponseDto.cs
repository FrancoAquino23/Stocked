namespace Stocked.Api.DTOs;

// DTO: Respuesta de login/registro exitoso
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DayOfWeek? PantryDayOfWeek { get; set; }
    public TimeOnly? PantryTime { get; set; }
}
