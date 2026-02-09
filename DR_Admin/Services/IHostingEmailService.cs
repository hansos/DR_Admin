using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting email accounts
/// </summary>
public interface IHostingEmailService
{
    // Email Account Management
    Task<HostingEmailAccountDto?> GetEmailAccountAsync(int id);
    Task<List<HostingEmailAccountDto>> GetEmailAccountsByHostingAccountAsync(int hostingAccountId);
    Task<List<HostingEmailAccountDto>> GetEmailAccountsByDomainAsync(int hostingAccountId, string domain);
    Task<HostingEmailAccountDto> CreateEmailAccountAsync(HostingEmailAccountCreateDto dto, bool syncToServer = false);
    Task<HostingEmailAccountDto> UpdateEmailAccountAsync(int id, HostingEmailAccountUpdateDto dto, bool syncToServer = false);
    Task<bool> DeleteEmailAccountAsync(int id, bool deleteFromServer = false);
    Task<bool> ChangeEmailPasswordAsync(int id, string newPassword, bool syncToServer = false);
    
    // Synchronization
    Task<SyncResultDto> SyncEmailAccountsFromServerAsync(int hostingAccountId);
    Task<SyncResultDto> SyncEmailAccountToServerAsync(int emailAccountId, string? password = null);
}
