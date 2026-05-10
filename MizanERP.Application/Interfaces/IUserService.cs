using Microsoft.AspNetCore.Identity;
using MizanERP.Application.Common;
using MizanERP.Application.DTOs.Users;
using MizanERP.Domain.Entities;

namespace MizanERP.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(UserQueryDto query);
    Task<ApiResponse<UserDto>> GetByIdAsync(string id);
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto);
    Task<ApiResponse<UserDto>> UpdateAsync(string id, UpdateUserDto dto);
    Task<ApiResponse> DeleteAsync(string id);
    Task<ApiResponse> LockAsync(string id);
    Task<ApiResponse> UnlockAsync(string id);
    Task<ApiResponse> AssignRoleAsync(string id, string roleName);
    Task<ApiResponse> RemoveRoleAsync(string id, string roleName);
    IdentityResult VerifyOTP(ApplicationUser user, string otpCode);
}