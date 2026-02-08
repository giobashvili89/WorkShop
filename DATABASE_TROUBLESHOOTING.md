# Database Troubleshooting Guide

This guide helps resolve common database issues in the WorkShop application, particularly the "relation 'Categories' does not exist" error.

## Understanding Database Initialization

The WorkShop application uses **automatic database initialization** on startup:

1. **Automatic Migrations**: When the application starts, it automatically runs all pending Entity Framework migrations
2. **Data Seeding**: After migrations, it seeds the database with default data including:
   - 10 default categories (Fiction, Non-Fiction, Science, History, etc.)
   - 100 sample books
   - 2 default users (admin/admin1 and customer/customer1)

This process is handled by `DbInitializer.InitializeAsync()` in `Program.cs`.

## Common Issue: "relation 'Categories' does not exist"

This error occurs when the application tries to query the Categories table before migrations have been applied.

### Root Causes

1. **Database doesn't exist**: PostgreSQL database hasn't been created
2. **Connection failure**: Application can't connect to PostgreSQL
3. **Migration failure**: Migrations failed to apply (logged but app continued running)
4. **Timing issue**: Application code ran before migrations completed

### Solution Steps

#### For Docker Deployment (Recommended)

1. **Ensure PostgreSQL is running**:
   ```bash
   docker-compose up -d postgres
   ```

2. **Check PostgreSQL health**:
   ```bash
   docker-compose ps postgres
   ```
   Status should show "healthy"

3. **Start the API**:
   ```bash
   docker-compose up -d api
   ```

4. **Check API logs for initialization**:
   ```bash
   docker-compose logs api | grep -i "database\|migration\|category"
   ```
   
   You should see logs indicating successful migration and seeding.

5. **If migrations failed**, restart with clean database:
   ```bash
   docker-compose down -v  # Removes volumes (deletes data)
   docker-compose up -d
   ```

#### For Local Development

1. **Ensure PostgreSQL is running**:
   ```bash
   # Check if PostgreSQL is running
   pg_isready -h localhost -p 5432
   ```

2. **Create database if it doesn't exist**:
   ```bash
   createdb workshop_db
   ```
   
   Or using psql:
   ```sql
   CREATE DATABASE workshop_db;
   ```

3. **Verify connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=workshop_db;Username=postgres;Password=your_password"
     }
   }
   ```

4. **Run the application**:
   ```bash
   cd src/WorkShop.API
   dotnet run
   ```

5. **Check logs** for successful initialization:
   ```
   info: Microsoft.EntityFrameworkCore.Migrations[...]
         Applied migration '20260208155031_AddCategoryEntity'
   ```

### Manual Migration (If Automatic Fails)

If automatic migrations don't work, you can apply them manually:

```bash
# Install EF Core tools if not already installed
dotnet tool install --global dotnet-ef

# Navigate to Infrastructure project
cd src/WorkShop.Infrastructure

# Apply migrations manually
dotnet ef database update --startup-project ../WorkShop.API

# List applied migrations
dotnet ef migrations list --startup-project ../WorkShop.API
```

### Verify Categories Table

Connect to PostgreSQL and verify the table exists:

```bash
# Using psql
psql -h localhost -U postgres -d workshop_db -c "\dt"
```

Or using docker:

```bash
docker exec -it workshop-postgres psql -U postgres -d workshop_db -c "\dt"
```

You should see the `Categories` table in the list.

Check if categories are seeded:

```bash
docker exec -it workshop-postgres psql -U postgres -d workshop_db -c "SELECT * FROM \"Categories\";"
```

## Migration Files

The application includes the following migrations:

1. `20260207000000_InitialCreate` - Initial database schema
2. `20260207170343_AddDeliveryInfoToOrder` - Adds delivery information
3. `20260208101402_RemoveProductsTable` - Removes legacy Products table
4. `20260208132718_ConvertToEnums` - Converts status fields to enums
5. `20260208155031_AddCategoryEntity` - **Creates the Categories table**

## Error Logging

Database initialization errors are logged but don't prevent the app from starting. Check application logs:

```bash
# Docker
docker-compose logs api

# Local development
# Check console output when running dotnet run
```

Look for errors like:
- "An error occurred while initializing the database"
- "Failed to connect to database"
- "Migration failed"

## Prevention

To prevent this issue:

1. **Use Docker Compose**: The `depends_on` configuration ensures PostgreSQL is healthy before starting the API
2. **Check connection strings**: Ensure they match your PostgreSQL configuration
3. **Monitor logs**: Always check logs during first startup to verify migrations succeeded
4. **Use health checks**: The docker-compose.yml includes PostgreSQL health checks

## Testing Database Initialization

Run the test suite to verify database operations work correctly:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/WorkShop.Infrastructure.Tests
```

All tests should pass, confirming the database layer is configured correctly.

## Still Having Issues?

If you continue to experience problems:

1. **Check PostgreSQL is accessible**:
   ```bash
   telnet localhost 5432
   ```

2. **Verify credentials**: Ensure username/password in connection string are correct

3. **Check PostgreSQL logs**:
   ```bash
   docker-compose logs postgres
   ```

4. **Try clean slate**:
   ```bash
   docker-compose down -v
   rm -rf src/WorkShop.API/bin src/WorkShop.API/obj
   rm -rf src/WorkShop.Infrastructure/bin src/WorkShop.Infrastructure/obj
   dotnet restore
   docker-compose up -d
   ```

## Summary

The "Categories does not exist" error is typically resolved by:

1. Ensuring PostgreSQL is running and accessible
2. Verifying connection strings are correct
3. Allowing the application to complete its automatic database initialization
4. Checking logs to confirm migrations were applied successfully

The application is designed to handle database initialization automatically, so under normal circumstances, no manual intervention is required.
