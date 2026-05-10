
using Microsoft.AspNetCore.Identity;

namespace MizanERP.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? DomainUserId { get; set; }

        public string? ProfilePicture { get; set; }

        public bool IsDeleted { get; protected set; }

        public void MarkDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
