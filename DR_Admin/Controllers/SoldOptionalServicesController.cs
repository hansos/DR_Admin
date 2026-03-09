using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SoldOptionalServicesController : ControllerBase
{
    private readonly ISoldOptionalServiceService _service;
    private static readonly Serilog.ILogger _log = Log.ForContext<SoldOptionalServicesController>();

    public SoldOptionalServicesController(ISoldOptionalServiceService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SoldOptionalServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SoldOptionalServiceDto>>> GetAll()
    {
        try
        {
            return Ok(await _service.GetAllAsync());
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sold optional services");
            return StatusCode(500, "An error occurred while retrieving sold optional services");
        }
    }

    [HttpGet("customer/{customerId:int}")]
    [ProducesResponseType(typeof(IEnumerable<SoldOptionalServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SoldOptionalServiceDto>>> GetByCustomer(int customerId)
    {
        try
        {
            return Ok(await _service.GetByCustomerIdAsync(customerId));
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sold optional services for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving sold optional services");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SoldOptionalServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoldOptionalServiceDto>> GetById(int id)
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
            _log.Error(ex, "Error retrieving sold optional service {Id}", id);
            return StatusCode(500, "An error occurred while retrieving sold optional service");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(SoldOptionalServiceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SoldOptionalServiceDto>> Create([FromBody] CreateSoldOptionalServiceDto createDto)
    {
        try
        {
            var created = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating sold optional service");
            return StatusCode(500, "An error occurred while creating sold optional service");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SoldOptionalServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoldOptionalServiceDto>> Update(int id, [FromBody] UpdateSoldOptionalServiceDto updateDto)
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
            _log.Error(ex, "Error updating sold optional service {Id}", id);
            return StatusCode(500, "An error occurred while updating sold optional service");
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
            _log.Error(ex, "Error deleting sold optional service {Id}", id);
            return StatusCode(500, "An error occurred while deleting sold optional service");
        }
    }
}
