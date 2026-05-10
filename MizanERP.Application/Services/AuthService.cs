using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MizanERP.Application.Common;
using MizanERP.Application.DTOs.Auth;
using MizanERP.Application.Interfaces;
using MizanERP.Domain.Entities;

namespace MizanERP.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IEmailService emailService,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    // ─── Login ────────────────────────────────────────────────────────────────
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.Fail("Your account is inactive.");

        if (!user.EmailConfirmed)
            return ApiResponse<LoginResponseDto>.Fail("Please verify your email before logging in.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            await _userManager.UpdateAsync(user);
            await _emailService.SendAccountLockedNotificationAsync(user.Email!, user.FullName);
            return ApiResponse<LoginResponseDto>.Fail("Account locked due to too many failed attempts.");
        }

        if (!result.Succeeded)
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        await _userManager.UpdateAsync(user);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                _configuration.GetSection("JwtSettings").GetValue<int>("AccessTokenMinutes", 15)),
            User = new UserSessionDto
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email!,
                Roles = roles.ToList()
            }
        }, "Login successful.");
    }

    // ─── Register ─────────────────────────────────────────────────────────────
    public async Task<ApiResponse> RegisterAsync(RegisterRequestDto request)
    {
        using var transaction = _unitOfWork.BeginTransactionAsync();

        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return ApiResponse.Fail("Email is already registered.");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return ApiResponse.Fail("Registration failed.", result.Errors.Select(e => e.Description).ToList());

            var defaultRole = _configuration["AppSettings:DefaultRole"] ?? "TestUser";

            var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
                return ApiResponse.Fail("Failed to assign role.", roleResult.Errors.Select(e => e.Description).ToList());

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var clientUrl = _configuration["AppSettings:ClientUrl"];
            var verificationLink =
                $"{clientUrl}/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailVerificationAsync(
                user.Email!,
                user.FullName,
                verificationLink);

            await _unitOfWork.CommitAsync();

            return ApiResponse.Ok("Registration successful. Please check your email.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return ApiResponse.Fail("Something went wrong", ex.Message);
        }
    }


    // ─── Forgot Password ──────────────────────────────────────────────────────
    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Always return OK to avoid user enumeration
        if (user == null || !user.IsActive)
            return ApiResponse.Ok("If this email exists, a reset link has been sent.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var clientUrl = _configuration["AppSettings:ClientUrl"];
        var resetLink = $"{clientUrl}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendPasswordResetAsync(user.Email!, user.FullName, resetLink);

        // Also send SMS OTP if phone exists
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            var otp = GenerateOtp();
            //await _smsService.SendPasswordResetOtpAsync(user.PhoneNumber, otp);
            // Note: store OTP in cache/db for verification in real implementation
        }

        return ApiResponse.Ok("If this email exists, a reset link has been sent.");
    }

    // ─── Reset Password ───────────────────────────────────────────────────────
    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("Invalid request.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
            return ApiResponse.Fail("Password reset failed.", result.Errors.Select(e => e.Description).ToList());

        // Unlock if was locked
        if (await _userManager.IsLockedOutAsync(user))
            await _userManager.SetLockoutEndDateAsync(user, null);


        await _userManager.UpdateAsync(user);



        await _emailService.SendPasswordChangedNotificationAsync(user.Email!, user.FullName);

        return ApiResponse.Ok("Password reset successfully. Please login with your new password.");
    }

    // ─── Change Password ──────────────────────────────────────────────────────
    public async Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return ApiResponse.Fail("Failed to change password.", result.Errors.Select(e => e.Description).ToList());

        await _emailService.SendPasswordChangedNotificationAsync(user.Email!, user.FullName);

        return ApiResponse.Ok("Password changed successfully.");
    }

    // ─── Verify Email ─────────────────────────────────────────────────────────
    public async Task<ApiResponse> VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("Invalid verification link.");

        if (user.EmailConfirmed)
            return ApiResponse.Ok("Email already verified.");

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
            return ApiResponse.Fail("Email verification failed. Link may have expired.");

        await _userManager.UpdateAsync(user);

        return ApiResponse.Ok("Email verified successfully. You can now login.");
    }

    // ─── Resend Verification Email ────────────────────────────────────────────
    public async Task<ApiResponse> ResendVerificationEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !user.IsActive || user.EmailConfirmed)
            return ApiResponse.Ok("If applicable, a verification email has been sent.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var clientUrl = _configuration["AppSettings:ClientUrl"];
        var verificationLink = $"{clientUrl}/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, verificationLink);

        return ApiResponse.Ok("Verification email sent.");
    }

    private static string GenerateOtp(int length = 6)
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, length).Select(_ => random.Next(0, 10).ToString()));
    }
}
