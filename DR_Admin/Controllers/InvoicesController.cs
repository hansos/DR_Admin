using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InvoicesController>();

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllInvoices()
    {
        try
        {
            _log.Information("API: GetAllInvoices called by user {User}", User.Identity?.Name);
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllInvoices");
            return StatusCode(500, "An error occurred while retrieving invoices");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        try
        {
            _log.Information("API: GetInvoiceById called for ID {InvoiceId} by user {User}", id, User.Identity?.Name);
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                _log.Information("API: Invoice with ID {InvoiceId} not found", id);
                return NotFound($"Invoice with ID {id} not found");
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoiceById for ID {InvoiceId}", id);
            return StatusCode(500, "An error occurred while retrieving the invoice");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] CreateInvoiceDto createDto)
    {
        try
        {
            _log.Information("API: CreateInvoice called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateInvoice");
                return BadRequest(ModelState);
            }

            var invoice = await _invoiceService.CreateInvoiceAsync(createDto);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateInvoice");
            return StatusCode(500, "An error occurred while creating the invoice");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateInvoice called for ID {InvoiceId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateInvoice");
                return BadRequest(ModelState);
            }

            var invoice = await _invoiceService.UpdateInvoiceAsync(id, updateDto);

            if (invoice == null)
            {
                _log.Information("API: Invoice with ID {InvoiceId} not found for update", id);
                return NotFound($"Invoice with ID {id} not found");
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateInvoice for ID {InvoiceId}", id);
            return StatusCode(500, "An error occurred while updating the invoice");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteInvoice(int id)
    {
        try
        {
            _log.Information("API: DeleteInvoice called for ID {InvoiceId} by user {User}", id, User.Identity?.Name);
            var result = await _invoiceService.DeleteInvoiceAsync(id);

            if (!result)
            {
                _log.Information("API: Invoice with ID {InvoiceId} not found for deletion", id);
                return NotFound($"Invoice with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteInvoice for ID {InvoiceId}", id);
            return StatusCode(500, "An error occurred while deleting the invoice");
        }
    }
}
