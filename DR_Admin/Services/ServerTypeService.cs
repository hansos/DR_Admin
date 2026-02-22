using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing server types
/// </summary>
public class ServerTypeService : IServerTypeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerTypeService>();

    public ServerTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all server types
    /// </summary>
    /// <returns>Collection of server type DTOs</returns>
    public async Task<IEnumerable<ServerTypeDto>> GetAllServerTypesAsync()
    {
        try
        {
            _log.Information("Fetching all server types");

            var serverTypes = await _context.ServerTypes
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} server types", serverTypes.Count);
            return serverTypes.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all server types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active server types only
    /// </summary>
    /// <returns>Collection of active server type DTOs</returns>
    public async Task<IEnumerable<ServerTypeDto>> GetActiveServerTypesAsync()
    {
        try
        {
            _log.Information("Fetching active server types");

            var serverTypes = await _context.ServerTypes
                .AsNoTracking()
                .Where(st => st.IsActive)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} active server types", serverTypes.Count);
            return serverTypes.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active server types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a server type by its unique identifier
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <returns>Server type DTO if found, otherwise null</returns>
    public async Task<ServerTypeDto?> GetServerTypeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching server type with ID: {ServerTypeId}", id);

            var serverType = await _context.ServerTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id);

            if (serverType == null)
            {
                _log.Warning("Server type with ID {ServerTypeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched server type with ID: {ServerTypeId}", id);
            return MapToDto(serverType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching server type with ID: {ServerTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new server type
    /// </summary>
    /// <param name="createDto">The server type creation data</param>
    /// <returns>The created server type DTO</returns>
    public async Task<ServerTypeDto> CreateServerTypeAsync(CreateServerTypeDto createDto)
    {
        try
        {
            _log.Information("Creating new server type with name: {Name}", createDto.Name);

            var serverType = new ServerType
            {
                Name = createDto.Name,
                DisplayName = createDto.DisplayName,
                Description = createDto.Description,
                IsActive = createDto.IsActive
            };

            _context.ServerTypes.Add(serverType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created server type with ID: {ServerTypeId}", serverType.Id);
            return MapToDto(serverType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating server type with name: {Name}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing server type
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <param name="updateDto">The server type update data</param>
    /// <returns>The updated server type DTO if found, otherwise null</returns>
    public async Task<ServerTypeDto?> UpdateServerTypeAsync(int id, UpdateServerTypeDto updateDto)
    {
        try
        {
            _log.Information("Updating server type with ID: {ServerTypeId}", id);

            var serverType = await _context.ServerTypes.FindAsync(id);

            if (serverType == null)
            {
                _log.Warning("Server type with ID {ServerTypeId} not found", id);
                return null;
            }

            serverType.Name = updateDto.Name;
            serverType.DisplayName = updateDto.DisplayName;
            serverType.Description = updateDto.Description;
            serverType.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated server type with ID: {ServerTypeId}", id);
            return MapToDto(serverType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating server type with ID: {ServerTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a server type
    /// </summary>
    /// <param name="id">The server type ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteServerTypeAsync(int id)
    {
        try
        {
            _log.Information("Deleting server type with ID: {ServerTypeId}", id);

            var serverType = await _context.ServerTypes.FindAsync(id);

            if (serverType == null)
            {
                _log.Warning("Server type with ID {ServerTypeId} not found", id);
                return false;
            }

            _context.ServerTypes.Remove(serverType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted server type with ID: {ServerTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting server type with ID: {ServerTypeId}", id);
            throw;
        }
    }

    private static ServerTypeDto MapToDto(ServerType serverType)
    {
        return new ServerTypeDto
        {
            Id = serverType.Id,
            Name = serverType.Name,
            DisplayName = serverType.DisplayName,
            Description = serverType.Description,
            IsActive = serverType.IsActive,
            CreatedAt = serverType.CreatedAt,
            UpdatedAt = serverType.UpdatedAt
        };
    }
}
