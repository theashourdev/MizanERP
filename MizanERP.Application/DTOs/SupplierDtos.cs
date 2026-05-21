using MizanERP.Domain.ValueObjects;

namespace MizanERP.Application.DTOs
{
    public class AddressDto
    {
        public string Line1 { get; set; } = string.Empty;
        public string? Line2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new AddressDto();
        public string? ContactInfo { get; set; }
    }

    public class SupplierEditDto : CreateSupplierDto
    {
        public Guid Id { get; set; }
    }
}
