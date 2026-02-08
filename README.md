# WorkShop - Online Bookstore Platform

This project is an online book shop for physical (paper) books, where customers can purchase books and have them delivered to a specified shipping address. The platform supports the full order lifecycle, from browsing and purchasing books to shipment and delivery tracking.

There are two types of users: Admins and Customers.

Admins (shop owners or authorized participants) are responsible for managing the store. They can add, update, and delete books, as well as manage customer orders, including updating shipping and tracking information.

Customers can browse the available books, add them to a shopping basket, and place orders. They can view detailed order information, including shipping status and tracking history. Customers can also register for a new account and recover their password if needed.

## Project Structure

```
WorkShop/
├── src/
│   ├── WorkShop.Domain/          # Domain layer - Core entities (Book, User, Order, OrderItem, Category)
│   ├── WorkShop.Application/     # Application layer - Business logic, DTOs, interfaces, validation rules
│   ├── WorkShop.Infrastructure/  # Infrastructure layer - Data access, service implementations, DbContext
│   └── WorkShop.API/             # API layer - Controllers, middleware, API endpoints
├── tests/
│   ├── WorkShop.API.Tests/           # API integration and unit tests
│   ├── WorkShop.Application.Tests/   # Application layer tests
│   └── WorkShop.Infrastructure.Tests/# Infrastructure layer tests
└── client/                       # React.js frontend application with Tailwind CSS
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

### Option 1: Running with Docker (Recommended)

The easiest way to run the complete application stack is using Docker Compose:

1. Clone the repository:
   ```bash
   git clone https://github.com/giobashvili89/WorkShop.git
   cd WorkShop
   ```

2. Start all services (API, React client, and PostgreSQL database):
   ```bash
   docker-compose up -d
   ```

3. Access the applications:
   - **React Client**: http://localhost:3000
   - **API**: http://localhost:5000
   - **PostgreSQL Database**: localhost:5432

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

6. Stop and remove all containers including volumes (this will delete all database data):
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

The `docker-compose.yml` file includes default environment variables for development. For production deployments, create a `.env` file with secure values:

```bash
cp .env.example .env
# Edit the .env file with your secure credentials
```

Example `.env` file configuration:
```env
POSTGRES_DB=workshop_db
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_secure_password
JWT_SECRET=your_super_secret_jwt_key_at_least_32_characters_long
```

### Option 2: Running Locally

#### Database Setup

1. Install PostgreSQL and create a database for the application:
   ```bash
   createdb workshop_db
   ```

2. Update the connection string in `src/WorkShop.API/appsettings.json` to match your PostgreSQL configuration:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=workshop_db;Username=postgres;Password=postgres"
   }
   ```

#### Backend (.NET API)

1. Navigate to the repository root directory:
   ```bash
   cd WorkShop
   ```

2. Restore NuGet packages and build the solution:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Run the API project:
   ```bash
   cd src/WorkShop.API
   dotnet run
   ```

   The API will be accessible at `http://localhost:5000` (HTTP) or `https://localhost:5001` (HTTPS)

#### Frontend (React)

1. Navigate to the client directory:
   ```bash
   cd client
   ```

2. Install npm dependencies:
   ```bash
   npm install
   ```

3. Start the Vite development server:
   ```bash
   npm run dev
   ```

   The React application will be available at `http://localhost:5173` (the exact port will be displayed in the terminal)

## API Endpoints

### Authentication (Public - No Authentication Required)
- `POST /api/auth/register` - Register a new customer account
- `POST /api/auth/login` - Login and receive JWT token with user role information

### Books (Public Read Access, Admin-Only Write Access)
- `GET /api/books` - Retrieve all available books (public)
- `GET /api/books/{id}` - Retrieve a specific book by ID (public)
- `GET /api/books/author/{author}` - Filter books by author name (public)
- `GET /api/books/category/{category}` - Filter books by category (public)
- `POST /api/books` - Add a new book to the catalog (Admin only)
- `PUT /api/books/{id}` - Update book information (Admin only)
- `DELETE /api/books/{id}` - Remove a book from the catalog (Admin only)

### Orders (Requires JWT Authentication)
- `POST /api/orders` - Place a new order with delivery information
- `GET /api/orders/{id}` - Retrieve order details including shipping and tracking information
- `GET /api/orders/my-orders` - Retrieve current user's complete order history
- `GET /api/orders` - Retrieve all orders (Admin only)
- `DELETE /api/orders/{id}` - Cancel an order

## Features

### User Roles

The platform supports two distinct user types:

#### Admin (Shop Owner/Participant)
- **Book Management**: Add new books to the catalog, update book information (title, author, price, stock), and remove books
- **Order Management**: View all customer orders and update tracking history with shipping information
- **Inventory Control**: Monitor and manage book stock levels

#### Customer (Buyer)
- **Book Browsing**: View the complete catalog of available books with detailed information
- **Shopping Cart**: Add books to basket and manage quantities before checkout
- **Order Placement**: Purchase books and provide delivery address information
- **Order Tracking**: View order details, shipping status, and tracking information
- **Account Management**: Register as a new user and recover forgotten passwords

