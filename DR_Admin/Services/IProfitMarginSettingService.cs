using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing profit margin settings by product class.
/// </summary>
public interface IProfitMarginSettingService
{
    /// <summary>
    /// Retrieves all profit margin settings.
    /// </summary>
    /// <returns>A collection of profit margin settings.</returns>
    Task<IEnumerable<ProfitMarginSettingDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a profit margin setting by id.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>The profit margin setting if found; otherwise null.</returns>
    Task<ProfitMarginSettingDto?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a profit margin setting for a product class.
    /// </summary>
    /// <param name="productClass">Product class.</param>
    /// <returns>The profit margin setting if found; otherwise null.</returns>
    Task<ProfitMarginSettingDto?> GetByProductClassAsync(ProfitProductClass productClass);

    /// <summary>
    /// Creates a new profit margin setting.
    /// </summary>
    /// <param name="dto">Create payload.</param>
    /// <returns>The created profit margin setting.</returns>
    Task<ProfitMarginSettingDto> CreateAsync(CreateProfitMarginSettingDto dto);

    /// <summary>
    /// Updates an existing profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <param name="dto">Update payload.</param>
    /// <returns>The updated profit margin setting if found; otherwise null.</returns>
    Task<ProfitMarginSettingDto?> UpdateAsync(int id, UpdateProfitMarginSettingDto dto);

    /// <summary>
    /// Deletes a profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    Task<bool> DeleteAsync(int id);
}
