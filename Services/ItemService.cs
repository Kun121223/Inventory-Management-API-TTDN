using InventoryManagementApi.Data;
using InventoryManagementApi.DTOs;
using InventoryManagementApi.Exceptions;
using InventoryManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApi.Services;

/// <summary>
/// Concrete implementation of IItemService.
/// Handles entity-DTO mapping, validation, and database operations.
/// </summary>
public class ItemService : IItemService
{
    private readonly InventoryDbContext _db;

    // Constructor injection — EF's DbContext is registered as scoped,
    // so the framework supplies a fresh instance per HTTP request.
    public ItemService(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ItemResponseDto>> GetAllAsync()
    {
        // Include() eagerly loads the related Supplier so we can show its name.
        // AsNoTracking() skips change-tracking for a read-only query (faster).
        var items = await _db.Items
            .Include(i => i.Supplier)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(ToDto);
    }

    public async Task<ItemResponseDto> GetByIdAsync(int id)
    {
        var item = await _db.Items
            .Include(i => i.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item is null)
            throw new NotFoundException($"Item with id {id} not found.");

        return ToDto(item);
    }

    /// <summary>
    /// Returns items whose stock has fallen at or below their reorder threshold.
    /// Useful for auto-generating purchase orders.
    /// </summary>
    public async Task<IEnumerable<ItemResponseDto>> GetLowStockAsync()
    {
        var items = await _db.Items
            .Include(i => i.Supplier)
            .Where(i => i.Quantity <= i.ReorderLevel)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(ToDto);
    }

    public async Task<ItemResponseDto> CreateAsync(ItemRequestDto request)
    {
        // Validate that the referenced supplier exists, if one was provided.
        if (request.SupplierId.HasValue)
        {
            var supplierExists = await _db.Suppliers
                .AnyAsync(s => s.Id == request.SupplierId.Value);
            if (!supplierExists)
                throw new NotFoundException($"Supplier with id {request.SupplierId} not found.");
        }

        var item = new Item
        {
            Name = request.Name,
            Sku = request.Sku,
            Description = request.Description,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            ReorderLevel = request.ReorderLevel,
            SupplierId = request.SupplierId
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        // Reload with supplier so the response includes the supplier name.
        await _db.Entry(item).Reference(i => i.Supplier).LoadAsync();
        return ToDto(item);
    }

    public async Task<ItemResponseDto> UpdateAsync(int id, ItemRequestDto request)
    {
        var item = await _db.Items.FindAsync(id)
            ?? throw new NotFoundException($"Item with id {id} not found.");

        if (request.SupplierId.HasValue && request.SupplierId != item.SupplierId)
        {
            var supplierExists = await _db.Suppliers
                .AnyAsync(s => s.Id == request.SupplierId.Value);
            if (!supplierExists)
                throw new NotFoundException($"Supplier with id {request.SupplierId} not found.");
        }

        item.Name = request.Name;
        item.Sku = request.Sku;
        item.Description = request.Description;
        item.Quantity = request.Quantity;
        item.UnitPrice = request.UnitPrice;
        item.ReorderLevel = request.ReorderLevel;
        item.SupplierId = request.SupplierId;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _db.Entry(item).Reference(i => i.Supplier).LoadAsync();
        return ToDto(item);
    }

    /// <summary>
    /// Adjusts stock by a delta (positive for restock, negative for sale/use).
    /// Throws if the adjustment would push quantity below zero.
    /// </summary>
    public async Task<ItemResponseDto> AdjustStockAsync(int id, int delta)
    {
        var item = await _db.Items
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new NotFoundException($"Item with id {id} not found.");

        var newQuantity = item.Quantity + delta;
        if (newQuantity < 0)
            throw new ArgumentException(
                $"Adjustment would result in negative stock (current: {item.Quantity}, delta: {delta}).");

        item.Quantity = newQuantity;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(item);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.Items.FindAsync(id)
            ?? throw new NotFoundException($"Item with id {id} not found.");

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Centralized entity-to-DTO mapping.
    /// Keeping this in one place prevents inconsistent response shapes.
    /// </summary>

    //API 13 – Tìm kiếm sản phẩm theo tên
    public async Task<IEnumerable<ItemResponseDto>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("Keyword cannot be empty.");
        }

        keyword = keyword.Trim();

        var items = await _db.Items
            .Include(i => i.Supplier)
            .Where(i => i.Name.Contains(keyword))
            .AsNoTracking()
            .ToListAsync();

        return items.Select(ToDto);
    }

    //Danh sách sản phẩm hết hàng
    public async Task<IEnumerable<ItemResponseDto>> GetOutOfStockAsync()
    {
        var items = await _db.Items
            .Include(i => i.Supplier)
            .Where(i => i.Quantity == 0)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(ToDto);
    }

    //Danh sách sản phẩm theo nhà cung cấp
    public async Task<IEnumerable<ItemResponseDto>> GetBySupplierAsync(int supplierId)
    {
        var items = await _db.Items
            .Include(i => i.Supplier)
            .Where(i => i.SupplierId == supplierId)
            .AsNoTracking()
            .ToListAsync();

        return items.Select(ToDto);
    }

    //Thống kê tổng giá trị hàng tồn kho
    public async Task<decimal> GetStockValueAsync()
    {
        var totalValue = await _db.Items
            .AsNoTracking()
            .SumAsync(i => (decimal?)(i.Quantity * i.UnitPrice));

        return totalValue ?? 0m;
    }

    //Cập nhật giá sản phẩm

    public async Task<ItemResponseDto> UpdatePriceAsync(int id, decimal newPrice)
    {
        if (newPrice < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }

        var item = await _db.Items
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            throw new NotFoundException($"Item with id {id} not found.");
        }

        item.UnitPrice = newPrice;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ToDto(item);
    }


    private static ItemResponseDto ToDto(Item item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Sku = item.Sku,
        Description = item.Description,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        ReorderLevel = item.ReorderLevel,
        IsLowStock = item.Quantity <= item.ReorderLevel,
        SupplierId = item.SupplierId,
        SupplierName = item.Supplier?.Name,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };
}
