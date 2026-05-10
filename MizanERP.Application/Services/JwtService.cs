using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MizanERP.Application.Interfaces;
using MizanERP.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MizanERP.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        var jwt = configuration.GetSection("JwtSettings");
        _secret = jwt["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");
        _issuer = jwt["Issuer"] ?? "MizanERP";
        _audience = jwt["Audience"] ?? "MizanERPUsers";
        _accessTokenMinutes = jwt.GetValue<int>("AccessTokenMinutes", 15);
    }

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new("fullName", user.FullName),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? GetUserIdFromExpiredToken(string accessToken)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false  // ← we allow expired here
            };

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(accessToken, tokenValidationParameters, out _);

            return principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }
        catch
        {
            return null;
        }
    }
}
