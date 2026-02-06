# WorkShop

A full-stack application with .NET 10 API (Clean Architecture) and React.js frontend featuring JWT authentication and PostgreSQL database.

## Project Structure

```
WorkShop/
├── src/
│   ├── WorkShop.Domain/          # Domain layer - Entities (Book, User, Product)
│   ├── WorkShop.Application/     # Application layer - Business logic, DTOs, Interfaces
│   ├── WorkShop.Infrastructure/  # Infrastructure layer - Implementations, DbContext
│   └── WorkShop.API/             # API layer - Controllers, Endpoints
├── tests/
│   └── WorkShop.API.Tests/       # Unit tests for API
└── client/                       # React.js frontend
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL 12 or higher
- Node.js (v16 or higher)
- npm or yarn

## Getting Started

### Database Setup

1. Install PostgreSQL and create a database:
   ```bash
   createdb workshop_db
   ```

2. Update the connection string in `src/WorkShop.API/appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=workshop_db;Username=postgres;Password=postgres"
   }
   ```

### Backend (.NET API)

1. Navigate to the repository root:
   ```bash
   cd WorkShop
   ```

2. Restore dependencies and build:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Run the API:
   ```bash
   cd src/WorkShop.API
   dotnet run
   ```

   The API will be available at `http://localhost:5000` (or `https://localhost:5001`)

### Frontend (React)

1. Navigate to the client directory:
   ```bash
   cd client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

   The React app will be available at `http://localhost:5173` (or the port shown in the terminal)

## API Endpoints

### Authentication (No auth required)
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Books (Requires JWT authentication)
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get a book by ID
- `GET /api/books/author/{author}` - Filter books by author
- `GET /api/books/category/{category}` - Filter books by category
- `POST /api/books` - Create a new book
- `PUT /api/books/{id}` - Update a book
- `DELETE /api/books/{id}` - Delete a book

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get a product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product

## Features

- **Clean Architecture**: The backend follows clean architecture principles with clear separation of concerns
- **JWT Authentication**: Secure API endpoints with JSON Web Tokens
- **PostgreSQL Database**: Persistent data storage with Entity Framework Core
- **Secure Password Hashing**: Using PBKDF2 with SHA256, 100,000 iterations, and random salts for password security
- **Book Management**: Full CRUD operations for books with filtering by author and category
- **User Authentication**: Registration and login with secure password hashing
- **RESTful API**: Full CRUD operations for product and book management
- **CORS Enabled**: API configured to accept requests from React frontend
- **Modern React**: Frontend built with React using Vite for fast development
- **Responsive UI**: Clean and responsive management interface
- **Unit Tests**: Comprehensive test coverage with xUnit

## Technologies Used

### Backend
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL (via Npgsql)
- JWT Bearer Authentication
- Clean Architecture pattern
- xUnit for testing

### Frontend
- React.js
- Vite
- CSS3

## Running Tests

Run all tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
cd tests/WorkShop.API.Tests
dotnet test --logger "console;verbosity=detailed"
```

## JWT Configuration

The JWT settings can be configured in `appsettings.json`:
```json
"Jwt": {
  "Secret": "YourSuperSecretKeyForJWTTokenGeneration12345",
  "Issuer": "WorkShopAPI",
  "Audience": "WorkShopClient"
}
```

**Important**: Change the `Secret` value in production to a secure random string (at least 32 characters).

## Security Best Practices

### Password Security
The application uses PBKDF2 (Password-Based Key Derivation Function 2) with the following configuration:
- **Algorithm**: SHA256
- **Iterations**: 100,000
- **Salt Size**: 16 bytes (randomly generated per password)
- **Hash Size**: 32 bytes

This provides strong protection against brute-force and rainbow table attacks.

### Configuration Security
For production deployments:
1. **Never commit sensitive credentials** to version control
2. Use **environment variables** or **Azure Key Vault** for:
   - Database connection strings
   - JWT secret keys
   - Any other sensitive configuration
3. See `appsettings.Sample.json` for the configuration template
4. Use **User Secrets** in development: `dotnet user-secrets init`

## Using the API with Authentication

1. Register a new user:
   ```bash
   curl -X POST http://localhost:5000/api/auth/register \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","email":"admin@example.com","password":"admin123"}'
   ```

2. Login to get a JWT token:
   ```bash
   curl -X POST http://localhost:5000/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

3. Use the token in subsequent requests:
   ```bash
   curl -X GET http://localhost:5000/api/books \
     -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
   ```