using InventoryManagementApi.DTOs;

namespace InventoryManagementApi.Services;

/// <summary>
/// Service contract for inventory item operations.
/// Programming to an interface (not implementation) follows the
/// Dependency Inversion Principle and makes the code easier to test/mock.
/// </summary>
public interface IItemService
{
    Task<IEnumerable<ItemResponseDto>> GetAllAsync();
    Task<ItemResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<ItemResponseDto>> GetLowStockAsync();
    Task<ItemResponseDto> CreateAsync(ItemRequestDto request);
    Task<ItemResponseDto> UpdateAsync(int id, ItemRequestDto request);
    Task<ItemResponseDto> AdjustStockAsync(int id, int delta);
    Task DeleteAsync(int id);

    // 5 phương thức mới
    Task<IEnumerable<ItemResponseDto>> SearchAsync(string keyword);
    Task<IEnumerable<ItemResponseDto>> GetOutOfStockAsync();
    Task<IEnumerable<ItemResponseDto>> GetBySupplierAsync(int supplierId);
    Task<decimal> GetStockValueAsync();
    Task<ItemResponseDto> UpdatePriceAsync(int id, decimal newPrice);
}
