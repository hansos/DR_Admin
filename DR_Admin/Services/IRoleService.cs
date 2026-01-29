using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing user roles
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Retrieves all roles from the system
    /// </summary>
    /// <returns>A collection of role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    
    /// <summary>
    /// Retrieves a specific role by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the role</param>
    /// <returns>The role DTO if found, otherwise null</returns>
    Task<RoleDto?> GetRoleByIdAsync(int id);
    
    /// <summary>
    /// Creates a new role in the system
    /// </summary>
    /// <param name="createDto">The data transfer object containing role information</param>
    /// <returns>The created role DTO</returns>
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createDto);
    
    /// <summary>
    /// Updates an existing role's information
    /// </summary>
    /// <param name="id">The unique identifier of the role to update</param>
    /// <param name="updateDto">The data transfer object containing updated role information</param>
    /// <returns>The updated role DTO if found, otherwise null</returns>
    Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateDto);
    
    /// <summary>
    /// Deletes a role from the system
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete</param>
    /// <returns>True if the role was deleted, false if the role was not found</returns>
    Task<bool> DeleteRoleAsync(int id);
    
    /// <summary>
    /// Ensures that a role with the specified name exists in the system, creating it if necessary
    /// </summary>
    /// <param name="roleName">The name of the role to ensure exists</param>
    /// <param name="description">Optional description for the role if it needs to be created</param>
    Task EnsureRoleExistsAsync(string roleName, string description = "");
}
