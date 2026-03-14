using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Defines read operations for communication threads.
/// </summary>
public interface ICommunicationThreadService
{
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
    /// <returns>A collection of communication thread summaries.</returns>
    Task<IEnumerable<CommunicationThreadDto>> GetThreadsAsync(
        int? customerId,
        int? userId,
        string? relatedEntityType,
        int? relatedEntityId,
        string? status,
        string? search,
        int maxItems);

    /// <summary>
    /// Retrieves a communication thread with participants and messages.
    /// </summary>
    /// <param name="threadId">The communication thread identifier.</param>
    /// <returns>The thread details when found; otherwise <see langword="null"/>.</returns>
    Task<CommunicationThreadDetailsDto?> GetThreadByIdAsync(int threadId);

    /// <summary>
    /// Updates the status of a communication thread.
    /// </summary>
    /// <param name="threadId">The communication thread identifier.</param>
    /// <param name="status">The target thread status.</param>
    /// <returns><see langword="true"/> if the thread was updated; otherwise <see langword="false"/>.</returns>
    Task<bool> UpdateThreadStatusAsync(int threadId, string status);

    /// <summary>
    /// Updates the read state of a communication message.
    /// </summary>
    /// <param name="messageId">The communication message identifier.</param>
    /// <param name="isRead">The target read state.</param>
    /// <returns><see langword="true"/> if the message was updated; otherwise <see langword="false"/>.</returns>
    Task<bool> UpdateMessageReadStateAsync(int messageId, bool isRead);

    /// <summary>
    /// Determines whether a communication message belongs to a thread accessible by customer/user scope.
    /// </summary>
    /// <param name="messageId">The communication message identifier.</param>
    /// <param name="customerId">The scoped customer identifier.</param>
    /// <param name="userId">The scoped user identifier.</param>
    /// <returns><see langword="true"/> when accessible in scope; otherwise <see langword="false"/>.</returns>
    Task<bool> CanAccessMessageAsync(int messageId, int customerId, int userId);
}
