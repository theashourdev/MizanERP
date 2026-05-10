# 🚀 MizanERP - Complete Setup & Running Guide

## ✅ Current Status

- ✅ All projects build successfully
- ✅ EF Core migrations created
- ✅ Database seeding fixed (async timing resolved)
- ✅ Identity roles & users integrated
- ✅ JWT authentication configured
- ✅ Swagger documentation ready
- ✅ API controllers implemented
- ✅ Exception handling middleware

---

## 🎯 Step-by-Step: Run the Application

### **Step 1: Verify SQL Server is Running**
```powershell
# Check SQL Server status
sqlcmd -S . -Q "SELECT @@VERSION"
```

If you get an error, start SQL Server:
```powershell
# Start SQL Server service (Windows Service)
net start MSSQLSERVER

# Or use SQL Server Configuration Manager GUI
```

### **Step 2: Update Database (Apply Migrations)**
```powershell
cd D:\Mohammed\Projects\MizanERP

# Apply all pending migrations
dotnet ef database update --project MizanERP.Infrastructure --startup-project MizanERP.Web
```

**Expected Output:**
```
Applying migration '20260509204343_InitialCreate'.
Done.
```

### **Step 3: Run the Application**

#### **Option A: From Visual Studio**
1. Open `MizanERP.sln`
2. Set `MizanERP.Web` as StartUp Project (right-click → Set as Startup Project)
3. Press `F5` or click "Play" button
4. Wait for console output showing:
   ```
   ✅ Migrations applied successfully.
   ✅ Identity roles initialized.
   ✅ Admin user initialized.
   ✅ Business data seeded successfully.
   ```

#### **Option B: From PowerShell**
```powershell
cd D:\Mohammed\Projects\MizanERP
dotnet run --project MizanERP.Web
```

**Expected Console Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

### **Step 4: Access the Application**

1. **Swagger UI (API Documentation)**
   - Open: `https://localhost:7001/` (or your port)
   - You'll see interactive Swagger documentation

2. **Generate JWT Token**
   - Expand `POST /api/auth/token`
   - Click "Try it out"
   - Enter:
     ```json
     {
       "username": "admin",
       "password": "Admin@123456",
       "role": "Admin"
     }
     ```
   - Click "Execute"
   - Copy the returned token

