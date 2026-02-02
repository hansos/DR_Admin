using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Base implementation for domain registrar clients providing common functionality
    /// and helper methods for all registrar implementations
    /// </summary>
    public abstract class BaseRegistrar : IDomainRegistrar
    {
        protected readonly string _apiUrl;
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the BaseRegistrar class
        /// </summary>
        /// <param name="apiUrl">The base API URL for the registrar service</param>
        protected BaseRegistrar(string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
        }

        /// <summary>
        /// Checks if a domain is available for registration
        /// </summary>
        /// <param name="domainName">The fully qualified domain name to check (e.g., example.com)</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the availability information</returns>
        public abstract Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName);

        /// <summary>
        /// Registers a new domain with the registrar
        /// </summary>
        /// <param name="request">The domain registration request containing all required information</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the registration result</returns>
        public abstract Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request);

        /// <summary>
        /// Renews an existing domain registration
        /// </summary>
        /// <param name="request">The domain renewal request containing the domain name and renewal period</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the renewal result</returns>
        public abstract Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request);

        /// <summary>
        /// Initiates a domain transfer from another registrar
        /// </summary>
        /// <param name="request">The domain transfer request containing the domain name and authorization code</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the transfer result</returns>
        public abstract Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request);

        /// <summary>
        /// Gets the DNS zone information for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the DNS zone with all records</returns>
        public abstract Task<DnsZone> GetDnsZoneAsync(string domainName);

        /// <summary>
        /// Updates the DNS zone records for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="dnsZone">The DNS zone containing the updated records</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone);

        /// <summary>
        /// Adds a new DNS record to a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="record">The DNS record to add</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record);

        /// <summary>
        /// Updates an existing DNS record for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="record">The DNS record with updated values</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record);

        /// <summary>
        /// Deletes a DNS record from a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="recordId">The unique identifier of the DNS record to delete</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId);

        /// <summary>
        /// Gets detailed information about a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the domain information</returns>
        public abstract Task<DomainInfoResult> GetDomainInfoAsync(string domainName);

        /// <summary>
        /// Updates the nameservers for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="nameservers">List of nameserver hostnames to set for the domain</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers);

        /// <summary>
        /// Enables or disables WHOIS privacy protection for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="enable">True to enable privacy protection, false to disable</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable);

        /// <summary>
        /// Enables or disables automatic renewal for a domain
        /// </summary>
        /// <param name="domainName">The fully qualified domain name</param>
        /// <param name="enable">True to enable auto-renewal, false to disable</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update result</returns>
        public abstract Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable);

        /// <summary>
        /// Gets all top-level domains supported by this registrar
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of supported TLDs with pricing</returns>
        public abstract Task<List<TldInfo>> GetSupportedTldsAsync();

        /// <summary>
        /// Gets all domains registered with this registrar.
        /// Default implementation returns not supported. Registrars should override this.
        /// Note: Contact information may not be included by all registrars in the list response.
        /// Some registrars require individual API calls per domain to retrieve contact details.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of registered domains</returns>
        public virtual Task<RegisteredDomainsResult> GetRegisteredDomainsAsync()
        {
            return Task.FromResult(new RegisteredDomainsResult
            {
                Success = false,
                Message = "GetRegisteredDomainsAsync is not implemented for this registrar",
                ErrorCode = "NOT_IMPLEMENTED"
            });
        }

        /// <summary>
        /// Gets supported TLDs filtered by a list of specific TLD extensions.
        /// Default implementation: calls the parameterless GetSupportedTldsAsync and filters by the provided list.
        /// Implementations may override for optimized behavior.
        /// </summary>
        /// <param name="requestTlds">List of TLD extensions to filter (e.g., ["com", "net", "org"])</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a filtered list of TLD information</returns>
        public virtual async Task<List<TldInfo>> GetSupportedTldsAsync(List<string> requestTlds)
        {
            if (requestTlds == null || requestTlds.Count == 0)
                return await GetSupportedTldsAsync();

            var all = await GetSupportedTldsAsync();
            var set = new HashSet<string>(requestTlds.Select(r => r.TrimStart('.')), StringComparer.OrdinalIgnoreCase);
            return all.Where(t => set.Contains(t.Name.TrimStart('.'))).ToList();
        }

        /// <summary>
        /// Gets supported TLDs filtered by a single TLD extension.
        /// Default implementation: delegates to the list overload.
        /// </summary>
        /// <param name="tld">The TLD extension to filter (e.g., "com")</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains TLD information for the specified extension</returns>
        public virtual Task<List<TldInfo>> GetSupportedTldsAsync(string tld)
        {
            if (string.IsNullOrWhiteSpace(tld))
                return GetSupportedTldsAsync();

            return GetSupportedTldsAsync(new List<string> { tld });
        }

        /// <summary>
        /// Creates a standardized error result for domain registration operations
        /// </summary>
        /// <param name="message">The error message describing what went wrong</param>
        /// <param name="errorCode">Optional error code for categorizing the error</param>
        /// <returns>A DomainRegistrationResult indicating failure with the provided message and code</returns>
        protected virtual DomainRegistrationResult CreateErrorResult(string message, string? errorCode = null)
        {
            return new DomainRegistrationResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        /// <summary>
        /// Creates a standardized error result for domain renewal operations
        /// </summary>
        /// <param name="message">The error message describing what went wrong</param>
        /// <param name="errorCode">Optional error code for categorizing the error</param>
        /// <returns>A DomainRenewalResult indicating failure with the provided message and code</returns>
        protected virtual DomainRenewalResult CreateRenewalErrorResult(string message, string? errorCode = null)
        {
            return new DomainRenewalResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        /// <summary>
        /// Creates a standardized error result for DNS operations
        /// </summary>
        /// <param name="message">The error message describing what went wrong</param>
        /// <param name="errorCode">Optional error code for categorizing the error</param>
        /// <returns>A DnsUpdateResult indicating failure with the provided message and code</returns>
        protected virtual DnsUpdateResult CreateDnsErrorResult(string message, string? errorCode = null)
        {
            return new DnsUpdateResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        /// <summary>
        /// Creates a standardized error result for domain update operations
        /// </summary>
        /// <param name="message">The error message describing what went wrong</param>
        /// <param name="errorCode">Optional error code for categorizing the error</param>
        /// <returns>A DomainUpdateResult indicating failure with the provided message and code</returns>
        protected virtual DomainUpdateResult CreateUpdateErrorResult(string message, string? errorCode = null)
        {
            return new DomainUpdateResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        /// <summary>
        /// Releases the unmanaged resources used by the BaseRegistrar and optionally releases the managed resources
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
