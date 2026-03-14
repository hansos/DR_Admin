using ISPAdmin.DTOs;
using ISPAdmin.Infrastructure;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides read operations for communication threads.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CommunicationThreadsController : ControllerBase
{
    private readonly ICommunicationThreadService _communicationThreadService;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IMyAccountService _myAccountService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CommunicationThreadsController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationThreadsController"/> class.
    /// </summary>
    /// <param name="communicationThreadService">The communication thread service.</param>
    /// <param name="emailQueueService">The email queue service.</param>
    /// <param name="myAccountService">The current-account service used for customer scoping.</param>
    public CommunicationThreadsController(
        ICommunicationThreadService communicationThreadService,
        IEmailQueueService emailQueueService,
        IMyAccountService myAccountService)
    {
        _communicationThreadService = communicationThreadService;
        _emailQueueService = emailQueueService;
        _myAccountService = myAccountService;
    }

    /// <summary>
    /// Retrieves communication threads with optional filters.
    /// </summary>
    /// <param name="customerId">Optional customer identifier filter.</param>
    /// <param name="userId">Optional user identifier filter.</param>
    /// <param name="relatedEntityType">Optional related entity type filter.</param>
    /// <param name="relatedEntityId">Optional related entity identifier filter.</param>
    /// <param name="status">Optional thread status filter.</param>
    /// <param name="search">Optional free-text search over subject and participants.</param>
    /// <param name="maxItems">Maximum number of items to return.</param>
    /// <returns>A list of communication threads.</returns>
    /// <response code="200">Returns communication threads.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "Communication.Read")]
    [ProducesResponseType(typeof(IEnumerable<CommunicationThreadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CommunicationThreadDto>>> GetThreads(
        [FromQuery] int? customerId = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? relatedEntityType = null,
        [FromQuery] int? relatedEntityId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] int maxItems = 50)
    {
        try
        {
            if (User.IsInRole(RoleNames.CUSTOMER))
            {
                var scope = await ResolveCurrentScopeAsync();
                if (!scope.HasValue)
                {
                    return Unauthorized();
                }

                customerId = scope.Value.customerId;
                userId = scope.Value.userId;
            }

            _log.Information("API: GetThreads called by user {User}", User.Identity?.Name);

            var items = await _communicationThreadService.GetThreadsAsync(
                customerId,
                userId,
                relatedEntityType,
                relatedEntityId,
                status,
                search,
                maxItems);

            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetThreads");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving communication threads");
        }
    }

    /// <summary>
    /// Retrieves a communication thread by identifier including participants and messages.
    /// </summary>
    /// <param name="id">The communication thread identifier.</param>
    /// <returns>Detailed communication thread data.</returns>
    /// <response code="200">Returns communication thread details.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="404">If thread was not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "Communication.Read")]
    [ProducesResponseType(typeof(CommunicationThreadDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CommunicationThreadDetailsDto>> GetThreadById(int id)
    {
        try
        {
            _log.Information("API: GetThreadById called for thread {ThreadId} by user {User}", id, User.Identity?.Name);

            var item = await _communicationThreadService.GetThreadByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Communication thread with ID {id} not found");
            }

            if (User.IsInRole(RoleNames.CUSTOMER))
            {
                var scope = await ResolveCurrentScopeAsync();
                if (!scope.HasValue)
                {
                    return Unauthorized();
                }

                var isOwnThread = IsOwnCustomerThread(item, scope.Value.customerId, scope.Value.userId);
                if (!isOwnThread)
                {
                    return Forbid();
                }
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetThreadById for thread {ThreadId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving communication thread details");
        }
    }

    /// <summary>
    /// Updates the status of a communication thread.
    /// </summary>
    /// <param name="id">The communication thread identifier.</param>
    /// <param name="dto">The target status payload.</param>
    /// <returns>No content on successful update.</returns>
    /// <response code="204">Thread status was updated.</response>
    /// <response code="400">If request payload is invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="404">If thread was not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "Communication.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateThreadStatus(int id, [FromBody] UpdateCommunicationThreadStatusDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Status is required.");
            }

            _log.Information("API: UpdateThreadStatus called for thread {ThreadId} by user {User}", id, User.Identity?.Name);

            if (User.IsInRole(RoleNames.CUSTOMER))
            {
                var scope = await ResolveCurrentScopeAsync();
                if (!scope.HasValue)
                {
                    return Unauthorized();
                }

                var thread = await _communicationThreadService.GetThreadByIdAsync(id);
                if (thread == null)
                {
                    return NotFound($"Communication thread with ID {id} not found");
                }

                if (!IsOwnCustomerThread(thread, scope.Value.customerId, scope.Value.userId))
                {
                    return Forbid();
                }
            }

            var updated = await _communicationThreadService.UpdateThreadStatusAsync(id, dto.Status);
            if (!updated)
            {
                return NotFound($"Communication thread with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateThreadStatus for thread {ThreadId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating communication thread status");
        }
    }

    /// <summary>
    /// Updates the read state of a communication message.
    /// </summary>
    /// <param name="messageId">The communication message identifier.</param>
    /// <param name="dto">The target read state payload.</param>
    /// <returns>No content on successful update.</returns>
    /// <response code="204">Message read state was updated.</response>
    /// <response code="400">If request payload is invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="404">If message was not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPatch("messages/{messageId:int}/read-state")]
    [Authorize(Policy = "Communication.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateMessageReadState(int messageId, [FromBody] UpdateCommunicationMessageReadStateDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Payload is required.");
            }

            _log.Information("API: UpdateMessageReadState called for message {MessageId} by user {User}", messageId, User.Identity?.Name);

            if (User.IsInRole(RoleNames.CUSTOMER))
            {
                var scope = await ResolveCurrentScopeAsync();
                if (!scope.HasValue)
                {
                    return Unauthorized();
                }

                var hasAccess = await _communicationThreadService.CanAccessMessageAsync(
                    messageId,
                    scope.Value.customerId,
                    scope.Value.userId);

                if (!hasAccess)
                {
                    return Forbid();
                }
            }

            var updated = await _communicationThreadService.UpdateMessageReadStateAsync(messageId, dto.IsRead);
            if (!updated)
            {
                return NotFound($"Communication message with ID {messageId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateMessageReadState for message {MessageId}", messageId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating communication message read state");
        }
    }

    /// <summary>
    /// Queues a reply email for a communication thread.
    /// </summary>
    /// <param name="id">The communication thread identifier.</param>
    /// <param name="dto">The reply payload.</param>
    /// <returns>Queue response details.</returns>
    /// <response code="200">Reply was queued successfully.</response>
    /// <response code="400">If payload is invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="404">If thread was not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("{id:int}/reply")]
    [Authorize(Policy = "Communication.Write")]
    [ProducesResponseType(typeof(QueueEmailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QueueEmailResponseDto>> QueueReply(int id, [FromBody] CreateCommunicationReplyDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Payload is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.To))
            {
                return BadRequest("Recipient (To) is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.BodyText) && string.IsNullOrWhiteSpace(dto.BodyHtml))
            {
                return BadRequest("At least one body format (BodyText or BodyHtml) is required.");
            }

            var thread = await _communicationThreadService.GetThreadByIdAsync(id);
            if (thread == null)
            {
                return NotFound($"Communication thread with ID {id} not found");
            }

            if (User.IsInRole(RoleNames.CUSTOMER))
            {
                var scope = await ResolveCurrentScopeAsync();
                if (!scope.HasValue)
                {
                    return Unauthorized();
                }

                if (!IsOwnCustomerThread(thread, scope.Value.customerId, scope.Value.userId))
                {
                    return Forbid();
                }
            }

            var subject = ResolveReplySubject(thread.Subject, dto.Subject);
            var response = await _emailQueueService.QueueEmailAsync(new QueueEmailDto
            {
                To = dto.To,
                Cc = dto.Cc,
                Bcc = dto.Bcc,
                Subject = subject,
                BodyText = dto.BodyText,
                BodyHtml = dto.BodyHtml,
                Provider = dto.Provider,
                CustomerId = thread.CustomerId,
                UserId = thread.UserId,
                RelatedEntityType = thread.RelatedEntityType,
                RelatedEntityId = thread.RelatedEntityId,
                AttachmentPaths = dto.AttachmentPaths
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in QueueReply for thread {ThreadId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while queuing communication reply");
        }
    }

    private static string ResolveReplySubject(string threadSubject, string? requestedSubject)
    {
        if (!string.IsNullOrWhiteSpace(requestedSubject))
        {
            return requestedSubject.Trim();
        }

        if (threadSubject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
        {
            return threadSubject;
        }

        return $"Re: {threadSubject}";
    }

    private async Task<(int customerId, int userId)?> ResolveCurrentScopeAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        var account = await _myAccountService.GetMyAccountAsync(userId);
        var customerId = account?.Customer?.Id ?? 0;
        if (customerId <= 0)
        {
            return null;
        }

        return (customerId, userId);
    }

    private static bool IsOwnCustomerThread(CommunicationThreadDetailsDto thread, int customerId, int userId)
    {
        return thread.CustomerId == customerId || thread.UserId == userId;
    }
}
