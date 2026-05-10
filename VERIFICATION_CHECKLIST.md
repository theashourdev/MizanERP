# ✅ MizanERP - Complete Verification Checklist

## Pre-Run Checklist

- [ ] Visual Studio is open with MizanERP.sln
- [ ] MizanERP.Web is set as StartUp Project
- [ ] SQL Server is running (`net start MSSQLSERVER`)
- [ ] Build completed successfully (F6 or Build → Build Solution)
- [ ] No NuGet restore errors

---

## Database Initialization (Run & Wait)

### During Application Startup

Watch the console output for:

```
⏳ Applying pending migrations...
✅ Migrations applied successfully.

⏳ Setting up Identity roles...
   ✓ Role 'Admin' created.
   ✓ Role 'Manager' created.
   ✓ Role 'Staff' created.
   ✓ Role 'Accountant' created.
   ✓ Role 'Warehouse' created.
   ✓ Role 'Sales' created.
   ✓ Role 'Production' created.
✅ Identity roles initialized.

⏳ Setting up default admin user...
   ✓ Default admin user 'admin' created successfully.
      Email: admin@mizanerp.com
      ⚠️  PASSWORD: Admin@123456
✅ Admin user initialized.

⏳ Seeding business data...
   📋 Starting business data seeding...
   ✓ Accounts seeded.
   ✓ Warehouses seeded.
   ✓ Products seeded.
   ✓ All changes saved to database.
✅ Business data seeded successfully.

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
```

**Status**: ✅ If you see all these messages without errors

---

## Swagger UI Test

### Access Swagger Documentation

1. Open browser: `https://localhost:7001/`
2. You should see Swagger UI with all endpoints listed

**Check these endpoints exist:**
- [ ] POST /api/auth/token
- [ ] GET /api/products
- [ ] GET /api/products/{id}
- [ ] POST /api/products
- [ ] PUT /api/products/{id}
- [ ] DELETE /api/products/{id}
- [ ] POST /api/inventory/stockin
- [ ] POST /api/inventory/stockout
- [ ] POST /api/inventory/adjust
- [ ] POST /api/purchases
- [ ] POST /api/purchases/{id}/receive
- [ ] GET /api/purchases/{id}
- [ ] POST /api/sales
- [ ] POST /api/sales/{id}/ship
- [ ] GET /api/sales/{id}
- [ ] POST /api/production
- [ ] POST /api/production/{id}/produce
- [ ] GET /api/production/{id}
- [ ] POST /api/accounting/journal-entries
- [ ] POST /api/accounting/post
- [ ] GET /api/accounting/trial-balance

---

## Authentication Test

### Generate JWT Token

1. In Swagger UI, expand: `POST /api/auth/token`
2. Click "Try it out"
3. Enter:
   ```json
   {
     "username": "admin",
     "password": "Admin@123456",
     "role": "Admin"
   }
   ```
4. Click "Execute"

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 7200
}
```

**Status**: ✅ If you receive a valid JWT token

---

## API Test Without Authentication

### Test Public Endpoint

1. Expand: `GET /api/products`
2. Click "Try it out"
3. Click "Execute"

**Expected Response:**
```json
[
  {
    "id": "guid-here",
    "code": "RM-001",
    "name": "Steel Sheet",
    "type": 0,
    "unit": "kg",
    "isActive": true,
    "inventoryQuantity": 0
  },
  ...
]
```

**Status**: ✅ If you see 9 products

---

## API Test With Authentication

### Authorize in Swagger

1. Click "Authorize" button (top right)
2. In Bearer token field, paste: `Bearer <your-token-from-above>`
3. Click "Authorize"
4. Click "Close"

### Test Protected Endpoint

1. Expand: `POST /api/products`
2. Click "Try it out"
3. Enter:
   ```json
   {
     "code": "TEST-001",
     "name": "Test Product",
     "description": "A test product",
     "price": 99.99,
     "currency": "USD",
     "isRawMaterial": false
   }
   ```
4. Click "Execute"

**Expected Response:**
```json
{
  "id": "guid-here"
}
```

**Status**: ✅ If you receive a success response (201 or 200)

---

## Database Verification

### Open SQL Server Management Studio

1. Connect to: `.` (localhost)
2. Expand Databases
3. Find: `MizanERP`

### Check Tables Exist

**Verify these tables:**
- [ ] Products (should have 9 rows)
- [ ] Warehouses (should have 3 rows)
- [ ] Accounts (should have 19 rows)
- [ ] AspNetUsers (should have 1 row - admin)
- [ ] AspNetRoles (should have 7 rows)
- [ ] AspNetUserRoles (should have 1 row)

**Status**: ✅ If all tables exist with expected row counts

### Verify Admin User

```sql
SELECT UserName, Email, EmailConfirmed FROM AspNetUsers;
```

**Expected Result:**
```
UserName: admin
Email: admin@mizanerp.com
EmailConfirmed: 1 (true)
```

**Status**: ✅ If admin user exists

### Verify Roles

```sql
SELECT Name FROM AspNetRoles ORDER BY Name;
```

**Expected Result:**
```
Accountant
Admin
Manager
Production
Sales
Staff
Warehouse
```

**Status**: ✅ If all 7 roles exist

### Verify User-Role Assignment

```sql
SELECT 
    u.UserName,
    r.Name as Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'admin';
