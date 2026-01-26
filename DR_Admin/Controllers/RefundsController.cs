using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages payment refunds
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RefundsController : ControllerBase
{
    private readonly IRefundService _refundService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RefundsController>();

    public RefundsController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    /// <summary>
    /// Retrieves all refunds in the system
    /// </summary>
    /// <returns>List of all refunds</returns>
    /// <response code="200">Returns the list of refunds</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<RefundDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RefundDto>>> GetAllRefunds()
    {
        try
        {
            _log.Information("API: GetAllRefunds called by user {User}", User.Identity?.Name);
            var refunds = await _refundService.GetAllRefundsAsync();
            return Ok(refunds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRefunds");
            return StatusCode(500, "An error occurred while retrieving refunds");
        }
    }

    /// <summary>
    /// Retrieves a specific refund by ID
    /// </summary>
    /// <param name="id">The refund ID</param>
    /// <returns>The refund details</returns>
    /// <response code="200">Returns the refund</response>
    /// <response code="404">If the refund is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(RefundDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RefundDto>> GetRefundById(int id)
    {
        try
        {
            _log.Information("API: GetRefundById called for ID: {RefundId} by user {User}", id, User.Identity?.Name);
            var refund = await _refundService.GetRefundByIdAsync(id);

            if (refund == null)
            {
                _log.Warning("API: Refund with ID {RefundId} not found", id);
                return NotFound($"Refund with ID {id} not found");
            }

            return Ok(refund);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRefundById for ID: {RefundId}", id);
            return StatusCode(500, "An error occurred while retrieving the refund");
        }
    }

    /// <summary>
    /// Retrieves all refunds for a specific invoice
    /// </summary>
    /// <param name="invoiceId">The invoice ID</param>
    /// <returns>List of refunds for the invoice</returns>
    /// <response code="200">Returns the list of refunds</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<RefundDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RefundDto>>> GetRefundsByInvoiceId(int invoiceId)
    {
        try
        {
            _log.Information("API: GetRefundsByInvoiceId called for invoice: {InvoiceId} by user {User}", 
                invoiceId, User.Identity?.Name);
            var refunds = await _refundService.GetRefundsByInvoiceIdAsync(invoiceId);
            return Ok(refunds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRefundsByInvoiceId for invoice: {InvoiceId}", invoiceId);
            return StatusCode(500, "An error occurred while retrieving refunds");
        }
    }

    /// <summary>
    /// Creates a new refund
    /// </summary>
    /// <param name="createDto">The refund creation data</param>
    /// <returns>The created refund</returns>
    /// <response code="201">Returns the newly created refund</response>
    /// <response code="400">If the refund data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RefundDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RefundDto>> CreateRefund([FromBody] CreateRefundDto createDto)
    {
        try
        {
            _log.Information("API: CreateRefund called for payment transaction: {PaymentTransactionId} by user {User}", 
                createDto.PaymentTransactionId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _log.Warning("API: Unable to extract user ID from claims");
                return BadRequest("Unable to identify user");
            }

            var refund = await _refundService.CreateRefundAsync(createDto, userId);
            
            _log.Information("API: Refund created with ID: {RefundId}", refund.Id);
            return CreatedAtAction(nameof(GetRefundById), new { id = refund.Id }, refund);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRefund");
            return StatusCode(500, "An error occurred while creating the refund");
        }
    }

    /// <summary>
    /// Processes a pending refund
    /// </summary>
    /// <param name="id">The refund ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the refund was successfully processed</response>
    /// <response code="404">If the refund is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/process")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ProcessRefund(int id)
    {
        try
        {
            _log.Information("API: ProcessRefund called for ID: {RefundId} by user {User}", id, User.Identity?.Name);

            var result = await _refundService.ProcessRefundAsync(id);

            if (!result)
            {
                _log.Warning("API: Failed to process refund with ID {RefundId}", id);
                return NotFound($"Refund with ID {id} not found or could not be processed");
            }

            _log.Information("API: Refund processed with ID: {RefundId}", id);
            return Ok(new { message = "Refund processed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ProcessRefund for ID: {RefundId}", id);
            return StatusCode(500, "An error occurred while processing the refund");
        }
    }
}
