using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing DNS zone packages
/// </summary>
public interface IDnsZonePackageService
{
    /// <summary>
    /// Retrieves all DNS zone packages
    /// </summary>
    /// <returns>Collection of DNS zone package DTOs</returns>
    Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesAsync();
    
    /// <summary>
    /// Retrieves all DNS zone packages with their records
    /// </summary>
    /// <returns>Collection of DNS zone package DTOs including records</returns>
    Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesWithRecordsAsync();
    
    /// <summary>
    /// Retrieves active DNS zone packages only
    /// </summary>
    /// <returns>Collection of active DNS zone package DTOs</returns>
    Task<IEnumerable<DnsZonePackageDto>> GetActiveDnsZonePackagesAsync();
    
    /// <summary>
    /// Retrieves the default DNS zone package
    /// </summary>
    /// <returns>The default DNS zone package DTO if found, otherwise null</returns>
    Task<DnsZonePackageDto?> GetDefaultDnsZonePackageAsync();
    
    /// <summary>
    /// Retrieves a DNS zone package by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>DNS zone package DTO if found, otherwise null</returns>
    Task<DnsZonePackageDto?> GetDnsZonePackageByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a DNS zone package with its records by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>DNS zone package DTO with records if found, otherwise null</returns>
    Task<DnsZonePackageDto?> GetDnsZonePackageWithRecordsByIdAsync(int id);
    
    /// <summary>
    /// Creates a new DNS zone package
    /// </summary>
    /// <param name="createDto">The DNS zone package creation data</param>
    /// <returns>The created DNS zone package DTO</returns>
    Task<DnsZonePackageDto> CreateDnsZonePackageAsync(CreateDnsZonePackageDto createDto);
    
    /// <summary>
    /// Updates an existing DNS zone package
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <param name="updateDto">The DNS zone package update data</param>
    /// <returns>The updated DNS zone package DTO if found, otherwise null</returns>
    Task<DnsZonePackageDto?> UpdateDnsZonePackageAsync(int id, UpdateDnsZonePackageDto updateDto);
    
    /// <summary>
    /// Deletes a DNS zone package
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    Task<bool> DeleteDnsZonePackageAsync(int id);
    
    /// <summary>
    /// Applies a DNS zone package to a domain by creating DNS records
    /// </summary>
    /// <param name="packageId">The DNS zone package ID</param>
    /// <param name="domainId">The domain ID to apply records to</param>
    /// <returns>True if applied successfully, otherwise false</returns>
    Task<bool> ApplyPackageToDomainAsync(int packageId, int domainId);
}
