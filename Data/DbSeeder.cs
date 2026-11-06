using InventoryManagementApi.Models;

namespace InventoryManagementApi.Data;

/// <summary>
/// Seeds the database with realistic sample data on first run.
/// Useful for demos, development, and integration testing.
/// </summary>
public static class DbSeeder
{
    public static void Seed(InventoryDbContext db)
    {
        // Skip seeding if data already exists
        if (db.Suppliers.Any() || db.Items.Any())
            return;

        var acme = new Supplier
        {
            Name = "Acme Wholesale",
            ContactPerson = "John Smith",
            Email = "sales@acmewholesale.example",
            Phone = "555-0101",
            Address = "100 Industrial Way, Chicago, IL"
        };

        var globex = new Supplier
        {
            Name = "Globex Distribution",
            ContactPerson = "Jane Doe",
            Email = "orders@globex.example",
            Phone = "555-0202",
            Address = "250 Commerce Blvd, Dallas, TX"
        };

        db.Suppliers.AddRange(acme, globex);
        db.SaveChanges();

        db.Items.AddRange(
            new Item { Name = "USB-C Cable",     Sku = "CBL-001", Quantity = 150, UnitPrice = 9.99m,  ReorderLevel = 25, SupplierId = acme.Id },
            new Item { Name = "Wireless Mouse",  Sku = "MSE-002", Quantity = 8,   UnitPrice = 24.50m, ReorderLevel = 15, SupplierId = acme.Id },
            new Item { Name = "Mechanical Keyboard", Sku = "KBD-003", Quantity = 42, UnitPrice = 89.99m, ReorderLevel = 10, SupplierId = globex.Id },
            new Item { Name = "27\" Monitor",    Sku = "MON-004", Quantity = 5,   UnitPrice = 249.99m, ReorderLevel = 8,  SupplierId = globex.Id }
        );

        db.SaveChanges();
    }
}
