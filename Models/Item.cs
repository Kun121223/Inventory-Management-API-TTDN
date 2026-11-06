using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementApi.Models;

/// <summary>
/// Represents a single inventory item (e.g., a product or part).
/// Mapped to the "Items" table by EF Core conventions.
/// </summary>
public class Item
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Sku { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Threshold below which the item is considered low-stock.
    /// Used by the low-stock report endpoint.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 10;

    // ===== Foreign key to Supplier =====
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    // ===== Auditing =====
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
