using System.ComponentModel.DataAnnotations;

namespace Stocked.Api.DTOs;

// DTO: Petición de login
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
