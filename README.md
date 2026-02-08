# WorkShop - Book Selling Platform

A full-stack book selling application with .NET 10 API (Clean Architecture) and React.js frontend featuring JWT authentication, role-based access control, shopping cart, and PostgreSQL database.

## Project Structure

```
WorkShop/
├── src/
│   ├── WorkShop.Domain/          # Domain layer - Entities (Book, User, Order, OrderItem, Product)
│   ├── WorkShop.Application/     # Application layer - Business logic, DTOs, Interfaces, Validators
│   ├── WorkShop.Infrastructure/  # Infrastructure layer - Implementations, DbContext, Services
│   └── WorkShop.API/             # API layer - Controllers, Endpoints
├── tests/
│   └── WorkShop.API.Tests/       # Unit tests for API
└── client/                       # React.js frontend with Tailwind CSS
```

## Prerequisites

### For Docker (Recommended)
- Docker
- Docker Compose

### For Local Development
- .NET 10 SDK
- PostgreSQL 12 or higher
- Node.js (v16 or higher)
- npm or yarn

## Getting Started

> **Having database issues?** Check the [DATABASE_TROUBLESHOOTING.md](DATABASE_TROUBLESHOOTING.md) guide for solutions to common database errors including "relation 'Categories' does not exist" and migration issues.

### Option 1: Running with Docker (Recommended)

The easiest way to run the entire application stack is using Docker Compose:

1. Clone the repository:
   ```bash
   git clone https://github.com/giobashvili89/WorkShop.git
   cd WorkShop
   ```

2. Start all services (API, React client, and PostgreSQL):
   ```bash
   docker-compose up -d
   ```

3. Access the applications:
   - **React Client**: http://localhost:3000
   - **API**: http://localhost:5000
   - **PostgreSQL**: localhost:5432

4. View logs:
   ```bash
   # All services
   docker-compose logs -f
   
   # Specific service
   docker-compose logs -f api
   docker-compose logs -f client
   docker-compose logs -f postgres
   ```

5. Stop all services:
   ```bash
   docker-compose down
   ```

6. Stop and remove volumes (this will delete the database data):
   ```bash
   docker-compose down -v
   ```

#### Building Individual Docker Images

Build the API image:
```bash
docker build -t workshop-api -f src/WorkShop.API/Dockerfile .
```

Build the React client image:
```bash
docker build -t workshop-client -f client/Dockerfile ./client
```

> **Note**: If you encounter npm timeout issues during Docker build in restricted network environments, you may need to:
> - Build with `--network=host` flag
> - Pre-download dependencies locally before building
> - Use a local npm registry mirror

#### Docker Environment Variables

The `docker-compose.yml` file includes default environment variables. For production, copy `.env.example` to `.env` and update the values:

```bash
cp .env.example .env
# Edit .env with your secure values
```

Example `.env` file:
```env
POSTGRES_DB=workshop_db
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_secure_password
JWT_SECRET=your_super_secret_jwt_key_at_least_32_characters_long
```

### Option 2: Running Locally

#### Database Setup

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

#### Backend (.NET API)

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

#### Frontend (React)

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
- `POST /api/auth/register` - Register a new user (defaults to Customer role)
- `POST /api/auth/login` - Login and receive JWT token with user role

### Books (Public GET, Admin-only POST/PUT/DELETE)
- `GET /api/books` - Get all books (public)
- `GET /api/books/{id}` - Get a book by ID (public)
- `GET /api/books/author/{author}` - Filter books by author (public)
- `GET /api/books/category/{category}` - Filter books by category (public)
- `POST /api/books` - Create a new book (Admin only)
- `PUT /api/books/{id}` - Update a book (Admin only)
- `DELETE /api/books/{id}` - Delete a book (Admin only)

### Orders (Requires JWT authentication)
- `POST /api/orders` - Create a new order (place an order)
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/my-orders` - Get current user's order history
- `GET /api/orders` - Get all orders (Admin only)
- `DELETE /api/orders/{id}` - Cancel an order

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get a product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product

## Features

### E-commerce Features
- **Book Shopping**: Browse books in an attractive card-based layout
- **Shopping Cart**: Add books to cart, adjust quantities, and checkout
- **Order Management**: Track order history and cancel orders
- **Stock Management**: Real-time stock tracking with quantity validation
- **Role-Based Access**: Admin users can manage inventory, customers can browse and purchase

### Authentication & Authorization
- **JWT Authentication**: Secure API endpoints with JSON Web Tokens
- **Role-Based Authorization**: Separate Admin and Customer roles
- **Secure Password Hashing**: Using PBKDF2 with SHA256

### Technical Features
- **Clean Architecture**: Backend follows clean architecture principles with clear separation of concerns
- **PostgreSQL Database**: Persistent data storage with Entity Framework Core
- **Docker Support**: Complete Docker and Docker Compose configuration for easy deployment
- **Book Management**: Full CRUD operations with filtering by author and category
- **RESTful API**: Complete REST API with proper status codes
- **CORS Enabled**: API configured to accept requests from React frontend
- **Modern React**: Frontend built with React using Vite and Tailwind CSS
- **Responsive UI**: Beautiful, responsive interface optimized for all devices
- **Unit Tests**: Comprehensive test coverage with xUnit
- **FluentValidation**: Input validation with FluentValidation library

### Demo Accounts
The application is seeded with demo accounts:
- **Admin**: username: `admin`, password: `admin1`
- **Customer**: username: `customer`, password: `customer1`

## Technologies Used

### DevOps & Deployment
- Docker & Docker Compose
- Multi-stage builds for optimized images
- PostgreSQL 16 Alpine for database

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
- Tailwind CSS
- Modern ES6+ JavaScript

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