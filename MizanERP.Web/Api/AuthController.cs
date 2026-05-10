//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace MizanERP.Web.Api
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public AuthController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        /// <summary>
//        /// Generate JWT token for authentication
//        /// </summary>
//        [HttpPost("token")]
//        public IActionResult GenerateToken([FromBody] LoginRequest request)
//        {
//            if (string.IsNullOrWhiteSpace(request.Username))
//                return BadRequest(new { error = "Username is required" });

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.Name, request.Username),
//                //new Claim(ClaimTypes.Role, request.Role ?? "Admin")
//            };

//            var jwtSettings = _configuration.GetSection("JwtSettings");
//            var secret = jwtSettings.GetValue<string>("Secret") ?? "DefaultSecretKeyForDevelopmentOnly123!";
//            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: jwtSettings["Issuer"] ?? "MizanERP",
//                audience: jwtSettings["Audience"] ?? "MizanERPUsers",
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2),
//                signingCredentials: creds
//            );

//            return Ok(new
//            {
//                token = new JwtSecurityTokenHandler().WriteToken(token),
//                expiresIn = 7200
//            });
//        }
//    }

//    public class LoginRequest
//    {
//        public string Username { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//        //public string? Role { get; set; }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MizanERP.Application.DTOs.Auth;
using MizanERP.Application.Interfaces;

namespace MizanERP.Web.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    private string IpAddress =>
        Request.Headers.TryGetValue("X-Forwarded-For", out var ip)
            ? ip.FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private string CurrentUserId =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? string.Empty;

    // ─── POST /api/auth/login ──────────────────────────────────────────────
    /// <summary>Login and receive JWT access + refresh tokens</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request, IpAddress);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    // ─── POST /api/auth/register ───────────────────────────────────────────
    /// <summary>Register a new user account</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }



    // ─── POST /api/auth/forgot-password ────────────────────────────────────
    /// <summary>Send password reset link to email</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return Ok(result);  // Always 200 to prevent user enumeration
    }

    // ─── POST /api/auth/reset-password ─────────────────────────────────────
    /// <summary>Reset password using token from email</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/auth/change-password ────────────────────────────────────
    /// <summary>Change current user's password (requires login)</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var result = await _authService.ChangePasswordAsync(CurrentUserId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/auth/verify-email ───────────────────────────────────────
    /// <summary>Verify email address using token from email link</summary>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
    {
        var result = await _authService.VerifyEmailAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/auth/resend-verification ────────────────────────────────
    /// <summary>Resend email verification link</summary>
    [HttpPost("resend-verification")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordRequestDto request)
    {
        var result = await _authService.ResendVerificationEmailAsync(request.Email);
        return Ok(result);
    }
}
