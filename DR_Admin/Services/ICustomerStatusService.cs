using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer statuses
/// </summary>
public interface ICustomerStatusService
{
    /// <summary>
    /// Retrieves all customer statuses in the system
    /// </summary>
    /// <returns>A collection of customer status DTOs</returns>
    Task<IEnumerable<CustomerStatusDto>> GetAllCustomerStatusesAsync();
    
    /// <summary>
    /// Retrieves all active customer statuses
    /// </summary>
    /// <returns>A collection of active customer status DTOs</returns>
    Task<IEnumerable<CustomerStatusDto>> GetActiveCustomerStatusesAsync();
    
    /// <summary>
    /// Retrieves a specific customer status by its unique identifier
    /// </summary>
    /// <param name="id">The customer status ID</param>
    /// <returns>The customer status DTO if found, otherwise null</returns>
    Task<CustomerStatusDto?> GetCustomerStatusByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a specific customer status by its code
    /// </summary>
    /// <param name="code">The customer status code</param>
    /// <returns>The customer status DTO if found, otherwise null</returns>
    Task<CustomerStatusDto?> GetCustomerStatusByCodeAsync(string code);
    
    /// <summary>
    /// Retrieves the default customer status
    /// </summary>
    /// <returns>The default customer status DTO if found, otherwise null</returns>
    Task<CustomerStatusDto?> GetDefaultCustomerStatusAsync();
    
    /// <summary>
    /// Creates a new customer status
    /// </summary>
    /// <param name="createDto">The customer status data for creation</param>
    /// <returns>The newly created customer status DTO</returns>
    Task<CustomerStatusDto> CreateCustomerStatusAsync(CreateCustomerStatusDto createDto);
    
    /// <summary>
    /// Updates an existing customer status
    /// </summary>
    /// <param name="id">The customer status ID to update</param>
    /// <param name="updateDto">The updated customer status data</param>
    /// <returns>The updated customer status DTO if successful, otherwise null</returns>
    Task<CustomerStatusDto?> UpdateCustomerStatusAsync(int id, UpdateCustomerStatusDto updateDto);
    
    /// <summary>
    /// Deletes a customer status
    /// </summary>
    /// <param name="id">The customer status ID to delete</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteCustomerStatusAsync(int id);
}
