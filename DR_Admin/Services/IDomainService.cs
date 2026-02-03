using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing domains
/// </summary>
public interface IDomainService
{
    /// <summary>
    /// Retrieves all domains
    /// </summary>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<DomainDto>> GetAllDomainsAsync();
    
    /// <summary>
    /// Retrieves domains for a specific customer
    /// </summary>
    /// <param name="customerId">The ID of the customer</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<DomainDto>> GetDomainsByCustomerIdAsync(int customerId);
    
    /// <summary>
    /// Retrieves domains by registrar
    /// </summary>
    /// <param name="registrarId">The ID of the registrar</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<DomainDto>> GetDomainsByRegistrarIdAsync(int registrarId);
    
    /// <summary>
    /// Retrieves domains by status
    /// </summary>
    /// <param name="status">The domain status</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<DomainDto>> GetDomainsByStatusAsync(string status);
    
    /// <summary>
    /// Retrieves domains expiring within specified days
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<DomainDto>> GetDomainsExpiringInDaysAsync(int days);
    
    /// <summary>
    /// Retrieves a domain by its ID
    /// </summary>
    /// <param name="id">The ID of the domain</param>
    /// <returns>The domain DTO, or null if not found</returns>
    Task<DomainDto?> GetDomainByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a domain by its name
    /// </summary>
    /// <param name="name">The domain name</param>
    /// <returns>The domain DTO, or null if not found</returns>
    Task<DomainDto?> GetDomainByNameAsync(string name);
    
    /// <summary>
    /// Creates a new domain
    /// </summary>
    /// <param name="dto">The domain data to create</param>
    /// <returns>The created domain DTO</returns>
    Task<DomainDto> CreateDomainAsync(CreateDomainDto dto);
    
    /// <summary>
    /// Updates an existing domain
    /// </summary>
    /// <param name="id">The ID of the domain to update</param>
    /// <param name="dto">The updated domain data</param>
    /// <returns>The updated domain DTO, or null if not found</returns>
    Task<DomainDto?> UpdateDomainAsync(int id, UpdateDomainDto dto);
    
    /// <summary>
    /// Deletes a domain
    /// </summary>
    /// <param name="id">The ID of the domain to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteDomainAsync(int id);
}
