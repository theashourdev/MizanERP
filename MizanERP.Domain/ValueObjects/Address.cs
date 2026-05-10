namespace MizanERP.Domain.ValueObjects
{
    public sealed class Address : IEquatable<Address>
    {
        public string Line1 { get; }
        public string? Line2 { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public string Country { get; }

        private Address() { }

        public Address(string line1, string city, string state, string postalCode, string country, string? line2 = null)
        {
            if (string.IsNullOrWhiteSpace(line1)) throw new ArgumentException("Address Line1 is required");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required");
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State is required");
            if (string.IsNullOrWhiteSpace(postalCode)) throw new ArgumentException("PostalCode is required");
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required");
            Line1 = line1;
            Line2 = line2;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public override bool Equals(object? obj) => Equals(obj as Address);
        public bool Equals(Address? other) => other != null &&
            Line1 == other.Line1 &&
            Line2 == other.Line2 &&
            City == other.City &&
            State == other.State &&
            PostalCode == other.PostalCode &&
            Country == other.Country;
        public override int GetHashCode() => HashCode.Combine(Line1, Line2, City, State, PostalCode, Country);
    }
}