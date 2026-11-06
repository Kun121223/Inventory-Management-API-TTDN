using InventoryManagementApi.DTOs;

namespace InventoryManagementApi.Services;

public interface ISupplierService
{
    Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
    Task<SupplierResponseDto> GetByIdAsync(int id);
    Task<SupplierResponseDto> CreateAsync(SupplierRequestDto request);
    Task<SupplierResponseDto> UpdateAsync(int id, SupplierRequestDto request);
    Task DeleteAsync(int id);
}
