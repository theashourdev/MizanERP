using MizanERP.Domain.Common;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; private set; }
        public Address? Address { get; private set; }
        public string? ContactInfo { get; private set; }
        public bool IsActive { get; private set; }
        private Customer() { Name = string.Empty; }


        public Customer(Guid id, string name, Address? address = null, string? contactInfo = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name is required");
            Id = id;
            Name = name;
            Address = address;
            ContactInfo = contactInfo;
            IsActive = true;
        }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    }
}