3. **Test API Endpoints**
   - Click "Authorize" (top right)
   - Paste: `Bearer <your-token>`
   - Click "Authorize"
   - Now test endpoints (they'll send your JWT automatically)

---

## 🧪 Verify Everything Works

### **Test 1: Check Database Initialization**
```powershell
# Open SQL Server Management Studio or use sqlcmd
sqlcmd -S . -d MizanERP -Q "SELECT COUNT(*) as ProductCount FROM Products"
sqlcmd -S . -d MizanERP -Q "SELECT COUNT(*) as WarehouseCount FROM Warehouses"
sqlcmd -S . -d MizanERP -Q "SELECT COUNT(*) as AccountCount FROM Accounts"
```

Expected:
```
ProductCount
9
WarehouseCount
3
AccountCount
19
```

### **Test 2: Check Identity Tables**
```sql
-- Check Roles
SELECT Name FROM AspNetRoles ORDER BY Name;

-- Check Admin User
SELECT UserName, Email, EmailConfirmed FROM AspNetUsers WHERE UserName = 'admin';

-- Check User-Role Assignment
SELECT r.Name as Role 
FROM AspNetRoles r
JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
JOIN AspNetUsers u ON u.Id = ur.UserId
WHERE u.UserName = 'admin';
```

Expected:
```
Name: Admin, Accountant, Manager, Production, Sales, Staff, Warehouse
UserName: admin, Email: admin@mizanerp.com, EmailConfirmed: 1
Role: Admin
```

### **Test 3: API Endpoint Tests**

#### **3a. Get All Products (No Auth Required)**
```bash
curl -X GET "https://localhost:7001/api/products" -k
```

Response should show 9 products with their details.

#### **3b. Generate JWT Token**
```bash
curl -X POST "https://localhost:7001/api/auth/token" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123456","role":"Admin"}' \
  -k
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 7200
}
```

#### **3c. Create a Product (Auth Required)**
```bash
TOKEN="your-token-from-step-3b"
curl -X POST "https://localhost:7001/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "code":"TEST-001",
    "name":"Test Product",
    "description":"A test product",
    "price":99.99,
    "currency":"USD",
    "isRawMaterial":false
  }' \
  -k
```

---

## 📊 Database Schema

### **Core Tables Created:**

| Table | Purpose | Rows |
|-------|---------|------|
| `Products` | Product catalog | 9 |
| `Warehouses` | Storage locations | 3 |
| `Accounts` | Chart of accounts | 19 |
| `AspNetUsers` | Identity users | 1 (admin) |
| `AspNetRoles` | Identity roles | 7 |
| `AspNetUserRoles` | User-role mapping | 1 |
| `Suppliers` | Purchase suppliers | 0 |
| `Customers` | Sales customers | 0 |
| `PurchaseOrders` | Purchase orders | 0 |
| `SalesOrders` | Sales orders | 0 |
| `ProductionOrders` | Production orders | 0 |
| `InventoryMovements` | Stock transactions | 0 |
| `AccountingEntries` | Journal entries | 0 |

---

## 🛠️ Troubleshooting

### **Issue: "Database 'MizanERP' does not exist"**
**Solution:**
```powershell
# Migrations will create the database automatically, but you can manually create it:
dotnet ef database update --project MizanERP.Infrastructure --startup-project MizanERP.Web
```

### **Issue: "Timeout expired. The timeout period elapsed..."**
**Solution:**
```powershell
# Your SQL Server might be slow. Increase timeout in appsettings.json:
# In Connection String, add: Connection Timeout=60
```

### **Issue: "Cannot connect to SQL Server"**
**Solution:**
```powershell
# 1. Verify SQL Server is running
net start MSSQLSERVER

# 2. Check connection string in appsettings.json
# 3. Try connecting manually:
sqlcmd -S . -U sa
```

### **Issue: "Admin user already exists" error**
**Solution:**
This is normal if you run the app multiple times. The seeding checks for existing data and skips creation. To reset:
```powershell
# Delete the database
sqlcmd -S . -Q "DROP DATABASE MizanERP"

# Re-run the application to recreate
dotnet run --project MizanERP.Web
```

### **Issue: Seeding still doesn't run**
**Solution:**
Check the console output for errors. Common causes:
- Missing NuGet packages
- Invalid connection string
- SQL Server permissions
- Corrupted migrations

If needed, delete migrations and recreate:
```powershell
# Remove existing migrations
Remove-Item "MizanERP.Infrastructure\Persistence\Migrations\*.cs" -Confirm:$false

# Create new migration
dotnet ef migrations add InitialCreate --project MizanERP.Infrastructure --startup-project MizanERP.Web

# Update database
dotnet ef database update --project MizanERP.Infrastructure --startup-project MizanERP.Web
```

---

## 📝 Configuration Files

### **appsettings.json** (MizanERP.Web)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MizanERP;Trusted_Connection=True;..."
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-chars",
    "Issuer": "MizanERP",
    "Audience": "MizanERPUsers"
  },
  "Seeding": {
    "DefaultAdminUsername": "admin",
    "DefaultAdminEmail": "admin@mizanerp.com",
    "DefaultAdminPassword": "Admin@123456",
    "EnableDefaultAdminCreation": true
  }
}
```

### **Production Considerations:**
1. Change `DefaultAdminPassword` to a strong password
2. Change JWT `Secret` to a long random string
3. Set `EnableDefaultAdminCreation: false` after first run
4. Use environment variables for sensitive data

---

## 🔐 Security Notes

⚠️ **IMPORTANT FOR PRODUCTION:**
1. Change default admin password immediately
2. Use environment variables for secrets (not in source code)
3. Enable HTTPS (already done in template)
4. Configure CORS properly (not allow all origins)
5. Implement rate limiting on auth endpoint
6. Use strong JWT secret (minimum 32 characters)
7. Enable SQL Server encryption

---

## 📚 API Endpoints Reference

### **Authentication**
- `POST /api/auth/token` - Generate JWT token

### **Products**
- `GET /api/products` - Get all products (public)
- `GET /api/products/{id}` - Get product by ID (public)
- `POST /api/products` - Create product (Admin, Manager)
- `PUT /api/products/{id}` - Update product (Admin, Manager)
- `DELETE /api/products/{id}` - Delete product (Admin)

### **Inventory**
- `POST /api/inventory/stockin` - Receive goods (Admin, Warehouse, Manager)
- `POST /api/inventory/stockout` - Issue goods (Admin, Warehouse, Manager)
- `POST /api/inventory/adjust` - Adjust inventory (Admin, Warehouse)

### **Purchases**
- `POST /api/purchases` - Create purchase order (Admin, Manager)
- `POST /api/purchases/{id}/receive` - Receive goods (Admin, Warehouse, Manager)
- `GET /api/purchases/{id}` - Get purchase order (all authenticated users)

### **Sales**
- `POST /api/sales` - Create sales order (Admin, Manager, Sales)
- `POST /api/sales/{id}/ship` - Ship goods (Admin, Warehouse, Manager)
- `GET /api/sales/{id}` - Get sales order (all authenticated users)

### **Production**
- `POST /api/production` - Create production order (Admin, Manager, Production)
- `POST /api/production/{id}/produce` - Complete production (Admin, Production, Manager)
- `GET /api/production/{id}` - Get production order (all authenticated users)

### **Accounting**
- `POST /api/accounting/journal-entries` - Generate journal entries (Admin, Accountant, Manager)
- `POST /api/accounting/post` - Post entries to ledger (Admin, Accountant, Manager)
- `GET /api/accounting/trial-balance` - Get trial balance (Admin, Accountant, Manager)

---

## ✅ Checklist Before Going to Production

- [ ] Database initialization works without breakpoints
- [ ] Admin user can login with JWT
- [ ] All API endpoints respond correctly
- [ ] Swagger UI displays all endpoints
- [ ] Seeding creates all required data
- [ ] Identity roles are properly assigned
- [ ] Authorization attributes work
- [ ] Error handling returns proper HTTP codes
- [ ] CORS is configured for your domain
- [ ] Password changed from default
- [ ] JWT secret changed from default
- [ ] Connection string uses encrypted connection
- [ ] Logging is configured properly
- [ ] All NuGet packages are up to date

---

## 🎓 Next Steps

After the application is running successfully:

1. **Create Razor Pages UI**
   - Dashboard page
   - Products management
   - Inventory management
   - Order management

2. **Implement Advanced Features**
   - Product search and filtering
   - Order status tracking
   - Reporting & analytics
   - Batch operations

3. **Add Testing**
   - Unit tests for services
   - Integration tests for APIs
   - E2E tests for workflows

4. **Deploy to Production**
   - Azure App Service
   - SQL Database
   - Configure monitoring
   - Set up CI/CD

---

## 📞 Support

For issues or questions:
1. Check console output first (contains detailed error messages)
2. Review SQL Server error logs
3. Check application logs in `Debug` console
4. Refer to ASP.NET Core documentation

Good luck with MizanERP! 🚀
