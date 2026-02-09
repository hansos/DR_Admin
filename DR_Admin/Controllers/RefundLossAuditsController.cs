using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages refund loss audits
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RefundLossAuditsController : ControllerBase
{
    private readonly IRefundLossAuditService _refundLossAuditService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RefundLossAuditsController>();

    public RefundLossAuditsController(IRefundLossAuditService refundLossAuditService)
    {
        _refundLossAuditService = refundLossAuditService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<RefundLossAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RefundLossAuditDto>>> GetAllRefundLossAudits()
    {
        try
        {
            _log.Information("API: GetAllRefundLossAudits called by user {User}", User.Identity?.Name);
            var audits = await _refundLossAuditService.GetAllRefundLossAuditsAsync();
            return Ok(audits);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRefundLossAudits");
            return StatusCode(500, "An error occurred while retrieving refund loss audits");
        }
    }

    [HttpGet("paged")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PagedResult<RefundLossAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RefundLossAuditDto>>> GetAllRefundLossAuditsPaged(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            _log.Information("API: GetAllRefundLossAuditsPaged called by user {User}", User.Identity?.Name);
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var audits = await _refundLossAuditService.GetAllRefundLossAuditsPagedAsync(parameters);
            return Ok(audits);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRefundLossAuditsPaged");
            return StatusCode(500, "An error occurred while retrieving refund loss audits");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(RefundLossAuditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefundLossAuditDto>> GetRefundLossAuditById(int id)
    {
        try
        {
            _log.Information("API: GetRefundLossAuditById called for ID: {Id}", id);
            var audit = await _refundLossAuditService.GetRefundLossAuditByIdAsync(id);
            if (audit == null)
                return NotFound($"Refund loss audit with ID {id} not found");
            return Ok(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRefundLossAuditById for ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the refund loss audit");
        }
    }

    [HttpGet("refund/{refundId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(RefundLossAuditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefundLossAuditDto>> GetRefundLossAuditByRefundId(int refundId)
    {
        try
        {
            _log.Information("API: GetRefundLossAuditByRefundId called for refund: {RefundId}", refundId);
            var audit = await _refundLossAuditService.GetRefundLossAuditByRefundIdAsync(refundId);
            if (audit == null)
                return NotFound($"Refund loss audit for refund ID {refundId} not found");
            return Ok(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRefundLossAuditByRefundId");
            return StatusCode(500, "An error occurred while retrieving the refund loss audit");
        }
    }

    [HttpGet("invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<RefundLossAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RefundLossAuditDto>>> GetRefundLossAuditsByInvoiceId(int invoiceId)
    {
        try
        {
            _log.Information("API: GetRefundLossAuditsByInvoiceId called for invoice: {InvoiceId}", invoiceId);
            var audits = await _refundLossAuditService.GetRefundLossAuditsByInvoiceIdAsync(invoiceId);
            return Ok(audits);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRefundLossAuditsByInvoiceId");
            return StatusCode(500, "An error occurred while retrieving refund loss audits");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(RefundLossAuditDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RefundLossAuditDto>> CreateRefundLossAudit([FromBody] CreateRefundLossAuditDto createDto)
    {
        try
        {
            _log.Information("API: CreateRefundLossAudit called for refund: {RefundId}", createDto.RefundId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audit = await _refundLossAuditService.CreateRefundLossAuditAsync(createDto);
            _log.Information("API: Refund loss audit created with ID: {Id}", audit.Id);
            return CreatedAtAction(nameof(GetRefundLossAuditById), new { id = audit.Id }, audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRefundLossAudit");
            return StatusCode(500, "An error occurred while creating the refund loss audit");
        }
    }

    [HttpPost("approve")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(RefundLossAuditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefundLossAuditDto>> ApproveRefundLoss([FromBody] ApproveRefundLossDto approveDto)
    {
        try
        {
            _log.Information("API: ApproveRefundLoss called for audit ID: {Id}", approveDto.RefundLossAuditId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audit = await _refundLossAuditService.ApproveRefundLossAsync(approveDto);
            if (audit == null)
                return NotFound($"Refund loss audit with ID {approveDto.RefundLossAuditId} not found");

            return Ok(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ApproveRefundLoss");
            return StatusCode(500, "An error occurred while approving the refund loss");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRefundLossAudit(int id)
    {
        try
        {
            _log.Information("API: DeleteRefundLossAudit called for ID: {Id}", id);
            var result = await _refundLossAuditService.DeleteRefundLossAuditAsync(id);
            if (!result)
                return NotFound($"Refund loss audit with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRefundLossAudit for ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the refund loss audit");
        }
    }
}
