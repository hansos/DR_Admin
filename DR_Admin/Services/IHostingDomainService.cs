using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting domains
/// </summary>
public interface IHostingDomainService
{
    // Domain Management
    Task<HostingDomainDto?> GetDomainAsync(int id);
    Task<List<HostingDomainDto>> GetDomainsByHostingAccountAsync(int hostingAccountId);
    Task<HostingDomainDto> CreateDomainAsync(HostingDomainCreateDto dto, bool syncToServer = false);
    Task<HostingDomainDto> UpdateDomainAsync(int id, HostingDomainUpdateDto dto, bool syncToServer = false);
    Task<bool> DeleteDomainAsync(int id, bool deleteFromServer = false);
    
    // Synchronization
    Task<SyncResultDto> SyncDomainsFromServerAsync(int hostingAccountId);
    Task<SyncResultDto> SyncDomainToServerAsync(int domainId);
}
