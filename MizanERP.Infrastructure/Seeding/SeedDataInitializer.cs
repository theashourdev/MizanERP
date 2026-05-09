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
                await context.Database.EnsureCreatedAsync();

                // Seed only if empty
                if (await context.Users.CountAsync() > 0)
                    return;

                // Seed Roles
                await SeedRolesAsync(context);

                // Seed Users
                await SeedUsersAsync(context);

                // Seed Accounts (Chart of Accounts)
                await SeedAccountsAsync(context);

                // Seed Warehouse
                await SeedWarehousesAsync(context);

                // Seed Products
                await SeedProductsAsync(context);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database seeding failed.", ex);
            }
        }

        private static async Task SeedRolesAsync(MizanERPDbContext context)
        {
            var roles = new[]
            {
                new Role(Guid.NewGuid(), "Admin"),
                new Role(Guid.NewGuid(), "Manager"),
                new Role(Guid.NewGuid(), "Staff"),
                new Role(Guid.NewGuid(), "Accountant")
            };

            foreach (var role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role.Name))
                {
                    await context.Roles.AddAsync(role);
                }
            }
        }

        private static async Task SeedUsersAsync(MizanERPDbContext context)
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null) return;

            var adminUser = new User(Guid.NewGuid(), "admin");
            adminUser.AddRole(adminRole);

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                await context.Users.AddAsync(adminUser);
            }
        }

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

                // Liability Accounts
                new Account(Guid.NewGuid(), "Accounts Payable", AccountType.Liability),
                new Account(Guid.NewGuid(), "Accrued Expenses", AccountType.Liability),

                // Equity Accounts
                new Account(Guid.NewGuid(), "Owner's Equity", AccountType.Equity),
                new Account(Guid.NewGuid(), "Retained Earnings", AccountType.Equity),

                // Revenue Accounts
                new Account(Guid.NewGuid(), "Sales Revenue", AccountType.Revenue),

                // Expense Accounts
                new Account(Guid.NewGuid(), "Cost of Goods Sold", AccountType.Expense),
                new Account(Guid.NewGuid(), "Inventory Variance", AccountType.Expense),
            };

            foreach (var account in accounts)
            {
                if (!context.Accounts.Any(a => a.Name == account.Name))
                {
                    await context.Accounts.AddAsync(account);
                }
            }
        }

        private static async Task SeedWarehousesAsync(MizanERPDbContext context)
        {
            var warehouseAddress = new Address(
                "123 Industrial Road",
                "Manchester",
                "Greater Manchester",
                "M1 1AA",
                "United Kingdom"
            );

            var warehouse = new Warehouse(
                Guid.NewGuid(),
                "Main Warehouse",
                warehouseAddress
            );

            if (!context.Warehouses.Any(w => w.Name == "Main Warehouse"))
            {
                await context.Warehouses.AddAsync(warehouse);
            }
        }

        private static async Task SeedProductsAsync(MizanERPDbContext context)
        {
            var rawMaterials = new[]
            {
                new Product(Guid.NewGuid(), "Steel Sheet", ProductType.RawMaterial, "kg", "RM-001"),
                new Product(Guid.NewGuid(), "Plastic Resin", ProductType.RawMaterial, "kg", "RM-002"),
                new Product(Guid.NewGuid(), "Electronic Component", ProductType.RawMaterial, "pcs", "RM-003"),
            };

            var finishedGoods = new[]
            {
                new Product(Guid.NewGuid(), "Metal Frame", ProductType.FinishedGood, "pcs", "FG-001"),
                new Product(Guid.NewGuid(), "Plastic Housing", ProductType.FinishedGood, "pcs", "FG-002"),
                new Product(Guid.NewGuid(), "Assembled Device", ProductType.FinishedGood, "pcs", "FG-003"),
            };

            var allProducts = new List<Product>();
            allProducts.AddRange(rawMaterials);
            allProducts.AddRange(finishedGoods);

            foreach (var product in allProducts)
            {
                if (!context.Products.Any(p => p.Code == product.Code))
                {
                    await context.Products.AddAsync(product);
                }
            }
        }
    }
}