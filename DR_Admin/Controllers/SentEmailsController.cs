using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages sent email records including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SentEmailsController : ControllerBase
{
    private readonly ISentEmailService _sentEmailService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SentEmailsController>();

    public SentEmailsController(ISentEmailService sentEmailService)
    {
        _sentEmailService = sentEmailService;
    }

    /// <summary>
    /// Retrieves all sent email records in the system
    /// </summary>
    /// <returns>List of all sent emails</returns>
    /// <response code="200">Returns the list of sent emails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<SentEmailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SentEmailDto>>> GetAllSentEmails()
    {
        try
        {
            _log.Information("API: GetAllSentEmails called by user {User}", User.Identity?.Name);
            
            var emails = await _sentEmailService.GetAllSentEmailsAsync();
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllSentEmails");
            return StatusCode(500, "An error occurred while retrieving sent emails");
        }
    }

    /// <summary>
    /// Retrieves sent emails for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of sent emails for the customer</returns>
    /// <response code="200">Returns the list of sent emails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-customer/{customerId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<SentEmailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SentEmailDto>>> GetSentEmailsByCustomer(int customerId)
    {
        try
        {
            _log.Information("API: GetSentEmailsByCustomer called for customer {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            
            var emails = await _sentEmailService.GetSentEmailsByCustomerAsync(customerId);
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailsByCustomer for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving sent emails");
        }
    }

    /// <summary>
    /// Retrieves sent emails by a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of sent emails by the user</returns>
    /// <response code="200">Returns the list of sent emails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-user/{userId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<SentEmailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SentEmailDto>>> GetSentEmailsByUser(int userId)
    {
        try
        {
            _log.Information("API: GetSentEmailsByUser called for user {UserId} by user {User}", 
                userId, User.Identity?.Name);
            
            var emails = await _sentEmailService.GetSentEmailsByUserAsync(userId);
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailsByUser for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving sent emails");
        }
    }

    /// <summary>
    /// Retrieves sent emails related to a specific entity
    /// </summary>
    /// <param name="entityType">The entity type (e.g., Invoice, Order)</param>
    /// <param name="entityId">The entity ID</param>
    /// <returns>List of sent emails for the entity</returns>
    /// <response code="200">Returns the list of sent emails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-entity/{entityType}/{entityId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<SentEmailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SentEmailDto>>> GetSentEmailsByEntity(string entityType, int entityId)
    {
        try
        {
            _log.Information("API: GetSentEmailsByEntity called for entity {EntityType}/{EntityId} by user {User}", 
                entityType, entityId, User.Identity?.Name);
            
            var emails = await _sentEmailService.GetSentEmailsByRelatedEntityAsync(entityType, entityId);
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailsByEntity for entity {EntityType}/{EntityId}", 
                entityType, entityId);
            return StatusCode(500, "An error occurred while retrieving sent emails");
        }
    }

    /// <summary>
    /// Retrieves sent emails within a date range
    /// </summary>
    /// <param name="startDate">The start date (format: yyyy-MM-dd)</param>
    /// <param name="endDate">The end date (format: yyyy-MM-dd)</param>
    /// <returns>List of sent emails within the date range</returns>
    /// <response code="200">Returns the list of sent emails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-date-range")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<SentEmailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SentEmailDto>>> GetSentEmailsByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            _log.Information("API: GetSentEmailsByDateRange called for {StartDate} to {EndDate} by user {User}", 
                startDate, endDate, User.Identity?.Name);
            
            var emails = await _sentEmailService.GetSentEmailsByDateRangeAsync(startDate, endDate);
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailsByDateRange for {StartDate} to {EndDate}", 
                startDate, endDate);
            return StatusCode(500, "An error occurred while retrieving sent emails");
        }
    }

    /// <summary>
    /// Retrieves a sent email by message ID
    /// </summary>
    /// <param name="messageId">The message ID</param>
    /// <returns>The sent email details</returns>
    /// <response code="200">Returns the sent email</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sent email is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-message-id/{messageId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(SentEmailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SentEmailDto>> GetSentEmailByMessageId(string messageId)
    {
        try
        {
            _log.Information("API: GetSentEmailByMessageId called with message ID {MessageId} by user {User}", 
                messageId, User.Identity?.Name);
            
            var email = await _sentEmailService.GetSentEmailByMessageIdAsync(messageId);
            
            if (email == null)
            {
                return NotFound($"Sent email with message ID {messageId} not found");
            }
            
            return Ok(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailByMessageId for message ID {MessageId}", messageId);
            return StatusCode(500, "An error occurred while retrieving the sent email");
        }
    }

    /// <summary>
    /// Retrieves a specific sent email by ID
    /// </summary>
    /// <param name="id">The sent email ID</param>
    /// <returns>The sent email details</returns>
    /// <response code="200">Returns the sent email</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sent email is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(SentEmailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SentEmailDto>> GetSentEmailById(int id)
    {
        try
        {
            _log.Information("API: GetSentEmailById called with ID {SentEmailId} by user {User}", 
                id, User.Identity?.Name);
            
            var email = await _sentEmailService.GetSentEmailByIdAsync(id);
            
            if (email == null)
            {
                return NotFound($"Sent email with ID {id} not found");
            }
            
            return Ok(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSentEmailById for ID {SentEmailId}", id);
            return StatusCode(500, "An error occurred while retrieving the sent email");
        }
    }

    /// <summary>
    /// Creates a new sent email record
    /// </summary>
    /// <param name="createDto">The sent email creation data</param>
    /// <returns>The created sent email</returns>
    /// <response code="201">Returns the newly created sent email</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(SentEmailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SentEmailDto>> CreateSentEmail([FromBody] CreateSentEmailDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateSentEmail called by user {User}", User.Identity?.Name);
            
            var email = await _sentEmailService.CreateSentEmailAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetSentEmailById),
                new { id = email.Id },
                email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateSentEmail");
            return StatusCode(500, "An error occurred while creating the sent email record");
        }
    }

    /// <summary>
    /// Updates an existing sent email record
    /// </summary>
    /// <param name="id">The sent email ID to update</param>
    /// <param name="updateDto">The updated sent email data</param>
    /// <returns>The updated sent email</returns>
    /// <response code="200">Returns the updated sent email</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sent email is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(SentEmailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SentEmailDto>> UpdateSentEmail(int id, [FromBody] UpdateSentEmailDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateSentEmail called for ID {SentEmailId} by user {User}", 
                id, User.Identity?.Name);
            
            var email = await _sentEmailService.UpdateSentEmailAsync(id, updateDto);
            
            if (email == null)
            {
                return NotFound($"Sent email with ID {id} not found");
            }
            
            return Ok(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateSentEmail for ID {SentEmailId}", id);
            return StatusCode(500, "An error occurred while updating the sent email record");
        }
    }

    /// <summary>
    /// Deletes a sent email record
    /// </summary>
    /// <param name="id">The sent email ID to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the sent email was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sent email is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSentEmail(int id)
    {
        try
        {
            _log.Information("API: DeleteSentEmail called for ID {SentEmailId} by user {User}", 
                id, User.Identity?.Name);
            
            var deleted = await _sentEmailService.DeleteSentEmailAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Sent email with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteSentEmail for ID {SentEmailId}", id);
            return StatusCode(500, "An error occurred while deleting the sent email record");
        }
    }
}
