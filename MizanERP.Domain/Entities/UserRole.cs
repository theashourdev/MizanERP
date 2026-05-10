using MizanERP.Domain.Common;

namespace MizanERP.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; private set; }
        public bool IsActive { get; private set; }
        public List<Role> Roles { get; private set; }

        private User() { UserName = string.Empty; Roles = new List<Role>(); }
        public User(Guid id, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name is required");
            Id = id;
            UserName = userName;
            IsActive = true;
            Roles = new List<Role>();
        }
        public void AddRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (!Roles.Contains(role))
                Roles.Add(role);
        }
        public void RemoveRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            Roles.Remove(role);
        }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    }

    public class Role : BaseEntity
    {
        public string Name { get; private set; }

        private Role() { Name = string.Empty; }
        public Role(Guid id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name is required");
            Id = id;
            Name = name;
        }
    }
}