# Inventory Management REST API

A production-style RESTful Inventory Management API built with **C# / .NET 8** and **ASP.NET Core**, designed to manage items, suppliers, and stock levels for small-to-medium businesses. The project demonstrates clean architecture, SOLID principles, and Entity Framework Core best practices.

## Features

- Full CRUD operations for **Items** and **Suppliers**
- **Low-stock report** endpoint that flags items needing reorder
- **Stock adjustment** endpoint for restocks and sales (positive/negative deltas)
- One-to-many **supplier → items** relationship with safe deletion (items remain after supplier removal)
- Centralized **exception-handling middleware** with consistent JSON error responses
- **DTO pattern** to decouple the API contract from EF entities
- **Data annotation validation** on all incoming requests
- **Swagger / OpenAPI** documentation at `/swagger`
- Seeded sample data for instant demo

## Tech Stack

| Layer       | Technology                                    |
|-------------|-----------------------------------------------|
| Language    | C# 12                                         |
| Framework   | ASP.NET Core 8.0                              |
| ORM         | Entity Framework Core 8                       |
| Database    | SQL Server (production), In-Memory (dev)      |
| Docs        | Swagger / Swashbuckle                         |
| Build Tool  | .NET CLI                                      |

## Project Structure

```
inventory-management-api/
├── InventoryManagementApi.csproj
├── Program.cs                      # Entry point + DI setup + middleware pipeline
├── appsettings.json
├── appsettings.Development.json
├── Controllers/
│   ├── ItemsController.cs
│   └── SuppliersController.cs
├── Services/
│   ├── IItemService.cs / ItemService.cs
│   └── ISupplierService.cs / SupplierService.cs
├── Models/
│   ├── Item.cs
│   └── Supplier.cs
├── DTOs/
│   ├── ItemDtos.cs
│   └── SupplierDtos.cs
├── Data/
│   ├── InventoryDbContext.cs
│   └── DbSeeder.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
└── Exceptions/
    └── NotFoundException.cs
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- (Optional) SQL Server / SQL Server Express for production-mode connection

### Run the Application

```bash
git clone https://github.com/<your-username>/inventory-management-api.git
cd inventory-management-api

dotnet restore
dotnet run
```

By default the API runs on `https://localhost:7001` (or whatever port .NET assigns).
Swagger UI: `https://localhost:7001/swagger`

If no SQL Server connection string is configured in `appsettings.json`, the app falls back to an in-memory database seeded with sample suppliers and items — perfect for instant testing.

### Connect to SQL Server (optional)

Add a connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Then create and apply migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## API Endpoints

### Items

| Method | Endpoint                       | Description                              |
|--------|--------------------------------|------------------------------------------|
| GET    | `/api/items`                   | List all items                           |
| GET    | `/api/items/{id}`              | Get item by id                           |
| GET    | `/api/items/low-stock`         | List items at/below reorder level        |
| POST   | `/api/items`                   | Create a new item                        |
| PUT    | `/api/items/{id}`              | Update item                              |
| PATCH  | `/api/items/{id}/stock`        | Adjust stock by delta                    |
| DELETE | `/api/items/{id}`              | Delete item                              |

### Suppliers

| Method | Endpoint                       | Description                              |
|--------|--------------------------------|------------------------------------------|
| GET    | `/api/suppliers`               | List all suppliers                       |
| GET    | `/api/suppliers/{id}`          | Get supplier by id                       |
| POST   | `/api/suppliers`               | Create a new supplier                    |
| PUT    | `/api/suppliers/{id}`          | Update supplier                          |
| DELETE | `/api/suppliers/{id}`          | Delete supplier (items kept, link cleared) |

### Example — Create Item

```bash
curl -X POST https://localhost:7001/api/items \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Wireless Headphones",
    "sku": "HDP-005",
    "description": "Bluetooth over-ear headphones",
    "quantity": 30,
    "unitPrice": 49.99,
    "reorderLevel": 10,
    "supplierId": 1
  }'
```

### Example — Adjust Stock (sell 3 units)

```bash
curl -X PATCH https://localhost:7001/api/items/1/stock \
  -H "Content-Type: application/json" \
  -d '{ "delta": -3 }'
```

## Architecture Highlights

- **Layered architecture** — Controllers → Services → DbContext, with each layer having a single responsibility.
- **Dependency Injection** — services and DbContext registered in `Program.cs`; controllers/services depend only on interfaces.
- **DTO pattern** — request/response DTOs are separate from EF entities, so internal model changes don't break API consumers.
- **Centralized error handling** — `ExceptionHandlingMiddleware` catches all unhandled exceptions and returns consistent JSON.
- **EF best practices** — `AsNoTracking()` for read-only queries, eager loading via `Include()`, projection inside the query for efficient SQL.
- **Safe relationship deletes** — deleting a supplier sets `SupplierId` to null on its items rather than cascade-deleting them.

## Future Enhancements

- JWT authentication and role-based authorization
- Pagination, filtering, and sorting on list endpoints
- Audit log for all stock changes
- Background job to email low-stock alerts to suppliers
- Docker Compose setup with SQL Server container
- CI/CD pipeline via GitHub Actions

## License

MIT

## Author

**Daedeep Krishna Vuppala**
[LinkedIn](https://www.linkedin.com/in/daedeepkrishna-vuppala-754648229) | [Portfolio](https://cheerful-beignet-1befdd.netlify.app/)