```

**Expected Result:**
```
UserName: admin
Role: Admin
```

**Status**: ✅ If admin user has Admin role

### Verify Products

```sql
SELECT Code, Name, Type FROM Products;
```

**Expected Result:** 9 products (5 raw materials, 4 finished goods)

**Status**: ✅ If 9 products exist

### Verify Warehouses

```sql
SELECT Name FROM Warehouses;
```

**Expected Result:**
```
Main Warehouse
Secondary Warehouse
Distribution Center
```

**Status**: ✅ If 3 warehouses exist

### Verify Chart of Accounts

```sql
SELECT COUNT(*) as AccountCount FROM Accounts;
```

**Expected Result:**
```
AccountCount: 19
```

**Status**: ✅ If 19 accounts exist

---

## Authorization Test

### Test Role-Based Access

1. In Swagger, click "Authorize"
2. Clear previous token and enter new one (if expired)
3. Try endpoint with role restriction: `POST /api/products`
4. You should be able to execute (Admin role)

### Test Restricted Access

1. Get a new token with role "Staff":
   ```json
   {
     "username": "admin",
     "password": "Admin@123456",
     "role": "Staff"
   }
   ```
2. Try: `POST /api/products`

**Expected**: Should get 403 Forbidden (no permission)

**Status**: ✅ If authorization checks work

---

## Error Handling Test

### Test Invalid Endpoint

1. Try: `GET /api/products/invalid-guid`

**Expected Response:**
```json
{
  "error": "ValidationError",
  "message": "Invalid GUID format",
  "details": null
}
```

**Status**: ✅ If error is returned as JSON (not HTML)

### Test Missing Authentication

1. Remove Authorization header (click Authorize → Clear)
2. Try protected endpoint: `POST /api/products`

**Expected**: 401 Unauthorized

**Status**: ✅ If you get 401 error

---

## Performance Check

### Measure Response Times

In Swagger UI, Network tab should show:
- [ ] Auth token endpoint: < 100ms
- [ ] Get products: < 100ms
- [ ] Create product: < 200ms

**Status**: ✅ If responses are fast

---

## Logging Check

### Application Logs

In Visual Studio Output window, you should see:
- [ ] EF Core SQL queries (if logging enabled)
- [ ] Initialization messages
- [ ] Request logs (if logging configured)
- [ ] Any warnings or errors

**Status**: ✅ If logging shows expected messages

---

## Final Comprehensive Check

### All-in-One Verification

Run through this sequence:

1. **Start app**
   - [ ] See "Now listening on" message

2. **Access Swagger**
   - [ ] Browser shows Swagger UI

3. **Login**
   - [ ] Get JWT token successfully

4. **Get Products**
   - [ ] See 9 products returned

5. **Create Product (with auth)**
   - [ ] Product created successfully

6. **Query Database**
   - [ ] New product appears in database

7. **Check Logs**
   - [ ] No errors in console

---

## Troubleshooting Matrix

| Symptom | Check | Fix |
|---------|-------|-----|
| App won't start | Console errors | Read error message |
| No seeding output | Check logs | Verify SQL Server running |
| Swagger not loading | URL correct? | Check port number |
| Login fails | Credentials correct? | Default: admin/Admin@123456 |
| API returns 500 | Exception message? | Check exception handling middleware |
| Products endpoint empty | DB initialized? | Run migrations |
| Authorization fails | Token valid? | Regenerate JWT |
| Database errors | Connection string? | Verify server/database names |

---

## Success Criteria

✅ **All of the following must be true:**

1. Application starts without errors
2. All initialization messages appear
3. Swagger UI loads
4. JWT authentication works
5. Public endpoints return data
6. Protected endpoints require authentication
7. Database contains all expected data
8. Admin user exists with correct role
9. No SQL errors in database
10. API responses are fast (< 200ms)

---

## What's Next?

Once you've verified all checks above ✅:

1. **Next Phase**: Build Razor Pages UI
2. **Create Pages**: Dashboard, Product CRUD, etc.
3. **Add Tests**: Unit and integration tests
4. **Deploy**: To Azure or production server

---

## Sign-Off Checklist

- [ ] Ran through entire checklist
- [ ] All items verified as working
- [ ] No errors or warnings
- [ ] Database properly seeded
- [ ] API endpoints responding
- [ ] Authentication working
- [ ] Authorization working
- [ ] Ready for UI development

---

**Verified By**: ___________________
**Date**: ___________________
**Notes**: ___________________

---

*For questions or issues, refer to:*
- *SEEDING_FIX_EXPLANATION.md*
- *RUN_GUIDE.md*
- *ASYNC_TIMING_DEEP_DIVE.md*
- *PROJECT_SUMMARY.md*
