using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages invoice line items representing individual charges on invoices
/// </summary>
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
    /// Retrieves all invoice lines in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all invoice lines or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of invoice lines or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "InvoiceLine.Read")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceLineDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<InvoiceLineDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllInvoiceLines([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllInvoiceLines (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _invoiceLineService.GetAllInvoiceLinesPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

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
    /// Retrieves a specific invoice line by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the invoice line</param>
    /// <returns>The invoice line information</returns>
    /// <response code="200">Returns the invoice line data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If invoice line is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "InvoiceLine.Read")]
    [ProducesResponseType(typeof(InvoiceLineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves all line items for a specific invoice
    /// </summary>
    /// <param name="invoiceId">The unique identifier of the invoice</param>
    /// <returns>List of invoice lines for the specified invoice</returns>
    /// <response code="200">Returns the list of invoice lines</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceLineDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Creates a new invoice line item
    /// </summary>
    /// <param name="createDto">Invoice line information for creation</param>
    /// <returns>The newly created invoice line</returns>
    /// <response code="201">Returns the newly created invoice line</response>
    /// <response code="400">If the invoice line data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(InvoiceLineDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
