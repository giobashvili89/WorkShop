# Database Troubleshooting Guide

This guide helps you resolve common database-related issues in the WorkShop project.

## Table of Contents
- [Common Errors](#common-errors)
- [Quick Solutions](#quick-solutions)
- [Detailed Setup Instructions](#detailed-setup-instructions)
- [Manual Database Reset](#manual-database-reset)
- [Verification Steps](#verification-steps)

---

## Common Errors

### Error: "relation 'Categories' does not exist"

**Full Error Message:**
```
Npgsql.PostgresException (0x80004005): 42P01: relation 'Categories' does not exist
```

**Cause:** The database migrations have not been applied, or the database is in an inconsistent state.

**Solution:** Follow the [Quick Solutions](#quick-solutions) below.

---

## Quick Solutions

### Solution 1: Using Docker Compose (Recommended)

If you're using Docker Compose, the easiest way to fix database issues is to reset everything:

```bash
# Stop and remove all containers, networks, and volumes
docker compose down -v

# Start the PostgreSQL database
docker compose up -d postgres

# Wait for PostgreSQL to be healthy (about 10-15 seconds)
docker compose ps

# Apply migrations
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure

# Run the API to seed data
dotnet run
```

### Solution 2: Manual Database Reset

If you're running PostgreSQL locally or need more control:

```bash
# 1. Drop and recreate the database
docker exec -i workshop-postgres psql -U postgres -c "DROP DATABASE IF EXISTS workshop_db;"
docker exec -i workshop-postgres psql -U postgres -c "CREATE DATABASE workshop_db;"

# 2. Apply migrations
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure

# 3. Run the API to seed initial data
dotnet run
```

### Solution 3: Using Local PostgreSQL

If you're running PostgreSQL locally (not in Docker):

```bash
# 1. Connect to PostgreSQL and reset the database
psql -U postgres -c "DROP DATABASE IF EXISTS workshop_db;"
psql -U postgres -c "CREATE DATABASE workshop_db;"

# 2. Apply migrations
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure

# 3. Run the API to seed initial data
dotnet run
```

---

## Detailed Setup Instructions

### Prerequisites

1. **PostgreSQL** must be running:
   - **With Docker:** `docker compose up -d postgres`
   - **Local installation:** Ensure PostgreSQL service is running

2. **Entity Framework Core Tools** must be installed:
   ```bash
   dotnet tool install --global dotnet-ef
   # Or update if already installed
   dotnet tool update --global dotnet-ef
   ```

3. **NuGet packages** must be restored:
   ```bash
   dotnet restore
   ```

### Step-by-Step Setup

#### Step 1: Start PostgreSQL

**Using Docker Compose:**
```bash
docker compose up -d postgres
```

**Check if PostgreSQL is healthy:**
```bash
docker compose ps
# Look for "healthy" status for postgres container
```

#### Step 2: Configure Connection String

The application uses the following connection string from `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=workshop_db;Username=postgres;Password=gio"
}
```

Make sure your PostgreSQL credentials match these settings, or update the connection string accordingly.

#### Step 3: Apply Migrations

Navigate to the API project and apply migrations:

```bash
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure
```

This will create all necessary tables including:
- Categories
- Books
- Users
- Orders
- OrderItems

#### Step 4: Seed Initial Data

Run the API application to automatically seed the database:

```bash
dotnet run
```

The `DbInitializer` will:
1. Check if data already exists
2. Seed 10 default categories (Fiction, Non-Fiction, Science, etc.)
3. Seed 100 sample books
4. Create default admin and customer users

---

## Manual Database Reset

If migrations are failing or the database is in an inconsistent state, you may need to reset everything:

### Complete Reset (Nuclear Option)

```bash
# 1. Stop all containers
docker compose down -v

# 2. Remove the database volume (this deletes all data!)
docker volume rm workshop_postgres_data 2>/dev/null || true

# 3. Start fresh
docker compose up -d postgres

# 4. Wait for PostgreSQL to be ready
sleep 10

# 5. Apply migrations
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure

# 6. Seed data
dotnet run
```

### Rebuild Migrations (Advanced)

If migrations are corrupted, you may need to recreate them:

```bash
# WARNING: This will delete existing migrations!

# 1. Backup your current migrations folder
cp -r src/WorkShop.Infrastructure/Migrations src/WorkShop.Infrastructure/Migrations.backup

# 2. Delete all migration files
rm -rf src/WorkShop.Infrastructure/Migrations/*.cs

# 3. Create a fresh migration
cd src/WorkShop.API
dotnet ef migrations add InitialMigration --project ../WorkShop.Infrastructure

# 4. Reset the database
docker exec -i workshop-postgres psql -U postgres -c "DROP DATABASE IF EXISTS workshop_db;"
docker exec -i workshop-postgres psql -U postgres -c "CREATE DATABASE workshop_db;"

# 5. Apply the new migration
dotnet ef database update --project ../WorkShop.Infrastructure
```

---

## Verification Steps

After applying migrations and seeding data, verify everything is working:

### 1. Check Database Tables

```bash
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "\dt"
```

Expected output should include:
- Books
- Categories
- OrderItems
- Orders
- Users
- __EFMigrationsHistory

### 2. Check Categories Table

```bash
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "SELECT * FROM \"Categories\";"
```

Should show 10 categories: Fiction, Non-Fiction, Science, History, Biography, Technology, Art, Philosophy, Self-Help, Mystery

### 3. Test the API Endpoint

```bash
# Start the API if not already running
cd src/WorkShop.API
dotnet run

# In another terminal, test the books endpoint
curl http://localhost:5000/api/books | jq '.[0]'
```

Expected response should include book data with category information.

### 4. Check Migration History

```bash
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "SELECT * FROM \"__EFMigrationsHistory\";"
```

Should show the applied migration with timestamp.

---

## Environment Variables

The project supports the following environment variables for database configuration:

```bash
# PostgreSQL Configuration
POSTGRES_DB=workshop_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=gio

# Connection String (alternative to appsettings.json)
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=workshop_db;Username=postgres;Password=gio"
```

You can create a `.env` file in the project root with these settings, or use the `.env.example` as a template.

---

## Common Issues and Solutions

### Issue: "Cannot connect to the database"

**Solution:**
1. Ensure PostgreSQL is running: `docker compose ps`
2. Check the connection string in `appsettings.json`
3. Verify the PostgreSQL container is healthy: `docker logs workshop-postgres`

### Issue: "Build failed" when running migrations

**Solution:**
```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Try migrations again
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure
```

### Issue: "Migration already applied" errors

**Solution:**
This usually means the database and migrations are out of sync. Follow the [Manual Database Reset](#manual-database-reset) steps.

### Issue: "TrackingStatus column does not exist" or similar column errors

**Solution:**
This indicates incomplete or failed migrations. Use the [Complete Reset](#complete-reset-nuclear-option) approach.

---

## Useful Commands Reference

```bash
# Check PostgreSQL container status
docker compose ps
docker logs workshop-postgres

# Connect to PostgreSQL CLI
docker exec -it workshop-postgres psql -U postgres -d workshop_db

# List all tables
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "\dt"

# Check specific table structure
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "\d \"TableName\""

# View migration history
docker exec -i workshop-postgres psql -U postgres -d workshop_db -c "SELECT * FROM \"__EFMigrationsHistory\";"

# Apply migrations
cd src/WorkShop.API
dotnet ef database update --project ../WorkShop.Infrastructure

# Create new migration
cd src/WorkShop.API
dotnet ef migrations add MigrationName --project ../WorkShop.Infrastructure

# Remove last migration (if not applied to database)
cd src/WorkShop.API
dotnet ef migrations remove --project ../WorkShop.Infrastructure
```

---

## Getting Help

If you continue to experience issues:

1. Check the logs: `docker compose logs postgres`
2. Verify your connection string matches your PostgreSQL setup
3. Ensure all NuGet packages are restored: `dotnet restore`
4. Try the [Complete Reset](#complete-reset-nuclear-option) approach
5. Check that your PostgreSQL version is compatible (PostgreSQL 16 recommended)

For additional support, please open an issue on the project repository with:
- The full error message
- Output of `docker compose ps`
- Output of `dotnet ef migrations list --project src/WorkShop.Infrastructure`
