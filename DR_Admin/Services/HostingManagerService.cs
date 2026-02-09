using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HostingPanelLib.Factories;
using HostingPanelLib.Models;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing hosting accounts and synchronization with hosting panels
/// </summary>
public class HostingManagerService : IHostingManagerService
{
    private readonly ApplicationDbContext _context;
    private readonly IHostingSyncService _syncService;
    private readonly ILogger<HostingManagerService> _logger;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingManagerService>();

    public HostingManagerService(
        ApplicationDbContext context,
        IHostingSyncService syncService,
        ILogger<HostingManagerService> logger)
    {
        _context = context;
        _syncService = syncService;
        _logger = logger;
    }

    public async Task<HostingAccountDto?> GetHostingAccountAsync(int id)
    {
        try
        {
            var account = await _context.HostingAccounts
                .Include(h => h.Customer)
                .Include(h => h.Server)
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .FirstOrDefaultAsync(h => h.Id == id);

            return account != null ? MapToDto(account) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting hosting account {AccountId}", id);
            throw;
        }
    }

    public async Task<HostingAccountDto?> GetHostingAccountWithDetailsAsync(int id)
    {
        try
        {
            var account = await _context.HostingAccounts
                .Include(h => h.Customer)
                .Include(h => h.Server)
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Include(h => h.Domains)
                .Include(h => h.EmailAccounts)
                .Include(h => h.Databases)
                    .ThenInclude(db => db.DatabaseUsers)
                .Include(h => h.FtpAccounts)
                .FirstOrDefaultAsync(h => h.Id == id);

            return account != null ? MapToDtoWithDetails(account) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting hosting account with details {AccountId}", id);
            throw;
        }
    }

    public async Task<List<HostingAccountDto>> GetAllHostingAccountsAsync()
    {
        try
        {
            var accounts = await _context.HostingAccounts
                .Include(h => h.Customer)
                .Include(h => h.Server)
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting all hosting accounts");
            throw;
        }
    }

