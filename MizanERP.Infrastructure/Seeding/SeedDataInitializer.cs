using Microsoft.EntityFrameworkCore;
using MizanERP.Domain.Entities;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;
using MizanERP.Infrastructure.Persistence;

namespace MizanERP.Infrastructure.Seeding
{
    public static class SeedDataInitializer
    {
        public static async Task InitializeAsync(MizanERPDbContext context)
        {
            try
            {
                // Check if data already exists (avoid duplicate seeding)
                if (await context.Products.CountAsync() > 0)
                {
                    Console.WriteLine("   ℹ️  Business data already exists, skipping seed.");
                    return;
                }

                Console.WriteLine("   📋 Starting business data seeding...");

                // Seed Accounts (Chart of Accounts)
                await SeedAccountsAsync(context);
                Console.WriteLine("   ✓ Accounts seeded.");

                // Seed Warehouse
                await SeedWarehousesAsync(context);
                Console.WriteLine("   ✓ Warehouses seeded.");

                // Seed Products
                await SeedProductsAsync(context);
                Console.WriteLine("   ✓ Products seeded.");

                // Save all changes at once
                await context.SaveChangesAsync();
                Console.WriteLine("   ✓ All changes saved to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Seeding error: {ex.Message}");
                throw new InvalidOperationException("Database seeding failed.", ex);
            }
        }

        /// <summary>
        /// Seed Chart of Accounts for accounting functionality
        /// </summary>
        private static async Task SeedAccountsAsync(MizanERPDbContext context)
        {
            var accounts = new[]
            {
                // Asset Accounts
                new Account(Guid.NewGuid(), "Cash", AccountType.Asset),
                new Account(Guid.NewGuid(), "Accounts Receivable", AccountType.Asset),
                new Account(Guid.NewGuid(), "Raw Material Inventory", AccountType.Asset),
                new Account(Guid.NewGuid(), "Finished Goods Inventory", AccountType.Asset),
                new Account(Guid.NewGuid(), "Inventory Adjustment", AccountType.Asset),
                new Account(Guid.NewGuid(), "Equipment", AccountType.Asset),
                new Account(Guid.NewGuid(), "Accumulated Depreciation", AccountType.Asset),

                // Liability Accounts
                new Account(Guid.NewGuid(), "Accounts Payable", AccountType.Liability),
                new Account(Guid.NewGuid(), "Accrued Expenses", AccountType.Liability),
                new Account(Guid.NewGuid(), "Short-term Loans", AccountType.Liability),

                // Equity Accounts
                new Account(Guid.NewGuid(), "Owner's Equity", AccountType.Equity),
                new Account(Guid.NewGuid(), "Retained Earnings", AccountType.Equity),

                // Revenue Accounts
                new Account(Guid.NewGuid(), "Sales Revenue", AccountType.Revenue),
                new Account(Guid.NewGuid(), "Service Revenue", AccountType.Revenue),

                // Expense Accounts
                new Account(Guid.NewGuid(), "Cost of Goods Sold", AccountType.Expense),
                new Account(Guid.NewGuid(), "Inventory Variance", AccountType.Expense),
                new Account(Guid.NewGuid(), "Salaries and Wages", AccountType.Expense),
                new Account(Guid.NewGuid(), "Utilities", AccountType.Expense),
                new Account(Guid.NewGuid(), "Depreciation Expense", AccountType.Expense),
            };

            foreach (var account in accounts)
            {
                if (!await context.Accounts.AnyAsync(a => a.Name == account.Name))
                {
                    await context.Accounts.AddAsync(account);
                }
            }
        }

        /// <summary>
        /// Seed warehouses for inventory management
        /// </summary>
        private static async Task SeedWarehousesAsync(MizanERPDbContext context)
        {
            var warehouses = new[]
            {
                new Warehouse(
                    Guid.NewGuid(),
                    "Main Warehouse",
                    new Address("123 Industrial Road", "Manchester", "Greater Manchester", "M1 1AA", "United Kingdom")
                ),
                new Warehouse(
                    Guid.NewGuid(),
                    "Secondary Warehouse",
                    new Address("456 Commerce Park", "London", "London", "E1 6AN", "United Kingdom")
                ),
                new Warehouse(
                    Guid.NewGuid(),
                    "Distribution Center",
                    new Address("789 Logistics Way", "Birmingham", "West Midlands", "B7 4BB", "United Kingdom")
                )
            };

            foreach (var warehouse in warehouses)
            {
                if (!await context.Warehouses.AnyAsync(w => w.Name == warehouse.Name))
                {
                    await context.Warehouses.AddAsync(warehouse);
                }
            }
        }

        /// <summary>
        /// Seed sample products (raw materials and finished goods)
        /// </summary>
        private static async Task SeedProductsAsync(MizanERPDbContext context)
        {
            var rawMaterials = new[]
            {
                new Product(Guid.NewGuid(), "Steel Sheet", ProductType.RawMaterial, "kg", "RM-001"),
                new Product(Guid.NewGuid(), "Plastic Resin", ProductType.RawMaterial, "kg", "RM-002"),
                new Product(Guid.NewGuid(), "Electronic Component", ProductType.RawMaterial, "pcs", "RM-003"),
                new Product(Guid.NewGuid(), "Aluminum Bar", ProductType.RawMaterial, "kg", "RM-004"),
                new Product(Guid.NewGuid(), "Rubber Seal", ProductType.RawMaterial, "pcs", "RM-005"),
            };

            var finishedGoods = new[]
            {
                new Product(Guid.NewGuid(), "Metal Frame", ProductType.FinishedGood, "pcs", "FG-001"),
                new Product(Guid.NewGuid(), "Plastic Housing", ProductType.FinishedGood, "pcs", "FG-002"),
                new Product(Guid.NewGuid(), "Assembled Device", ProductType.FinishedGood, "pcs", "FG-003"),
                new Product(Guid.NewGuid(), "Electronic Module", ProductType.FinishedGood, "pcs", "FG-004"),
            };

            var allProducts = new List<Product>();
            allProducts.AddRange(rawMaterials);
            allProducts.AddRange(finishedGoods);

            foreach (var product in allProducts)
            {
                if (!await context.Products.AnyAsync(p => p.Code == product.Code))
                {
                    await context.Products.AddAsync(product);
                }
            }
        }
    }
}