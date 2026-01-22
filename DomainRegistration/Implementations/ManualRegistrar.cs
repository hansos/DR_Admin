using DomainRegistrationLib.Models;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Manual Registrar - For tracking domains registered at other registrars
    /// This implementation doesn't make actual API calls but allows domain tracking
    /// </summary>
    public class ManualRegistrar : BaseRegistrar
    {
        private readonly bool _allowOperations;

        public ManualRegistrar(bool allowOperations = false)
            : base("https://manual.local")
        {
            _allowOperations = allowOperations;
        }

        public override Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            return Task.FromResult(new DomainAvailabilityResult
            {
                Success = true,
                DomainName = domainName,
                IsAvailable = false,
                Message = "Manual registrar - please check availability manually at the external registrar"
            });
        }

        public override Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request)
        {
            if (!_allowOperations)
            {
                return Task.FromResult(CreateErrorResult(
                    "Manual registrar does not support automated registration. Please register this domain manually at your chosen registrar and then add it to the system for tracking.",
                    "MANUAL_OPERATION_REQUIRED"));
            }

            // Allow tracking of manually registered domains
            return Task.FromResult(new DomainRegistrationResult
            {
                Success = true,
                DomainName = request.DomainName,
                Message = "Domain added to tracking system (registered manually elsewhere)",
                RegistrationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(request.Years)
            });
        }

        public override Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            return Task.FromResult(new DomainRenewalResult
            {
                Success = false,
                DomainName = request.DomainName,
                Message = "Manual registrar - please renew this domain manually at the external registrar and update the expiration date in the system",
                ErrorCode = "MANUAL_OPERATION_REQUIRED"
            });
        }

        public override Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            return Task.FromResult(new DomainTransferResult
            {
                Success = false,
                Message = "Manual registrar - please handle domain transfer manually",
                ErrorCode = "MANUAL_OPERATION_REQUIRED"
            });
        }

        public override Task<DnsZone> GetDnsZoneAsync(string domainName)
        {
            return Task.FromResult(new DnsZone
            {
                DomainName = domainName,
                Records = new List<DnsRecordModel>()
            });
        }

        public override Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            return Task.FromResult(new DnsUpdateResult
            {
                Success = false,
                DomainName = domainName,
                Message = "Manual registrar - please update DNS records manually at the external registrar",
                ErrorCode = "MANUAL_OPERATION_REQUIRED"
            });
        }

        public override Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            return Task.FromResult(CreateDnsErrorResult(
                "Manual registrar - please add DNS records manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }

        public override Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            return Task.FromResult(CreateDnsErrorResult(
                "Manual registrar - please update DNS records manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }

        public override Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            return Task.FromResult(CreateDnsErrorResult(
                "Manual registrar - please delete DNS records manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }

        public override Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            return Task.FromResult(new DomainInfoResult
            {
                Success = false,
                DomainName = domainName,
                Message = "Manual registrar - domain information must be maintained manually in the system",
                ErrorCode = "MANUAL_OPERATION_REQUIRED"
            });
        }

        public override Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers)
        {
            return Task.FromResult(CreateUpdateErrorResult(
                "Manual registrar - please update nameservers manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }

        public override Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            return Task.FromResult(CreateUpdateErrorResult(
                "Manual registrar - please update privacy protection manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }

        public override Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            return Task.FromResult(CreateUpdateErrorResult(
                "Manual registrar - please update auto-renew settings manually at the external registrar",
                "MANUAL_OPERATION_REQUIRED"));
        }
    }
}
