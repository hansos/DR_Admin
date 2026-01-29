using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages email queue operations for sending emails asynchronously
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmailQueueController : ControllerBase
{
    private readonly IEmailQueueService _emailQueueService;
    private static readonly Serilog.ILogger _log = Log.ForContext<EmailQueueController>();

    public EmailQueueController(IEmailQueueService emailQueueService)
    {
        _emailQueueService = emailQueueService;
    }

    /// <summary>
    /// Queues an email for asynchronous sending
    /// </summary>
    /// <param name="queueEmailDto">Email details to queue</param>
    /// <returns>Queue response with email ID and status</returns>
    /// <response code="201">Email queued successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("queue")]
    [Authorize(Policy = "EmailQueue.Write")]
    [ProducesResponseType(typeof(QueueEmailResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QueueEmailResponseDto>> QueueEmail([FromBody] QueueEmailDto queueEmailDto)
    {
        try
        {
            _log.Information("API: QueueEmail called by user {User} for recipient {To}", 
                User.Identity?.Name, queueEmailDto.To);

            if (string.IsNullOrWhiteSpace(queueEmailDto.To))
            {
                return BadRequest("Recipient (To) is required");
            }

            if (string.IsNullOrWhiteSpace(queueEmailDto.Subject))
            {
                return BadRequest("Subject is required");
            }

            if (string.IsNullOrWhiteSpace(queueEmailDto.BodyText) && string.IsNullOrWhiteSpace(queueEmailDto.BodyHtml))
            {
                return BadRequest("At least one body format (BodyText or BodyHtml) is required");
            }

            var result = await _emailQueueService.QueueEmailAsync(queueEmailDto);
            
            return CreatedAtAction(nameof(GetEmailStatus), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in QueueEmail for recipient {To}", queueEmailDto.To);
            return StatusCode(500, "An error occurred while queueing the email");
        }
    }

    /// <summary>
    /// Gets the status of a queued email
    /// </summary>
    /// <param name="id">Email ID</param>
    /// <returns>Email status information</returns>
    /// <response code="200">Returns email status</response>
    /// <response code="404">Email not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("status/{id}")]
    [Authorize(Policy = "EmailQueue.Read")]
    [ProducesResponseType(typeof(SentEmailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetEmailStatus(int id)
    {
        try
        {
            _log.Information("API: GetEmailStatus called for email ID {EmailId} by user {User}", 
                id, User.Identity?.Name);

            var email = await _emailQueueService.GetEmailByIdAsync(id);
            
            if (email == null)
            {
                return NotFound($"Email with ID {id} not found");
            }

            var dto = new SentEmailDto
            {
                Id = email.Id,
                SentDate = email.SentDate,
                From = email.From,
                To = email.To,
                Cc = email.Cc,
                Bcc = email.Bcc,
                Subject = email.Subject,
                Body = email.BodyHtml ?? email.BodyText, // For backwards compatibility
                MessageId = email.MessageId,
                Status = email.Status,
                ErrorMessage = email.ErrorMessage,
                RetryCount = email.RetryCount,
                CustomerId = email.CustomerId,
                UserId = email.UserId,
                RelatedEntityType = email.RelatedEntityType,
                RelatedEntityId = email.RelatedEntityId,
                Attachments = email.Attachments,
                CreatedAt = email.CreatedAt,
                UpdatedAt = email.UpdatedAt
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetEmailStatus for email ID {EmailId}", id);
            return StatusCode(500, "An error occurred while retrieving email status");
        }
    }
}
