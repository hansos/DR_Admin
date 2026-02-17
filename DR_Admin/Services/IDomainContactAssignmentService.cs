using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing domain contact assignments
/// </summary>
public interface IDomainContactAssignmentService
{
    /// <summary>
    /// Retrieves all contact assignments for a specific domain
    /// </summary>
    /// <param name="registeredDomainId">The registered domain ID</param>
    /// <returns>A collection of domain contact assignment DTOs</returns>
    Task<IEnumerable<DomainContactAssignmentDto>> GetAssignmentsByDomainAsync(int registeredDomainId);
    
    /// <summary>
    /// Retrieves all domains assigned to a specific contact person
    /// </summary>
    /// <param name="contactPersonId">The contact person ID</param>
    /// <returns>A collection of domain contact assignment DTOs</returns>
    Task<IEnumerable<DomainContactAssignmentDto>> GetAssignmentsByContactPersonAsync(int contactPersonId);
    
    /// <summary>
    /// Retrieves a specific assignment by ID
    /// </summary>
    /// <param name="id">The assignment ID</param>
    /// <returns>The domain contact assignment DTO if found, otherwise null</returns>
    Task<DomainContactAssignmentDto?> GetAssignmentByIdAsync(int id);
    
    /// <summary>
    /// Creates a new domain contact assignment
    /// </summary>
    /// <param name="createDto">The assignment creation data</param>
    /// <returns>The newly created assignment DTO</returns>
    Task<DomainContactAssignmentDto> CreateAssignmentAsync(CreateDomainContactAssignmentDto createDto);
    
    /// <summary>
    /// Updates an existing domain contact assignment
    /// </summary>
    /// <param name="id">The assignment ID</param>
    /// <param name="updateDto">The assignment update data</param>
    /// <returns>The updated assignment DTO if found, otherwise null</returns>
    Task<DomainContactAssignmentDto?> UpdateAssignmentAsync(int id, UpdateDomainContactAssignmentDto updateDto);
    
    /// <summary>
    /// Deletes a domain contact assignment
    /// </summary>
    /// <param name="id">The assignment ID</param>
    /// <returns>True if the assignment was deleted, otherwise false</returns>
    Task<bool> DeleteAssignmentAsync(int id);
    
    /// <summary>
    /// Assigns a contact person to a domain with a specific role
    /// </summary>
    /// <param name="registeredDomainId">The registered domain ID</param>
    /// <param name="contactPersonId">The contact person ID</param>
    /// <param name="roleType">The role type (Registrant, Administrative, Technical, Billing)</param>
    /// <returns>The newly created assignment DTO</returns>
    Task<DomainContactAssignmentDto> AssignContactToDomainAsync(int registeredDomainId, int contactPersonId, string roleType);
    
    /// <summary>
    /// Synchronizes contact person data to domain contacts for a specific assignment
    /// This creates or updates DomainContact records based on the ContactPerson data
    /// </summary>
    /// <param name="assignmentId">The assignment ID</param>
    /// <returns>True if synchronization was successful</returns>
    Task<bool> SyncContactPersonToDomainContactAsync(int assignmentId);
    
    /// <summary>
    /// Marks all domain contacts linked to a contact person as needing sync
    /// </summary>
    /// <param name="contactPersonId">The contact person ID</param>
    /// <returns>The number of domain contacts marked for sync</returns>
    Task<int> MarkContactsNeedingSyncAsync(int contactPersonId);
}
