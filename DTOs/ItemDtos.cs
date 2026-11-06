using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApi.DTOs;

/// <summary>
/// Request payload for creating or updating an inventory item.
/// Decoupling DTOs from entities follows the Single Responsibility
/// Principle — the API contract evolves independently of the database.
/// </summary>
public class ItemRequestDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Sku { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0.0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 10;

    public int? SupplierId { get; set; }
}

/// <summary>
/// Response payload returned to clients.
/// Includes computed fields like IsLowStock to save the client a calculation.
/// </summary>
public class ItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdatePriceDto
{
    [Required(ErrorMessage = "UnitPrice is required.")]
    [Range(typeof(decimal), "0", "9999999999999999.99",
        ErrorMessage = "UnitPrice must be a valid non-negative price.")]
    public decimal? UnitPrice { get; set; }
}
