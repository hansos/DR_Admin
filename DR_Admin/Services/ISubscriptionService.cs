using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Interface for subscription service operations
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Retrieves all subscriptions
    /// </summary>
    /// <returns>Collection of subscription DTOs</returns>
    Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync();

    /// <summary>
    /// Retrieves all subscriptions for a specific customer
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <returns>Collection of subscription DTOs for the customer</returns>
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByCustomerIdAsync(int customerId);

    /// <summary>
    /// Retrieves all subscriptions with a specific status
    /// </summary>
    /// <param name="status">The subscription status to filter by</param>
    /// <returns>Collection of subscription DTOs with the specified status</returns>
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByStatusAsync(SubscriptionStatus status);

    /// <summary>
    /// Retrieves subscriptions that are due for billing
    /// </summary>
    /// <param name="dueDate">The date to check for due subscriptions (defaults to now)</param>
    /// <returns>Collection of subscription DTOs that are due for billing</returns>
    Task<IEnumerable<SubscriptionDto>> GetDueSubscriptionsAsync(DateTime? dueDate = null);

    /// <summary>
    /// Retrieves a subscription by its unique identifier
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <returns>The subscription DTO, or null if not found</returns>
    Task<SubscriptionDto?> GetSubscriptionByIdAsync(int id);

    /// <summary>
    /// Creates a new subscription
    /// </summary>
    /// <param name="createDto">The subscription creation data</param>
    /// <returns>The created subscription DTO</returns>
    Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto);

    /// <summary>
    /// Updates an existing subscription
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <param name="updateDto">The subscription update data</param>
    /// <returns>The updated subscription DTO, or null if not found</returns>
    Task<SubscriptionDto?> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto updateDto);

    /// <summary>
    /// Cancels a subscription
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <param name="cancelDto">The cancellation data</param>
    /// <returns>The cancelled subscription DTO, or null if not found</returns>
    Task<SubscriptionDto?> CancelSubscriptionAsync(int id, CancelSubscriptionDto cancelDto);

    /// <summary>
    /// Pauses a subscription
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <param name="pauseDto">The pause data</param>
    /// <returns>The paused subscription DTO, or null if not found</returns>
    Task<SubscriptionDto?> PauseSubscriptionAsync(int id, PauseSubscriptionDto pauseDto);

    /// <summary>
    /// Resumes a paused subscription
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <returns>The resumed subscription DTO, or null if not found</returns>
    Task<SubscriptionDto?> ResumeSubscriptionAsync(int id);

    /// <summary>
    /// Deletes a subscription
    /// </summary>
    /// <param name="id">The subscription identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteSubscriptionAsync(int id);

    /// <summary>
    /// Processes billing for a specific subscription
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier</param>
    /// <returns>True if billing was successful, false otherwise</returns>
    Task<bool> ProcessSubscriptionBillingAsync(int subscriptionId);
}
