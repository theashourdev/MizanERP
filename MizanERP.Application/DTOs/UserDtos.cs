namespace MizanERP.Application.DTOs.Users;

// ─── User Read ────────────────────────────────────────────
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

// ─── Create User (by Admin) ───────────────────────────────
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool SendWelcomeEmail { get; set; } = true;
}

// ─── Update User ──────────────────────────────────────────
public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
}

// ─── Assign Role ──────────────────────────────────────────
public class AssignRoleDto
{
    public string RoleName { get; set; } = string.Empty;
}

// ─── User List Query ──────────────────────────────────────
public class UserQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }             // name or email
    public string? Role { get; set; }
}