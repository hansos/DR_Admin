using DomainRegistrationLib.Models;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing domain registration operations through registrars
/// </summary>
public interface IDomainManagerService
{
    /// <summary>
    /// Registers a domain with the specified registrar
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="registeredDomainId">The ID of the registered domain in the database</param>
    /// <returns>Domain registration result from the registrar</returns>
    Task<DomainRegistrationResult> RegisterDomainAsync(string registrarCode, int registeredDomainId);

    /// <summary>
    /// Checks if a domain is available for registration based on registered domain ID
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="registeredDomainId">The ID of the registered domain in the database</param>
    /// <returns>Domain availability result from the registrar</returns>
    Task<DomainAvailabilityResult> CheckDomainAvailabilityByIdAsync(string registrarCode, int registeredDomainId);

    /// <summary>
    /// Checks if a domain is available for registration based on domain name
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="domainName">The domain name to check</param>
    /// <returns>Domain availability result from the registrar</returns>
    Task<DomainAvailabilityResult> CheckDomainAvailabilityByNameAsync(string registrarCode, string domainName);

    /// <summary>
    /// Downloads DNS records from the registrar for all domains assigned to that registrar
    /// and merges them into the local DnsRecord table
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <returns>Aggregated sync result with per-domain details</returns>
    Task<DnsBulkSyncResult> SyncDnsRecordsForAllDomainsAsync(string registrarCode);

    /// <summary>
    /// Downloads DNS records from the registrar for a single domain identified by name
    /// and merges them into the local DnsRecord table
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="domainName">The fully-qualified domain name</param>
    /// <returns>Sync result for the domain</returns>
    Task<DnsRecordSyncResult> SyncDnsRecordsByDomainNameAsync(string registrarCode, string domainName);

    /// <summary>
    /// Pushes a single DNS record from the local database to the registrar's DNS server.
    /// Non-deleted records are upserted; soft-deleted records are removed from the server and hard-deleted locally.
    /// Clears IsPendingSync on success.
    /// </summary>
    /// <param name="dnsRecordId">The local database ID of the DNS record to push.</param>
    /// <returns>Result of the push operation.</returns>
    Task<DnsPushRecordResult> PushDnsRecordAsync(int dnsRecordId);

    /// <summary>
    /// Pushes all pending-sync DNS records for a given domain to the registrar's DNS server.
    /// Non-deleted records are upserted; soft-deleted records are removed and hard-deleted locally.
    /// </summary>
    /// <param name="domainId">The local database ID of the registered domain.</param>
    /// <returns>Aggregated push result with per-record details.</returns>
    Task<DnsPushPendingResult> PushPendingSyncRecordsAsync(int domainId);
}
