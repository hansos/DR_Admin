using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing control panel types
/// </summary>
public interface IControlPanelTypeService
{
    /// <summary>
    /// Retrieves all control panel types
    /// </summary>
    /// <returns>Collection of control panel type DTOs</returns>
    Task<IEnumerable<ControlPanelTypeDto>> GetAllControlPanelTypesAsync();
    
    /// <summary>
    /// Retrieves active control panel types only
    /// </summary>
    /// <returns>Collection of active control panel type DTOs</returns>
    Task<IEnumerable<ControlPanelTypeDto>> GetActiveControlPanelTypesAsync();
    
    /// <summary>
    /// Retrieves a control panel type by its unique identifier
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <returns>Control panel type DTO if found, otherwise null</returns>
    Task<ControlPanelTypeDto?> GetControlPanelTypeByIdAsync(int id);
    
    /// <summary>
    /// Creates a new control panel type
    /// </summary>
    /// <param name="createDto">The control panel type creation data</param>
    /// <returns>The created control panel type DTO</returns>
    Task<ControlPanelTypeDto> CreateControlPanelTypeAsync(CreateControlPanelTypeDto createDto);
    
    /// <summary>
    /// Updates an existing control panel type
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <param name="updateDto">The control panel type update data</param>
    /// <returns>The updated control panel type DTO if found, otherwise null</returns>
    Task<ControlPanelTypeDto?> UpdateControlPanelTypeAsync(int id, UpdateControlPanelTypeDto updateDto);
    
    /// <summary>
    /// Deletes a control panel type
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteControlPanelTypeAsync(int id);
}
