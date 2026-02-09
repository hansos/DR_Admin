using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing hosting domains
/// </summary>
public class HostingDomainService : IHostingDomainService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HostingDomainService> _logger;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingDomainService>();

    public HostingDomainService(
        ApplicationDbContext context,
        ILogger<HostingDomainService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HostingDomainDto?> GetDomainAsync(int id)
    {
        try
        {
            var domain = await _context.HostingDomains
                .Include(d => d.HostingAccount)
                .FirstOrDefaultAsync(d => d.Id == id);

            return domain != null ? MapToDto(domain) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting domain {DomainId}", id);
            throw;
        }
    }

    public async Task<List<HostingDomainDto>> GetDomainsByHostingAccountAsync(int hostingAccountId)
    {
        try
        {
            var domains = await _context.HostingDomains
                .Include(d => d.HostingAccount)
                .Where(d => d.HostingAccountId == hostingAccountId)
                .OrderBy(d => d.DomainName)
                .ToListAsync();

            return domains.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting domains for hosting account {AccountId}", hostingAccountId);
            throw;
        }
    }

    public async Task<HostingDomainDto> CreateDomainAsync(HostingDomainCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating domain {DomainName}", dto.DomainName);

            var domain = new HostingDomain
            {
                HostingAccountId = dto.HostingAccountId,
                DomainName = dto.DomainName,
                DomainType = dto.DomainType,
                DocumentRoot = dto.DocumentRoot,
                PhpEnabled = dto.PhpEnabled,
                PhpVersion = dto.PhpVersion,
                Notes = dto.Notes,
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingDomains.Add(domain);
            await _context.SaveChangesAsync();

            _log.Information("Domain created with ID {DomainId}", domain.Id);

            if (syncToServer)
            {
                await SyncDomainToServerAsync(domain.Id);
            }

            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating domain");
            throw;
        }
    }

    public async Task<HostingDomainDto> UpdateDomainAsync(int id, HostingDomainUpdateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Updating domain {DomainId}", id);

            var domain = await _context.HostingDomains.FindAsync(id);
            if (domain == null)
            {
                throw new InvalidOperationException($"Domain with ID {id} not found");
            }

            if (dto.DocumentRoot != null) domain.DocumentRoot = dto.DocumentRoot;
            if (dto.PhpEnabled.HasValue) domain.PhpEnabled = dto.PhpEnabled.Value;
            if (dto.PhpVersion != null) domain.PhpVersion = dto.PhpVersion;
            if (dto.SslEnabled.HasValue) domain.SslEnabled = dto.SslEnabled.Value;
            if (dto.Notes != null) domain.Notes = dto.Notes;

            if (syncToServer)
            {
                domain.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            _log.Information("Domain {DomainId} updated successfully", id);

            if (syncToServer)
            {
                await SyncDomainToServerAsync(id);
            }

            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating domain {DomainId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDomainAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting domain {DomainId}", id);

            var domain = await _context.HostingDomains.FindAsync(id);
            if (domain == null)
            {
                return false;
            }

            // TODO: Delete from server if requested
            if (deleteFromServer && !string.IsNullOrEmpty(domain.ExternalDomainId))
            {
                _log.Information("Server deletion not yet implemented for domain {DomainId}", id);
            }

            _context.HostingDomains.Remove(domain);
            await _context.SaveChangesAsync();

            _log.Information("Domain {DomainId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting domain {DomainId}", id);
            throw;
        }
    }

    public async Task<SyncResultDto> SyncDomainsFromServerAsync(int hostingAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing domains from server for hosting account {AccountId}", hostingAccountId);

            var hostingAccount = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (hostingAccount == null || hostingAccount.ServerControlPanel == null)
            {
                result.Message = "Hosting account or server control panel not found";
                return result;
            }

            // TODO: Implement actual sync from server
            // This would require panel API methods for listing addon domains, parked domains, subdomains

            result.Success = true;
            result.Message = "Domain sync from server not yet fully implemented";
            result.RecordsSynced = 0;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing domains: {ex.Message}";
            _log.Error(ex, "Error syncing domains for hosting account {AccountId}", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncDomainToServerAsync(int domainId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            var domain = await _context.HostingDomains
                .Include(d => d.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(d => d.Id == domainId);

            if (domain == null)
            {
                result.Message = "Domain not found";
                return result;
            }

            // TODO: Implement actual sync to server
            // This would require calling panel API to add addon domain, parked domain, or subdomain

            domain.LastSyncedAt = DateTime.UtcNow;
            domain.SyncStatus = "Synced";
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Domain sync to server not yet fully implemented";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing domain: {ex.Message}";
            _log.Error(ex, "Error syncing domain {DomainId} to server", domainId);
            return result;
        }
    }

    private HostingDomainDto MapToDto(HostingDomain domain)
    {
        return new HostingDomainDto
        {
            Id = domain.Id,
            DomainName = domain.DomainName,
            DomainType = domain.DomainType,
            DocumentRoot = domain.DocumentRoot,
            SslEnabled = domain.SslEnabled,
            SslExpirationDate = domain.SslExpirationDate,
            SslIssuer = domain.SslIssuer,
            PhpEnabled = domain.PhpEnabled,
            PhpVersion = domain.PhpVersion,
            ExternalDomainId = domain.ExternalDomainId,
            LastSyncedAt = domain.LastSyncedAt,
            SyncStatus = domain.SyncStatus,
            CreatedAt = domain.CreatedAt
        };
    }
}
