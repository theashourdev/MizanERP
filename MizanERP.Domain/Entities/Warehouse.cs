using System;
using MizanERP.Domain.Common;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; private set; }
        public Address? Address { get; private set; }
        public bool IsActive { get; private set; }

        public Warehouse(Guid id, string name, Address? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Warehouse name is required");
            Id = id;
            Name = name;
            Address = address;
            IsActive = true;
        }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    }
}