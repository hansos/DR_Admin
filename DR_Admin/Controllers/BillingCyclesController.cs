using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BillingCyclesController : ControllerBase
{
    private readonly IBillingCycleService _billingCycleService;
    private readonly Serilog.ILogger _logger;

    public BillingCyclesController(IBillingCycleService billingCycleService, Serilog.ILogger logger)
    {
        _billingCycleService = billingCycleService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<BillingCycleDto>>> GetAllBillingCycles()
    {
        try
        {
            _logger.Information("API: GetAllBillingCycles called by user {User}", User.Identity?.Name);
            var billingCycles = await _billingCycleService.GetAllBillingCyclesAsync();
            return Ok(billingCycles);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetAllBillingCycles");
            return StatusCode(500, "An error occurred while retrieving billing cycles");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<BillingCycleDto>> GetBillingCycleById(int id)
    {
        try
        {
            _logger.Information("API: GetBillingCycleById called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);
            var billingCycle = await _billingCycleService.GetBillingCycleByIdAsync(id);

            if (billingCycle == null)
            {
                _logger.Information("API: Billing cycle with ID {BillingCycleId} not found", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return Ok(billingCycle);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetBillingCycleById for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while retrieving the billing cycle");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BillingCycleDto>> CreateBillingCycle([FromBody] CreateBillingCycleDto createDto)
    {
        try
        {
            _logger.Information("API: CreateBillingCycle called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for CreateBillingCycle");
                return BadRequest(ModelState);
            }

            var billingCycle = await _billingCycleService.CreateBillingCycleAsync(createDto);
            return CreatedAtAction(nameof(GetBillingCycleById), new { id = billingCycle.Id }, billingCycle);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in CreateBillingCycle");
            return StatusCode(500, "An error occurred while creating the billing cycle");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BillingCycleDto>> UpdateBillingCycle(int id, [FromBody] UpdateBillingCycleDto updateDto)
    {
        try
        {
            _logger.Information("API: UpdateBillingCycle called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for UpdateBillingCycle");
                return BadRequest(ModelState);
            }

            var billingCycle = await _billingCycleService.UpdateBillingCycleAsync(id, updateDto);

            if (billingCycle == null)
            {
                _logger.Information("API: Billing cycle with ID {BillingCycleId} not found for update", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return Ok(billingCycle);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in UpdateBillingCycle for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while updating the billing cycle");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBillingCycle(int id)
    {
        try
        {
            _logger.Information("API: DeleteBillingCycle called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);
            var result = await _billingCycleService.DeleteBillingCycleAsync(id);

            if (!result)
            {
                _logger.Information("API: Billing cycle with ID {BillingCycleId} not found for deletion", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in DeleteBillingCycle for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while deleting the billing cycle");
        }
    }
}
