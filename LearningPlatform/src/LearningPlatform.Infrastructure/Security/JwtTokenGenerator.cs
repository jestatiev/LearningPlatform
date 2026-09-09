using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LearningPlatform.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    public JwtTokenGenerator(IConfiguration config) => _config = config;

    public (string token, DateTime expiresAt) GenerateToken(User user, IEnumerable<string> roles)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key nije postavljen u konfiguraciji.");
        var expiresMinutes = int.Parse(jwtSection["ExpiresInMinutes"] ?? "120");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("uid", user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
