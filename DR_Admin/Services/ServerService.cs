using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing servers
/// </summary>
public class ServerService : IServerService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerService>();

    public ServerService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all servers
    /// </summary>
    /// <returns>Collection of server DTOs</returns>
    public async Task<IEnumerable<ServerDto>> GetAllServersAsync()
    {
        try
        {
            _log.Information("Fetching all servers");
            
            var servers = await _context.Servers
                .AsNoTracking()
                .ToListAsync();

            var serverDtos = servers.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} servers", servers.Count);
            return serverDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all servers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a server by its unique identifier
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <returns>Server DTO if found, otherwise null</returns>
    public async Task<ServerDto?> GetServerByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching server with ID: {ServerId}", id);
            
            var server = await _context.Servers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (server == null)
            {
                _log.Warning("Server with ID {ServerId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched server with ID: {ServerId}", id);
            return MapToDto(server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching server with ID: {ServerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new server
    /// </summary>
    /// <param name="createDto">The server creation data</param>
    /// <returns>The created server DTO</returns>
    public async Task<ServerDto> CreateServerAsync(CreateServerDto createDto)
    {
        try
        {
            _log.Information("Creating new server with name: {ServerName}", createDto.Name);

            var server = new Server
            {
                Name = createDto.Name,
                ServerType = createDto.ServerType,
                HostProvider = createDto.HostProvider,
                Location = createDto.Location,
                OperatingSystem = createDto.OperatingSystem,
                Status = createDto.Status,
                CpuCores = createDto.CpuCores,
                RamMB = createDto.RamMB,
                DiskSpaceGB = createDto.DiskSpaceGB,
                Notes = createDto.Notes
            };

            _context.Servers.Add(server);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created server with ID: {ServerId}", server.Id);
            return MapToDto(server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating server with name: {ServerName}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing server
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <param name="updateDto">The server update data</param>
    /// <returns>The updated server DTO if found, otherwise null</returns>
    public async Task<ServerDto?> UpdateServerAsync(int id, UpdateServerDto updateDto)
    {
        try
        {
            _log.Information("Updating server with ID: {ServerId}", id);

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
            {
                _log.Warning("Server with ID {ServerId} not found", id);
                return null;
            }

            server.Name = updateDto.Name;
            server.ServerType = updateDto.ServerType;
            server.HostProvider = updateDto.HostProvider;
            server.Location = updateDto.Location;
            server.OperatingSystem = updateDto.OperatingSystem;
            server.Status = updateDto.Status;
            server.CpuCores = updateDto.CpuCores;
            server.RamMB = updateDto.RamMB;
            server.DiskSpaceGB = updateDto.DiskSpaceGB;
            server.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated server with ID: {ServerId}", id);
            return MapToDto(server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating server with ID: {ServerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a server
    /// </summary>
    /// <param name="id">The server ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteServerAsync(int id)
    {
        try
        {
            _log.Information("Deleting server with ID: {ServerId}", id);

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
            {
                _log.Warning("Server with ID {ServerId} not found", id);
                return false;
            }

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted server with ID: {ServerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting server with ID: {ServerId}", id);
            throw;
        }
    }

    private static ServerDto MapToDto(Server server)
    {
        return new ServerDto
        {
            Id = server.Id,
            Name = server.Name,
            ServerType = server.ServerType,
            HostProvider = server.HostProvider,
            Location = server.Location,
            OperatingSystem = server.OperatingSystem,
            Status = server.Status,
            CpuCores = server.CpuCores,
            RamMB = server.RamMB,
            DiskSpaceGB = server.DiskSpaceGB,
            Notes = server.Notes,
            CreatedAt = server.CreatedAt,
            UpdatedAt = server.UpdatedAt
        };
    }
}
