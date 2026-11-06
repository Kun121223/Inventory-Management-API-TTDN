using InventoryManagementApi.Data;
using InventoryManagementApi.Middleware;
using InventoryManagementApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Service Registration =====

// EF Core with SQL Server (production) — uses connection string from appsettings.json.
// For development without SQL Server installed, swap to UseInMemoryDatabase("InventoryDb").
builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback for quick local runs: in-memory database
        options.UseInMemoryDatabase("InventoryDb");
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Register application services.
// AddScoped: a new instance is created per HTTP request, which is the standard
// lifetime for services that work with EF Core (DbContext is also scoped).
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();

// MVC controllers + JSON serialization
builder.Services.AddControllers();

// Swagger for interactive API documentation at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Inventory Management API", Version = "v1" });
});

var app = builder.Build();

// ===== Middleware Pipeline =====

// Custom exception handler must come early so it catches errors from later middleware.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed the database with sample data on first run (in-memory only)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    DbSeeder.Seed(db);
}

app.Run();
