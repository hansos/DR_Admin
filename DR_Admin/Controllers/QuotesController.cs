using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages sales quotes and proposals
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;
    private static readonly Serilog.ILogger _log = Log.ForContext<QuotesController>();

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    /// <summary>
    /// Retrieves all quotes in the system
    /// </summary>
    /// <returns>List of all quotes</returns>
    /// <response code="200">Returns the list of quotes</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Sales,Support")]
    [ProducesResponseType(typeof(IEnumerable<QuoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetAllQuotes()
    {
        try
        {
            _log.Information("API: GetAllQuotes called by user {User}", User.Identity?.Name);
            var quotes = await _quoteService.GetAllQuotesAsync();
            return Ok(quotes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllQuotes");
            return StatusCode(500, "An error occurred while retrieving quotes");
        }
    }

    /// <summary>
    /// Retrieves a specific quote by ID
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The quote details</returns>
    /// <response code="200">Returns the quote</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Sales,Support")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuoteDto>> GetQuoteById(int id)
    {
        try
        {
            _log.Information("API: GetQuoteById called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);
            var quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found", id);
                return NotFound($"Quote with ID {id} not found");
            }

            return Ok(quote);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetQuoteById for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while retrieving the quote");
        }
    }

    /// <summary>
    /// Retrieves all quotes for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of quotes for the customer</returns>
    /// <response code="200">Returns the list of quotes</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,Sales,Support")]
    [ProducesResponseType(typeof(IEnumerable<QuoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetQuotesByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetQuotesByCustomerId called for customer: {CustomerId} by user {User}", customerId, User.Identity?.Name);
            var quotes = await _quoteService.GetQuotesByCustomerIdAsync(customerId);
            return Ok(quotes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetQuotesByCustomerId for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving quotes");
        }
    }

    /// <summary>
    /// Retrieves quotes filtered by status
    /// </summary>
    /// <param name="status">The quote status</param>
    /// <returns>List of quotes with the specified status</returns>
    /// <response code="200">Returns the list of quotes</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,Sales,Support")]
    [ProducesResponseType(typeof(IEnumerable<QuoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetQuotesByStatus(QuoteStatus status)
    {
        try
        {
            _log.Information("API: GetQuotesByStatus called for status: {Status} by user {User}", status, User.Identity?.Name);
            var quotes = await _quoteService.GetQuotesByStatusAsync(status);
            return Ok(quotes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetQuotesByStatus for status: {Status}", status);
            return StatusCode(500, "An error occurred while retrieving quotes");
        }
    }

    /// <summary>
    /// Creates a new quote
    /// </summary>
    /// <param name="createDto">The quote creation data</param>
    /// <returns>The created quote</returns>
    /// <response code="201">Returns the newly created quote</response>
    /// <response code="400">If the quote data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuoteDto>> CreateQuote([FromBody] CreateQuoteDto createDto)
    {
        try
        {
            _log.Information("API: CreateQuote called by user {User}", User.Identity?.Name);

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

            var quote = await _quoteService.CreateQuoteAsync(createDto, userId);
            
            _log.Information("API: Quote created with ID: {QuoteId}", quote.Id);
            return CreatedAtAction(nameof(GetQuoteById), new { id = quote.Id }, quote);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateQuote");
            return StatusCode(500, "An error occurred while creating the quote");
        }
    }

    /// <summary>
    /// Updates an existing quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <param name="updateDto">The quote update data</param>
    /// <returns>The updated quote</returns>
    /// <response code="200">Returns the updated quote</response>
    /// <response code="400">If the quote data is invalid</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuoteDto>> UpdateQuote(int id, [FromBody] UpdateQuoteDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateQuote called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quote = await _quoteService.UpdateQuoteAsync(id, updateDto);

            if (quote == null)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found for update", id);
                return NotFound($"Quote with ID {id} not found");
            }

            _log.Information("API: Quote updated with ID: {QuoteId}", id);
            return Ok(quote);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateQuote for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while updating the quote");
        }
    }

    /// <summary>
    /// Deletes a quote (soft delete)
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the quote was successfully deleted</response>
    /// <response code="404">If the quote is not found</response>
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
    public async Task<ActionResult> DeleteQuote(int id)
    {
        try
        {
            _log.Information("API: DeleteQuote called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            var result = await _quoteService.DeleteQuoteAsync(id);

            if (!result)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found for deletion", id);
                return NotFound($"Quote with ID {id} not found");
            }

            _log.Information("API: Quote deleted with ID: {QuoteId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteQuote for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while deleting the quote");
        }
    }

    /// <summary>
    /// Sends a quote to the customer via email
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the quote was successfully sent</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/send")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendQuote(int id)
    {
        try
        {
            _log.Information("API: SendQuote called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            var result = await _quoteService.SendQuoteAsync(id);

            if (!result)
            {
                _log.Warning("API: Failed to send quote with ID {QuoteId}", id);
                return NotFound($"Quote with ID {id} not found or could not be sent");
            }

            _log.Information("API: Quote sent with ID: {QuoteId}", id);
            return Ok(new { message = "Quote sent successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SendQuote for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while sending the quote");
        }
    }

    /// <summary>
    /// Accepts a quote using the acceptance token (public endpoint for customers)
    /// </summary>
    /// <param name="token">The acceptance token</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the quote was successfully accepted</response>
    /// <response code="404">If the quote is not found or token is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("accept/{token}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AcceptQuote(string token)
    {
        try
        {
            _log.Information("API: AcceptQuote called with token");

            var result = await _quoteService.AcceptQuoteAsync(token);

            if (!result)
            {
                _log.Warning("API: Invalid or expired acceptance token");
                return NotFound("Invalid or expired quote acceptance link");
            }

            _log.Information("API: Quote accepted successfully");
            return Ok(new { message = "Quote accepted successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AcceptQuote");
            return StatusCode(500, "An error occurred while accepting the quote");
        }
    }

    /// <summary>
    /// Rejects a quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <param name="reason">The rejection reason</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the quote was successfully rejected</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RejectQuote(int id, [FromBody] string reason)
    {
        try
        {
            _log.Information("API: RejectQuote called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            var result = await _quoteService.RejectQuoteAsync(id, reason);

            if (!result)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found for rejection", id);
                return NotFound($"Quote with ID {id} not found");
            }

            _log.Information("API: Quote rejected with ID: {QuoteId}", id);
            return Ok(new { message = "Quote rejected successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RejectQuote for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while rejecting the quote");
        }
    }

    /// <summary>
    /// Converts a quote to an order
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The created order ID</returns>
    /// <response code="200">Returns the created order ID</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/convert")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> ConvertQuoteToOrder(int id)
    {
        try
        {
            _log.Information("API: ConvertQuoteToOrder called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            var orderId = await _quoteService.ConvertQuoteToOrderAsync(id);

            if (orderId == null)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found for conversion", id);
                return NotFound($"Quote with ID {id} not found or cannot be converted");
            }

            _log.Information("API: Quote {QuoteId} converted to order {OrderId}", id, orderId);
            return Ok(new { orderId = orderId.Value, message = "Quote converted to order successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ConvertQuoteToOrder for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while converting the quote");
        }
    }

    /// <summary>
    /// Generates a PDF for the quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The PDF file</returns>
    /// <response code="200">Returns the PDF file</response>
    /// <response code="404">If the quote is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/pdf")]
    [Authorize(Roles = "Admin,Sales,Support")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GenerateQuotePdf(int id)
    {
        try
        {
            _log.Information("API: GenerateQuotePdf called for ID: {QuoteId} by user {User}", id, User.Identity?.Name);

            var pdfBytes = await _quoteService.GenerateQuotePdfAsync(id);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                _log.Warning("API: Quote with ID {QuoteId} not found for PDF generation", id);
                return NotFound($"Quote with ID {id} not found");
            }

            _log.Information("API: PDF generated for quote {QuoteId}", id);
            return File(pdfBytes, "application/pdf", $"Quote-{id}.pdf");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GenerateQuotePdf for ID: {QuoteId}", id);
            return StatusCode(500, "An error occurred while generating the PDF");
        }
    }
}
