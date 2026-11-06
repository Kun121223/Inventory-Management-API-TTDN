using InventoryManagementApi.DTOs;
using InventoryManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApi.Controllers;

/// <summary>
/// REST endpoints for managing inventory items.
///
/// Routes:
///   GET    /api/items               - list all items
///   GET    /api/items/{id}          - get one item
///   GET    /api/items/low-stock     - items at or below reorder threshold
///   POST   /api/items               - create item
///   PUT    /api/items/{id}          - update item
///   PATCH  /api/items/{id}/stock    - adjust stock by delta
///   DELETE /api/items/{id}          - delete item
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetAll()
    {
        var items = await _itemService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemResponseDto>> GetById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);
        return Ok(item);
    }

    /// <summary>Returns items that need to be reordered.</summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetLowStock()
    {
        var items = await _itemService.GetLowStockAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ItemResponseDto>> Create([FromBody] ItemRequestDto request)
    {
        // [ApiController] auto-validates the model and returns 400 with field errors
        // if validation fails — no manual ModelState.IsValid check needed.
        var created = await _itemService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemResponseDto>> Update(int id, [FromBody] ItemRequestDto request)
    {
        var updated = await _itemService.UpdateAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// Adjusts stock by a positive (restock) or negative (sale) delta.
    /// Example body: { "delta": -3 }
    /// </summary>
    [HttpPatch("{id:int}/stock")]
    public async Task<ActionResult<ItemResponseDto>> AdjustStock(int id, [FromBody] StockAdjustmentDto adjustment)
    {
        var result = await _itemService.AdjustStockAsync(id, adjustment.Delta);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _itemService.DeleteAsync(id);
        return NoContent();
    }

    // API 13 - Search items
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> Search(
        [FromQuery] string keyword)
    {
        var items = await _itemService.SearchAsync(keyword);
        return Ok(items);
    }


    // API 14 - Out of stock items
    [HttpGet("out-of-stock")]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetOutOfStock()
    {
        var items = await _itemService.GetOutOfStockAsync();
        return Ok(items);
    }


    // API 15 - Items by supplier
    [HttpGet("by-supplier/{supplierId:int}")]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetBySupplier(
        int supplierId)
    {
        var items = await _itemService.GetBySupplierAsync(supplierId);
        return Ok(items);
    }


    // API 16 - Total stock value
    [HttpGet("stock-value")]
    public async Task<IActionResult> GetStockValue()
    {
        var totalValue = await _itemService.GetStockValueAsync();

        return Ok(new
        {
            totalValue
        });
    }


    // API 17 - Update item price
    [HttpPatch("{id:int}/price")]
    public async Task<ActionResult<ItemResponseDto>> UpdatePrice(
        int id,
        [FromBody] UpdatePriceDto request)
    {
        var item = await _itemService.UpdatePriceAsync(
            id, request.UnitPrice!.Value);

        return Ok(item);
    }

}

/// <summary>Tiny DTO for the stock-adjustment endpoint.</summary>
public record StockAdjustmentDto(int Delta);
