using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing email queue operations
/// </summary>
public interface IEmailQueueService
{
    /// <summary>
    /// Queues an email for sending
    /// </summary>
    /// <param name="queueEmailDto">Email details to queue</param>
    /// <returns>Queue response with email ID and status</returns>
    Task<QueueEmailResponseDto> QueueEmailAsync(QueueEmailDto queueEmailDto);

    /// <summary>
    /// Gets pending emails ready to be sent
    /// </summary>
    /// <param name="batchSize">Maximum number of emails to retrieve</param>
    /// <returns>Collection of email IDs ready for sending</returns>
    Task<IEnumerable<int>> GetPendingEmailIdsAsync(int batchSize = 10);

    /// <summary>
    /// Marks an email as in-progress
    /// </summary>
    /// <param name="emailId">The email ID</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> MarkEmailInProgressAsync(int emailId);

    /// <summary>
    /// Marks an email as successfully sent
    /// </summary>
    /// <param name="emailId">The email ID</param>
    /// <param name="messageId">The message ID from the provider</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> MarkEmailSentAsync(int emailId, string messageId);

    /// <summary>
    /// Marks an email as failed and schedules retry if applicable
    /// </summary>
    /// <param name="emailId">The email ID</param>
    /// <param name="errorMessage">The error message</param>
    /// <returns>True if retry scheduled, false if max retries reached</returns>
    Task<bool> MarkEmailFailedAsync(int emailId, string errorMessage);

    /// <summary>
    /// Gets email details for sending
    /// </summary>
    /// <param name="emailId">The email ID</param>
    /// <returns>Sent email entity or null if not found</returns>
    Task<Data.Entities.SentEmail?> GetEmailByIdAsync(int emailId);
}
