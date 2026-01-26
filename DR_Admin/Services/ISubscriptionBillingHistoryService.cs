using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Interface for subscription billing history service operations
/// </summary>
public interface ISubscriptionBillingHistoryService
{
    /// <summary>
    /// Retrieves all billing history records
    /// </summary>
    /// <returns>Collection of billing history DTOs</returns>
    Task<IEnumerable<SubscriptionBillingHistoryDto>> GetAllBillingHistoriesAsync();

    /// <summary>
    /// Retrieves all billing history records for a specific subscription
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier</param>
    /// <returns>Collection of billing history DTOs for the subscription</returns>
    Task<IEnumerable<SubscriptionBillingHistoryDto>> GetBillingHistoriesBySubscriptionIdAsync(int subscriptionId);

    /// <summary>
    /// Retrieves a billing history record by its unique identifier
    /// </summary>
    /// <param name="id">The billing history identifier</param>
    /// <returns>The billing history DTO, or null if not found</returns>
    Task<SubscriptionBillingHistoryDto?> GetBillingHistoryByIdAsync(int id);

    /// <summary>
    /// Creates a new billing history record
    /// </summary>
    /// <param name="createDto">The billing history creation data</param>
    /// <returns>The created billing history DTO</returns>
    Task<SubscriptionBillingHistoryDto> CreateBillingHistoryAsync(CreateSubscriptionBillingHistoryDto createDto);

    /// <summary>
    /// Deletes a billing history record
    /// </summary>
    /// <param name="id">The billing history identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteBillingHistoryAsync(int id);
}
