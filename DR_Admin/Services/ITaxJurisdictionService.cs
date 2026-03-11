using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing tax jurisdiction operations.
/// </summary>
public interface ITaxJurisdictionService
{
    /// <summary>
    /// Retrieves all tax jurisdictions.
    /// </summary>
    /// <returns>Collection of tax jurisdiction DTOs.</returns>
    Task<IEnumerable<TaxJurisdictionDto>> GetAllTaxJurisdictionsAsync();

    /// <summary>
    /// Retrieves a tax jurisdiction by identifier.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>Tax jurisdiction DTO if found; otherwise null.</returns>
    Task<TaxJurisdictionDto?> GetTaxJurisdictionByIdAsync(int id);

    /// <summary>
    /// Creates a new tax jurisdiction.
    /// </summary>
    /// <param name="dto">Tax jurisdiction create request.</param>
    /// <returns>Created tax jurisdiction DTO.</returns>
    Task<TaxJurisdictionDto> CreateTaxJurisdictionAsync(CreateTaxJurisdictionDto dto);

    /// <summary>
    /// Updates an existing tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <param name="dto">Tax jurisdiction update request.</param>
    /// <returns>Updated tax jurisdiction DTO if found; otherwise null.</returns>
    Task<TaxJurisdictionDto?> UpdateTaxJurisdictionAsync(int id, UpdateTaxJurisdictionDto dto);

    /// <summary>
    /// Deletes a tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    Task<bool> DeleteTaxJurisdictionAsync(int id);
}
