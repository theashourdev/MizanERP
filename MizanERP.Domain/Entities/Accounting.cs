using MizanERP.Domain.Common;
using MizanERP.Domain.Enums;

namespace MizanERP.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string Name { get; private set; }
        public AccountType Type { get; private set; }
        public bool IsActive { get; private set; }

        public Account(Guid id, string name, AccountType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name is required");
            Id = id;
            Name = name;
            Type = type;
            IsActive = true;
        }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    }

    public class AccountingEntry : BaseEntity
    {
        public DateTime Date { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Debit { get; private set; }
        public decimal Credit { get; private set; }
        public string? Reference { get; private set; }

        public AccountingEntry(Guid id, DateTime date, Guid accountId, decimal debit, decimal credit, string? reference = null)
        {
            if (debit < 0) throw new ArgumentException("Debit cannot be negative");
            if (credit < 0) throw new ArgumentException("Credit cannot be negative");
            if (debit == 0 && credit == 0) throw new ArgumentException("Either debit or credit must be non-zero");
            Id = id;
            Date = date;
            AccountId = accountId;
            Debit = debit;
            Credit = credit;
            Reference = reference;
        }
    }
}