using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing host providers
/// </summary>
public class HostProviderService : IHostProviderService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostProviderService>();

    public HostProviderService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all host providers
    /// </summary>
    /// <returns>Collection of host provider DTOs</returns>
    public async Task<IEnumerable<HostProviderDto>> GetAllHostProvidersAsync()
    {
        try
        {
            _log.Information("Fetching all host providers");

            var hostProviders = await _context.HostProviders
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} host providers", hostProviders.Count);
            return hostProviders.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all host providers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active host providers only
    /// </summary>
    /// <returns>Collection of active host provider DTOs</returns>
    public async Task<IEnumerable<HostProviderDto>> GetActiveHostProvidersAsync()
    {
        try
        {
            _log.Information("Fetching active host providers");

            var hostProviders = await _context.HostProviders
                .AsNoTracking()
                .Where(hp => hp.IsActive)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} active host providers", hostProviders.Count);
            return hostProviders.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active host providers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a host provider by its unique identifier
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <returns>Host provider DTO if found, otherwise null</returns>
    public async Task<HostProviderDto?> GetHostProviderByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching host provider with ID: {HostProviderId}", id);

            var hostProvider = await _context.HostProviders
                .AsNoTracking()
                .FirstOrDefaultAsync(hp => hp.Id == id);

            if (hostProvider == null)
            {
                _log.Warning("Host provider with ID {HostProviderId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched host provider with ID: {HostProviderId}", id);
            return MapToDto(hostProvider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching host provider with ID: {HostProviderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new host provider
    /// </summary>
    /// <param name="createDto">The host provider creation data</param>
    /// <returns>The created host provider DTO</returns>
    public async Task<HostProviderDto> CreateHostProviderAsync(CreateHostProviderDto createDto)
    {
        try
        {
            _log.Information("Creating new host provider with name: {Name}", createDto.Name);

            var hostProvider = new HostProvider
            {
                Name = createDto.Name,
                DisplayName = createDto.DisplayName,
                Description = createDto.Description,
                WebsiteUrl = createDto.WebsiteUrl,
                IsActive = createDto.IsActive
            };

            _context.HostProviders.Add(hostProvider);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created host provider with ID: {HostProviderId}", hostProvider.Id);
            return MapToDto(hostProvider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating host provider with name: {Name}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing host provider
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <param name="updateDto">The host provider update data</param>
    /// <returns>The updated host provider DTO if found, otherwise null</returns>
    public async Task<HostProviderDto?> UpdateHostProviderAsync(int id, UpdateHostProviderDto updateDto)
    {
        try
        {
            _log.Information("Updating host provider with ID: {HostProviderId}", id);

            var hostProvider = await _context.HostProviders.FindAsync(id);

            if (hostProvider == null)
            {
                _log.Warning("Host provider with ID {HostProviderId} not found", id);
                return null;
            }

            hostProvider.Name = updateDto.Name;
            hostProvider.DisplayName = updateDto.DisplayName;
            hostProvider.Description = updateDto.Description;
            hostProvider.WebsiteUrl = updateDto.WebsiteUrl;
            hostProvider.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated host provider with ID: {HostProviderId}", id);
            return MapToDto(hostProvider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating host provider with ID: {HostProviderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a host provider
    /// </summary>
    /// <param name="id">The host provider ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteHostProviderAsync(int id)
    {
        try
        {
            _log.Information("Deleting host provider with ID: {HostProviderId}", id);

            var hostProvider = await _context.HostProviders.FindAsync(id);

            if (hostProvider == null)
            {
                _log.Warning("Host provider with ID {HostProviderId} not found", id);
                return false;
            }

            _context.HostProviders.Remove(hostProvider);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted host provider with ID: {HostProviderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting host provider with ID: {HostProviderId}", id);
            throw;
        }
    }

    private static HostProviderDto MapToDto(HostProvider hostProvider)
    {
        return new HostProviderDto
        {
            Id = hostProvider.Id,
            Name = hostProvider.Name,
            DisplayName = hostProvider.DisplayName,
            Description = hostProvider.Description,
            WebsiteUrl = hostProvider.WebsiteUrl,
            IsActive = hostProvider.IsActive,
            CreatedAt = hostProvider.CreatedAt,
            UpdatedAt = hostProvider.UpdatedAt
        };
    }
}
