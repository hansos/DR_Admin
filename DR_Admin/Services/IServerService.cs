using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing servers
/// </summary>
public interface IServerService
{
    /// <summary>
    /// Retrieves all servers
    /// </summary>
    /// <returns>Collection of server DTOs</returns>
    Task<IEnumerable<ServerDto>> GetAllServersAsync();
    
    /// <summary>
    /// Retrieves a server by its unique identifier
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <returns>Server DTO if found, otherwise null</returns>
    Task<ServerDto?> GetServerByIdAsync(int id);
    
    /// <summary>
    /// Creates a new server
    /// </summary>
    /// <param name="createDto">The server creation data</param>
    /// <returns>The created server DTO</returns>
    Task<ServerDto> CreateServerAsync(CreateServerDto createDto);
    
    /// <summary>
    /// Updates an existing server
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <param name="updateDto">The server update data</param>
    /// <returns>The updated server DTO if found, otherwise null</returns>
    Task<ServerDto?> UpdateServerAsync(int id, UpdateServerDto updateDto);
    
    /// <summary>
    /// Deletes a server
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteServerAsync(int id);
}
