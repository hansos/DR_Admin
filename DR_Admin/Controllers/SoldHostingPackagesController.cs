using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SoldHostingPackagesController : ControllerBase
{
    private readonly ISoldHostingPackageService _service;
    private static readonly Serilog.ILogger _log = Log.ForContext<SoldHostingPackagesController>();

    public SoldHostingPackagesController(ISoldHostingPackageService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SoldHostingPackageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SoldHostingPackageDto>>> GetAll()
    {
        try
        {
            return Ok(await _service.GetAllAsync());
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sold hosting packages");
            return StatusCode(500, "An error occurred while retrieving sold hosting packages");
        }
    }

    [HttpGet("customer/{customerId:int}")]
    [ProducesResponseType(typeof(IEnumerable<SoldHostingPackageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SoldHostingPackageDto>>> GetByCustomer(int customerId)
    {
        try
        {
            return Ok(await _service.GetByCustomerIdAsync(customerId));
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sold hosting packages for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving sold hosting packages");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SoldHostingPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoldHostingPackageDto>> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sold hosting package {Id}", id);
            return StatusCode(500, "An error occurred while retrieving sold hosting package");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(SoldHostingPackageDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SoldHostingPackageDto>> Create([FromBody] CreateSoldHostingPackageDto createDto)
    {
        try
        {
            var created = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating sold hosting package");
            return StatusCode(500, "An error occurred while creating sold hosting package");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SoldHostingPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoldHostingPackageDto>> Update(int id, [FromBody] UpdateSoldHostingPackageDto updateDto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, updateDto);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating sold hosting package {Id}", id);
            return StatusCode(500, "An error occurred while updating sold hosting package");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting sold hosting package {Id}", id);
            return StatusCode(500, "An error occurred while deleting sold hosting package");
        }
    }
}
