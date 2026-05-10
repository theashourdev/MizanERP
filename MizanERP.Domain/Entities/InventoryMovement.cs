using MizanERP.Domain.Common;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Domain.Entities
{
    public class InventoryMovement : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Guid? FromWarehouseId { get; private set; }
        public Guid? ToWarehouseId { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money? Cost { get; private set; }
        public MovementType MovementType { get; private set; }
        public DateTime Date { get; private set; }
        public string? Reference { get; private set; } // e.g., PO number, SO number, etc.

        private InventoryMovement() { }

        public InventoryMovement(Guid id, Guid productId, Quantity quantity, MovementType movementType, DateTime date, Guid? fromWarehouseId = null, Guid? toWarehouseId = null, Money? cost = null, string? reference = null)
        {
            if (quantity == null || quantity.Value <= 0)
                throw new ArgumentException("Quantity must be positive");
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            MovementType = movementType;
            Date = date;
            FromWarehouseId = fromWarehouseId;
            ToWarehouseId = toWarehouseId;
            Cost = cost;
            Reference = reference;
        }
    }
}