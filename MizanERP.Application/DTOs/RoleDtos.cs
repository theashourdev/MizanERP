namespace MizanERP.Application.DTOs.Roles;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSystem { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; } = false;
}

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}