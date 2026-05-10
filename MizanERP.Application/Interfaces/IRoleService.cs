using MizanERP.Application.Common;
using MizanERP.Application.DTOs.Roles;

namespace MizanERP.Application.Interfaces;

public interface IRoleService
{
    Task<ApiResponse<List<RoleDto>>> GetAllAsync();
    Task<ApiResponse<RoleDto>> GetByIdAsync(string id);
    Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto dto);
    Task<ApiResponse<RoleDto>> UpdateAsync(string id, UpdateRoleDto dto);
    Task<ApiResponse> DeleteAsync(string id);
}