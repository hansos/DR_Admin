using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing the reseller's own company profile.
/// </summary>
public interface IMyCompanyService
{
    /// <summary>
    /// Retrieves the current company profile.
    /// </summary>
    /// <returns>The company profile if present, otherwise null.</returns>
    Task<MyCompanyDto?> GetMyCompanyAsync();

    /// <summary>
    /// Creates or updates the company profile.
    /// </summary>
    /// <param name="dto">The company profile data.</param>
    /// <returns>The updated company profile.</returns>
    Task<MyCompanyDto> UpsertMyCompanyAsync(UpsertMyCompanyDto dto);
}