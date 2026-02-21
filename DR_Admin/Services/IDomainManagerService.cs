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
}
