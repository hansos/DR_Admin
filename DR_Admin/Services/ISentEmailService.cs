using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing sent email records
/// </summary>
public interface ISentEmailService
{
    /// <summary>
    /// Retrieves all sent email records
    /// </summary>
    /// <returns>Collection of sent email DTOs</returns>
    Task<IEnumerable<SentEmailDto>> GetAllSentEmailsAsync();
    
    /// <summary>
    /// Retrieves sent emails by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>Collection of sent email DTOs for the specified customer</returns>
    Task<IEnumerable<SentEmailDto>> GetSentEmailsByCustomerAsync(int customerId);
    
    /// <summary>
    /// Retrieves sent emails by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of sent email DTOs sent by the specified user</returns>
    Task<IEnumerable<SentEmailDto>> GetSentEmailsByUserAsync(int userId);
    
    /// <summary>
    /// Retrieves sent emails by related entity
    /// </summary>
    /// <param name="entityType">The entity type (e.g., Invoice, Order)</param>
    /// <param name="entityId">The entity ID</param>
    /// <returns>Collection of sent email DTOs for the specified entity</returns>
    Task<IEnumerable<SentEmailDto>> GetSentEmailsByRelatedEntityAsync(string entityType, int entityId);
    
    /// <summary>
    /// Retrieves sent emails by date range
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>Collection of sent email DTOs within the specified date range</returns>
    Task<IEnumerable<SentEmailDto>> GetSentEmailsByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Retrieves sent emails by message ID
    /// </summary>
    /// <param name="messageId">The message ID</param>
    /// <returns>The sent email DTO, or null if not found</returns>
    Task<SentEmailDto?> GetSentEmailByMessageIdAsync(string messageId);
    
    /// <summary>
    /// Retrieves a sent email by its ID
    /// </summary>
    /// <param name="id">The ID of the sent email record</param>
    /// <returns>The sent email DTO, or null if not found</returns>
    Task<SentEmailDto?> GetSentEmailByIdAsync(int id);
    
    /// <summary>
    /// Creates a new sent email record
    /// </summary>
    /// <param name="dto">The sent email data to create</param>
    /// <returns>The created sent email DTO</returns>
    Task<SentEmailDto> CreateSentEmailAsync(CreateSentEmailDto dto);
    
    /// <summary>
    /// Updates an existing sent email record (typically for status updates)
    /// </summary>
    /// <param name="id">The ID of the sent email record to update</param>
    /// <param name="dto">The updated sent email data</param>
    /// <returns>The updated sent email DTO, or null if not found</returns>
    Task<SentEmailDto?> UpdateSentEmailAsync(int id, UpdateSentEmailDto dto);
    
    /// <summary>
    /// Deletes a sent email record
    /// </summary>
    /// <param name="id">The ID of the sent email record to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteSentEmailAsync(int id);
}
