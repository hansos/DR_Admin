using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing server IP addresses
/// </summary>
public class ServerIpAddressService : IServerIpAddressService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerIpAddressService>();

    public ServerIpAddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all server IP addresses
    /// </summary>
    /// <returns>Collection of server IP address DTOs</returns>
    public async Task<IEnumerable<ServerIpAddressDto>> GetAllServerIpAddressesAsync()
    {
        try
        {
            _log.Information("Fetching all server IP addresses");
            
            var ipAddresses = await _context.ServerIpAddresses
                .AsNoTracking()
                .ToListAsync();

            var ipAddressDtos = ipAddresses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} server IP addresses", ipAddresses.Count);
            return ipAddressDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all server IP addresses");
            throw;
        }
    }

    /// <summary>
    /// Retrieves IP addresses for a specific server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <returns>Collection of server IP address DTOs</returns>
    public async Task<IEnumerable<ServerIpAddressDto>> GetServerIpAddressesByServerIdAsync(int serverId)
    {
        try
        {
            _log.Information("Fetching IP addresses for server ID: {ServerId}", serverId);
            
            var ipAddresses = await _context.ServerIpAddresses
                .AsNoTracking()
                .Where(ip => ip.ServerId == serverId)
                .ToListAsync();

            var ipAddressDtos = ipAddresses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} IP addresses for server ID: {ServerId}", ipAddresses.Count, serverId);
            return ipAddressDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching IP addresses for server ID: {ServerId}", serverId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a server IP address by its unique identifier
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <returns>Server IP address DTO if found, otherwise null</returns>
    public async Task<ServerIpAddressDto?> GetServerIpAddressByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching server IP address with ID: {IpAddressId}", id);
            
            var ipAddress = await _context.ServerIpAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(ip => ip.Id == id);

            if (ipAddress == null)
            {
                _log.Warning("Server IP address with ID {IpAddressId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched server IP address with ID: {IpAddressId}", id);
            return MapToDto(ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching server IP address with ID: {IpAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new server IP address
    /// </summary>
    /// <param name="createDto">The IP address creation data</param>
    /// <returns>The created server IP address DTO</returns>
    public async Task<ServerIpAddressDto> CreateServerIpAddressAsync(CreateServerIpAddressDto createDto)
    {
        try
        {
            _log.Information("Creating new server IP address: {IpAddress}", createDto.IpAddress);

            var ipAddress = new ServerIpAddress
            {
                ServerId = createDto.ServerId,
                IpAddress = createDto.IpAddress,
                IpVersion = createDto.IpVersion,
                IsPrimary = createDto.IsPrimary,
                Status = createDto.Status,
                AssignedTo = createDto.AssignedTo,
                Notes = createDto.Notes
            };

            _context.ServerIpAddresses.Add(ipAddress);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created server IP address with ID: {IpAddressId}", ipAddress.Id);
            return MapToDto(ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating server IP address: {IpAddress}", createDto.IpAddress);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing server IP address
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <param name="updateDto">The IP address update data</param>
    /// <returns>The updated server IP address DTO if found, otherwise null</returns>
    public async Task<ServerIpAddressDto?> UpdateServerIpAddressAsync(int id, UpdateServerIpAddressDto updateDto)
    {
        try
        {
            _log.Information("Updating server IP address with ID: {IpAddressId}", id);

            var ipAddress = await _context.ServerIpAddresses.FindAsync(id);

            if (ipAddress == null)
            {
                _log.Warning("Server IP address with ID {IpAddressId} not found", id);
                return null;
            }

            ipAddress.IpAddress = updateDto.IpAddress;
            ipAddress.IpVersion = updateDto.IpVersion;
            ipAddress.IsPrimary = updateDto.IsPrimary;
            ipAddress.Status = updateDto.Status;
            ipAddress.AssignedTo = updateDto.AssignedTo;
            ipAddress.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated server IP address with ID: {IpAddressId}", id);
            return MapToDto(ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating server IP address with ID: {IpAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a server IP address
    /// </summary>
    /// <param name="id">The IP address ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteServerIpAddressAsync(int id)
    {
        try
        {
            _log.Information("Deleting server IP address with ID: {IpAddressId}", id);

            var ipAddress = await _context.ServerIpAddresses.FindAsync(id);

            if (ipAddress == null)
            {
                _log.Warning("Server IP address with ID {IpAddressId} not found", id);
                return false;
            }

            _context.ServerIpAddresses.Remove(ipAddress);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted server IP address with ID: {IpAddressId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting server IP address with ID: {IpAddressId}", id);
            throw;
        }
    }

    private static ServerIpAddressDto MapToDto(ServerIpAddress ipAddress)
    {
        return new ServerIpAddressDto
        {
            Id = ipAddress.Id,
            ServerId = ipAddress.ServerId,
            IpAddress = ipAddress.IpAddress,
            IpVersion = ipAddress.IpVersion,
            IsPrimary = ipAddress.IsPrimary,
            Status = ipAddress.Status,
            AssignedTo = ipAddress.AssignedTo,
            Notes = ipAddress.Notes,
            CreatedAt = ipAddress.CreatedAt,
            UpdatedAt = ipAddress.UpdatedAt
        };
    }
}
