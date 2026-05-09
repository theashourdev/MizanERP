using System;
using System.Threading.Tasks;
using MizanERP.Application;
using MizanERP.Application.Repositories;
using MizanERP.Infrastructure.Persistence.Repositories;

namespace MizanERP.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MizanERPDbContext _context;
        private IProductRepository? _productRepository;
        private ISupplierRepository? _supplierRepository;
        private ICustomerRepository? _customerRepository;
        private IWarehouseRepository? _warehouseRepository;
        private IInventoryMovementRepository? _inventoryMovementRepository;
        private IPurchaseOrderRepository? _purchaseOrderRepository;
        private ISalesOrderRepository? _salesOrderRepository;
        private IProductionOrderRepository? _productionOrderRepository;
        private IAccountRepository? _accountRepository;
        private IAccountingEntryRepository? _accountingEntryRepository;
        private IUserRepository? _userRepository;
        private IRoleRepository? _roleRepository;

        public UnitOfWork(MizanERPDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public ISupplierRepository Suppliers => _supplierRepository ??= new SupplierRepository(_context);
        public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_context);
        public IWarehouseRepository Warehouses => _warehouseRepository ??= new WarehouseRepository(_context);
        public IInventoryMovementRepository InventoryMovements => _inventoryMovementRepository ??= new InventoryMovementRepository(_context);
        public IPurchaseOrderRepository PurchaseOrders => _purchaseOrderRepository ??= new PurchaseOrderRepository(_context);
        public ISalesOrderRepository SalesOrders => _salesOrderRepository ??= new SalesOrderRepository(_context);
        public IProductionOrderRepository ProductionOrders => _productionOrderRepository ??= new ProductionOrderRepository(_context);
        public IAccountRepository Accounts => _accountRepository ??= new AccountRepository(_context);
        public IAccountingEntryRepository AccountingEntries => _accountingEntryRepository ??= new AccountingEntryRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}