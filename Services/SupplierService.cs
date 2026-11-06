using InventoryManagementApi.Data;
using InventoryManagementApi.DTOs;
using InventoryManagementApi.Exceptions;
using InventoryManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApi.Services;

/// <summary>
/// Concrete implementation of ISupplierService.
/// </summary>
public class SupplierService : ISupplierService
{
    private readonly InventoryDbContext _db;

    public SupplierService(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync()
    {
        // Project directly to DTO inside the query so EF generates a single
        // efficient SQL query that includes the item count.
        return await _db.Suppliers
            .AsNoTracking()
            .Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                ItemCount = s.Items.Count,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<SupplierResponseDto> GetByIdAsync(int id)
    {
        var dto = await _db.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                ItemCount = s.Items.Count,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (dto is null)
            throw new NotFoundException($"Supplier with id {id} not found.");

        return dto;
    }

    public async Task<SupplierResponseDto> CreateAsync(SupplierRequestDto request)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };

        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();

        return ToDto(supplier, itemCount: 0);
    }

    public async Task<SupplierResponseDto> UpdateAsync(int id, SupplierRequestDto request)
    {
        var supplier = await _db.Suppliers
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"Supplier with id {id} not found.");

        supplier.Name = request.Name;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Address = request.Address;

        await _db.SaveChangesAsync();
        return ToDto(supplier, supplier.Items.Count);
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id)
            ?? throw new NotFoundException($"Supplier with id {id} not found.");

        // Note: items linked to this supplier will have their SupplierId set
        // to null automatically (configured via OnDelete in DbContext).
        _db.Suppliers.Remove(supplier);
        await _db.SaveChangesAsync();
    }

    private static SupplierResponseDto ToDto(Supplier s, int itemCount) => new()
    {
        Id = s.Id,
        Name = s.Name,
        ContactPerson = s.ContactPerson,
        Email = s.Email,
        Phone = s.Phone,
        Address = s.Address,
        ItemCount = itemCount,
        CreatedAt = s.CreatedAt
    };
}
