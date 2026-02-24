using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing domains
/// </summary>
public interface IRegisteredDomainService
{
    /// <summary>
    /// Retrieves all domains
    /// </summary>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<RegisteredDomainDto>> GetAllDomainsAsync();

    /// <summary>
    /// Retrieves all domains with pagination
    /// </summary>
    /// <param name="parameters">Pagination parameters including page number and page size</param>
    /// <returns>A paged result containing domains and pagination metadata</returns>
    Task<PagedResult<RegisteredDomainDto>> GetAllDomainsPagedAsync(PaginationParameters parameters);
    
    /// <summary>
    /// Retrieves domains for a specific customer
    /// </summary>
    /// <param name="customerId">The ID of the customer</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<RegisteredDomainDto>> GetDomainsByCustomerIdAsync(int customerId);
    
    /// <summary>
    /// Retrieves domains by registrar
    /// </summary>
    /// <param name="registrarId">The ID of the registrar</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<RegisteredDomainDto>> GetDomainsByRegistrarIdAsync(int registrarId);
    
    /// <summary>
    /// Retrieves domains by status
    /// </summary>
    /// <param name="status">The domain status</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<RegisteredDomainDto>> GetDomainsByStatusAsync(string status);
    
    /// <summary>
    /// Retrieves domains expiring within specified days
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>Collection of domain DTOs</returns>
    Task<IEnumerable<RegisteredDomainDto>> GetDomainsExpiringInDaysAsync(int days);
    
    /// <summary>
    /// Retrieves a domain by its ID
    /// </summary>
    /// <param name="id">The ID of the domain</param>
    /// <returns>The domain DTO, or null if not found</returns>
    Task<RegisteredDomainDto?> GetDomainByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a domain by its name
    /// </summary>
    /// <param name="name">The domain name</param>
    /// <returns>The domain DTO, or null if not found</returns>
    Task<RegisteredDomainDto?> GetDomainByNameAsync(string name);
    
    /// <summary>
    /// Creates a new domain
    /// </summary>
    /// <param name="dto">The domain data to create</param>
    /// <returns>The created domain DTO</returns>
    Task<RegisteredDomainDto> CreateDomainAsync(CreateRegisteredDomainDto dto);
    
    /// <summary>
    /// Updates an existing domain
    /// </summary>
    /// <param name="id">The ID of the domain to update</param>
    /// <param name="dto">The updated domain data</param>
    /// <returns>The updated domain DTO, or null if not found</returns>
    Task<RegisteredDomainDto?> UpdateDomainAsync(int id, UpdateRegisteredDomainDto dto);
    
    /// <summary>
    /// Deletes a domain
    /// </summary>
    /// <param name="id">The ID of the domain to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteDomainAsync(int id);

    /// <summary>
    /// Registers a new domain for a customer (customer self-service)
    /// </summary>
    /// <param name="dto">Domain registration details</param>
    /// <param name="customerId">The customer ID registering the domain</param>
    /// <returns>Registration response with order and invoice details</returns>
    Task<DomainRegistrationResponseDto> RegisterDomainAsync(RegisterDomainDto dto, int customerId);

    /// <summary>
    /// Registers a new domain for a specific customer (sales/admin)
    /// </summary>
    /// <param name="dto">Domain registration details including customer ID</param>
    /// <returns>Registration response with order and invoice details</returns>
    Task<DomainRegistrationResponseDto> RegisterDomainForCustomerAsync(RegisterDomainForCustomerDto dto);

    /// <summary>
    /// Checks if a domain is available for registration
    /// </summary>
    /// <param name="domainName">The domain name to check</param>
    /// <returns>Availability information</returns>
    Task<DomainAvailabilityResponseDto> CheckDomainAvailabilityAsync(string domainName);

    /// <summary>
    /// Gets pricing for a specific TLD
    /// </summary>
    /// <param name="tld">The top-level domain (e.g., "com", "net")</param>
    /// <param name="registrarId">Optional: specific registrar ID</param>
    /// <returns>Pricing information</returns>
    Task<DomainPricingDto?> GetDomainPricingAsync(string tld, int? registrarId = null);

    /// <summary>
    /// Gets all available TLDs with pricing
    /// </summary>
    /// <returns>List of available TLDs</returns>
    Task<IEnumerable<AvailableTldDto>> GetAvailableTldsAsync();
}
