using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing DNS zone package records
/// </summary>
public interface IDnsZonePackageRecordService
{
    /// <summary>
    /// Retrieves all DNS zone package records
    /// </summary>
    /// <returns>Collection of DNS zone package record DTOs</returns>
    Task<IEnumerable<DnsZonePackageRecordDto>> GetAllDnsZonePackageRecordsAsync();
    
    /// <summary>
    /// Retrieves DNS zone package records for a specific package
    /// </summary>
    /// <param name="packageId">The DNS zone package ID</param>
    /// <returns>Collection of DNS zone package record DTOs</returns>
    Task<IEnumerable<DnsZonePackageRecordDto>> GetRecordsByPackageIdAsync(int packageId);
    
    /// <summary>
    /// Retrieves a DNS zone package record by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <returns>DNS zone package record DTO if found, otherwise null</returns>
    Task<DnsZonePackageRecordDto?> GetDnsZonePackageRecordByIdAsync(int id);
    
    /// <summary>
    /// Creates a new DNS zone package record
    /// </summary>
    /// <param name="createDto">The DNS zone package record creation data</param>
    /// <returns>The created DNS zone package record DTO</returns>
    Task<DnsZonePackageRecordDto> CreateDnsZonePackageRecordAsync(CreateDnsZonePackageRecordDto createDto);
    
    /// <summary>
    /// Updates an existing DNS zone package record
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <param name="updateDto">The DNS zone package record update data</param>
    /// <returns>The updated DNS zone package record DTO if found, otherwise null</returns>
    Task<DnsZonePackageRecordDto?> UpdateDnsZonePackageRecordAsync(int id, UpdateDnsZonePackageRecordDto updateDto);
    
    /// <summary>
    /// Deletes a DNS zone package record
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteDnsZonePackageRecordAsync(int id);
}
