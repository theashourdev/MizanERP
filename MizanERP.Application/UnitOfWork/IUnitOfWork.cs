using MizanERP.Application.Repositories;

namespace MizanERP.Application
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ISupplierRepository Suppliers { get; }
        ICustomerRepository Customers { get; }
        IWarehouseRepository Warehouses { get; }
        IInventoryMovementRepository InventoryMovements { get; }
        IPurchaseOrderRepository PurchaseOrders { get; }
        ISalesOrderRepository SalesOrders { get; }
        IProductionOrderRepository ProductionOrders { get; }
        IAccountRepository Accounts { get; }
        IAccountingEntryRepository AccountingEntries { get; }
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ICapitalTransactionRepository CapitalTransactions { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}