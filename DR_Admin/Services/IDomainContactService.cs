using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing domain contact persons
/// </summary>
public interface IDomainContactService
{
    /// <summary>
    /// Retrieves all domain contacts in the system
    /// </summary>
    /// <returns>A collection of domain contact DTOs</returns>
    Task<IEnumerable<DomainContactDto>> GetAllDomainContactsAsync();

    /// <summary>
    /// Retrieves all domain contacts with pagination
    /// </summary>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>A paginated result of domain contact DTOs</returns>
    Task<PagedResult<DomainContactDto>> GetAllDomainContactsPagedAsync(PaginationParameters parameters);

    /// <summary>
    /// Retrieves all domain contacts for a specific domain
    /// </summary>
    /// <param name="domainId">The domain ID</param>
    /// <returns>A collection of domain contact DTOs for the specified domain</returns>
    Task<IEnumerable<DomainContactDto>> GetDomainContactsByDomainIdAsync(int domainId);

    /// <summary>
    /// Retrieves all domain contacts of a specific type for a domain
    /// </summary>
    /// <param name="domainId">The domain ID</param>
    /// <param name="contactType">The contact type (Registrant, Admin, Technical, Billing)</param>
    /// <returns>A collection of domain contact DTOs matching the criteria</returns>
    Task<IEnumerable<DomainContactDto>> GetDomainContactsByTypeAsync(int domainId, string contactType);

    /// <summary>
    /// Retrieves a specific domain contact by their unique identifier
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>The domain contact DTO if found, otherwise null</returns>
    Task<DomainContactDto?> GetDomainContactByIdAsync(int id);

    /// <summary>
    /// Creates a new domain contact
    /// </summary>
    /// <param name="createDto">The domain contact creation data</param>
    /// <returns>The newly created domain contact DTO</returns>
    Task<DomainContactDto> CreateDomainContactAsync(CreateDomainContactDto createDto);

    /// <summary>
    /// Updates an existing domain contact
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <param name="updateDto">The domain contact update data</param>
    /// <returns>The updated domain contact DTO if found, otherwise null</returns>
    Task<DomainContactDto?> UpdateDomainContactAsync(int id, UpdateDomainContactDto updateDto);

    /// <summary>
    /// Deletes a domain contact
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>True if the domain contact was deleted, otherwise false</returns>
    Task<bool> DeleteDomainContactAsync(int id);

    /// <summary>
    /// Checks if a domain contact exists
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>True if the domain contact exists, otherwise false</returns>
    Task<bool> DomainContactExistsAsync(int id);
}
