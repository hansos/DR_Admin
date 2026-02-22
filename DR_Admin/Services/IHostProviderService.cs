using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing host providers
/// </summary>
public interface IHostProviderService
{
    /// <summary>
    /// Retrieves all host providers
    /// </summary>
    /// <returns>Collection of host provider DTOs</returns>
    Task<IEnumerable<HostProviderDto>> GetAllHostProvidersAsync();

    /// <summary>
    /// Retrieves active host providers only
    /// </summary>
    /// <returns>Collection of active host provider DTOs</returns>
    Task<IEnumerable<HostProviderDto>> GetActiveHostProvidersAsync();

    /// <summary>
    /// Retrieves a host provider by its unique identifier
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <returns>Host provider DTO if found, otherwise null</returns>
    Task<HostProviderDto?> GetHostProviderByIdAsync(int id);

    /// <summary>
    /// Creates a new host provider
    /// </summary>
    /// <param name="createDto">The host provider creation data</param>
    /// <returns>The created host provider DTO</returns>
    Task<HostProviderDto> CreateHostProviderAsync(CreateHostProviderDto createDto);

    /// <summary>
    /// Updates an existing host provider
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <param name="updateDto">The host provider update data</param>
    /// <returns>The updated host provider DTO if found, otherwise null</returns>
    Task<HostProviderDto?> UpdateHostProviderAsync(int id, UpdateHostProviderDto updateDto);

    /// <summary>
    /// Deletes a host provider
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteHostProviderAsync(int id);
}