    public async Task<List<HostingAccountDto>> GetHostingAccountsByCustomerAsync(int customerId)
    {
        try
        {
            var accounts = await _context.HostingAccounts
                .Include(h => h.Customer)
                .Include(h => h.Server)
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Where(h => h.CustomerId == customerId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting hosting accounts for customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<List<HostingAccountDto>> GetHostingAccountsByServerAsync(int serverId)
    {
        try
        {
            var accounts = await _context.HostingAccounts
                .Include(h => h.Customer)
                .Include(h => h.Server)
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Where(h => h.ServerId == serverId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting hosting accounts for server {ServerId}", serverId);
            throw;
        }
    }

    public async Task<HostingAccountDto> CreateHostingAccountAsync(HostingAccountCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating hosting account for customer {CustomerId}", dto.CustomerId);

            var account = new HostingAccount
            {
                CustomerId = dto.CustomerId,
                ServiceId = dto.ServiceId,
                ServerId = dto.ServerId,
                ServerControlPanelId = dto.ServerControlPanelId,
                Provider = dto.Provider ?? string.Empty,
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Status = dto.Status ?? "Active",
                ExpirationDate = dto.ExpirationDate,
                PlanName = dto.PlanName,
                DiskQuotaMB = dto.DiskQuotaMB,
                BandwidthLimitMB = dto.BandwidthLimitMB,
                MaxEmailAccounts = dto.MaxEmailAccounts,
                MaxDatabases = dto.MaxDatabases,
                MaxFtpAccounts = dto.MaxFtpAccounts,
                MaxSubdomains = dto.MaxSubdomains,
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingAccounts.Add(account);
            await _context.SaveChangesAsync();

            _log.Information("Hosting account created with ID {AccountId}", account.Id);

            // Sync to server if requested
            if (syncToServer && dto.ServerControlPanelId.HasValue)
            {
                await _syncService.SyncAccountToServerAsync(account.Id);
            }

            return MapToDto(account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating hosting account");
            throw;
        }
    }

    public async Task<HostingAccountDto> UpdateHostingAccountAsync(int id, HostingAccountUpdateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Updating hosting account {AccountId}", id);

            var account = await _context.HostingAccounts.FindAsync(id);
            if (account == null)
            {
                throw new InvalidOperationException($"Hosting account with ID {id} not found");
            }

            // Update fields
            if (dto.Status != null) account.Status = dto.Status;
            if (dto.PlanName != null) account.PlanName = dto.PlanName;
            if (dto.DiskQuotaMB.HasValue) account.DiskQuotaMB = dto.DiskQuotaMB;
            if (dto.BandwidthLimitMB.HasValue) account.BandwidthLimitMB = dto.BandwidthLimitMB;
            if (dto.MaxEmailAccounts.HasValue) account.MaxEmailAccounts = dto.MaxEmailAccounts;
            if (dto.MaxDatabases.HasValue) account.MaxDatabases = dto.MaxDatabases;
            if (dto.MaxFtpAccounts.HasValue) account.MaxFtpAccounts = dto.MaxFtpAccounts;
            if (dto.MaxSubdomains.HasValue) account.MaxSubdomains = dto.MaxSubdomains;
            if (dto.ExpirationDate.HasValue) account.ExpirationDate = dto.ExpirationDate.Value;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                account.PasswordHash = HashPassword(dto.Password);
            }

            if (syncToServer)
            {
                account.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            _log.Information("Hosting account {AccountId} updated successfully", id);

            // Sync to server if requested
            if (syncToServer && account.ServerControlPanelId.HasValue)
            {
                await _syncService.SyncAccountToServerAsync(account.Id);
            }

            return MapToDto(account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating hosting account {AccountId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteHostingAccountAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting hosting account {AccountId}", id);

            var account = await _context.HostingAccounts
                .Include(h => h.Domains)
                .Include(h => h.EmailAccounts)
                .Include(h => h.Databases)
                .Include(h => h.FtpAccounts)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (account == null)
            {
                return false;
            }

            // Delete from server if requested
            if (deleteFromServer && account.ServerControlPanelId.HasValue && !string.IsNullOrEmpty(account.ExternalAccountId))
            {
                await _syncService.DeleteAccountFromServerAsync(account.Id);
            }

            _context.HostingAccounts.Remove(account);
            await _context.SaveChangesAsync();

            _log.Information("Hosting account {AccountId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting hosting account {AccountId}", id);
            throw;
        }
    }

    public async Task<SyncResultDto> SyncAccountFromServerAsync(int serverControlPanelId, string externalAccountId)
    {
        return await _syncService.SyncAccountFromServerAsync(serverControlPanelId, externalAccountId);
    }

    public async Task<SyncResultDto> SyncAccountToServerAsync(int hostingAccountId)
    {
        return await _syncService.SyncAccountToServerAsync(hostingAccountId);
    }

    public async Task<SyncResultDto> SyncAllAccountsFromServerAsync(int serverControlPanelId)
    {
        return await _syncService.SyncAllAccountsFromServerAsync(serverControlPanelId);
    }

    public async Task<SyncResultDto> ProvisionAccountOnCPanelAsync(int hostingAccountId, int? domainId = null)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Provisioning hosting account {AccountId} on CPanel using domain from database", hostingAccountId);

            // Load hosting account with related data
            var account = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Include(h => h.Domains)
                .Include(h => h.Customer)
                .Include(h => h.Service)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (account == null)
            {
                result.Message = "Hosting account not found";
                return result;
            }

            if (account.ServerControlPanel == null)
            {
                result.Message = "Server control panel not configured for this account";
                return result;
            }

            if (!string.IsNullOrEmpty(account.ExternalAccountId))
            {
                result.Message = $"Account already provisioned on CPanel with ID: {account.ExternalAccountId}";
                result.Success = false;
                return result;
            }

            // Get the domain to use
            HostingDomain? domain = null;
            if (domainId.HasValue)
            {
                domain = account.Domains.FirstOrDefault(d => d.Id == domainId.Value);
                if (domain == null)
                {
                    result.Message = $"Domain with ID {domainId} not found for this account";
                    return result;
                }
            }
            else
            {
                // Use the main domain, or first domain if no main domain specified
                domain = account.Domains.FirstOrDefault(d => d.DomainType == "Main") 
                        ?? account.Domains.FirstOrDefault();
                
                if (domain == null)
                {
                    result.Message = "No domains found for this hosting account. Please create a domain first.";
                    return result;
                }
            }

            // Check if we have a password
            if (string.IsNullOrEmpty(account.PasswordHash))
            {
                result.Message = "Account password is required to provision on CPanel. Please set a password first.";
                return result;
            }

            _log.Information("Using domain {DomainName} to provision account {AccountId}", domain.DomainName, hostingAccountId);

            // Create HostingAccountRequest for CPanel
            var request = new HostingAccountRequest
            {
                Username = account.Username,
                Password = account.PasswordHash, // Note: This should be plaintext, might need special handling
                Domain = domain.DomainName,
                Email = account.Customer?.Email ?? $"admin@{domain.DomainName}",
                Plan = account.PlanName ?? "default",
                DiskQuotaMB = account.DiskQuotaMB,
                BandwidthLimitMB = account.BandwidthLimitMB,
                MaxEmailAccounts = account.MaxEmailAccounts,
                MaxDatabases = account.MaxDatabases,
                MaxFtpAccounts = account.MaxFtpAccounts,
                MaxSubdomains = account.MaxSubdomains
            };

            // Call sync service to create on server
            var syncResult = await _syncService.CreateAccountOnServerAsync(account.ServerControlPanel, request);

            if (!syncResult.Success)
            {
                result.Message = $"Failed to provision account on CPanel: {syncResult.Message}";
                account.SyncStatus = "Error";
                await _context.SaveChangesAsync();
                return result;
            }

            // Update account with external ID
            account.ExternalAccountId = syncResult.AccountId;
            account.SyncStatus = "Synced";
            account.LastSyncedAt = DateTime.UtcNow;

            // Update domain with sync status
            domain.SyncStatus = "Synced";
            domain.LastSyncedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully provisioned account {AccountId} on CPanel with username {Username}", 
                hostingAccountId, account.Username);

            result.Success = true;
            result.Message = $"Successfully provisioned account on CPanel using domain {domain.DomainName}. Username: {account.Username}";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error provisioning account on CPanel: {ex.Message}";
            _log.Error(ex, "Error provisioning hosting account {AccountId} on CPanel", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncStatusDto> GetSyncStatusAsync(int hostingAccountId)
    {
        var account = await _context.HostingAccounts.FindAsync(hostingAccountId);
        if (account == null)
        {
            throw new InvalidOperationException($"Hosting account {hostingAccountId} not found");
        }

        return new SyncStatusDto
        {
            HostingAccountId = account.Id,
            SyncStatus = account.SyncStatus ?? "NotSynced",
            LastSyncedAt = account.LastSyncedAt,
            ExternalAccountId = account.ExternalAccountId
        };
    }

    public async Task<SyncComparisonDto> CompareDatabaseWithServerAsync(int hostingAccountId)
    {
        return await _syncService.CompareDatabaseWithServerAsync(hostingAccountId);
    }

    public async Task<ResourceUsageDto> GetResourceUsageAsync(int hostingAccountId)
    {
        var account = await _context.HostingAccounts.FindAsync(hostingAccountId);
        if (account == null)
        {
            throw new InvalidOperationException($"Hosting account {hostingAccountId} not found");
        }

        var emailCount = await _context.HostingEmailAccounts.CountAsync(e => e.HostingAccountId == hostingAccountId);
        var databaseCount = await _context.HostingDatabases.CountAsync(d => d.HostingAccountId == hostingAccountId);
        var ftpCount = await _context.HostingFtpAccounts.CountAsync(f => f.HostingAccountId == hostingAccountId);
        var domainCount = await _context.HostingDomains.CountAsync(d => d.HostingAccountId == hostingAccountId);

        return new ResourceUsageDto
        {
            HostingAccountId = account.Id,
            DiskUsageMB = account.DiskUsageMB,
            DiskQuotaMB = account.DiskQuotaMB,
            BandwidthUsageMB = account.BandwidthUsageMB,
            BandwidthLimitMB = account.BandwidthLimitMB,
            EmailAccountCount = emailCount,
            MaxEmailAccounts = account.MaxEmailAccounts,
            DatabaseCount = databaseCount,
            MaxDatabases = account.MaxDatabases,
            FtpAccountCount = ftpCount,
            MaxFtpAccounts = account.MaxFtpAccounts,
            DomainCount = domainCount,
            MaxSubdomains = account.MaxSubdomains
        };
    }

    public async Task<bool> UpdateResourceUsageAsync(int hostingAccountId)
    {
        try
        {
            var account = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (account == null || account.ServerControlPanel == null || string.IsNullOrEmpty(account.ExternalAccountId))
            {
                return false;
            }

            // This would call the hosting panel API to get current usage
            // Implementation depends on HostingPanelLib providing usage data methods
            // For now, we'll leave this as a placeholder for future implementation
            
            _log.Information("Resource usage update requested for hosting account {AccountId}", hostingAccountId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating resource usage for hosting account {AccountId}", hostingAccountId);
            throw;
        }
    }

    #region Private Helper Methods

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private HostingAccountDto MapToDto(HostingAccount account)
    {
        return new HostingAccountDto
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            CustomerName = account.Customer?.Name ?? string.Empty,
            ServiceId = account.ServiceId,
            ServerId = account.ServerId,
            ServerName = account.Server?.Name,
            ServerControlPanelId = account.ServerControlPanelId,
            ControlPanelType = account.ServerControlPanel?.ControlPanelType?.Name,
            Provider = account.Provider,
            Username = account.Username,
            Status = account.Status,
            ExpirationDate = account.ExpirationDate,
            ExternalAccountId = account.ExternalAccountId,
            LastSyncedAt = account.LastSyncedAt,
            SyncStatus = account.SyncStatus,
            PlanName = account.PlanName,
            DiskUsageMB = account.DiskUsageMB,
            DiskQuotaMB = account.DiskQuotaMB,
            BandwidthUsageMB = account.BandwidthUsageMB,
            BandwidthLimitMB = account.BandwidthLimitMB,
            MaxEmailAccounts = account.MaxEmailAccounts,
            MaxDatabases = account.MaxDatabases,
            MaxFtpAccounts = account.MaxFtpAccounts,
            MaxSubdomains = account.MaxSubdomains,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }

    private HostingAccountDto MapToDtoWithDetails(HostingAccount account)
    {
        var dto = MapToDto(account);
        
        dto.Domains = account.Domains?.Select(d => new HostingDomainDto
        {
            Id = d.Id,
            DomainName = d.DomainName,
            DomainType = d.DomainType,
            SslEnabled = d.SslEnabled,
            SslExpirationDate = d.SslExpirationDate,
            SyncStatus = d.SyncStatus,
            CreatedAt = d.CreatedAt
        }).ToList();

        dto.EmailAccountCount = account.EmailAccounts?.Count ?? 0;
        dto.DatabaseCount = account.Databases?.Count ?? 0;
        dto.FtpAccountCount = account.FtpAccounts?.Count ?? 0;

        return dto;
    }

    #endregion
}
