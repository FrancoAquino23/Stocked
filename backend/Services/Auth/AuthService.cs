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
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(UserRepository userRepository, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
    }

    // Registrar un usuario nuevo; null si el correo ya está en uso
    public async Task<AuthResult?> RegisterAsync(
        string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            return null;
        }

        var user = new User { Username = username, Email = normalizedEmail, CreatedAt = DateTime.UtcNow };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _userRepository.AddAsync(user, cancellationToken);

        return new AuthResult(GenerateToken(user), user.Username, user.Email);
    }

    // Iniciar sesión (null si el correo no existe o la contraseña no coincide)
    public async Task<AuthResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLowerInvariant(), cancellationToken);
        if (user is null)
        {
            return null;
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new AuthResult(GenerateToken(user), user.Username, user.Email);
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
