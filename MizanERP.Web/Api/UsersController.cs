using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MizanERP.Application.DTOs.Users;
using MizanERP.Application.Interfaces;

namespace MizanERP.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin,SuperAdmin")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // ─── GET /api/users ────────────────────────────────────────────────────
    /// <summary>Get all users (paged, searchable)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryDto query)
    {
        var result = await _userService.GetAllAsync(query);
        return Ok(result);
    }

    // ─── GET /api/users/{id} ───────────────────────────────────────────────
    /// <summary>Get user by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // ─── POST /api/users ───────────────────────────────────────────────────
    /// <summary>Create a new user (Admin only)</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    // ─── PUT /api/users/{id} ───────────────────────────────────────────────
    /// <summary>Update user info</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── DELETE /api/users/{id} ────────────────────────────────────────────
    /// <summary>Soft-delete a user</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/users/{id}/lock ─────────────────────────────────────────
    /// <summary>Lock a user account</summary>
    [HttpPost("{id}/lock")]
    public async Task<IActionResult> Lock(string id)
    {
        var result = await _userService.LockAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/users/{id}/unlock ───────────────────────────────────────
    /// <summary>Unlock a user account</summary>
    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> Unlock(string id)
    {
        var result = await _userService.UnlockAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── POST /api/users/{id}/assign-role ──────────────────────────────────
    /// <summary>Assign a role to a user</summary>
    [HttpPost("{id}/assign-role")]
    public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleDto dto)
    {
        var result = await _userService.AssignRoleAsync(id, dto.RoleName);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── DELETE /api/users/{id}/remove-role ────────────────────────────────
    /// <summary>Remove a role from a user</summary>
    [HttpDelete("{id}/remove-role")]
    public async Task<IActionResult> RemoveRole(string id, [FromBody] AssignRoleDto dto)
    {
        var result = await _userService.RemoveRoleAsync(id, dto.RoleName);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
