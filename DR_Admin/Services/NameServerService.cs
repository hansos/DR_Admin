using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing name servers associated with domains
/// </summary>
public class NameServerService : INameServerService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<NameServerService>();

    public NameServerService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all name servers in the system
    /// </summary>
    /// <returns>A collection of all name servers</returns>
    public async Task<IEnumerable<NameServerDto>> GetAllNameServersAsync()
    {
        try
        {
            _log.Information("Fetching all name servers");
            
            var nameServers = await _context.NameServers
                .AsNoTracking()
                .Include(n => n.NameServerDomains)
                .OrderBy(n => n.Hostname)
                .ThenBy(n => n.SortOrder)
                .ToListAsync();

            var nameServerDtos = nameServers.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} name servers", nameServers.Count);
            return nameServerDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all name servers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a paginated list of name servers
    /// </summary>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>A paginated result of name servers</returns>
    public async Task<PagedResult<NameServerDto>> GetAllNameServersPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated name servers - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.NameServers
                .AsNoTracking()
                .CountAsync();

            var nameServers = await _context.NameServers
                .AsNoTracking()
                .Include(n => n.NameServerDomains)
                .OrderBy(n => n.Hostname)
                .ThenBy(n => n.SortOrder)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var nameServerDtos = nameServers.Select(MapToDto).ToList();
            
            var result = new PagedResult<NameServerDto>(
                nameServerDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of name servers - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, nameServerDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated name servers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific name server by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the name server</param>
    /// <returns>The name server if found; otherwise, null</returns>
    public async Task<NameServerDto?> GetNameServerByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching name server with ID: {NameServerId}", id);
            
            var nameServer = await _context.NameServers
                .AsNoTracking()
                .Include(n => n.NameServerDomains)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nameServer == null)
            {
                _log.Warning("Name server with ID {NameServerId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched name server with ID: {NameServerId}", id);
            return MapToDto(nameServer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching name server with ID: {NameServerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all name servers for a specific domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>A collection of name servers for the specified domain</returns>
    public async Task<IEnumerable<NameServerDto>> GetNameServersByDomainIdAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching name servers for domain ID: {DomainId}", domainId);
            
            var nameServers = await _context.NameServers
                .AsNoTracking()
                .Include(n => n.NameServerDomains)
                .Where(n => n.NameServerDomains.Any(nd => nd.DomainId == domainId))
                .OrderBy(n => n.SortOrder)
                .ToListAsync();

            var nameServerDtos = nameServers.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} name servers for domain ID: {DomainId}", nameServers.Count, domainId);
            return nameServerDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching name servers for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new name server
    /// </summary>
    /// <param name="createDto">The name server creation data</param>
    /// <returns>The newly created name server</returns>
    public async Task<NameServerDto> CreateNameServerAsync(CreateNameServerDto createDto)
    {
        try
        {
            _log.Information("Creating new name server for {DomainCount} domains", createDto.DomainIds.Count);

            var domainIds = createDto.DomainIds.Distinct().ToList();
            await ValidateDomainIdsAsync(domainIds);
            await ValidateServerIdAsync(createDto.ServerId);

            // Validate hostname
            if (string.IsNullOrWhiteSpace(createDto.Hostname))
            {
                throw new InvalidOperationException("Hostname is required");
            }

            var nameServer = new NameServer
            {
                ServerId = createDto.ServerId,
                Hostname = createDto.Hostname.Trim(),
                IpAddress = string.IsNullOrWhiteSpace(createDto.IpAddress) ? null : createDto.IpAddress.Trim(),
                IsPrimary = createDto.IsPrimary,
                SortOrder = createDto.SortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var domainId in domainIds)
            {
                nameServer.NameServerDomains.Add(new NameServerDomain
                {
                    DomainId = domainId,
                    NameServer = nameServer
                });
            }

            _context.NameServers.Add(nameServer);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created name server with ID: {NameServerId}", nameServer.Id);

            // Fetch the created record with navigation properties
            var createdRecord = await _context.NameServers
                .AsNoTracking()
                .Include(n => n.NameServerDomains)
                .FirstOrDefaultAsync(n => n.Id == nameServer.Id);

            return MapToDto(createdRecord!);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating name server");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to update</param>
    /// <param name="updateDto">The name server update data</param>
    /// <returns>The updated name server if found; otherwise, null</returns>
    public async Task<NameServerDto?> UpdateNameServerAsync(int id, UpdateNameServerDto updateDto)
    {
        try
        {
            _log.Information("Updating name server with ID: {NameServerId}", id);

            var nameServer = await _context.NameServers
                .Include(n => n.NameServerDomains)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nameServer == null)
            {
                _log.Warning("Name server with ID {NameServerId} not found for update", id);
                return null;
            }

            var domainIds = updateDto.DomainIds.Distinct().ToList();
            await ValidateDomainIdsAsync(domainIds);
            await ValidateServerIdAsync(updateDto.ServerId);

            // Validate hostname
            if (string.IsNullOrWhiteSpace(updateDto.Hostname))
            {
                throw new InvalidOperationException("Hostname is required");
            }

            nameServer.ServerId = updateDto.ServerId;
            nameServer.Hostname = updateDto.Hostname.Trim();
            nameServer.IpAddress = string.IsNullOrWhiteSpace(updateDto.IpAddress) ? null : updateDto.IpAddress.Trim();
            nameServer.IsPrimary = updateDto.IsPrimary;
            nameServer.SortOrder = updateDto.SortOrder;
            nameServer.UpdatedAt = DateTime.UtcNow;

            nameServer.NameServerDomains.Clear();
            foreach (var domainId in domainIds)
            {
                nameServer.NameServerDomains.Add(new NameServerDomain
                {
                    NameServerId = nameServer.Id,
                    DomainId = domainId
                });
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated name server with ID: {NameServerId}", id);
            return MapToDto(nameServer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating name server with ID: {NameServerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to delete</param>
    /// <returns>True if the name server was deleted; otherwise, false</returns>
    public async Task<bool> DeleteNameServerAsync(int id)
    {
        try
        {
            _log.Information("Deleting name server with ID: {NameServerId}", id);

            var nameServer = await _context.NameServers.FindAsync(id);

            if (nameServer == null)
            {
                _log.Warning("Name server with ID {NameServerId} not found for deletion", id);
                return false;
            }

            _context.NameServers.Remove(nameServer);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted name server with ID: {NameServerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting name server with ID: {NameServerId}", id);
            throw;
        }
    }

    private static NameServerDto MapToDto(NameServer nameServer)
    {
        return new NameServerDto
        {
            Id = nameServer.Id,
            DomainIds = nameServer.NameServerDomains.Select(nd => nd.DomainId).OrderBy(id => id).ToList(),
            ServerId = nameServer.ServerId,
            Hostname = nameServer.Hostname,
            IpAddress = nameServer.IpAddress,
            IsPrimary = nameServer.IsPrimary,
            SortOrder = nameServer.SortOrder,
            CreatedAt = nameServer.CreatedAt,
            UpdatedAt = nameServer.UpdatedAt
        };
    }

    private async Task ValidateDomainIdsAsync(IReadOnlyCollection<int> domainIds)
    {
        if (domainIds.Count == 0)
        {
            return;
        }

        var validCount = await _context.RegisteredDomains
            .CountAsync(d => domainIds.Contains(d.Id));

        if (validCount != domainIds.Count)
        {
            throw new InvalidOperationException("One or more domain IDs were not found");
        }
    }

    private async Task ValidateServerIdAsync(int? serverId)
    {
        if (!serverId.HasValue)
        {
            return;
        }

        var exists = await _context.Servers.AnyAsync(s => s.Id == serverId.Value);
        if (!exists)
        {
            throw new InvalidOperationException($"Server with ID {serverId.Value} not found");
        }
    }
}
