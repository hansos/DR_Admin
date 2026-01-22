using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvoiceLinesController : ControllerBase
{
    private readonly IInvoiceLineService _invoiceLineService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InvoiceLinesController>();

    public InvoiceLinesController(IInvoiceLineService invoiceLineService)
    {
        _invoiceLineService = invoiceLineService;
    }

    /// <summary>
    /// Get all invoice lines
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<InvoiceLineDto>>> GetAllInvoiceLines()
    {
        try
        {
            _log.Information("API: GetAllInvoiceLines called by user {User}", User.Identity?.Name);
            
            var invoiceLines = await _invoiceLineService.GetAllInvoiceLinesAsync();
            return Ok(invoiceLines);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllInvoiceLines");
            return StatusCode(500, "An error occurred while retrieving invoice lines");
        }
    }

    /// <summary>
    /// Get invoice line by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<InvoiceLineDto>> GetInvoiceLineById(int id)
    {
        try
        {
            _log.Information("API: GetInvoiceLineById called for ID {InvoiceLineId} by user {User}", id, User.Identity?.Name);
            
            var invoiceLine = await _invoiceLineService.GetInvoiceLineByIdAsync(id);

            if (invoiceLine == null)
            {
                _log.Information("API: Invoice line with ID {InvoiceLineId} not found", id);
                return NotFound($"Invoice line with ID {id} not found");
            }

            return Ok(invoiceLine);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoiceLineById for ID {InvoiceLineId}", id);
            return StatusCode(500, "An error occurred while retrieving the invoice line");
        }
    }

    /// <summary>
    /// Get invoice lines by invoice ID
    /// </summary>
    [HttpGet("invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<InvoiceLineDto>>> GetInvoiceLinesByInvoiceId(int invoiceId)
    {
        try
        {
            _log.Information("API: GetInvoiceLinesByInvoiceId called for invoice ID {InvoiceId} by user {User}", invoiceId, User.Identity?.Name);
            
            var invoiceLines = await _invoiceLineService.GetInvoiceLinesByInvoiceIdAsync(invoiceId);
            return Ok(invoiceLines);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoiceLinesByInvoiceId for invoice ID {InvoiceId}", invoiceId);
            return StatusCode(500, "An error occurred while retrieving invoice lines");
        }
    }

    /// <summary>
    /// Create a new invoice line
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<InvoiceLineDto>> CreateInvoiceLine([FromBody] CreateInvoiceLineDto createDto)
    {
        try
        {
            _log.Information("API: CreateInvoiceLine called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateInvoiceLine");
                return BadRequest(ModelState);
            }

            var invoiceLine = await _invoiceLineService.CreateInvoiceLineAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetInvoiceLineById),
                new { id = invoiceLine.Id },
                invoiceLine);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateInvoiceLine");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateInvoiceLine");
            return StatusCode(500, "An error occurred while creating the invoice line");
        }
    }

    /// <summary>
    /// Update an existing invoice line
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<InvoiceLineDto>> UpdateInvoiceLine(int id, [FromBody] UpdateInvoiceLineDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateInvoiceLine called for ID {InvoiceLineId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateInvoiceLine");
                return BadRequest(ModelState);
            }

            var invoiceLine = await _invoiceLineService.UpdateInvoiceLineAsync(id, updateDto);

            if (invoiceLine == null)
            {
                _log.Information("API: Invoice line with ID {InvoiceLineId} not found for update", id);
                return NotFound($"Invoice line with ID {id} not found");
            }

            return Ok(invoiceLine);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateInvoiceLine");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateInvoiceLine for ID {InvoiceLineId}", id);
            return StatusCode(500, "An error occurred while updating the invoice line");
        }
    }

    /// <summary>
    /// Delete an invoice line
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteInvoiceLine(int id)
    {
        try
        {
            _log.Information("API: DeleteInvoiceLine called for ID {InvoiceLineId} by user {User}", id, User.Identity?.Name);

            var result = await _invoiceLineService.DeleteInvoiceLineAsync(id);

            if (!result)
            {
                _log.Information("API: Invoice line with ID {InvoiceLineId} not found for deletion", id);
                return NotFound($"Invoice line with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteInvoiceLine for ID {InvoiceLineId}", id);
            return StatusCode(500, "An error occurred while deleting the invoice line");
        }
    }
}
