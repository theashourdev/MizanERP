using System;
using System.Collections.Generic;
using MizanERP.Domain.Common;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Domain.Entities
{
    // Aggregate Root
    public class PurchaseOrder : BaseEntity
    {
        public Guid SupplierId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public List<PurchaseOrderLine> Lines { get; private set; }

        public PurchaseOrder(Guid id, Guid supplierId, DateTime orderDate)
        {
            Id = id;
            SupplierId = supplierId;
            OrderDate = orderDate;
            Status = OrderStatus.Draft;
            Lines = new List<PurchaseOrderLine>();
        }

        public void AddLine(PurchaseOrderLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            Lines.Add(line);
            UpdatedAt = DateTime.UtcNow;
        }

        public Money GetTotal()
        {
            if (Lines.Count == 0) return new Money(0, "USD"); // Default currency
            var currency = Lines[0].Price.Currency;
            decimal total = 0;
            foreach (var line in Lines)
            {
                if (line.Price.Currency != currency)
                    throw new InvalidOperationException("All lines must have the same currency");
                total += line.Price.Amount * line.Quantity.Value;
            }
            return new Money(total, currency);
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

        public void Receive()
        {
            if (Status != OrderStatus.Approved)
                throw new InvalidOperationException("Only approved orders can be received.");
            Status = OrderStatus.Received;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Received)
                throw new InvalidOperationException("Only received orders can be completed.");
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

    public class PurchaseOrderLine : BaseEntity
    {
        public Guid PurchaseOrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money Price { get; private set; }

        public PurchaseOrderLine(Guid id, Guid purchaseOrderId, Guid productId, Quantity quantity, Money price)
        {
            if (quantity == null || quantity.Value <= 0) throw new ArgumentException("Quantity must be positive");
            if (price == null || price.Amount < 0) throw new ArgumentException("Price cannot be negative");
            Id = id;
            PurchaseOrderId = purchaseOrderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
    }
}