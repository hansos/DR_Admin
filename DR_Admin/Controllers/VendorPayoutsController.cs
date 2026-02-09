using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages vendor payouts
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class VendorPayoutsController : ControllerBase
{
    private readonly IVendorPayoutService _vendorPayoutService;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorPayoutsController>();

    public VendorPayoutsController(IVendorPayoutService vendorPayoutService)
    {
        _vendorPayoutService = vendorPayoutService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<VendorPayoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendorPayoutDto>>> GetAllVendorPayouts()
    {
        try
        {
            _log.Information("API: GetAllVendorPayouts called by user {User}", User.Identity?.Name);
            var payouts = await _vendorPayoutService.GetAllVendorPayoutsAsync();
            return Ok(payouts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllVendorPayouts");
            return StatusCode(500, "An error occurred while retrieving vendor payouts");
        }
    }

    [HttpGet("paged")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PagedResult<VendorPayoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<VendorPayoutDto>>> GetAllVendorPayoutsPaged(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            _log.Information("API: GetAllVendorPayoutsPaged called by user {User}", User.Identity?.Name);
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var payouts = await _vendorPayoutService.GetAllVendorPayoutsPagedAsync(parameters);
            return Ok(payouts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllVendorPayoutsPaged");
            return StatusCode(500, "An error occurred while retrieving vendor payouts");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(VendorPayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorPayoutDto>> GetVendorPayoutById(int id)
    {
        try
        {
            _log.Information("API: GetVendorPayoutById called for ID: {Id}", id);
            var payout = await _vendorPayoutService.GetVendorPayoutByIdAsync(id);
            if (payout == null)
                return NotFound($"Vendor payout with ID {id} not found");
            return Ok(payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorPayoutById for ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the vendor payout");
        }
    }

    [HttpGet("vendor/{vendorId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<VendorPayoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendorPayoutDto>>> GetVendorPayoutsByVendorId(int vendorId)
    {
        try
        {
            _log.Information("API: GetVendorPayoutsByVendorId called for vendor: {VendorId}", vendorId);
            var payouts = await _vendorPayoutService.GetVendorPayoutsByVendorIdAsync(vendorId);
            return Ok(payouts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorPayoutsByVendorId");
            return StatusCode(500, "An error occurred while retrieving vendor payouts");
        }
    }

    [HttpGet("summary/vendor/{vendorId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorPayoutSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorPayoutSummaryDto>> GetVendorPayoutSummaryByVendorId(int vendorId)
    {
        try
        {
            _log.Information("API: GetVendorPayoutSummaryByVendorId called for vendor: {VendorId}", vendorId);
            var summary = await _vendorPayoutService.GetVendorPayoutSummaryByVendorIdAsync(vendorId);
            if (summary == null)
                return NotFound($"No payouts found for vendor ID {vendorId}");
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorPayoutSummaryByVendorId");
            return StatusCode(500, "An error occurred while retrieving vendor payout summary");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorPayoutDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VendorPayoutDto>> CreateVendorPayout([FromBody] CreateVendorPayoutDto createDto)
    {
        try
        {
            _log.Information("API: CreateVendorPayout called for vendor: {VendorId}", createDto.VendorId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payout = await _vendorPayoutService.CreateVendorPayoutAsync(createDto);
            _log.Information("API: Vendor payout created with ID: {Id}", payout.Id);
            return CreatedAtAction(nameof(GetVendorPayoutById), new { id = payout.Id }, payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateVendorPayout");
            return StatusCode(500, "An error occurred while creating the vendor payout");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorPayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorPayoutDto>> UpdateVendorPayout(int id, [FromBody] UpdateVendorPayoutDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateVendorPayout called for ID: {Id}", id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payout = await _vendorPayoutService.UpdateVendorPayoutAsync(id, updateDto);
            if (payout == null)
                return NotFound($"Vendor payout with ID {id} not found");

            return Ok(payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateVendorPayout for ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the vendor payout");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVendorPayout(int id)
    {
        try
        {
            _log.Information("API: DeleteVendorPayout called for ID: {Id}", id);
            var result = await _vendorPayoutService.DeleteVendorPayoutAsync(id);
            if (!result)
                return NotFound($"Vendor payout with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteVendorPayout for ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the vendor payout");
        }
    }

    [HttpPost("process")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorPayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorPayoutDto>> ProcessVendorPayout([FromBody] ProcessVendorPayoutDto processDto)
    {
        try
        {
            _log.Information("API: ProcessVendorPayout called for payout ID: {Id}", processDto.VendorPayoutId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payout = await _vendorPayoutService.ProcessVendorPayoutAsync(processDto);
            if (payout == null)
                return NotFound($"Vendor payout with ID {processDto.VendorPayoutId} not found");

            return Ok(payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ProcessVendorPayout");
            return StatusCode(500, "An error occurred while processing the vendor payout");
        }
    }

    [HttpPost("resolve-intervention")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorPayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorPayoutDto>> ResolvePayoutIntervention([FromBody] ResolvePayoutInterventionDto resolveDto)
    {
        try
        {
            _log.Information("API: ResolvePayoutIntervention called for payout ID: {Id}", resolveDto.VendorPayoutId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payout = await _vendorPayoutService.ResolvePayoutInterventionAsync(resolveDto);
            if (payout == null)
                return NotFound($"Vendor payout with ID {resolveDto.VendorPayoutId} not found");

            return Ok(payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ResolvePayoutIntervention");
            return StatusCode(500, "An error occurred while resolving the payout intervention");
        }
    }
}
