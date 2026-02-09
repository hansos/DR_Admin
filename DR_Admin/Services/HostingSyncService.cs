using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HostingPanelLib.Factories;
using HostingPanelLib.Infrastructure.Settings;
using HostingPanelLib.Interfaces;
using HostingPanelLib.Models;

namespace ISPAdmin.Services;

/// <summary>
/// Service for synchronizing hosting account data between database and hosting panels
/// </summary>
public class HostingSyncService : IHostingSyncService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HostingSyncService> _logger;
    private readonly IHostingDomainService _domainService;
    private readonly IHostingEmailService _emailService;
    private readonly IHostingDatabaseService _databaseService;
    private readonly IHostingFtpService _ftpService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingSyncService>();

    public HostingSyncService(
        ApplicationDbContext context,
        ILogger<HostingSyncService> logger,
        IHostingDomainService domainService,
        IHostingEmailService emailService,
        IHostingDatabaseService databaseService,
        IHostingFtpService ftpService)
    {
        _context = context;
        _logger = logger;
        _domainService = domainService;
        _emailService = emailService;
        _databaseService = databaseService;
        _ftpService = ftpService;
    }

    public async Task<SyncResultDto> SyncAccountFromServerAsync(int serverControlPanelId, string externalAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing account {ExternalAccountId} from server control panel {PanelId}", 
                externalAccountId, serverControlPanelId);

            var controlPanel = await GetControlPanelWithSettingsAsync(serverControlPanelId);
            if (controlPanel == null)
            {
                result.Message = $"Server control panel {serverControlPanelId} not found";
                _log.Warning(result.Message);
                return result;
            }

            var panel = CreateHostingPanel(controlPanel);
            var accountInfo = await panel.GetWebHostingAccountInfoAsync(externalAccountId);

            if (!accountInfo.Success)
            {
                result.Message = $"Failed to get account info from server: {accountInfo.Message}";
                _log.Warning(result.Message);
                return result;
            }

            // Find existing account or create new
            var account = await _context.HostingAccounts
                .FirstOrDefaultAsync(h => h.ExternalAccountId == externalAccountId && 
                                        h.ServerControlPanelId == serverControlPanelId);

            if (account == null)
            {
                _log.Information("Creating new hosting account for {ExternalAccountId}", externalAccountId);
                // Account doesn't exist in database, need to create it
                // This requires additional info like CustomerId, ServiceId - may need to be passed in or configured
                result.Message = "Account not found in database. Use import endpoint to create new account.";
                return result;
            }

            // Update existing account
            account.Username = accountInfo.Username ?? account.Username;
            account.Status = accountInfo.Status ?? account.Status;
            account.DiskUsageMB = accountInfo.DiskUsageMB;
            account.DiskQuotaMB = accountInfo.DiskQuotaMB;
            account.BandwidthUsageMB = accountInfo.BandwidthUsageMB;
            account.BandwidthLimitMB = accountInfo.BandwidthLimitMB;
            account.LastSyncedAt = DateTime.UtcNow;
            account.SyncStatus = "Synced";

            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully synced account {externalAccountId}";
            result.RecordsSynced = 1;

            _log.Information("Successfully synced account {AccountId}", account.Id);
            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing account: {ex.Message}";
            _log.Error(ex, "Error syncing account {ExternalAccountId} from server {PanelId}", 
                externalAccountId, serverControlPanelId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncAccountToServerAsync(int hostingAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing account {AccountId} to server", hostingAccountId);

            var account = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Include(h => h.Server)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (account == null)
            {
                result.Message = $"Hosting account {hostingAccountId} not found";
                return result;
            }

            if (account.ServerControlPanel == null)
            {
                result.Message = "No server control panel configured for this account";
                return result;
            }

            var panel = CreateHostingPanel(account.ServerControlPanel);

            // If account doesn't have external ID, create it on server
            if (string.IsNullOrEmpty(account.ExternalAccountId))
            {
                var createResult = await CreateAccountOnServerAsync(panel, account);
                if (!createResult.Success)
                {
                    result.Message = $"Failed to create account on server: {createResult.Message}";
                    account.SyncStatus = "Error";
                    await _context.SaveChangesAsync();
                    return result;
                }

                account.ExternalAccountId = createResult.AccountId;
            }
            else
            {
                // Update existing account on server
                var updateResult = await UpdateAccountOnServerAsync(panel, account);
                if (!updateResult.Success)
                {
                    result.Message = $"Failed to update account on server: {updateResult.Message}";
                    account.SyncStatus = "Error";
                    await _context.SaveChangesAsync();
                    return result;
                }
            }

            account.LastSyncedAt = DateTime.UtcNow;
            account.SyncStatus = "Synced";
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully synced account to server";
            result.RecordsSynced = 1;

            _log.Information("Successfully synced account {AccountId} to server", hostingAccountId);
            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing account to server: {ex.Message}";
            _log.Error(ex, "Error syncing account {AccountId} to server", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncAllAccountsFromServerAsync(int serverControlPanelId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing all accounts from server control panel {PanelId}", serverControlPanelId);

            var controlPanel = await GetControlPanelWithSettingsAsync(serverControlPanelId);
            if (controlPanel == null)
            {
                result.Message = $"Server control panel {serverControlPanelId} not found";
                return result;
            }

            var panel = CreateHostingPanel(controlPanel);
            var accounts = await panel.ListWebHostingAccountsAsync();

            int syncedCount = 0;
            var errors = new List<string>();

            foreach (var accountInfo in accounts)
            {
                if (string.IsNullOrEmpty(accountInfo.Username))
                    continue;

                try
                {
                    var syncResult = await SyncAccountFromServerAsync(serverControlPanelId, accountInfo.Username);
                    if (syncResult.Success)
                    {
                        syncedCount++;
                    }
                    else
                    {
                        errors.Add($"{accountInfo.Username}: {syncResult.Message}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{accountInfo.Username}: {ex.Message}");
                    _log.Warning(ex, "Error syncing account {Username}", accountInfo.Username);
                }
            }

            result.Success = errors.Count < accounts.Count;
            result.RecordsSynced = syncedCount;
            result.Message = $"Synced {syncedCount} of {accounts.Count} accounts";
            if (errors.Any())
            {
                result.Message += $". Errors: {string.Join("; ", errors.Take(5))}";
            }

            _log.Information("Completed syncing accounts from server {PanelId}. Synced: {Synced}, Total: {Total}", 
                serverControlPanelId, syncedCount, accounts.Count);

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing accounts from server: {ex.Message}";
            _log.Error(ex, "Error syncing all accounts from server {PanelId}", serverControlPanelId);
            return result;
        }
    }

    public async Task<HostingAccountResult> CreateAccountOnServerAsync(
        ServerControlPanel serverControlPanel, 
        HostingAccountRequest request)
    {
        try
        {
            _log.Information("Creating account on server for username {Username}, domain {Domain}", 
                request.Username, request.Domain);

            var panel = CreateHostingPanel(serverControlPanel);
            var result = await panel.CreateWebHostingAccountAsync(request);

            if (result.Success)
            {
                _log.Information("Successfully created account {Username} on server", request.Username);
            }
            else
            {
                _log.Warning("Failed to create account {Username} on server: {Message}", 
                    request.Username, result.Message);
            }

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating account on server for username {Username}", request.Username);
            return new HostingAccountResult
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                ErrorCode = "EXCEPTION"
            };
        }
    }

    public async Task<bool> DeleteAccountFromServerAsync(int hostingAccountId)
    {
        try
        {
            _log.Information("Deleting account {AccountId} from server", hostingAccountId);

            var account = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (account == null || account.ServerControlPanel == null || string.IsNullOrEmpty(account.ExternalAccountId))
            {
                return false;
            }

            var panel = CreateHostingPanel(account.ServerControlPanel);
            var deleteResult = await panel.DeleteWebHostingAccountAsync(account.ExternalAccountId);

            if (deleteResult.Success)
            {
                _log.Information("Successfully deleted account {AccountId} from server", hostingAccountId);
                return true;
            }
            else
            {
                _log.Warning("Failed to delete account {AccountId} from server: {Message}", 
                    hostingAccountId, deleteResult.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting account {AccountId} from server", hostingAccountId);
            return false;
        }
    }

    public async Task<SyncComparisonDto> CompareDatabaseWithServerAsync(int hostingAccountId)
    {
        var comparison = new SyncComparisonDto
        {
            HostingAccountId = hostingAccountId,
            Differences = new List<string>()
        };

        try
        {
            var account = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (account == null || account.ServerControlPanel == null || string.IsNullOrEmpty(account.ExternalAccountId))
            {
                comparison.Differences.Add("Account not properly configured for sync");
                return comparison;
            }

            var panel = CreateHostingPanel(account.ServerControlPanel);
            var serverAccount = await panel.GetWebHostingAccountInfoAsync(account.ExternalAccountId);

            if (!serverAccount.Success)
            {
                comparison.Differences.Add($"Failed to get server account info: {serverAccount.Message}");
                return comparison;
            }

            // Compare fields
            if (account.DiskQuotaMB != serverAccount.DiskQuotaMB)
                comparison.Differences.Add($"DiskQuota: DB={account.DiskQuotaMB}, Server={serverAccount.DiskQuotaMB}");

            if (account.BandwidthLimitMB != serverAccount.BandwidthLimitMB)
                comparison.Differences.Add($"BandwidthLimit: DB={account.BandwidthLimitMB}, Server={serverAccount.BandwidthLimitMB}");

            if (account.Status != serverAccount.Status)
                comparison.Differences.Add($"Status: DB={account.Status}, Server={serverAccount.Status}");

            comparison.InSync = comparison.Differences.Count == 0;
            comparison.LastChecked = DateTime.UtcNow;

            return comparison;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error comparing account {AccountId} with server", hostingAccountId);
            comparison.Differences.Add($"Error during comparison: {ex.Message}");
            return comparison;
        }
    }

    public async Task<SyncResultDto> SyncDomainsFromServerAsync(int hostingAccountId)
    {
        return await _domainService.SyncDomainsFromServerAsync(hostingAccountId);
    }

    public async Task<SyncResultDto> SyncEmailAccountsFromServerAsync(int hostingAccountId)
    {
        return await _emailService.SyncEmailAccountsFromServerAsync(hostingAccountId);
    }

    public async Task<SyncResultDto> SyncDatabasesFromServerAsync(int hostingAccountId)
    {
        return await _databaseService.SyncDatabasesFromServerAsync(hostingAccountId);
    }

    public async Task<SyncResultDto> SyncFtpAccountsFromServerAsync(int hostingAccountId)
    {
        return await _ftpService.SyncFtpAccountsFromServerAsync(hostingAccountId);
    }

    #region Private Helper Methods

    private async Task<ServerControlPanel?> GetControlPanelWithSettingsAsync(int id)
    {
        return await _context.ServerControlPanels
            .Include(scp => scp.ControlPanelType)
            .Include(scp => scp.Server)
            .FirstOrDefaultAsync(scp => scp.Id == id);
    }

    private IHostingPanel CreateHostingPanel(ServerControlPanel controlPanel)
    {
        var settings = new HostingPanelSettings
        {
            Provider = controlPanel.ControlPanelType.Name.ToLower()
        };

        // Configure provider-specific settings based on control panel type
        switch (controlPanel.ControlPanelType.Name.ToLower())
        {
            case "cpanel":
                settings.Cpanel = new CpanelSettings
                {
                    ApiUrl = controlPanel.ApiUrl,
                    ApiToken = controlPanel.ApiToken ?? string.Empty,
                    Username = controlPanel.Username ?? "root",
                    Port = controlPanel.Port,
                    UseHttps = controlPanel.UseHttps
                };
                break;

            case "plesk":
                settings.Plesk = new PleskSettings
                {
                    ApiUrl = controlPanel.ApiUrl,
                    ApiKey = controlPanel.ApiKey ?? string.Empty,
                    Username = controlPanel.Username ?? string.Empty,
                    Password = controlPanel.PasswordHash ?? string.Empty, // TODO: Decrypt
                    Port = controlPanel.Port,
                    UseHttps = controlPanel.UseHttps
                };
                break;

            // Add other panel types as needed

            default:
                throw new NotSupportedException($"Hosting panel type '{controlPanel.ControlPanelType.Name}' is not supported");
        }

        var factory = new HostingPanelFactory(settings);
        return factory.CreatePanel();
    }

    private async Task<HostingAccountResult> CreateAccountOnServerAsync(IHostingPanel panel, HostingAccount account)
    {
        var request = new HostingAccountRequest
        {
            Domain = account.Domains.FirstOrDefault()?.DomainName ?? $"{account.Username}.temp.local",
            Username = account.Username,
            Password = "TempPassword123!", // TODO: Use actual password or generate one
            Email = string.Empty, // TODO: Get from customer
            Plan = account.PlanName,
            DiskQuotaMB = account.DiskQuotaMB,
            BandwidthLimitMB = account.BandwidthLimitMB,
            MaxEmailAccounts = account.MaxEmailAccounts,
            MaxDatabases = account.MaxDatabases,
            MaxFtpAccounts = account.MaxFtpAccounts,
            MaxSubdomains = account.MaxSubdomains
        };

        return await panel.CreateWebHostingAccountAsync(request);
    }

    private async Task<AccountUpdateResult> UpdateAccountOnServerAsync(IHostingPanel panel, HostingAccount account)
    {
        var request = new HostingAccountRequest
        {
            Domain = account.Domains.FirstOrDefault()?.DomainName,
            DiskQuotaMB = account.DiskQuotaMB,
            BandwidthLimitMB = account.BandwidthLimitMB,
            MaxEmailAccounts = account.MaxEmailAccounts,
            MaxDatabases = account.MaxDatabases,
            MaxFtpAccounts = account.MaxFtpAccounts,
            MaxSubdomains = account.MaxSubdomains
        };

        return await panel.UpdateWebHostingAccountAsync(account.ExternalAccountId!, request);
    }

    #endregion
}
