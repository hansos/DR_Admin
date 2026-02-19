using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing system settings
/// </summary>
public interface ISystemSettingService
{
    /// <summary>
    /// Retrieves all system settings
    /// </summary>
    /// <returns>A collection of all system setting DTOs</returns>
    Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync();

    /// <summary>
    /// Retrieves a specific system setting by its unique identifier
    /// </summary>
    /// <param name="id">The system setting ID</param>
    /// <returns>The system setting DTO if found, otherwise null</returns>
    Task<SystemSettingDto?> GetSystemSettingByIdAsync(int id);

    /// <summary>
    /// Retrieves a specific system setting by its unique key
    /// </summary>
    /// <param name="key">The system setting key (e.g., "CustomerNumber.NextValue")</param>
    /// <returns>The system setting DTO if found, otherwise null</returns>
    Task<SystemSettingDto?> GetSystemSettingByKeyAsync(string key);

    /// <summary>
    /// Creates a new system setting
    /// </summary>
    /// <param name="createDto">The system setting data for creation</param>
    /// <returns>The newly created system setting DTO</returns>
    Task<SystemSettingDto> CreateSystemSettingAsync(CreateSystemSettingDto createDto);

    /// <summary>
    /// Updates an existing system setting
    /// </summary>
    /// <param name="id">The system setting ID to update</param>
    /// <param name="updateDto">The updated system setting data</param>
    /// <returns>The updated system setting DTO if successful, otherwise null</returns>
    Task<SystemSettingDto?> UpdateSystemSettingAsync(int id, UpdateSystemSettingDto updateDto);

    /// <summary>
    /// Updates a system setting by its key, or creates it if it does not exist
    /// </summary>
    /// <param name="key">The system setting key</param>
    /// <param name="value">The new value</param>
    /// <param name="description">Optional description (used only when creating)</param>
    /// <returns>The upserted system setting DTO</returns>
    Task<SystemSettingDto> UpsertSystemSettingAsync(string key, string value, string? description = null);

    /// <summary>
    /// Deletes a system setting
    /// </summary>
    /// <param name="id">The system setting ID to delete</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteSystemSettingAsync(int id);
}
