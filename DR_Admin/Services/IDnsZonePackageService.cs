using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing DNS zone packages
/// </summary>
public interface IDnsZonePackageService
{
    Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesAsync();
    Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesWithRecordsAsync();
    Task<IEnumerable<DnsZonePackageDto>> GetActiveDnsZonePackagesAsync();
    Task<DnsZonePackageDto?> GetDefaultDnsZonePackageAsync();
    Task<DnsZonePackageDto?> GetDnsZonePackageByIdAsync(int id);
    Task<DnsZonePackageDto?> GetDnsZonePackageWithRecordsByIdAsync(int id);
    Task<DnsZonePackageDto?> GetDnsZonePackageWithAssignmentsAsync(int id);
    Task<DnsZonePackageDto> CreateDnsZonePackageAsync(CreateDnsZonePackageDto createDto);
    Task<DnsZonePackageDto?> UpdateDnsZonePackageAsync(int id, UpdateDnsZonePackageDto updateDto);
    Task<bool> DeleteDnsZonePackageAsync(int id);
    Task<bool> ApplyPackageToDomainAsync(int packageId, int domainId);

    // Control panel assignments
    Task<IEnumerable<DnsZonePackageDto>> GetPackagesByControlPanelAsync(int controlPanelId);
    Task<bool> AssignControlPanelAsync(int packageId, int controlPanelId);
    Task<bool> RemoveControlPanelAsync(int packageId, int controlPanelId);

    // Server assignments
    Task<IEnumerable<DnsZonePackageDto>> GetPackagesByServerAsync(int serverId);
    Task<bool> AssignServerAsync(int packageId, int serverId);
    Task<bool> RemoveServerAsync(int packageId, int serverId);
}