### E-commerce Features
- **Book Shopping**: Browse physical books in an attractive card-based layout
- **Shopping Cart**: Add books to cart, adjust quantities, and proceed to checkout
- **Order Management**: Track order history, view shipping details, and cancel orders if needed
- **Delivery Management**: Customers provide shipping addresses; sellers update tracking information
- **Stock Management**: Real-time stock tracking with quantity validation to prevent overselling
- **Role-Based Access**: Administrators can manage inventory and orders; customers can browse and purchase

### Authentication & Authorization
- **JWT Authentication**: Secure API endpoints with JSON Web Tokens
- **Role-Based Authorization**: Separate Admin and Customer roles
- **Secure Password Hashing**: Using PBKDF2 with SHA256

### Technical Features
- **Clean Architecture**: Backend follows clean architecture principles with clear separation of concerns across Domain, Application, Infrastructure, and API layers
- **PostgreSQL Database**: Persistent data storage using Entity Framework Core with code-first migrations
- **Docker Support**: Complete Docker and Docker Compose configuration for streamlined deployment
- **Book Management**: Full CRUD (Create, Read, Update, Delete) operations with filtering capabilities by author and category
- **RESTful API**: Well-structured REST API implementation with appropriate HTTP status codes
- **CORS Configuration**: API configured to accept cross-origin requests from the React frontend
- **Modern React**: Frontend built with React 18+ using Vite build tool and Tailwind CSS for styling
- **Responsive Design**: Beautiful, responsive user interface optimized for desktop, tablet, and mobile devices
- **Comprehensive Testing**: Unit test coverage using xUnit testing framework
- **Input Validation**: Robust request validation using FluentValidation library

### Demo Accounts
The application includes pre-configured demo accounts for testing:
- **Admin Account**: 
  - Username: `admin`
  - Password: `admin1`
  - Capabilities: Full access to book and order management
- **Customer Account**: 
  - Username: `customer`
  - Password: `customer1`
  - Capabilities: Browse, purchase, and track orders

## Technologies Used

### DevOps & Deployment
- Docker & Docker Compose for containerization
- Multi-stage Docker builds for optimized container images
- PostgreSQL 16 Alpine for lightweight database container

### Backend
- .NET 10 SDK
- ASP.NET Core Web API
- Entity Framework Core 10 with code-first migrations
- PostgreSQL database (via Npgsql provider)
- JWT Bearer Authentication for secure API access
- Clean Architecture design pattern
- xUnit testing framework

### Frontend
- React.js 18+ with Hooks
- Vite - Next-generation frontend build tool
- Tailwind CSS for utility-first styling
- Modern ES6+ JavaScript/JSX

## Running Tests

Execute all tests across the solution:
```bash
dotnet test
```

Run tests with detailed console output:
```bash
cd tests/WorkShop.API.Tests
dotnet test --logger "console;verbosity=detailed"
```

Run tests for a specific project:
```bash
dotnet test tests/WorkShop.Application.Tests/WorkShop.Application.Tests.csproj
```

## JWT Configuration

JWT settings can be customized in the `appsettings.json` file:
```json
"Jwt": {
  "Secret": "YourSuperSecretKeyForJWTTokenGeneration12345",
  "Issuer": "WorkShopAPI",
  "Audience": "WorkShopClient"
}
```

**Security Warning**: Always change the `Secret` value in production environments to a cryptographically secure random string (minimum 32 characters recommended).

## Security Best Practices

### Password Security
The application implements PBKDF2 (Password-Based Key Derivation Function 2) for secure password hashing with the following configuration:
- **Algorithm**: SHA256
- **Iterations**: 100,000 (protection against brute-force attacks)
- **Salt Size**: 16 bytes (randomly generated per password)
- **Hash Size**: 32 bytes

This implementation provides robust protection against brute-force and rainbow table attacks.

### Configuration Security
For production deployments, follow these best practices:
1. **Never commit sensitive credentials** to version control systems
2. Use **environment variables** or **Azure Key Vault** for storing:
   - Database connection strings
   - JWT secret keys
   - Any other sensitive configuration values
3. Refer to `appsettings.Sample.json` for the configuration template
4. Use **User Secrets** during development: `dotnet user-secrets init`

## Using the API with Authentication

Example workflow for API authentication:

1. Register a new user account:
   ```bash
   curl -X POST http://localhost:5000/api/auth/register \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","email":"admin@example.com","password":"admin123"}'
   ```

2. Login to receive a JWT token:
   ```bash
   curl -X POST http://localhost:5000/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

3. Include the token in subsequent authenticated requests:
   ```bash
   curl -X GET http://localhost:5000/api/books \
     -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
   ```

## Documentation

For more detailed information about specific aspects of the project, please refer to the following documentation:

- **[Development Workflow](DEVELOPMENT_WORKFLOW.md)** - Development process and best practices for contributing to the project
- **[Error Handling Implementation](ERROR_HANDLING_IMPLEMENTATION.md)** - Error handling architecture and implementation details
- **[Routing Implementation](ROUTING_IMPLEMENTATION.md)** - Routing configuration and implementation guide
- **[Implementation Summary](IMPLEMENTATION_SUMMARY.md)** - Overview of key implementation decisions and architectural choices