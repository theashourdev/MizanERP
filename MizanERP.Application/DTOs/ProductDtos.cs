using MizanERP.Domain.Enums;

namespace MizanERP.Application.DTOs
{
    public class ProductDto
    {

        public string Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }
        public decimal InventoryQuantity { get; set; }

    }
}
