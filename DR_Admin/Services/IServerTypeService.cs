using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing server types
/// </summary>
public interface IServerTypeService
{
    /// <summary>
    /// Retrieves all server types
    /// </summary>
    /// <returns>Collection of server type DTOs</returns>
    Task<IEnumerable<ServerTypeDto>> GetAllServerTypesAsync();

    /// <summary>
    /// Retrieves active server types only
    /// </summary>
    /// <returns>Collection of active server type DTOs</returns>
    Task<IEnumerable<ServerTypeDto>> GetActiveServerTypesAsync();

    /// <summary>
    /// Retrieves a server type by its unique identifier
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <returns>Server type DTO if found, otherwise null</returns>
    Task<ServerTypeDto?> GetServerTypeByIdAsync(int id);

    /// <summary>
    /// Creates a new server type
    /// </summary>
    /// <param name="createDto">The server type creation data</param>
    /// <returns>The created server type DTO</returns>
    Task<ServerTypeDto> CreateServerTypeAsync(CreateServerTypeDto createDto);

    /// <summary>
    /// Updates an existing server type
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <param name="updateDto">The server type update data</param>
    /// <returns>The updated server type DTO if found, otherwise null</returns>
    Task<ServerTypeDto?> UpdateServerTypeAsync(int id, UpdateServerTypeDto updateDto);

    /// <summary>
    /// Deletes a server type
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteServerTypeAsync(int id);
}
