using ISPAdmin.DTOs;
using HostingPanelLib.Models;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for synchronizing hosting account data between database and hosting panels
/// </summary>
public interface IHostingSyncService
{
    /// <summary>
    /// Syncs a hosting account from the server to the database
    /// </summary>
    Task<SyncResultDto> SyncAccountFromServerAsync(int serverControlPanelId, string externalAccountId);
    
    /// <summary>
    /// Syncs a hosting account from the database to the server
    /// </summary>
    Task<SyncResultDto> SyncAccountToServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Creates a hosting account on the server using provided configuration
    /// </summary>
    Task<HostingAccountResult> CreateAccountOnServerAsync(ISPAdmin.Data.Entities.ServerControlPanel serverControlPanel, HostingAccountRequest request);
    
    /// <summary>
    /// Syncs all accounts from a server to the database
    /// </summary>
    Task<SyncResultDto> SyncAllAccountsFromServerAsync(int serverControlPanelId);
    
    /// <summary>
    /// Deletes an account from the hosting server
    /// </summary>
    Task<bool> DeleteAccountFromServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Compares database state with server state
    /// </summary>
    Task<SyncComparisonDto> CompareDatabaseWithServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Syncs domains for a hosting account from server
    /// </summary>
    Task<SyncResultDto> SyncDomainsFromServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Syncs email accounts for a hosting account from server
    /// </summary>
    Task<SyncResultDto> SyncEmailAccountsFromServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Syncs databases for a hosting account from server
    /// </summary>
    Task<SyncResultDto> SyncDatabasesFromServerAsync(int hostingAccountId);
    
    /// <summary>
    /// Syncs FTP accounts for a hosting account from server
    /// </summary>
    Task<SyncResultDto> SyncFtpAccountsFromServerAsync(int hostingAccountId);
}
