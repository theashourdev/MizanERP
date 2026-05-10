using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MizanERP.Application.Common;
using MizanERP.Application.DTOs.Users;
using MizanERP.Application.Interfaces;
using MizanERP.Domain.Entities;

namespace MizanERP.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    // ─── Get All (Paged) ──────────────────────────────────────────────────────
    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(UserQueryDto query)
    {
        var usersQuery = _userManager.Users
            .Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            usersQuery = usersQuery.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email!.ToLower().Contains(search));
        }


        var totalCount = await usersQuery.CountAsync();

        var users = await usersQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Map to DTOs + load roles
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (query.Role != null && !roles.Contains(query.Role))
                continue;

            userDtos.Add(MapToDto(user, roles));
        }

        return ApiResponse<PagedResult<UserDto>>.Ok(
            PagedResult<UserDto>.Create(userDtos, totalCount, query.Page, query.PageSize));
    }

    // ─── Get By Id ────────────────────────────────────────────────────────────
    public async Task<ApiResponse<UserDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || user.IsDeleted)
            return ApiResponse<UserDto>.Fail("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(MapToDto(user, roles));
    }

    // ─── Create ───────────────────────────────────────────────────────────────
    public async Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return ApiResponse<UserDto>.Fail("Email is already in use.");

        var tempPassword = dto.Password;

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, tempPassword);

        if (!result.Succeeded)
            return ApiResponse<UserDto>.Fail("Failed to create user.", result.Errors.Select(e => e.Description).ToList());

        // Assign roles
        foreach (var role in dto.Roles)
            await _userManager.AddToRoleAsync(user, role);

        if (dto.SendWelcomeEmail)
            await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName, tempPassword);

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(MapToDto(user, roles), "User created successfully.");
    }

    // ─── Update ───────────────────────────────────────────────────────────────
    public async Task<ApiResponse<UserDto>> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || user.IsDeleted)
            return ApiResponse<UserDto>.Fail("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.ProfilePicture = dto.ProfilePicture;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return ApiResponse<UserDto>.Fail("Failed to update user.", result.Errors.Select(e => e.Description).ToList());

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(MapToDto(user, roles), "User updated successfully.");
    }

    // ─── Soft Delete ──────────────────────────────────────────────────────────
    public async Task<ApiResponse> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        user.Email = $"deleted_{user.Email}"; // avoid unique constraint collision
        user.UserName = user.Email;

        await _userManager.UpdateAsync(user);
        return ApiResponse.Ok("User deleted successfully.");
    }

    // ─── Lock ─────────────────────────────────────────────────────────────────
    public async Task<ApiResponse> LockAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        await _userManager.UpdateAsync(user);

        return ApiResponse.Ok("User account locked.");
    }

    // ─── Unlock ───────────────────────────────────────────────────────────────
    public async Task<ApiResponse> UnlockAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.UpdateAsync(user);

        return ApiResponse.Ok("User account unlocked.");
    }

    // ─── Assign Role ──────────────────────────────────────────────────────────
    public async Task<ApiResponse> AssignRoleAsync(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        if (await _userManager.IsInRoleAsync(user, roleName))
            return ApiResponse.Fail($"User already has role '{roleName}'.");

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
            return ApiResponse.Fail("Failed to assign role.", result.Errors.Select(e => e.Description).ToList());

        return ApiResponse.Ok($"Role '{roleName}' assigned successfully.");
    }

    // ─── Remove Role ──────────────────────────────────────────────────────────
    public async Task<ApiResponse> RemoveRoleAsync(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || !user.IsActive)
            return ApiResponse.Fail("User not found.");

        if (!await _userManager.IsInRoleAsync(user, roleName))
            return ApiResponse.Fail($"User does not have role '{roleName}'.");

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
            return ApiResponse.Fail("Failed to remove role.", result.Errors.Select(e => e.Description).ToList());

        return ApiResponse.Ok($"Role '{roleName}' removed successfully.");
    }

    // ─── Private Mapping ──────────────────────────────────────────────────────
    private static UserDto MapToDto(ApplicationUser user, IList<string> roles) => new()
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = user.FullName,
        Email = user.Email!,
        PhoneNumber = user.PhoneNumber,
        ProfilePicture = user.ProfilePicture,
        EmailConfirmed = user.EmailConfirmed,
        Roles = roles.ToList(),
        CreatedAt = user.CreatedAt,
    };
}
