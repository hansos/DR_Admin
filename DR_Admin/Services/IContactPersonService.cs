using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing contact persons associated with customers
/// </summary>
public interface IContactPersonService
{
    /// <summary>
    /// Retrieves all contact persons in the system
    /// </summary>
    /// <returns>A collection of contact person DTOs</returns>
    Task<IEnumerable<ContactPersonDto>> GetAllContactPersonsAsync();

    /// <summary>
    /// Retrieves all contact persons marked as domain global
    /// </summary>
    /// <returns>A collection of domain global contact person DTOs</returns>
    Task<IEnumerable<ContactPersonDto>> GetDomainGlobalContactPersonsAsync();
    
    Task<PagedResult<ContactPersonDto>> GetAllContactPersonsPagedAsync(PaginationParameters parameters);
    
    /// <summary>
    /// Retrieves all contact persons for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of contact person DTOs for the specified customer</returns>
    Task<IEnumerable<ContactPersonDto>> GetContactPersonsByCustomerIdAsync(int customerId);
    
    /// <summary>
    /// Retrieves a specific contact person by their unique identifier
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <returns>The contact person DTO if found, otherwise null</returns>
    Task<ContactPersonDto?> GetContactPersonByIdAsync(int id);
    
    /// <summary>
    /// Creates a new contact person
    /// </summary>
    /// <param name="createDto">The contact person creation data</param>
    /// <returns>The newly created contact person DTO</returns>
    Task<ContactPersonDto> CreateContactPersonAsync(CreateContactPersonDto createDto);
    
    /// <summary>
    /// Updates an existing contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <param name="updateDto">The contact person update data</param>
    /// <returns>The updated contact person DTO if found, otherwise null</returns>
    Task<ContactPersonDto?> UpdateContactPersonAsync(int id, UpdateContactPersonDto updateDto);

    /// <summary>
    /// Updates the domain global flag for an existing contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <param name="isDomainGlobal">The new domain global value</param>
    /// <returns>The updated contact person DTO if found, otherwise null</returns>
    Task<ContactPersonDto?> UpdateContactPersonIsDomainGlobalAsync(int id, bool isDomainGlobal);
    
    /// <summary>
    /// Deletes a contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <returns>True if the contact person was deleted, otherwise false</returns>
    Task<bool> DeleteContactPersonAsync(int id);

    /// <summary>
    /// Retrieves contact persons for a specific customer categorized by role preference and usage
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="roleType">The contact role type to filter and sort by</param>
    /// <returns>Categorized list of contact persons sorted by preference</returns>
    Task<CategorizedContactPersonListResponse> GetContactPersonsForRoleAsync(int customerId, ContactRoleType roleType);
}
