using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stocked.Api.Models;
using Stocked.Api.Repositories;

namespace Stocked.Api.Services.Auth;

public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(UserRepository userRepository, IOptions<JwtOptions> jwtOptions, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    // Registrar un usuario nuevo; null si el correo ya está en uso
    public async Task<AuthResult?> RegisterAsync(
        string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            _logger.LogWarning("Intento de registro con un correo ya en uso: {Email}", normalizedEmail);
            return null;
        }

        var user = new User { Username = username, Email = normalizedEmail, CreatedAt = DateTime.UtcNow };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _userRepository.AddAsync(user, cancellationToken);
        _logger.LogInformation("Usuario nuevo registrado: {UserId} ({Email})", user.Id, normalizedEmail);

        return new AuthResult(GenerateToken(user), user.Username, user.Email, user.PantryDayOfWeek, user.PantryTime);
    }

    // Iniciar sesión (null si el correo no existe o la contraseña no coincide)
    public async Task<AuthResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Intento de login con un correo que no existe: {Email}", normalizedEmail);
            return null;
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Intento de login con contraseña incorrecta: {UserId} ({Email})", user.Id, normalizedEmail);
            return null;
        }

        _logger.LogInformation("Login exitoso: {UserId} ({Email})", user.Id, normalizedEmail);
        return new AuthResult(GenerateToken(user), user.Username, user.Email, user.PantryDayOfWeek, user.PantryTime);
    }

    // Guardar el día/hora de despensa
    public async Task<AuthResult?> UpdatePantryScheduleAsync(
        int userId, DayOfWeek dayOfWeek, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.PantryDayOfWeek = dayOfWeek;
        user.PantryTime = time;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResult(GenerateToken(user), user.Username, user.Email, user.PantryDayOfWeek, user.PantryTime);
    }

    // Firmar un JWT con el ID del usuario como identificador
    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtOptions.ExpiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
