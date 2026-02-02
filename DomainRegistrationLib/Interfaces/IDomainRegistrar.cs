using DomainRegistrationLib.Models;

namespace DomainRegistrationLib.Interfaces
{
    public interface IDomainRegistrar
    {
        /// <summary>
        /// Checks if a domain is available for registration
        /// </summary>
        Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName);

        /// <summary>
        /// Registers a new domain
        /// </summary>
        Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request);

        /// <summary>
        /// Renews an existing domain
        /// </summary>
        Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request);

        /// <summary>
        /// Transfers a domain from another registrar
        /// </summary>
        Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request);

        /// <summary>
        /// Gets DNS zone information for a domain
        /// </summary>
        Task<DnsZone> GetDnsZoneAsync(string domainName);

        /// <summary>
        /// Updates DNS zone records for a domain
        /// </summary>
        Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone);

        /// <summary>
        /// Adds a DNS record to a domain
        /// </summary>
        Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record);

        /// <summary>
        /// Updates a DNS record for a domain
        /// </summary>
        Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record);

        /// <summary>
        /// Deletes a DNS record from a domain
        /// </summary>
        Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId);

        /// <summary>
        /// Gets domain information
        /// </summary>
        Task<DomainInfoResult> GetDomainInfoAsync(string domainName);

        /// <summary>
        /// Updates domain nameservers
        /// </summary>
        Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers);

        /// <summary>
        /// Enables/disables privacy protection
        /// </summary>
        Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable);

        /// <summary>
        /// Enables/disables auto-renewal
        /// </summary>
        Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable);

        /// <summary>
        /// Gets all supported top-level domains for this registrar
        /// </summary>
        Task<List<TldInfo>> GetSupportedTldsAsync();

        Task<List<TldInfo>> GetSupportedTldsAsync(string tld);

        Task<List<TldInfo>> GetSupportedTldsAsync(List<string> requestTlds);


    }
}
