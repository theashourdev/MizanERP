using System;

namespace MizanERP.Domain.ValueObjects
{
    public sealed class Quantity : IEquatable<Quantity>
    {
        public decimal Value { get; }
        public string Unit { get; }

        public Quantity(decimal value, string unit)
        {
            if (value < 0) throw new ArgumentException("Quantity cannot be negative");
            if (string.IsNullOrWhiteSpace(unit)) throw new ArgumentException("Unit is required");
            Value = value;
            Unit = unit;
        }

        public Quantity Add(Quantity other)
        {
            EnsureSameUnit(other);
            return new Quantity(Value + other.Value, Unit);
        }

        public Quantity Subtract(Quantity other)
        {
            EnsureSameUnit(other);
            var result = Value - other.Value;
            if (result < 0) throw new InvalidOperationException("Resulting quantity cannot be negative");
            return new Quantity(result, Unit);
        }

        private void EnsureSameUnit(Quantity other)
        {
            if (Unit != other.Unit)
                throw new InvalidOperationException("Unit mismatch");
        }

        public override bool Equals(object? obj) => Equals(obj as Quantity);
        public bool Equals(Quantity? other) => other != null && Value == other.Value && Unit == other.Unit;
        public override int GetHashCode() => HashCode.Combine(Value, Unit);
    }
}