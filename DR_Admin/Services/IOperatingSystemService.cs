using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing operating systems
/// </summary>
public interface IOperatingSystemService
{
    /// <summary>
    /// Retrieves all operating systems
    /// </summary>
    /// <returns>Collection of operating system DTOs</returns>
    Task<IEnumerable<OperatingSystemDto>> GetAllOperatingSystemsAsync();

    /// <summary>
    /// Retrieves active operating systems only
    /// </summary>
    /// <returns>Collection of active operating system DTOs</returns>
    Task<IEnumerable<OperatingSystemDto>> GetActiveOperatingSystemsAsync();

    /// <summary>
    /// Retrieves an operating system by its unique identifier
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <returns>Operating system DTO if found, otherwise null</returns>
    Task<OperatingSystemDto?> GetOperatingSystemByIdAsync(int id);

    /// <summary>
    /// Creates a new operating system
    /// </summary>
    /// <param name="createDto">The operating system creation data</param>
    /// <returns>The created operating system DTO</returns>
    Task<OperatingSystemDto> CreateOperatingSystemAsync(CreateOperatingSystemDto createDto);

    /// <summary>
    /// Updates an existing operating system
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <param name="updateDto">The operating system update data</param>
    /// <returns>The updated operating system DTO if found, otherwise null</returns>
    Task<OperatingSystemDto?> UpdateOperatingSystemAsync(int id, UpdateOperatingSystemDto updateDto);

    /// <summary>
    /// Deletes an operating system
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteOperatingSystemAsync(int id);
}
