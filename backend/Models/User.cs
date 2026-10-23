namespace Stocked.Api.Models;

// Modelo: Usuario
public class User
{
    public int Id { get; set; }

    // Solo para mostrarse en el perfil, no se usa para iniciar sesión
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Día/hora en que el usuario suele hacer despensa
    public DayOfWeek? PantryDayOfWeek { get; set; }
    public TimeOnly? PantryTime { get; set; }
}
