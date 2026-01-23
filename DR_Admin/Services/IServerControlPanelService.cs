using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing server control panels
/// </summary>
public interface IServerControlPanelService
{
    /// <summary>
    /// Retrieves all server control panels
    /// </summary>
    /// <returns>Collection of server control panel DTOs</returns>
    Task<IEnumerable<ServerControlPanelDto>> GetAllServerControlPanelsAsync();
    
    /// <summary>
    /// Retrieves control panels for a specific server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <returns>Collection of server control panel DTOs</returns>
    Task<IEnumerable<ServerControlPanelDto>> GetServerControlPanelsByServerIdAsync(int serverId);
    
    /// <summary>
    /// Retrieves a server control panel by its unique identifier
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>Server control panel DTO if found, otherwise null</returns>
    Task<ServerControlPanelDto?> GetServerControlPanelByIdAsync(int id);
    
    /// <summary>
    /// Creates a new server control panel
    /// </summary>
    /// <param name="createDto">The control panel creation data</param>
    /// <returns>The created server control panel DTO</returns>
    Task<ServerControlPanelDto> CreateServerControlPanelAsync(CreateServerControlPanelDto createDto);
    
    /// <summary>
    /// Updates an existing server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <param name="updateDto">The control panel update data</param>
    /// <returns>The updated server control panel DTO if found, otherwise null</returns>
    Task<ServerControlPanelDto?> UpdateServerControlPanelAsync(int id, UpdateServerControlPanelDto updateDto);
    
    /// <summary>
    /// Deletes a server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteServerControlPanelAsync(int id);
    
    /// <summary>
    /// Tests the connection to a server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>True if connection is successful, otherwise false</returns>
    Task<bool> TestConnectionAsync(int id);
}
