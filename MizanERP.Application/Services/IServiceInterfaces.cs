using System;
using System.Threading.Tasks;
using MizanERP.Application.DTOs;

namespace MizanERP.Application.Services
{
    public interface IInventoryService
    {
        Task StockInAsync(InventoryTransactionDto dto);
        Task StockOutAsync(InventoryTransactionDto dto);
        Task AdjustAsync(InventoryTransactionDto dto);
    }

    public interface IPurchaseService
    {
        Task<Guid> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task ReceiveGoodsAsync(Guid purchaseOrderId);
    }

    public interface ISalesService
    {
        Task<Guid> CreateSalesOrderAsync(CreateSalesOrderDto dto);
        Task ShipGoodsAsync(Guid salesOrderId);
    }

    public interface IProductionService
    {
        Task<Guid> CreateProductionOrderAsync(CreateProductionOrderDto dto);
        Task ConsumeRawMaterialsAsync(Guid productionOrderId);
        Task ProduceFinishedGoodsAsync(Guid productionOrderId);
    }

    public interface IAccountingService
    {
        Task GenerateJournalEntriesForPurchaseAsync(Guid purchaseOrderId);
        Task GenerateJournalEntriesForSaleAsync(Guid salesOrderId);
        Task GenerateJournalEntriesForProductionAsync(Guid productionOrderId);
        Task GenerateJournalEntriesForInventoryAdjustmentAsync(Guid inventoryTransactionId);
    }
}