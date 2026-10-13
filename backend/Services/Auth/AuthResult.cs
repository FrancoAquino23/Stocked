namespace Stocked.Api.Services.Auth;

// Resultado de un login/registro exitoso
public record AuthResult(string Token, string Username, string Email);
