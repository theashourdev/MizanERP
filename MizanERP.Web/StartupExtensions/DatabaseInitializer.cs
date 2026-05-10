using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MizanERP.Domain.Entities;
using MizanERP.Infrastructure.Persistence;
using MizanERP.Infrastructure.Seeding;

namespace MizanERP.Web.StartupExtensions
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initialize database with migrations, ASP.NET Identity, and business data seeding.
        /// Must be called BEFORE app.UseRouting() and with proper async context.
        /// </summary>
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MizanERPDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            
            try
            {
                // Step 1: Apply any pending migrations
                var migrations = await db.Database.GetPendingMigrationsAsync();
                if (migrations.Any())
                {
                    Console.WriteLine("⏳ Applying pending migrations...");
                    await db.Database.MigrateAsync();
                    Console.WriteLine("✅ Migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("✅ Database is up to date.");
                }

                // Step 2: Initialize ASP.NET Identity roles
                Console.WriteLine("⏳ Setting up Identity roles...");
                await InitializeIdentityRolesAsync(roleManager);
                Console.WriteLine("✅ Identity roles initialized.");

                // Step 3: Initialize default admin user (if enabled in config)
                if (config.GetValue<bool>("Seeding:EnableDefaultAdminCreation"))
                {
                    Console.WriteLine("⏳ Setting up default admin user...");
                    await InitializeDefaultAdminAsync(userManager, roleManager, config);
                    Console.WriteLine("✅ Admin user initialized.");
                }
                else
                {
                    Console.WriteLine("ℹ️  Default admin creation is disabled in configuration.");
                }

                // Step 4: Seed business data
                Console.WriteLine("⏳ Seeding business data...");
                await SeedDataInitializer.InitializeAsync(db);
                Console.WriteLine("✅ Business data seeded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization error: {ex.Message}");
                Console.WriteLine($"   Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ensure ASP.NET Identity roles are created for role-based authorization
        /// </summary>
        private static async Task InitializeIdentityRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { "Admin", "Manager", "Staff", "Accountant", "Warehouse", "Sales", "Production" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                    Console.WriteLine($"   ✓ Role '{role}' created.");
                }
                else
                {
                    Console.WriteLine($"   ✓ Role '{role}' already exists.");
                }
            }
        }

        /// <summary>
        /// Create default admin user if not already exists.
        /// Credentials are loaded from configuration (appsettings.json)
        /// </summary>
        private static async Task InitializeDefaultAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration config)
        {
            var adminUsername = config.GetValue<string>("Seeding:DefaultAdminUsername") ?? "admin";
            var adminEmail = config.GetValue<string>("Seeding:DefaultAdminEmail") ?? "admin@mizanerp.com";
            var adminPassword = config.GetValue<string>("Seeding:DefaultAdminPassword") ?? "Admin@123456";

            var existingUser = await userManager.FindByNameAsync(adminUsername);
            if (existingUser != null)
            {
                Console.WriteLine($"   ✓ Admin user '{adminUsername}' already exists.");
                return;
            }

            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                Console.WriteLine("   ⚠️  Admin role not found, cannot create default admin user.");
                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"   ✓ Default admin user '{adminUsername}' created successfully.");
                Console.WriteLine($"      Email: {adminEmail}");
                Console.WriteLine($"      ⚠️  PASSWORD: {adminPassword}");
                if (config["ASPNETCORE_ENVIRONMENT"] == "Production")
                {
                    Console.WriteLine($"      🔒 PRODUCTION: Change password immediately!");
                }
            }
            else
            {
                Console.WriteLine($"   ⚠️  Failed to create admin user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"      - {error.Description}");
                }
            }
        }
    }
}