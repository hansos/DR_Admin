using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing control panel types
/// </summary>
public class ControlPanelTypeService : IControlPanelTypeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ControlPanelTypeService>();

    public ControlPanelTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all control panel types
    /// </summary>
    /// <returns>Collection of control panel type DTOs</returns>
    public async Task<IEnumerable<ControlPanelTypeDto>> GetAllControlPanelTypesAsync()
    {
        try
        {
            _log.Information("Fetching all control panel types");
            
            var controlPanelTypes = await _context.ControlPanelTypes
                .AsNoTracking()
                .ToListAsync();

            var controlPanelTypeDtos = controlPanelTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} control panel types", controlPanelTypes.Count);
            return controlPanelTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all control panel types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active control panel types only
    /// </summary>
    /// <returns>Collection of active control panel type DTOs</returns>
    public async Task<IEnumerable<ControlPanelTypeDto>> GetActiveControlPanelTypesAsync()
    {
        try
        {
            _log.Information("Fetching active control panel types");
            
            var controlPanelTypes = await _context.ControlPanelTypes
                .AsNoTracking()
                .Where(cpt => cpt.IsActive)
                .ToListAsync();

            var controlPanelTypeDtos = controlPanelTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active control panel types", controlPanelTypes.Count);
            return controlPanelTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active control panel types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a control panel type by its unique identifier
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <returns>Control panel type DTO if found, otherwise null</returns>
    public async Task<ControlPanelTypeDto?> GetControlPanelTypeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching control panel type with ID: {ControlPanelTypeId}", id);
            
            var controlPanelType = await _context.ControlPanelTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(cpt => cpt.Id == id);

            if (controlPanelType == null)
            {
                _log.Warning("Control panel type with ID {ControlPanelTypeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched control panel type with ID: {ControlPanelTypeId}", id);
            return MapToDto(controlPanelType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching control panel type with ID: {ControlPanelTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new control panel type
    /// </summary>
    /// <param name="createDto">The control panel type creation data</param>
    /// <returns>The created control panel type DTO</returns>
    public async Task<ControlPanelTypeDto> CreateControlPanelTypeAsync(CreateControlPanelTypeDto createDto)
    {
        try
        {
            _log.Information("Creating new control panel type with name: {Name}", createDto.Name);

            var controlPanelType = new ControlPanelType
            {
                Name = createDto.Name,
                DisplayName = createDto.DisplayName,
                Description = createDto.Description,
                Version = createDto.Version,
                WebsiteUrl = createDto.WebsiteUrl,
                IsActive = createDto.IsActive
            };

            _context.ControlPanelTypes.Add(controlPanelType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created control panel type with ID: {ControlPanelTypeId}", controlPanelType.Id);
            return MapToDto(controlPanelType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating control panel type with name: {Name}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing control panel type
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <param name="updateDto">The control panel type update data</param>
    /// <returns>The updated control panel type DTO if found, otherwise null</returns>
    public async Task<ControlPanelTypeDto?> UpdateControlPanelTypeAsync(int id, UpdateControlPanelTypeDto updateDto)
    {
        try
        {
            _log.Information("Updating control panel type with ID: {ControlPanelTypeId}", id);

            var controlPanelType = await _context.ControlPanelTypes.FindAsync(id);

            if (controlPanelType == null)
            {
                _log.Warning("Control panel type with ID {ControlPanelTypeId} not found", id);
                return null;
            }

            controlPanelType.Name = updateDto.Name;
            controlPanelType.DisplayName = updateDto.DisplayName;
            controlPanelType.Description = updateDto.Description;
            controlPanelType.Version = updateDto.Version;
            controlPanelType.WebsiteUrl = updateDto.WebsiteUrl;
            controlPanelType.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated control panel type with ID: {ControlPanelTypeId}", id);
            return MapToDto(controlPanelType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating control panel type with ID: {ControlPanelTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a control panel type
    /// </summary>
    /// <param name="id">The control panel type ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteControlPanelTypeAsync(int id)
    {
        try
        {
            _log.Information("Deleting control panel type with ID: {ControlPanelTypeId}", id);

            var controlPanelType = await _context.ControlPanelTypes.FindAsync(id);

            if (controlPanelType == null)
            {
                _log.Warning("Control panel type with ID {ControlPanelTypeId} not found", id);
                return false;
            }

            _context.ControlPanelTypes.Remove(controlPanelType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted control panel type with ID: {ControlPanelTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting control panel type with ID: {ControlPanelTypeId}", id);
            throw;
        }
    }

    private static ControlPanelTypeDto MapToDto(ControlPanelType controlPanelType)
    {
        return new ControlPanelTypeDto
        {
            Id = controlPanelType.Id,
            Name = controlPanelType.Name,
            DisplayName = controlPanelType.DisplayName,
            Description = controlPanelType.Description,
            Version = controlPanelType.Version,
            WebsiteUrl = controlPanelType.WebsiteUrl,
            IsActive = controlPanelType.IsActive,
            CreatedAt = controlPanelType.CreatedAt,
            UpdatedAt = controlPanelType.UpdatedAt
        };
    }
}
