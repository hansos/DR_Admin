using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting packages
/// </summary>
public interface IHostingPackageService
{
    /// <summary>
    /// Retrieves all hosting packages
    /// </summary>
    /// <returns>Collection of hosting package DTOs</returns>
    Task<IEnumerable<HostingPackageDto>> GetAllHostingPackagesAsync();
    
    /// <summary>
    /// Retrieves active hosting packages only
    /// </summary>
    /// <returns>Collection of active hosting package DTOs</returns>
    Task<IEnumerable<HostingPackageDto>> GetActiveHostingPackagesAsync();
    
    /// <summary>
    /// Retrieves a hosting package by its unique identifier
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <returns>Hosting package DTO if found, otherwise null</returns>
    Task<HostingPackageDto?> GetHostingPackageByIdAsync(int id);
    
    /// <summary>
    /// Creates a new hosting package
    /// </summary>
    /// <param name="createDto">The hosting package creation data</param>
    /// <returns>The created hosting package DTO</returns>
    Task<HostingPackageDto> CreateHostingPackageAsync(CreateHostingPackageDto createDto);
    
    /// <summary>
    /// Updates an existing hosting package
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <param name="updateDto">The hosting package update data</param>
    /// <returns>The updated hosting package DTO if found, otherwise null</returns>
    Task<HostingPackageDto?> UpdateHostingPackageAsync(int id, UpdateHostingPackageDto updateDto);
    
    /// <summary>
    /// Deletes a hosting package
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteHostingPackageAsync(int id);
}
