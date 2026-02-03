using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing name servers associated with domains
/// </summary>
public interface INameServerService
{
    /// <summary>
    /// Retrieves all name servers in the system
    /// </summary>
    /// <returns>A collection of all name servers</returns>
    Task<IEnumerable<NameServerDto>> GetAllNameServersAsync();
    
    /// <summary>
    /// Retrieves a paginated list of name servers
    /// </summary>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>A paginated result of name servers</returns>
    Task<PagedResult<NameServerDto>> GetAllNameServersPagedAsync(PaginationParameters parameters);
    
    /// <summary>
    /// Retrieves a specific name server by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the name server</param>
    /// <returns>The name server if found; otherwise, null</returns>
    Task<NameServerDto?> GetNameServerByIdAsync(int id);
    
    /// <summary>
    /// Retrieves all name servers for a specific domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>A collection of name servers for the specified domain</returns>
    Task<IEnumerable<NameServerDto>> GetNameServersByDomainIdAsync(int domainId);
    
    /// <summary>
    /// Creates a new name server
    /// </summary>
    /// <param name="createDto">The name server creation data</param>
    /// <returns>The newly created name server</returns>
    Task<NameServerDto> CreateNameServerAsync(CreateNameServerDto createDto);
    
    /// <summary>
    /// Updates an existing name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to update</param>
    /// <param name="updateDto">The name server update data</param>
    /// <returns>The updated name server if found; otherwise, null</returns>
    Task<NameServerDto?> UpdateNameServerAsync(int id, UpdateNameServerDto updateDto);
    
    /// <summary>
    /// Deletes a name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to delete</param>
    /// <returns>True if the name server was deleted; otherwise, false</returns>
    Task<bool> DeleteNameServerAsync(int id);
}
