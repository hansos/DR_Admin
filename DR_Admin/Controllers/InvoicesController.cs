using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer invoices including creation, retrieval, updates, and deletion
/// </summary>
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

    /// <summary>
    /// Retrieves all invoices in the system
    /// </summary>
    /// <returns>List of all invoices</returns>
    /// <response code="200">Returns the list of invoices</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Retrieves all invoices for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>List of invoices for the specified customer</returns>
    /// <response code="200">Returns the list of customer invoices</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetInvoicesByCustomerId called for customer {CustomerId} by user {User}", customerId, User.Identity?.Name);
            var invoices = await _invoiceService.GetInvoicesByCustomerIdAsync(customerId);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoicesByCustomerId for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving invoices");
        }
    }

    /// <summary>
    /// Retrieves all invoices with a specific status
    /// </summary>
    /// <param name="status">The invoice status to filter by</param>
    /// <returns>List of invoices with the specified status</returns>
    /// <response code="200">Returns the list of invoices matching the status</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByStatus(InvoiceStatus status)
    {
        try
        {
            _log.Information("API: GetInvoicesByStatus called for status {Status} by user {User}", status, User.Identity?.Name);
            var invoices = await _invoiceService.GetInvoicesByStatusAsync(status);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoicesByStatus for status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving invoices");
        }
    }

    /// <summary>
    /// Retrieves a specific invoice by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <returns>The invoice information</returns>
    /// <response code="200">Returns the invoice data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If invoice is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Retrieves a specific invoice by its invoice number
    /// </summary>
    /// <param name="invoiceNumber">The invoice number</param>
    /// <returns>The invoice information</returns>
    /// <response code="200">Returns the invoice data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If invoice is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("number/{invoiceNumber}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceByNumber(string invoiceNumber)
    {
        try
        {
            _log.Information("API: GetInvoiceByNumber called for number {InvoiceNumber} by user {User}", invoiceNumber, User.Identity?.Name);
            var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);

            if (invoice == null)
            {
                _log.Information("API: Invoice with number {InvoiceNumber} not found", invoiceNumber);
                return NotFound($"Invoice with number {invoiceNumber} not found");
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetInvoiceByNumber for number {InvoiceNumber}", invoiceNumber);
            return StatusCode(500, "An error occurred while retrieving the invoice");
        }
    }

    /// <summary>
    /// Creates a new invoice in the system
    /// </summary>
    /// <param name="createDto">Invoice information for creation</param>
    /// <returns>The newly created invoice</returns>
    /// <response code="201">Returns the newly created invoice</response>
    /// <response code="400">If the invoice data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            
            return CreatedAtAction(
                nameof(GetInvoiceById),
                new { id = invoice.Id },
                invoice);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateInvoice");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateInvoice");
            return StatusCode(500, "An error occurred while creating the invoice");
        }
    }

    /// <summary>
    /// Updates an existing invoice's information
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to update</param>
    /// <param name="updateDto">Updated invoice information</param>
    /// <returns>The updated invoice</returns>
    /// <response code="200">Returns the updated invoice</response>
    /// <response code="400">If the invoice data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If invoice is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateInvoice");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateInvoice for ID {InvoiceId}", id);
            return StatusCode(500, "An error occurred while updating the invoice");
        }
    }

    /// <summary>
    /// Delete an invoice (soft delete)
    /// </summary>
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

