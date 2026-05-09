using System;
using MizanERP.Domain.Common;
using MizanERP.Domain.Enums;

namespace MizanERP.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public ProductType Type { get; private set; }
        public string Unit { get; private set; } // e.g., kg, pcs
        public string? Code { get; private set; }
        public bool IsActive { get; private set; }
        public decimal InventoryQuantity { get; private set; }

        public Product(Guid id, string name, ProductType type, string unit, string? code = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required");
            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit is required");
            Id = id;
            Name = name;
            Type = type;
            Unit = unit;
            Code = code;
            IsActive = true;
            InventoryQuantity = 0;
        }

        public void IncreaseInventory(decimal quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Increase quantity must be positive");
            InventoryQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecreaseInventory(decimal quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Decrease quantity must be positive");
            if (InventoryQuantity < quantity) throw new InvalidOperationException("Insufficient inventory");
            InventoryQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    }
}