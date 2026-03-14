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

            await LinkQueuedEmailToCommunicationAsync(email, queueEmailDto);

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
    /// Applies a provider delivery event to queued email and communication state.
    /// </summary>
    /// <param name="providerEventDto">The provider event payload.</param>
    /// <returns>True when the related email/message was found and updated; otherwise false.</returns>
    public async Task<bool> ApplyProviderEventAsync(EmailProviderEventDto providerEventDto)
    {
        try
        {
            var providerMessageId = providerEventDto.ProviderMessageId?.Trim();
            var eventType = providerEventDto.EventType?.Trim();

            if (string.IsNullOrWhiteSpace(providerMessageId) || string.IsNullOrWhiteSpace(eventType))
            {
                return false;
            }

            var email = await _context.SentEmails
                .FirstOrDefaultAsync(e => e.MessageId == providerMessageId);

            if (email == null)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var normalizedEventType = eventType.ToLowerInvariant();

            if (normalizedEventType is "delivered" or "sent")
            {
                email.Status = EmailStatus.Sent;
                email.SentDate ??= now;
                email.ErrorMessage = null;
            }
            else if (normalizedEventType is "bounce" or "bounced" or "complaint" or "failed")
            {
                email.Status = EmailStatus.Failed;
                email.ErrorMessage = providerEventDto.Details ?? eventType;
            }

            email.UpdatedAt = now;
            await _context.SaveChangesAsync();

            var communicationMessage = await _context.CommunicationMessages
                .FirstOrDefaultAsync(m => m.ExternalMessageId == providerMessageId || m.SentEmailId == email.Id);

            if (communicationMessage == null)
            {
                return true;
            }

            if (normalizedEventType is "delivered" or "sent")
            {
                communicationMessage.SentAtUtc ??= now;
                communicationMessage.IsRead = true;
            }

            communicationMessage.UpdatedAt = now;

            _context.CommunicationStatusEvents.Add(new CommunicationStatusEvent
            {
                CommunicationMessageId = communicationMessage.Id,
                Status = eventType,
                Details = providerEventDto.Details,
                Source = string.IsNullOrWhiteSpace(providerEventDto.Source) ? nameof(EmailQueueService) : providerEventDto.Source,
                OccurredAtUtc = providerEventDto.OccurredAtUtc ?? now,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error applying provider event {EventType} for message {ProviderMessageId}", providerEventDto.EventType, providerEventDto.ProviderMessageId);
            return false;
        }
    }

    /// <summary>
    /// Reschedules an existing email for immediate retry.
    /// </summary>
    /// <param name="emailId">The email identifier.</param>
    /// <returns>True when retry was queued; otherwise false.</returns>
    public async Task<bool> RetryEmailAsync(int emailId)
    {
        try
        {
            var email = await _context.SentEmails.FindAsync(emailId);
            if (email == null)
            {
                _log.Warning("Email with ID {EmailId} not found for manual retry", emailId);
                return false;
            }

            if (string.Equals(email.Status, EmailStatus.Sent, StringComparison.OrdinalIgnoreCase))
            {
                _log.Warning("Email {EmailId} is already sent and cannot be retried", emailId);
                return false;
            }

            email.Status = EmailStatus.Pending;
            email.ErrorMessage = null;
            email.NextAttemptAt = DateTime.UtcNow;
            email.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await AppendCommunicationStatusEventAsync(emailId, "ManualRetryQueued", "Email manually queued for retry.");

            await _emailNotificationChannel.Writer.WriteAsync(emailId);

            _log.Information("Email {EmailId} was manually queued for retry", emailId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrying email {EmailId}", emailId);
            return false;
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
            await AppendCommunicationStatusEventAsync(emailId, "InProgress", "Email is being processed for delivery.");
            
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
            await UpdateCommunicationOnSentAsync(emailId, messageId);
            
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
                await AppendCommunicationStatusEventAsync(emailId, "Failed", errorMessage);
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
                await AppendCommunicationStatusEventAsync(emailId, "RetryScheduled", errorMessage);
                
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

    private async Task LinkQueuedEmailToCommunicationAsync(SentEmail email, QueueEmailDto queueEmailDto)
    {
        try
        {
            var now = DateTime.UtcNow;
            var normalizedSubject = NormalizeThreadSubject(queueEmailDto.Subject);

            var candidateThreads = await _context.CommunicationThreads
                .Where(t => t.CustomerId == queueEmailDto.CustomerId
                    && t.UserId == queueEmailDto.UserId
                    && t.RelatedEntityType == queueEmailDto.RelatedEntityType
                    && t.RelatedEntityId == queueEmailDto.RelatedEntityId
                    && t.Status == CommunicationThreadStatus.Open)
                .OrderByDescending(t => t.LastMessageAtUtc ?? t.CreatedAt)
                .Take(100)
                .ToListAsync();

            var thread = candidateThreads
                .FirstOrDefault(t => string.Equals(
                    NormalizeThreadSubject(t.Subject),
                    normalizedSubject,
                    StringComparison.OrdinalIgnoreCase));

            if (thread == null)
            {
                thread = new CommunicationThread
                {
                    Subject = queueEmailDto.Subject,
                    CustomerId = queueEmailDto.CustomerId,
                    UserId = queueEmailDto.UserId,
                    RelatedEntityType = queueEmailDto.RelatedEntityType,
                    RelatedEntityId = queueEmailDto.RelatedEntityId,
                    Status = CommunicationThreadStatus.Open,
                    LastMessageAtUtc = now,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.CommunicationThreads.Add(thread);
                await _context.SaveChangesAsync();
            }
            else
            {
                thread.LastMessageAtUtc = now;
                thread.UpdatedAt = now;
            }

            var message = new CommunicationMessage
            {
                CommunicationThreadId = thread.Id,
                Direction = CommunicationMessageDirection.Outbound,
                ExternalMessageId = email.MessageId,
                InternetMessageId = email.MessageId,
                FromAddress = email.From,
                ToAddresses = email.To,
                CcAddresses = email.Cc,
                BccAddresses = email.Bcc,
                Subject = email.Subject,
                BodyText = email.BodyText,
                BodyHtml = email.BodyHtml,
                Provider = email.Provider,
                SentEmailId = email.Id,
                IsRead = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.CommunicationMessages.Add(message);

            await EnsureParticipantsAsync(thread.Id, email.From, CommunicationParticipantRole.From, true, now);
            await EnsureParticipantsAsync(thread.Id, email.To, CommunicationParticipantRole.To, true, now);
            await EnsureParticipantsAsync(thread.Id, email.Cc, CommunicationParticipantRole.Cc, false, now);
            await EnsureParticipantsAsync(thread.Id, email.Bcc, CommunicationParticipantRole.Bcc, false, now);

            _context.CommunicationStatusEvents.Add(new CommunicationStatusEvent
            {
                CommunicationMessage = message,
                Status = "Queued",
                Details = "Email queued for delivery.",
                Source = nameof(EmailQueueService),
                OccurredAtUtc = now,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error linking queued email {EmailId} to communication thread", email.Id);
        }
    }

    private async Task UpdateCommunicationOnSentAsync(int emailId, string providerMessageId)
    {
        try
        {
            var message = await _context.CommunicationMessages
                .Include(m => m.CommunicationThread)
                .FirstOrDefaultAsync(m => m.SentEmailId == emailId);

            if (message == null)
            {
                return;
            }

            var now = DateTime.UtcNow;
            message.SentAtUtc = now;
            message.ExternalMessageId = providerMessageId;
            message.InternetMessageId ??= providerMessageId;
            message.IsRead = true;
            message.UpdatedAt = now;

            if (message.CommunicationThread != null)
            {
                message.CommunicationThread.LastMessageAtUtc = now;
                message.CommunicationThread.UpdatedAt = now;
            }

            _context.CommunicationStatusEvents.Add(new CommunicationStatusEvent
            {
                CommunicationMessageId = message.Id,
                Status = "Sent",
                Details = "Email delivered by sender provider.",
                Source = nameof(EmailQueueService),
                OccurredAtUtc = now,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating communication message for sent email {EmailId}", emailId);
        }
    }

    private async Task AppendCommunicationStatusEventAsync(int emailId, string status, string? details)
    {
        try
        {
            var messageId = await _context.CommunicationMessages
                .Where(m => m.SentEmailId == emailId)
                .Select(m => (int?)m.Id)
                .FirstOrDefaultAsync();

            if (!messageId.HasValue)
            {
                return;
            }

            var now = DateTime.UtcNow;
            _context.CommunicationStatusEvents.Add(new CommunicationStatusEvent
            {
                CommunicationMessageId = messageId.Value,
                Status = status,
                Details = details,
                Source = nameof(EmailQueueService),
                OccurredAtUtc = now,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error appending communication status event for email {EmailId}", emailId);
        }
    }

    private async Task EnsureParticipantsAsync(int threadId, string? addresses, string role, bool isPrimary, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(addresses))
        {
            return;
        }

        var parsedAddresses = ParseAddresses(addresses);
        if (parsedAddresses.Count == 0)
        {
            return;
        }

        var existing = await _context.CommunicationParticipants
            .Where(p => p.CommunicationThreadId == threadId && p.Role == role)
            .Select(p => p.EmailAddress.ToLower())
            .ToListAsync();

        foreach (var address in parsedAddresses)
        {
            if (existing.Contains(address.ToLower()))
            {
                continue;
            }

            _context.CommunicationParticipants.Add(new CommunicationParticipant
            {
                CommunicationThreadId = threadId,
                EmailAddress = address,
                Role = role,
                IsPrimary = isPrimary,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }

    private static List<string> ParseAddresses(string addresses)
    {
        return addresses
            .Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeThreadSubject(string? subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return "(no subject)";
        }

        var value = subject.Trim();
        var changed = true;
        while (changed)
        {
            changed = false;
            if (value.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[3..].TrimStart();
                changed = true;
            }
            else if (value.StartsWith("Fw:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[3..].TrimStart();
                changed = true;
            }
            else if (value.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[4..].TrimStart();
                changed = true;
            }
        }

        return value.ToLowerInvariant();
    }
}
