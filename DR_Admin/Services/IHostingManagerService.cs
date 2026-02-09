using ISPAdmin.DTOs;
using HostingPanelLib.Models;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting accounts and synchronization with hosting panels
/// </summary>
public interface IHostingManagerService
{
    // Hosting Account Management
    Task<HostingAccountDto?> GetHostingAccountAsync(int id);
    Task<HostingAccountDto?> GetHostingAccountWithDetailsAsync(int id);
    Task<List<HostingAccountDto>> GetAllHostingAccountsAsync();
    Task<List<HostingAccountDto>> GetHostingAccountsByCustomerAsync(int customerId);
    Task<List<HostingAccountDto>> GetHostingAccountsByServerAsync(int serverId);
    Task<HostingAccountDto> CreateHostingAccountAsync(HostingAccountCreateDto dto, bool syncToServer = false);
    Task<HostingAccountDto> UpdateHostingAccountAsync(int id, HostingAccountUpdateDto dto, bool syncToServer = false);
    Task<bool> DeleteHostingAccountAsync(int id, bool deleteFromServer = false);
    
    // Synchronization Operations
    Task<SyncResultDto> SyncAccountFromServerAsync(int serverControlPanelId, string externalAccountId);
    Task<SyncResultDto> SyncAccountToServerAsync(int hostingAccountId);
    Task<SyncResultDto> SyncAllAccountsFromServerAsync(int serverControlPanelId);
    Task<SyncResultDto> ProvisionAccountOnCPanelAsync(int hostingAccountId, int? domainId = null);
    Task<SyncStatusDto> GetSyncStatusAsync(int hostingAccountId);
    Task<SyncComparisonDto> CompareDatabaseWithServerAsync(int hostingAccountId);
    
    // Resource Management
    Task<ResourceUsageDto> GetResourceUsageAsync(int hostingAccountId);
    Task<bool> UpdateResourceUsageAsync(int hostingAccountId);
}
