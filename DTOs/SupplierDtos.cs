using System.ComponentModel.DataAnnotations;

namespace InventoryManagementApi.DTOs;

public class SupplierRequestDto
{
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
}

public class SupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
