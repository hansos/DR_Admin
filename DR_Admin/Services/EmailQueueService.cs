using System.Threading.Channels;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing email queue operations with in-memory notification
/// </summary>
public class EmailQueueService : IEmailQueueService
{
    private readonly ApplicationDbContext _context;
    private readonly Channel<int> _emailNotificationChannel;
    private static readonly Serilog.ILogger _log = Log.ForContext<EmailQueueService>();

    public EmailQueueService(ApplicationDbContext context, Channel<int> emailNotificationChannel)
    {
        _context = context;
        _emailNotificationChannel = emailNotificationChannel;
    }

    /// <summary>
    /// Queues an email for sending
    /// </summary>
    public async Task<QueueEmailResponseDto> QueueEmailAsync(QueueEmailDto queueEmailDto)
    {
        try
        {
            _log.Information("Queueing email to {To} with subject: {Subject}", 
                queueEmailDto.To, queueEmailDto.Subject);

            var email = new SentEmail
            {
                From = GetSystemFromAddress(),
                To = queueEmailDto.To,
                Cc = queueEmailDto.Cc,
                Bcc = queueEmailDto.Bcc,
                Subject = queueEmailDto.Subject,
                BodyText = queueEmailDto.BodyText,
                BodyHtml = queueEmailDto.BodyHtml,
                Status = EmailStatus.Pending,
                RetryCount = 0,
                MaxRetries = 3,
                NextAttemptAt = DateTime.UtcNow,
                Provider = queueEmailDto.Provider,
                CustomerId = queueEmailDto.CustomerId,
                UserId = queueEmailDto.UserId,
                RelatedEntityType = queueEmailDto.RelatedEntityType,
                RelatedEntityId = queueEmailDto.RelatedEntityId,
                Attachments = queueEmailDto.AttachmentPaths != null && queueEmailDto.AttachmentPaths.Any()
                    ? string.Join(";", queueEmailDto.AttachmentPaths)
                    : null,
                MessageId = Guid.NewGuid().ToString()
            };

            await _context.SentEmails.AddAsync(email);
            await _context.SaveChangesAsync();

            // Signal the background service via in-memory channel
            await _emailNotificationChannel.Writer.WriteAsync(email.Id);

            _log.Information("Successfully queued email with ID: {EmailId}", email.Id);

            return new QueueEmailResponseDto
            {
                Id = email.Id,
                Status = EmailStatus.Pending,
                Message = "Email queued successfully"
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error queueing email to {To}", queueEmailDto.To);
            throw;
        }
    }

    /// <summary>
    /// Gets pending emails ready to be sent
    /// </summary>
    public async Task<IEnumerable<int>> GetPendingEmailIdsAsync(int batchSize = 10)
    {
        try
        {
            var now = DateTime.UtcNow;
            var emailIds = await _context.SentEmails
                .AsNoTracking()
                .Where(e => (e.Status == EmailStatus.Pending || e.Status == EmailStatus.Ready) 
                    && e.NextAttemptAt <= now)
                .OrderBy(e => e.NextAttemptAt)
                .Take(batchSize)
                .Select(e => e.Id)
                .ToListAsync();

            _log.Information("Retrieved {Count} pending email(s) for processing", emailIds.Count);
            return emailIds;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving pending emails");
            throw;
        }
    }

    /// <summary>
    /// Marks an email as in-progress
    /// </summary>
    public async Task<bool> MarkEmailInProgressAsync(int emailId)
    {
        try
        {
            var email = await _context.SentEmails.FindAsync(emailId);
            if (email == null)
            {
                _log.Warning("Email with ID {EmailId} not found", emailId);
                return false;
            }

            if (email.Status != EmailStatus.Pending && email.Status != EmailStatus.Ready)
            {
                _log.Warning("Email {EmailId} is in invalid state for processing: {Status}", 
                    emailId, email.Status);
                return false;
            }

            email.Status = EmailStatus.InProgress;
            email.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            _log.Information("Marked email {EmailId} as in-progress", emailId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error marking email {EmailId} as in-progress", emailId);
            return false;
        }
    }

    /// <summary>
    /// Marks an email as successfully sent
    /// </summary>
    public async Task<bool> MarkEmailSentAsync(int emailId, string messageId)
    {
        try
        {
            var email = await _context.SentEmails.FindAsync(emailId);
            if (email == null)
            {
                _log.Warning("Email with ID {EmailId} not found", emailId);
                return false;
            }

            email.Status = EmailStatus.Sent;
            email.SentDate = DateTime.UtcNow;
            email.MessageId = messageId;
            email.ErrorMessage = null;
            email.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            _log.Information("Marked email {EmailId} as sent with message ID: {MessageId}", 
                emailId, messageId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error marking email {EmailId} as sent", emailId);
            return false;
        }
    }

    /// <summary>
    /// Marks an email as failed and schedules retry if applicable
    /// </summary>
    public async Task<bool> MarkEmailFailedAsync(int emailId, string errorMessage)
    {
        try
        {
            var email = await _context.SentEmails.FindAsync(emailId);
            if (email == null)
            {
                _log.Warning("Email with ID {EmailId} not found", emailId);
                return false;
            }

            email.RetryCount++;
            email.ErrorMessage = errorMessage;
            email.UpdatedAt = DateTime.UtcNow;

            if (email.RetryCount >= email.MaxRetries)
            {
                email.Status = EmailStatus.Failed;
                email.NextAttemptAt = null;
                
                _log.Warning("Email {EmailId} failed after {RetryCount} attempts. Error: {Error}", 
                    emailId, email.RetryCount, errorMessage);
                
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                // Exponential backoff: 2^retryCount minutes
                var delayMinutes = Math.Pow(2, email.RetryCount);
                email.NextAttemptAt = DateTime.UtcNow.AddMinutes(delayMinutes);
                email.Status = EmailStatus.Pending;
                
                _log.Information("Email {EmailId} retry scheduled for {NextAttempt} (attempt {RetryCount}/{MaxRetries})", 
                    emailId, email.NextAttemptAt, email.RetryCount, email.MaxRetries);
                
                await _context.SaveChangesAsync();
                
                // Re-signal the channel for retry
                await _emailNotificationChannel.Writer.WriteAsync(emailId);
                
                return true;
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error marking email {EmailId} as failed", emailId);
            return false;
        }
    }

    /// <summary>
    /// Gets email details for sending
    /// </summary>
    public async Task<SentEmail?> GetEmailByIdAsync(int emailId)
    {
        try
        {
            return await _context.SentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == emailId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving email {EmailId}", emailId);
            throw;
        }
    }

    private string GetSystemFromAddress()
    {
        // TODO: Read from configuration
        return "noreply@system.com";
    }
}
