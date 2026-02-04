# WorkShop

A full-stack application with .NET 10 API (Clean Architecture) and React.js frontend.

## Project Structure

```
WorkShop/
├── src/
│   ├── WorkShop.Domain/          # Domain layer - Entities
│   ├── WorkShop.Application/     # Application layer - Business logic, DTOs, Interfaces
│   ├── WorkShop.Infrastructure/  # Infrastructure layer - Implementations
│   └── WorkShop.API/             # API layer - Controllers, Endpoints
└── client/                       # React.js frontend
```

## Prerequisites

- .NET 10 SDK
- Node.js (v16 or higher)
- npm or yarn

## Getting Started

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

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get a product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product

## Features

- **Clean Architecture**: The backend follows clean architecture principles with clear separation of concerns
- **RESTful API**: Full CRUD operations for product management
- **CORS Enabled**: API configured to accept requests from React frontend
- **Modern React**: Frontend built with React using Vite for fast development
- **Responsive UI**: Clean and responsive product management interface

## Technologies Used

### Backend
- .NET 10
- ASP.NET Core Web API
- Clean Architecture pattern

### Frontend
- React.js
- Vite
- CSS3