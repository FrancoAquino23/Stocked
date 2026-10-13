namespace Stocked.Api.Services.Auth;

// Configuración para firmar y validar los JWT de autenticación
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Stocked";
    public string Audience { get; set; } = "Stocked";
    public int ExpiryDays { get; set; } = 7;
}
