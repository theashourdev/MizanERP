using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MizanERP.Application.Interfaces;
using MizanERP.Application.Services;
using MizanERP.Domain.Entities;
using MizanERP.Infrastructure;
using MizanERP.Infrastructure.Persistence;
using MizanERP.Infrastructure.Services;
using MizanERP.Web.Configuration;
using MizanERP.Web.Middleware;
using MizanERP.Web.StartupExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
//builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ✅ DB Context
builder.Services.AddDbContext<MizanERPDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var notyfSettings = builder.Configuration.GetSection("NotyfSettings").Get<NotyfSettings>()
            ?? new NotyfSettings
            {
                DurationInSeconds = 5,
                IsDismissable = true,
                Position = "TopRight"
            };

if (!Enum.TryParse<NotyfPosition>(notyfSettings.Position, true, out var position))
{
    position = NotyfPosition.TopRight;
}

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = notyfSettings.DurationInSeconds > 0
        ? notyfSettings.DurationInSeconds
        : 5;

    config.IsDismissable = notyfSettings.IsDismissable;
    config.Position = position;
});





// ✅ Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<MizanERPDbContext>()
.AddDefaultTokenProviders();

// ✅ JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// ✅ Swagger/OpenAPI
builder.Services.AddSwaggerWithJwt();

// ✅ Infrastructure DI (repositories and unit of work)
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

// ✅ Application Services DI
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IAccountingService, AccountingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// ✅ CORS (if needed for frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

// ✅ Initialize Database BEFORE app.Run()
// This ensures migrations and seeding complete before any requests are handled
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MizanERPDbContext>();
        await context.Database.MigrateAsync();
        await app.InitializeDatabaseAsync();
        Console.WriteLine("✅ Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MizanERP API v1");
        //c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ Exception handling middleware

app.UseMiddleware<RequestContextMiddleware>();

//app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();



app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowFrontend");

app.MapControllers();
app.MapRazorPages();
//app.MapStaticAssets();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
