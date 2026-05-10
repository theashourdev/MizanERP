namespace MizanERP.Application.DTOs
{
    public class CreatePurchaseOrderDto
    {
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CreatePurchaseOrderLineDto> Lines { get; set; } = new();
    }
    public class CreatePurchaseOrderLineDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class CreateSalesOrderDto
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CreateSalesOrderLineDto> Lines { get; set; } = new();
    }
    public class CreateSalesOrderLineDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class CreateProductionOrderDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CreateProductionOrderLineDto> Lines { get; set; } = new();
    }
    public class CreateProductionOrderLineDto
    {
        public Guid RawMaterialProductId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class InventoryTransactionDto
    {
        public Guid ProductId { get; set; }
        public Guid? FromWarehouseId { get; set; }
        public Guid? ToWarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public DateTime Date { get; set; }
        public decimal? Cost { get; set; }
        public string? Currency { get; set; }
    }

    // Product DTOs
    public class CreateProductDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsRawMaterial { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
    }

    // Accounting DTOs
    public class AccountingTransactionDto
    {
        public Guid AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Reference { get; set; }
    }
}