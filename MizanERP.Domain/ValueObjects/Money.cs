using System;
using System.Globalization;

namespace MizanERP.Domain.ValueObjects
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required");
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount - other.Amount, Currency);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Currency mismatch");
        }

        public override bool Equals(object? obj) => Equals(obj as Money);
        public bool Equals(Money? other) => other != null && Amount == other.Amount && Currency == other.Currency;
        public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    }
}