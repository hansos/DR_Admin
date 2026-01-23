using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing sales agents
/// </summary>
public interface ISalesAgentService
{
    /// <summary>
    /// Retrieves all sales agents
    /// </summary>
    /// <returns>Collection of sales agent DTOs</returns>
    Task<IEnumerable<SalesAgentDto>> GetAllSalesAgentsAsync();
    
    /// <summary>
    /// Retrieves active sales agents only
    /// </summary>
    /// <returns>Collection of active sales agent DTOs</returns>
    Task<IEnumerable<SalesAgentDto>> GetActiveSalesAgentsAsync();
    
    /// <summary>
    /// Retrieves sales agents by reseller company
    /// </summary>
    /// <param name="resellerCompanyId">The ID of the reseller company</param>
    /// <returns>Collection of sales agent DTOs for the specified reseller company</returns>
    Task<IEnumerable<SalesAgentDto>> GetSalesAgentsByResellerCompanyAsync(int resellerCompanyId);
    
    /// <summary>
    /// Retrieves a sales agent by their ID
    /// </summary>
    /// <param name="id">The ID of the sales agent</param>
    /// <returns>The sales agent DTO, or null if not found</returns>
    Task<SalesAgentDto?> GetSalesAgentByIdAsync(int id);
    
    /// <summary>
    /// Creates a new sales agent
    /// </summary>
    /// <param name="dto">The sales agent data to create</param>
    /// <returns>The created sales agent DTO</returns>
    Task<SalesAgentDto> CreateSalesAgentAsync(CreateSalesAgentDto dto);
    
    /// <summary>
    /// Updates an existing sales agent
    /// </summary>
    /// <param name="id">The ID of the sales agent to update</param>
    /// <param name="dto">The updated sales agent data</param>
    /// <returns>The updated sales agent DTO, or null if not found</returns>
    Task<SalesAgentDto?> UpdateSalesAgentAsync(int id, UpdateSalesAgentDto dto);
    
    /// <summary>
    /// Deletes a sales agent
    /// </summary>
    /// <param name="id">The ID of the sales agent to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteSalesAgentAsync(int id);
}
