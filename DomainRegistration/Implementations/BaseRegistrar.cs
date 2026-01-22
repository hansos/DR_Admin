using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;

namespace DomainRegistrationLib.Implementations
{
    public abstract class BaseRegistrar : IDomainRegistrar
    {
        protected readonly string _apiUrl;
        protected readonly HttpClient _httpClient;

        protected BaseRegistrar(string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
        }

        public abstract Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName);
        public abstract Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request);
        public abstract Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request);
        public abstract Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request);
        public abstract Task<DnsZone> GetDnsZoneAsync(string domainName);
        public abstract Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone);
        public abstract Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record);
        public abstract Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record);
        public abstract Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId);
        public abstract Task<DomainInfoResult> GetDomainInfoAsync(string domainName);
        public abstract Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers);
        public abstract Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable);
        public abstract Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable);

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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
