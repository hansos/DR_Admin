using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing reseller companies
/// </summary>
public interface IResellerCompanyService
{
    /// <summary>
    /// Retrieves all reseller companies
    /// </summary>
    /// <returns>Collection of reseller company DTOs</returns>
    Task<IEnumerable<ResellerCompanyDto>> GetAllResellerCompaniesAsync();
    
    /// <summary>
    /// Retrieves active reseller companies only
    /// </summary>
    /// <returns>Collection of active reseller company DTOs</returns>
    Task<IEnumerable<ResellerCompanyDto>> GetActiveResellerCompaniesAsync();
    
    /// <summary>
    /// Retrieves the default reseller company
    /// </summary>
    /// <returns>The default reseller company DTO, or null if not found</returns>
    Task<ResellerCompanyDto?> GetDefaultResellerCompanyAsync();
    
    /// <summary>
    /// Retrieves a reseller company by its ID
    /// </summary>
    /// <param name="id">The ID of the reseller company</param>
    /// <returns>The reseller company DTO, or null if not found</returns>
    Task<ResellerCompanyDto?> GetResellerCompanyByIdAsync(int id);
    
    /// <summary>
    /// Creates a new reseller company
    /// </summary>
    /// <param name="dto">The reseller company data to create</param>
    /// <returns>The created reseller company DTO</returns>
    Task<ResellerCompanyDto> CreateResellerCompanyAsync(CreateResellerCompanyDto dto);
    
    /// <summary>
    /// Updates an existing reseller company
    /// </summary>
    /// <param name="id">The ID of the reseller company to update</param>
    /// <param name="dto">The updated reseller company data</param>
    /// <returns>The updated reseller company DTO, or null if not found</returns>
    Task<ResellerCompanyDto?> UpdateResellerCompanyAsync(int id, UpdateResellerCompanyDto dto);
    
    /// <summary>
    /// Deletes a reseller company
    /// </summary>
    /// <param name="id">The ID of the reseller company to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteResellerCompanyAsync(int id);
}
