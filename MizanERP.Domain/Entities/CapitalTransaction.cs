using MizanERP.Domain.Common;

namespace MizanERP.Domain.Entities
{
    public class CapitalTransaction : BaseEntity
    {
        public DateTime Date { get; private set; }

        public decimal Amount { get; private set; }

        public decimal BalanceBefore { get; private set; }

        public decimal BalanceAfter { get; private set; }

        public TransactionType Type { get; private set; }

        public string Currency { get; private set; }

        public string? Description { get; private set; }

        public Guid? PurchaseOrderId { get; private set; }

        private CapitalTransaction() { }

        public CapitalTransaction(
            Guid id,
            DateTime date,
            decimal amount,
            decimal balanceBefore,
            decimal balanceAfter,
            TransactionType type,
            string currency,
            string? description = null,
            Guid? purchaseOrderId = null)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            Id = id;
            Date = date;
            Amount = amount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
            Type = type;
            Currency = currency;
            Description = description;
            PurchaseOrderId = purchaseOrderId;
        }
    }
    public enum TransactionType
    {
        OpeningBalance = 1,
        Deposit = 2,
        Purchase = 3,
        Sale = 4,
        Expense = 5,
        Refund = 6
    }
}
