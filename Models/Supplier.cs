using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApi.Models;

/// <summary>
/// Represents a supplier/vendor that provides inventory items.
/// One supplier can have many items (one-to-many relationship).
/// </summary>
public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property — items provided by this supplier
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
