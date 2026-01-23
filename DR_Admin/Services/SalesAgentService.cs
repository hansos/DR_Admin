using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing sales agents
/// </summary>
public class SalesAgentService : ISalesAgentService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SalesAgentService>();

    public SalesAgentService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all sales agents
    /// </summary>
    /// <returns>Collection of sales agent DTOs</returns>
    public async Task<IEnumerable<SalesAgentDto>> GetAllSalesAgentsAsync()
    {
        try
        {
            _log.Information("Fetching all sales agents");
            
            var agents = await _context.SalesAgents
                .AsNoTracking()
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            var agentDtos = agents.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sales agents", agents.Count);
            return agentDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all sales agents");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active sales agents only
    /// </summary>
    /// <returns>Collection of active sales agent DTOs</returns>
    public async Task<IEnumerable<SalesAgentDto>> GetActiveSalesAgentsAsync()
    {
        try
        {
            _log.Information("Fetching active sales agents");
            
            var agents = await _context.SalesAgents
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            var agentDtos = agents.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active sales agents", agents.Count);
            return agentDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active sales agents");
            throw;
        }
    }

    /// <summary>
    /// Retrieves sales agents by reseller company
    /// </summary>
    /// <param name="resellerCompanyId">The ID of the reseller company</param>
    /// <returns>Collection of sales agent DTOs for the specified reseller company</returns>
    public async Task<IEnumerable<SalesAgentDto>> GetSalesAgentsByResellerCompanyAsync(int resellerCompanyId)
    {
        try
        {
            _log.Information("Fetching sales agents for reseller company ID: {ResellerCompanyId}", resellerCompanyId);
            
            var agents = await _context.SalesAgents
                .AsNoTracking()
                .Where(s => s.ResellerCompanyId == resellerCompanyId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            var agentDtos = agents.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sales agents for reseller company ID: {ResellerCompanyId}", 
                agents.Count, resellerCompanyId);
            return agentDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sales agents for reseller company ID: {ResellerCompanyId}", 
                resellerCompanyId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a sales agent by their ID
    /// </summary>
    /// <param name="id">The ID of the sales agent</param>
    /// <returns>The sales agent DTO, or null if not found</returns>
    public async Task<SalesAgentDto?> GetSalesAgentByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching sales agent with ID: {SalesAgentId}", id);
            
            var agent = await _context.SalesAgents
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (agent == null)
            {
                _log.Warning("Sales agent with ID: {SalesAgentId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched sales agent with ID: {SalesAgentId}", id);
            return MapToDto(agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sales agent with ID: {SalesAgentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new sales agent
    /// </summary>
    /// <param name="dto">The sales agent data to create</param>
    /// <returns>The created sales agent DTO</returns>
    public async Task<SalesAgentDto> CreateSalesAgentAsync(CreateSalesAgentDto dto)
    {
        try
        {
            _log.Information("Creating new sales agent: {FirstName} {LastName}", dto.FirstName, dto.LastName);

            var agent = new SalesAgent
            {
                ResellerCompanyId = dto.ResellerCompanyId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                MobilePhone = dto.MobilePhone,
                IsActive = dto.IsActive,
                Notes = dto.Notes
            };

            _context.SalesAgents.Add(agent);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created sales agent with ID: {SalesAgentId}", agent.Id);
            return MapToDto(agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating sales agent: {FirstName} {LastName}", 
                dto.FirstName, dto.LastName);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing sales agent
    /// </summary>
    /// <param name="id">The ID of the sales agent to update</param>
    /// <param name="dto">The updated sales agent data</param>
    /// <returns>The updated sales agent DTO, or null if not found</returns>
    public async Task<SalesAgentDto?> UpdateSalesAgentAsync(int id, UpdateSalesAgentDto dto)
    {
        try
        {
            _log.Information("Updating sales agent with ID: {SalesAgentId}", id);
            
            var agent = await _context.SalesAgents.FindAsync(id);
            
            if (agent == null)
            {
                _log.Warning("Sales agent with ID: {SalesAgentId} not found for update", id);
                return null;
            }

            agent.ResellerCompanyId = dto.ResellerCompanyId;
            agent.FirstName = dto.FirstName;
            agent.LastName = dto.LastName;
            agent.Email = dto.Email;
            agent.Phone = dto.Phone;
            agent.MobilePhone = dto.MobilePhone;
            agent.IsActive = dto.IsActive;
            agent.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated sales agent with ID: {SalesAgentId}", id);
            return MapToDto(agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating sales agent with ID: {SalesAgentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a sales agent
    /// </summary>
    /// <param name="id">The ID of the sales agent to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    public async Task<bool> DeleteSalesAgentAsync(int id)
    {
        try
        {
            _log.Information("Deleting sales agent with ID: {SalesAgentId}", id);
            
            var agent = await _context.SalesAgents.FindAsync(id);
            
            if (agent == null)
            {
                _log.Warning("Sales agent with ID: {SalesAgentId} not found for deletion", id);
                return false;
            }

            _context.SalesAgents.Remove(agent);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted sales agent with ID: {SalesAgentId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting sales agent with ID: {SalesAgentId}", id);
            throw;
        }
    }

    private static SalesAgentDto MapToDto(SalesAgent agent)
    {
        return new SalesAgentDto
        {
            Id = agent.Id,
            ResellerCompanyId = agent.ResellerCompanyId,
            FirstName = agent.FirstName,
            LastName = agent.LastName,
            Email = agent.Email,
            Phone = agent.Phone,
            MobilePhone = agent.MobilePhone,
            IsActive = agent.IsActive,
            Notes = agent.Notes,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt
        };
    }
}
