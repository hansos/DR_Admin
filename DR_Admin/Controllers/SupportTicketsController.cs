using ISPAdmin.Data;
using ISPAdmin.DTOs;
using ISPAdmin.Infrastructure;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides support ticket endpoints for customers and support staff.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SupportTicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ISupportTicketService _supportTicketService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SupportTicketsController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SupportTicketsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    /// <param name="supportTicketService">Support ticket service.</param>
    public SupportTicketsController(ApplicationDbContext context, ISupportTicketService supportTicketService)
    {
        _context = context;
        _supportTicketService = supportTicketService;
    }

    /// <summary>
    /// Retrieves support tickets scoped to the authenticated user.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="pageNumber">Optional page number (1-based).</param>
    /// <param name="pageSize">Optional page size.</param>
    /// <returns>Support tickets list or paged result.</returns>
    /// <response code="200">Returns support tickets.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "SupportTicket.Read")]
    [ProducesResponseType(typeof(IEnumerable<SupportTicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<SupportTicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetTickets([FromQuery] string? status = null, [FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            var userContext = await GetCurrentUserContextAsync();
            var tickets = await _supportTicketService.GetTicketsAsync(userContext.IsSupportUser, userContext.UserId, userContext.CustomerId, status, pageNumber, pageSize);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error while retrieving support tickets");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving support tickets");
        }
    }

    /// <summary>
    /// Retrieves one support ticket by identifier.
    /// </summary>
    /// <param name="id">Support ticket identifier.</param>
    /// <returns>The support ticket.</returns>
    /// <response code="200">Returns support ticket data.</response>
    /// <response code="404">If support ticket is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "SupportTicket.Read")]
    [ProducesResponseType(typeof(SupportTicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportTicketDto>> GetById(int id)
    {
        try
        {
            var userContext = await GetCurrentUserContextAsync();
            var ticket = await _supportTicketService.GetTicketByIdAsync(id, userContext.IsSupportUser, userContext.CustomerId);
            if (ticket == null)
            {
                return NotFound($"Support ticket with ID {id} not found");
            }

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error while retrieving support ticket {SupportTicketId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving support ticket");
        }
    }

    /// <summary>
    /// Creates a new support ticket for the authenticated customer.
    /// </summary>
    /// <param name="dto">Ticket creation payload.</param>
    /// <returns>The created support ticket.</returns>
    /// <response code="201">Returns created ticket.</response>
    /// <response code="400">If input data is invalid.</response>
    /// <response code="403">If user is not linked to a customer account.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [Authorize(Policy = "SupportTicket.Write")]
    [ProducesResponseType(typeof(SupportTicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportTicketDto>> Create([FromBody] CreateSupportTicketDto dto)
    {
        try
        {
            var userContext = await GetCurrentUserContextAsync();
            if (userContext.CustomerId == null)
            {
                return Forbid();
            }

            var created = await _supportTicketService.CreateTicketAsync(userContext.UserId, userContext.CustomerId.Value, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error while creating support ticket");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating support ticket");
        }
    }

    /// <summary>
    /// Adds a message to a support ticket conversation.
    /// </summary>
    /// <param name="id">Support ticket identifier.</param>
    /// <param name="dto">Message payload.</param>
    /// <returns>The updated support ticket.</returns>
    /// <response code="200">Returns updated support ticket with messages.</response>
    /// <response code="400">If input data is invalid.</response>
    /// <response code="404">If support ticket is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("{id}/messages")]
    [Authorize(Policy = "SupportTicket.Write")]
    [ProducesResponseType(typeof(SupportTicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportTicketDto>> AddMessage(int id, [FromBody] CreateSupportTicketMessageDto dto)
    {
        try
        {
            var userContext = await GetCurrentUserContextAsync();
            var updated = await _supportTicketService.AddMessageAsync(id, userContext.UserId, userContext.CustomerId, userContext.IsSupportUser, dto);
            if (updated == null)
            {
                return NotFound($"Support ticket with ID {id} not found");
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error while adding message to support ticket {SupportTicketId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding support message");
        }
    }

    /// <summary>
    /// Updates the status and assignee of a support ticket.
    /// </summary>
    /// <param name="id">Support ticket identifier.</param>
    /// <param name="dto">Status update payload.</param>
    /// <returns>The updated support ticket.</returns>
    /// <response code="200">Returns updated support ticket.</response>
    /// <response code="400">If status is invalid.</response>
    /// <response code="404">If support ticket is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = "SupportTicket.Manage")]
    [ProducesResponseType(typeof(SupportTicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportTicketDto>> UpdateStatus(int id, [FromBody] UpdateSupportTicketStatusDto dto)
    {
        try
        {
            var userContext = await GetCurrentUserContextAsync();
            var updated = await _supportTicketService.UpdateStatusAsync(id, dto.Status, userContext.UserId);
            if (updated == null)
            {
                return NotFound($"Support ticket with ID {id} not found");
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error while updating support ticket {SupportTicketId} status", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating support ticket");
        }
    }

    private async Task<(int UserId, int? CustomerId, bool IsSupportUser)> GetCurrentUserContextAsync()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claimValue, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        var customerId = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.CustomerId)
            .FirstOrDefaultAsync();

        var isSupportUser = User.IsInRole(RoleNames.ADMIN) || User.IsInRole(RoleNames.SUPPORT);
        return (userId, customerId, isSupportUser);
    }
}
