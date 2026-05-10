using MizanERP.Application.Common;
using MizanERP.Application.DTOs.Auth;

namespace MizanERP.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress);
    Task<ApiResponse> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    Task<ApiResponse> VerifyEmailAsync(VerifyEmailRequestDto request);
    Task<ApiResponse> ResendVerificationEmailAsync(string email);
}