using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting FTP accounts
/// </summary>
public interface IHostingFtpService
{
    // FTP Account Management
    Task<HostingFtpAccountDto?> GetFtpAccountAsync(int id);
    Task<List<HostingFtpAccountDto>> GetFtpAccountsByHostingAccountAsync(int hostingAccountId);
    Task<HostingFtpAccountDto> CreateFtpAccountAsync(HostingFtpAccountCreateDto dto, bool syncToServer = false);
    Task<HostingFtpAccountDto> UpdateFtpAccountAsync(int id, HostingFtpAccountUpdateDto dto, bool syncToServer = false);
    Task<bool> DeleteFtpAccountAsync(int id, bool deleteFromServer = false);
    Task<bool> ChangeFtpPasswordAsync(int id, string newPassword, bool syncToServer = false);
    
    // Synchronization
    Task<SyncResultDto> SyncFtpAccountsFromServerAsync(int hostingAccountId);
    Task<SyncResultDto> SyncFtpAccountToServerAsync(int ftpAccountId);
}
