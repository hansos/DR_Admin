using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing seller tax registration operations.
/// </summary>
public interface ITaxRegistrationService
{
    /// <summary>
    /// Retrieves all seller tax registrations.
    /// </summary>
    /// <returns>Collection of tax registration DTOs.</returns>
    Task<IEnumerable<TaxRegistrationDto>> GetAllTaxRegistrationsAsync();

    /// <summary>
    /// Retrieves a seller tax registration by identifier.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>Tax registration DTO if found; otherwise null.</returns>
    Task<TaxRegistrationDto?> GetTaxRegistrationByIdAsync(int id);

    /// <summary>
    /// Creates a new seller tax registration.
    /// </summary>
    /// <param name="dto">Tax registration create request.</param>
    /// <returns>Created tax registration DTO.</returns>
    Task<TaxRegistrationDto> CreateTaxRegistrationAsync(CreateTaxRegistrationDto dto);

    /// <summary>
    /// Updates an existing seller tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <param name="dto">Tax registration update request.</param>
    /// <returns>Updated tax registration DTO if found; otherwise null.</returns>
    Task<TaxRegistrationDto?> UpdateTaxRegistrationAsync(int id, UpdateTaxRegistrationDto dto);

    /// <summary>
    /// Deletes a seller tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    Task<bool> DeleteTaxRegistrationAsync(int id);
}
