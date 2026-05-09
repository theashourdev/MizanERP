using Microsoft.EntityFrameworkCore;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence
{
    public class MizanERPDbContext : DbContext
    {
        public MizanERPDbContext(DbContextOptions<MizanERPDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
        public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
        public DbSet<ProductionOrderLine> ProductionOrderLines => Set<ProductionOrderLine>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<AccountingEntry> AccountingEntries => Set<AccountingEntry>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MizanERPDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}