using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IDnsRecordService
{
    /// <summary>Returns all non-deleted DNS records in the system.</summary>
    Task<IEnumerable<DnsRecordDto>> GetAllDnsRecordsAsync();

    /// <summary>Returns a paginated list of non-deleted DNS records.</summary>
    Task<PagedResult<DnsRecordDto>> GetAllDnsRecordsPagedAsync(PaginationParameters parameters);

    /// <summary>Returns a single non-deleted DNS record by its identifier.</summary>
    Task<DnsRecordDto?> GetDnsRecordByIdAsync(int id);

    /// <summary>Returns all non-deleted DNS records for the specified domain.</summary>
    Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByDomainIdAsync(int domainId);

    /// <summary>Returns all non-deleted DNS records of the given type (e.g., "A", "MX").</summary>
    Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByTypeAsync(string type);

    /// <summary>Returns all records for the specified domain that are flagged as pending synchronisation.</summary>
    Task<IEnumerable<DnsRecordDto>> GetPendingSyncRecordsAsync(int domainId);

    /// <summary>Returns the count of DNS records flagged as pending synchronisation.</summary>
    Task<int> GetPendingSyncCountAsync();

    /// <summary>Returns all soft-deleted records for the specified domain.</summary>
    Task<IEnumerable<DnsRecordDto>> GetDeletedDnsRecordsAsync(int domainId);

    /// <summary>Creates a new DNS record flagged as pending synchronisation.</summary>
    Task<DnsRecordDto> CreateDnsRecordAsync(CreateDnsRecordDto createDto);

    /// <summary>
    /// Updates an existing DNS record and marks it as pending synchronisation.
    /// Returns null when the record does not exist.
    /// </summary>
    Task<DnsRecordDto?> UpdateDnsRecordAsync(int id, UpdateDnsRecordDto updateDto);

    /// <summary>
    /// Soft-deletes a DNS record by setting IsDeleted = true and IsPendingSync = true.
    /// The record is retained until hard-deleted after confirmation from the DNS server.
    /// Returns false when the record does not exist.
    /// </summary>
    Task<bool> SoftDeleteDnsRecordAsync(int id);

    /// <summary>
    /// Permanently removes a DNS record from the database.
    /// Use this only after the deletion has been confirmed on the DNS server.
    /// Returns false when the record does not exist.
    /// </summary>
    Task<bool> HardDeleteDnsRecordAsync(int id);

    /// <summary>
    /// Restores a soft-deleted DNS record and marks it as pending synchronisation.
    /// Returns null when the record does not exist or is not soft-deleted.
    /// </summary>
    Task<DnsRecordDto?> RestoreDnsRecordAsync(int id);

    /// <summary>
    /// Clears the IsPendingSync flag after the record has been successfully pushed to the DNS server.
    /// Returns null when the record does not exist.
    /// </summary>
    Task<DnsRecordDto?> MarkAsSyncedAsync(int id);
}
