using InventoryManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApi.Data;

/// <summary>
/// Entity Framework Core DbContext — the bridge between code and database.
/// Each DbSet maps to a database table.
/// </summary>
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Item -> Supplier relationship.
        // Deleting a supplier should NOT cascade-delete its items;
        // instead, set the SupplierId to null on those items.
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Supplier)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index on SKU for faster lookups (SKUs are commonly searched)
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Sku);

        base.OnModelCreating(modelBuilder);
    }
}
