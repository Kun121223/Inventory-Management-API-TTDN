using InventoryManagementApi.DTOs;
using InventoryManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementApi.Controllers;

/// <summary>
/// REST endpoints for managing suppliers/vendors.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierResponseDto>>> GetAll()
    {
        return Ok(await _supplierService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierResponseDto>> GetById(int id)
    {
        return Ok(await _supplierService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponseDto>> Create([FromBody] SupplierRequestDto request)
    {
        var created = await _supplierService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierResponseDto>> Update(int id, [FromBody] SupplierRequestDto request)
    {
        return Ok(await _supplierService.UpdateAsync(id, request));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _supplierService.DeleteAsync(id);
        return NoContent();
    }
}
