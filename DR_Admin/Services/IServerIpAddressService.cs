using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing server IP addresses
/// </summary>
public interface IServerIpAddressService
{
    /// <summary>
    /// Retrieves all server IP addresses
    /// </summary>
    /// <returns>Collection of server IP address DTOs</returns>
    Task<IEnumerable<ServerIpAddressDto>> GetAllServerIpAddressesAsync();
    
    /// <summary>
    /// Retrieves IP addresses for a specific server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <returns>Collection of server IP address DTOs</returns>
    Task<IEnumerable<ServerIpAddressDto>> GetServerIpAddressesByServerIdAsync(int serverId);
    
    /// <summary>
    /// Retrieves a server IP address by its unique identifier
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <returns>Server IP address DTO if found, otherwise null</returns>
    Task<ServerIpAddressDto?> GetServerIpAddressByIdAsync(int id);
    
    /// <summary>
    /// Creates a new server IP address
    /// </summary>
    /// <param name="createDto">The IP address creation data</param>
    /// <returns>The created server IP address DTO</returns>
    Task<ServerIpAddressDto> CreateServerIpAddressAsync(CreateServerIpAddressDto createDto);
    
    /// <summary>
    /// Updates an existing server IP address
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <param name="updateDto">The IP address update data</param>
    /// <returns>The updated server IP address DTO if found, otherwise null</returns>
    Task<ServerIpAddressDto?> UpdateServerIpAddressAsync(int id, UpdateServerIpAddressDto updateDto);
    
    /// <summary>
    /// Deletes a server IP address
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteServerIpAddressAsync(int id);
}
