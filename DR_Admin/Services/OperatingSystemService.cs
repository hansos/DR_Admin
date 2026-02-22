using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OperatingSystem = ISPAdmin.Data.Entities.OperatingSystem;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing operating systems
/// </summary>
public class OperatingSystemService : IOperatingSystemService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<OperatingSystemService>();

    public OperatingSystemService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all operating systems
    /// </summary>
    /// <returns>Collection of operating system DTOs</returns>
    public async Task<IEnumerable<OperatingSystemDto>> GetAllOperatingSystemsAsync()
    {
        try
        {
            _log.Information("Fetching all operating systems");

            var operatingSystems = await _context.OperatingSystems
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} operating systems", operatingSystems.Count);
            return operatingSystems.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all operating systems");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active operating systems only
    /// </summary>
    /// <returns>Collection of active operating system DTOs</returns>
    public async Task<IEnumerable<OperatingSystemDto>> GetActiveOperatingSystemsAsync()
    {
        try
        {
            _log.Information("Fetching active operating systems");

            var operatingSystems = await _context.OperatingSystems
                .AsNoTracking()
                .Where(os => os.IsActive)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} active operating systems", operatingSystems.Count);
            return operatingSystems.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active operating systems");
            throw;
        }
    }

    /// <summary>
    /// Retrieves an operating system by its unique identifier
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <returns>Operating system DTO if found, otherwise null</returns>
    public async Task<OperatingSystemDto?> GetOperatingSystemByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching operating system with ID: {OperatingSystemId}", id);

            var operatingSystem = await _context.OperatingSystems
                .AsNoTracking()
                .FirstOrDefaultAsync(os => os.Id == id);

            if (operatingSystem == null)
            {
                _log.Warning("Operating system with ID {OperatingSystemId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched operating system with ID: {OperatingSystemId}", id);
            return MapToDto(operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching operating system with ID: {OperatingSystemId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new operating system
    /// </summary>
    /// <param name="createDto">The operating system creation data</param>
    /// <returns>The created operating system DTO</returns>
    public async Task<OperatingSystemDto> CreateOperatingSystemAsync(CreateOperatingSystemDto createDto)
    {
        try
        {
            _log.Information("Creating new operating system with name: {Name}", createDto.Name);

            var operatingSystem = new OperatingSystem
            {
                Name = createDto.Name,
                DisplayName = createDto.DisplayName,
                Description = createDto.Description,
                Version = createDto.Version,
                IsActive = createDto.IsActive
            };

            _context.OperatingSystems.Add(operatingSystem);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created operating system with ID: {OperatingSystemId}", operatingSystem.Id);
            return MapToDto(operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating operating system with name: {Name}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing operating system
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <param name="updateDto">The operating system update data</param>
    /// <returns>The updated operating system DTO if found, otherwise null</returns>
    public async Task<OperatingSystemDto?> UpdateOperatingSystemAsync(int id, UpdateOperatingSystemDto updateDto)
    {
        try
        {
            _log.Information("Updating operating system with ID: {OperatingSystemId}", id);

            var operatingSystem = await _context.OperatingSystems.FindAsync(id);

            if (operatingSystem == null)
            {
                _log.Warning("Operating system with ID {OperatingSystemId} not found", id);
                return null;
            }

            operatingSystem.Name = updateDto.Name;
            operatingSystem.DisplayName = updateDto.DisplayName;
            operatingSystem.Description = updateDto.Description;
            operatingSystem.Version = updateDto.Version;
            operatingSystem.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated operating system with ID: {OperatingSystemId}", id);
            return MapToDto(operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating operating system with ID: {OperatingSystemId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an operating system
    /// </summary>
    /// <param name="id">The operating system ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteOperatingSystemAsync(int id)
    {
        try
        {
            _log.Information("Deleting operating system with ID: {OperatingSystemId}", id);

            var operatingSystem = await _context.OperatingSystems.FindAsync(id);

            if (operatingSystem == null)
            {
                _log.Warning("Operating system with ID {OperatingSystemId} not found", id);
                return false;
            }

            _context.OperatingSystems.Remove(operatingSystem);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted operating system with ID: {OperatingSystemId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting operating system with ID: {OperatingSystemId}", id);
            throw;
        }
    }

    private static OperatingSystemDto MapToDto(OperatingSystem operatingSystem)
    {
        return new OperatingSystemDto
        {
            Id = operatingSystem.Id,
            Name = operatingSystem.Name,
            DisplayName = operatingSystem.DisplayName,
            Description = operatingSystem.Description,
            Version = operatingSystem.Version,
            IsActive = operatingSystem.IsActive,
            CreatedAt = operatingSystem.CreatedAt,
            UpdatedAt = operatingSystem.UpdatedAt
        };
    }
}
