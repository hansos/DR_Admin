using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages vendor costs
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class VendorCostsController : ControllerBase
{
    private readonly IVendorCostService _vendorCostService;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorCostsController>();

    public VendorCostsController(IVendorCostService vendorCostService)
    {
        _vendorCostService = vendorCostService;
    }

    /// <summary>
    /// Retrieves all vendor costs in the system
    /// </summary>
    /// <returns>List of all vendor costs</returns>
    /// <response code="200">Returns the list of vendor costs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<VendorCostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VendorCostDto>>> GetAllVendorCosts()
    {
        try
        {
            _log.Information("API: GetAllVendorCosts called by user {User}", User.Identity?.Name);
            var costs = await _vendorCostService.GetAllVendorCostsAsync();
            return Ok(costs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllVendorCosts");
            return StatusCode(500, "An error occurred while retrieving vendor costs");
        }
    }

    /// <summary>
    /// Retrieves paginated vendor costs
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1)</param>
    /// <param name="pageSize">The page size (default: 20)</param>
    /// <returns>Paginated list of vendor costs</returns>
    /// <response code="200">Returns the paginated list of vendor costs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("paged")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PagedResult<VendorCostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<VendorCostDto>>> GetAllVendorCostsPaged(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _log.Information("API: GetAllVendorCostsPaged called (Page: {PageNumber}, Size: {PageSize}) by user {User}", 
                pageNumber, pageSize, User.Identity?.Name);
            
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var costs = await _vendorCostService.GetAllVendorCostsPagedAsync(parameters);
            return Ok(costs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllVendorCostsPaged");
            return StatusCode(500, "An error occurred while retrieving vendor costs");
        }
    }

    /// <summary>
    /// Retrieves a specific vendor cost by ID
    /// </summary>
    /// <param name="id">The vendor cost ID</param>
    /// <returns>The vendor cost details</returns>
    /// <response code="200">Returns the vendor cost</response>
    /// <response code="404">If the vendor cost is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(VendorCostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VendorCostDto>> GetVendorCostById(int id)
    {
        try
        {
            _log.Information("API: GetVendorCostById called for ID: {Id} by user {User}", id, User.Identity?.Name);
            var cost = await _vendorCostService.GetVendorCostByIdAsync(id);

            if (cost == null)
            {
                _log.Warning("API: Vendor cost with ID {Id} not found", id);
                return NotFound($"Vendor cost with ID {id} not found");
            }

            return Ok(cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorCostById for ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the vendor cost");
        }
    }

    /// <summary>
    /// Retrieves vendor costs by invoice line ID
    /// </summary>
    /// <param name="invoiceLineId">The invoice line ID</param>
    /// <returns>List of vendor costs for the invoice line</returns>
    /// <response code="200">Returns the list of vendor costs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("invoice-line/{invoiceLineId}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(IEnumerable<VendorCostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VendorCostDto>>> GetVendorCostsByInvoiceLineId(int invoiceLineId)
    {
        try
        {
            _log.Information("API: GetVendorCostsByInvoiceLineId called for invoice line: {InvoiceLineId} by user {User}", 
                invoiceLineId, User.Identity?.Name);
            var costs = await _vendorCostService.GetVendorCostsByInvoiceLineIdAsync(invoiceLineId);
            return Ok(costs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorCostsByInvoiceLineId for invoice line: {InvoiceLineId}", invoiceLineId);
            return StatusCode(500, "An error occurred while retrieving vendor costs");
        }
    }

    /// <summary>
    /// Retrieves vendor costs by payout ID
    /// </summary>
    /// <param name="payoutId">The payout ID</param>
    /// <returns>List of vendor costs for the payout</returns>
    /// <response code="200">Returns the list of vendor costs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("payout/{payoutId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<VendorCostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VendorCostDto>>> GetVendorCostsByPayoutId(int payoutId)
    {
        try
        {
            _log.Information("API: GetVendorCostsByPayoutId called for payout: {PayoutId} by user {User}", 
                payoutId, User.Identity?.Name);
            var costs = await _vendorCostService.GetVendorCostsByPayoutIdAsync(payoutId);
            return Ok(costs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorCostsByPayoutId for payout: {PayoutId}", payoutId);
            return StatusCode(500, "An error occurred while retrieving vendor costs");
        }
    }

    /// <summary>
    /// Retrieves vendor cost summary for an invoice
    /// </summary>
    /// <param name="invoiceId">The invoice ID</param>
    /// <returns>The vendor cost summary</returns>
    /// <response code="200">Returns the vendor cost summary</response>
    /// <response code="404">If the invoice is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("summary/invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorCostSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VendorCostSummaryDto>> GetVendorCostSummaryByInvoiceId(int invoiceId)
    {
        try
        {
            _log.Information("API: GetVendorCostSummaryByInvoiceId called for invoice: {InvoiceId} by user {User}", 
                invoiceId, User.Identity?.Name);
            var summary = await _vendorCostService.GetVendorCostSummaryByInvoiceIdAsync(invoiceId);

            if (summary == null)
            {
                _log.Warning("API: Vendor cost summary for invoice {InvoiceId} not found", invoiceId);
                return NotFound($"Invoice with ID {invoiceId} not found");
            }

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorCostSummaryByInvoiceId for invoice: {InvoiceId}", invoiceId);
            return StatusCode(500, "An error occurred while retrieving vendor cost summary");
        }
    }

    /// <summary>
    /// Creates a new vendor cost
    /// </summary>
    /// <param name="createDto">The vendor cost creation data</param>
    /// <returns>The created vendor cost</returns>
    /// <response code="201">Returns the newly created vendor cost</response>
    /// <response code="400">If the vendor cost data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorCostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VendorCostDto>> CreateVendorCost([FromBody] CreateVendorCostDto createDto)
    {
        try
        {
            _log.Information("API: CreateVendorCost called for invoice line: {InvoiceLineId} by user {User}", 
                createDto.InvoiceLineId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cost = await _vendorCostService.CreateVendorCostAsync(createDto);
            
            _log.Information("API: Vendor cost created with ID: {Id}", cost.Id);
            return CreatedAtAction(nameof(GetVendorCostById), new { id = cost.Id }, cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateVendorCost");
            return StatusCode(500, "An error occurred while creating the vendor cost");
        }
    }

    /// <summary>
    /// Updates an existing vendor cost
    /// </summary>
    /// <param name="id">The vendor cost ID</param>
    /// <param name="updateDto">The updated vendor cost data</param>
    /// <returns>The updated vendor cost</returns>
    /// <response code="200">Returns the updated vendor cost</response>
    /// <response code="400">If the vendor cost data is invalid</response>
    /// <response code="404">If the vendor cost is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorCostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VendorCostDto>> UpdateVendorCost(int id, [FromBody] UpdateVendorCostDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateVendorCost called for ID: {Id} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cost = await _vendorCostService.UpdateVendorCostAsync(id, updateDto);

            if (cost == null)
            {
                _log.Warning("API: Vendor cost with ID {Id} not found for update", id);
                return NotFound($"Vendor cost with ID {id} not found");
            }

            _log.Information("API: Vendor cost with ID {Id} updated successfully", id);
            return Ok(cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateVendorCost for ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the vendor cost");
        }
    }

    /// <summary>
    /// Deletes a vendor cost
    /// </summary>
    /// <param name="id">The vendor cost ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the vendor cost was successfully deleted</response>
    /// <response code="404">If the vendor cost is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteVendorCost(int id)
    {
        try
        {
            _log.Information("API: DeleteVendorCost called for ID: {Id} by user {User}", id, User.Identity?.Name);

            var result = await _vendorCostService.DeleteVendorCostAsync(id);

            if (!result)
            {
                _log.Warning("API: Vendor cost with ID {Id} not found for deletion", id);
                return NotFound($"Vendor cost with ID {id} not found");
            }

            _log.Information("API: Vendor cost with ID {Id} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteVendorCost for ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the vendor cost");
        }
    }
}
