using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MizanERP.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate JWT token for authentication
        /// </summary>
        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { error = "Username is required" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                //new Claim(ClaimTypes.Role, request.Role ?? "Admin")
            };

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings.GetValue<string>("Secret") ?? "DefaultSecretKeyForDevelopmentOnly123!";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "MizanERP",
                audience: jwtSettings["Audience"] ?? "MizanERPUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = 7200
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //public string? Role { get; set; }
    }
}