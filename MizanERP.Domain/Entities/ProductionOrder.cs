using MizanERP.Domain.Common;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Domain.Entities
{
    // Aggregate Root
    public class ProductionOrder : BaseEntity
    {
        public Guid ProductId { get; private set; } // Finished product
        public Quantity Quantity { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public List<ProductionOrderLine> Lines { get; private set; } // Raw materials

        public ProductionOrder(Guid id, Guid productId, Quantity quantity, DateTime orderDate)
        {
            if (quantity == null || quantity.Value <= 0) throw new ArgumentException("Quantity must be positive");
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            OrderDate = orderDate;
            Status = OrderStatus.Draft;
            Lines = new List<ProductionOrderLine>();
        }

        public void AddLine(ProductionOrderLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            Lines.Add(line);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Submit()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be submitted.");
            Status = OrderStatus.Submitted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve()
        {
            if (Status != OrderStatus.Submitted)
                throw new InvalidOperationException("Only submitted orders can be approved.");
            Status = OrderStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Approved)
                throw new InvalidOperationException("Only approved orders can be completed.");
            Status = OrderStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Completed orders cannot be cancelled.");
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class ProductionOrderLine : BaseEntity
    {
        public Guid ProductionOrderId { get; private set; }
        public Guid RawMaterialProductId { get; private set; }
        public Quantity Quantity { get; private set; }

        public ProductionOrderLine(Guid id, Guid productionOrderId, Guid rawMaterialProductId, Quantity quantity)
        {
            if (quantity == null || quantity.Value <= 0) throw new ArgumentException("Quantity must be positive");
            Id = id;
            ProductionOrderId = productionOrderId;
            RawMaterialProductId = rawMaterialProductId;
            Quantity = quantity;
        }
    }
}