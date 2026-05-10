namespace MizanERP.Application.DTOs.Auth;

// ─── Login ───────────────────────────────────────────────
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public UserSessionDto User { get; set; } = null!;
}

public class UserSessionDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

// ─── Register ─────────────────────────────────────────────
public class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ─── Refresh Token ────────────────────────────────────────
public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

// ─── Revoke Token ─────────────────────────────────────────
public class RevokeTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

// ─── Change Password ──────────────────────────────────────
public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

// ─── Forgot Password ──────────────────────────────────────
public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

// ─── Reset Password ───────────────────────────────────────
public class ResetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

// ─── Email Verification ───────────────────────────────────
public class VerifyEmailRequestDto
{
    public string UserId { get; set; } = string.Empty;
    //public string? Token { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}