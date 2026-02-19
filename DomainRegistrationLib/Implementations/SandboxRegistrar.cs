using DomainRegistrationLib.Models;
using Serilog;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Sandbox registrar implementation that simulates all domain registration operations.
    /// Used when sandbox mode is enabled to avoid real registrar API calls.
    /// Domain availability checks use the free VeriSign RDAP API.
    /// </summary>
    public class SandboxRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private static readonly HttpClient _rdapClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private const string VerisignRdapBaseUrl = "https://rdap.verisign.com";

        /// <summary>
        /// TLDs supported by the VeriSign RDAP service for availability lookups
        /// </summary>
        private static readonly HashSet<string> VerisignSupportedTlds = new(StringComparer.OrdinalIgnoreCase)
        {
            "com", "net", "cc", "tv", "name"
        };

        public SandboxRegistrar()
            : base("https://sandbox.local")
        {
            _logger = Log.ForContext<SandboxRegistrar>();
            _logger.Information("[SANDBOX] SandboxRegistrar initialized — all operations will be simulated");
        }

        /// <summary>
        /// Checks domain availability using the VeriSign RDAP API (free, no auth required).
        /// For TLDs not supported by VeriSign RDAP, returns a simulated available result.
        /// RDAP returns HTTP 200 if the domain is registered, HTTP 404 if it is available.
        /// </summary>
        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            _logger.Information("[SANDBOX] Checking domain availability for {DomainName} via VeriSign RDAP", domainName);

            try
            {
                var tld = ExtractTld(domainName);

                if (!VerisignSupportedTlds.Contains(tld))
                {
                    _logger.Information("[SANDBOX] TLD '{Tld}' is not supported by VeriSign RDAP — returning simulated available result", tld);
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = domainName,
                        IsAvailable = true,
                        IsTldSupported = false,
                        Message = $"[SANDBOX] TLD '.{tld}' is not supported by VeriSign RDAP. Simulated as available."
                    };
                }

                var rdapUrl = $"{VerisignRdapBaseUrl}/{tld}/v1/domain/{domainName}";
                var response = await _rdapClient.GetAsync(rdapUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.Information("[SANDBOX] Domain {DomainName} is AVAILABLE (VeriSign RDAP 404)", domainName);
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = domainName,
                        IsAvailable = true,
                        IsTldSupported = true,
                        Message = "[SANDBOX] Domain is available (verified via VeriSign RDAP)"
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("[SANDBOX] Domain {DomainName} is REGISTERED (VeriSign RDAP 200)", domainName);
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = domainName,
                        IsAvailable = false,
                        IsTldSupported = true,
                        Message = "[SANDBOX] Domain is already registered (verified via VeriSign RDAP)"
                    };
                }

                _logger.Warning("[SANDBOX] Unexpected RDAP response {StatusCode} for {DomainName}", response.StatusCode, domainName);
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = true,
                    IsTldSupported = true,
                    Message = $"[SANDBOX] Unexpected RDAP status {(int)response.StatusCode}. Simulated as available."
                };
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "[SANDBOX] VeriSign RDAP lookup failed for {DomainName} — returning simulated result", domainName);
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = true,
                    IsTldSupported = false,
                    Message = $"[SANDBOX] RDAP lookup failed ({ex.Message}). Simulated as available."
                };
            }
        }

        /// <summary>
        /// Simulates domain registration. Returns dummy success data without contacting any registrar.
        /// </summary>
        public override Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request)
        {
            _logger.Information("[SANDBOX] Simulating domain registration for {DomainName} ({Years} year(s))",
                request.DomainName, request.Years);

            var result = new DomainRegistrationResult
            {
                Success = true,
                Message = "[SANDBOX] Domain registration simulated successfully",
                DomainName = request.DomainName,
                OrderId = $"SBX-ORD-{Guid.NewGuid():N}",
                TransactionId = $"SBX-TXN-{Guid.NewGuid():N}",
                RegistrationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(request.Years),
                TotalCost = 0.00m
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Simulates domain renewal. Returns dummy success data.
        /// </summary>
        public override Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            _logger.Information("[SANDBOX] Simulating domain renewal for {DomainName} ({Years} year(s))",
                request.DomainName, request.Years);

            var result = new DomainRenewalResult
            {
                Success = true,
                Message = "[SANDBOX] Domain renewal simulated successfully",
                DomainName = request.DomainName,
                OrderId = $"SBX-REN-{Guid.NewGuid():N}",
                TransactionId = $"SBX-TXN-{Guid.NewGuid():N}",
                NewExpirationDate = DateTime.UtcNow.AddYears(request.Years),
                TotalCost = 0.00m
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Simulates domain transfer. Returns dummy success data.
        /// </summary>
        public override Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            _logger.Information("[SANDBOX] Simulating domain transfer for {DomainName}", request.DomainName);

            var result = new DomainTransferResult
            {
                Success = true,
                Message = "[SANDBOX] Domain transfer simulated successfully",
                DomainName = request.DomainName,
                OrderId = $"SBX-TRF-{Guid.NewGuid():N}",
                TransactionId = $"SBX-TXN-{Guid.NewGuid():N}",
                TransferStatus = "SIMULATED_COMPLETE",
                TotalCost = 0.00m
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Returns a simulated DNS zone with sample records.
        /// </summary>
        public override Task<DnsZone> GetDnsZoneAsync(string domainName)
        {
            _logger.Information("[SANDBOX] Returning simulated DNS zone for {DomainName}", domainName);

            var zone = new DnsZone
            {
                DomainName = domainName,
                Nameservers = new List<string>
                {
                    "ns1.sandbox.local",
                    "ns2.sandbox.local"
                },
                Records = new List<DnsRecordModel>
                {
                    new() { Id = 1, Type = "A", Name = "@", Value = "192.0.2.1", TTL = 3600 },
                    new() { Id = 2, Type = "CNAME", Name = "www", Value = domainName, TTL = 3600 },
                    new() { Id = 3, Type = "MX", Name = "@", Value = "mail.sandbox.local", TTL = 3600, Priority = 10 },
                    new() { Id = 4, Type = "TXT", Name = "@", Value = "v=spf1 include:sandbox.local ~all", TTL = 3600 }
                }
            };

            return Task.FromResult(zone);
        }

        /// <summary>
        /// Simulates updating a DNS zone. Returns success.
        /// </summary>
        public override Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            _logger.Information("[SANDBOX] Simulating DNS zone update for {DomainName} ({RecordCount} records)",
                domainName, dnsZone.Records.Count);

            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "[SANDBOX] DNS zone update simulated successfully"
            });
        }

        /// <summary>
        /// Simulates adding a DNS record. Returns success.
        /// </summary>
        public override Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            _logger.Information("[SANDBOX] Simulating add DNS record {Type} '{Name}' for {DomainName}",
                record.Type, record.Name, domainName);

            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"[SANDBOX] DNS {record.Type} record added (simulated)"
            });
        }

        /// <summary>
        /// Simulates updating a DNS record. Returns success.
        /// </summary>
        public override Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            _logger.Information("[SANDBOX] Simulating update DNS record {RecordId} for {DomainName}",
                record.Id, domainName);

            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"[SANDBOX] DNS record {record.Id} updated (simulated)"
            });
        }

        /// <summary>
        /// Simulates deleting a DNS record. Returns success.
        /// </summary>
        public override Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            _logger.Information("[SANDBOX] Simulating delete DNS record {RecordId} for {DomainName}",
                recordId, domainName);

            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"[SANDBOX] DNS record {recordId} deleted (simulated)"
            });
        }

        /// <summary>
        /// Returns simulated domain information.
        /// </summary>
        public override Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            _logger.Information("[SANDBOX] Returning simulated domain info for {DomainName}", domainName);

            var result = new DomainInfoResult
            {
                Success = true,
                Message = "[SANDBOX] Domain info retrieved (simulated)",
                DomainName = domainName,
                Status = "ACTIVE",
                RegistrationDate = DateTime.UtcNow.AddYears(-1),
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                UpdatedDate = DateTime.UtcNow,
                AutoRenew = true,
                PrivacyProtection = true,
                Locked = true,
                Nameservers = new List<string> { "ns1.sandbox.local", "ns2.sandbox.local" },
                RegistrantContact = new ContactInformation
                {
                    ContactType = "PERSON",
                    FirstName = "Sandbox",
                    LastName = "User",
                    Organization = "Sandbox Corp",
                    Email = "sandbox@sandbox.local",
                    Phone = "+1.5555550100",
                    Address1 = "123 Sandbox Street",
                    City = "Sandbox City",
                    State = "SB",
                    PostalCode = "00000",
                    Country = "US"
                }
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Simulates updating nameservers. Returns success.
        /// </summary>
        public override Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers)
        {
            _logger.Information("[SANDBOX] Simulating nameserver update for {DomainName}: {Nameservers}",
                domainName, string.Join(", ", nameservers));

            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "[SANDBOX] Nameservers updated (simulated)"
            });
        }

        /// <summary>
        /// Simulates toggling privacy protection. Returns success.
        /// </summary>
        public override Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            _logger.Information("[SANDBOX] Simulating privacy protection {Action} for {DomainName}",
                enable ? "enable" : "disable", domainName);

            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"[SANDBOX] Privacy protection {(enable ? "enabled" : "disabled")} (simulated)"
            });
        }

        /// <summary>
        /// Simulates toggling auto-renewal. Returns success.
        /// </summary>
        public override Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            _logger.Information("[SANDBOX] Simulating auto-renew {Action} for {DomainName}",
                enable ? "enable" : "disable", domainName);

            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"[SANDBOX] Auto-renewal {(enable ? "enabled" : "disabled")} (simulated)"
            });
        }

        /// <summary>
        /// Returns a simulated list of popular TLDs with dummy pricing.
        /// </summary>
        public override Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            _logger.Information("[SANDBOX] Returning simulated supported TLD list");

            var tlds = new List<TldInfo>
            {
                new() { Name = "com", RegistrationPrice = 9.99m, RenewalPrice = 12.99m, TransferPrice = 9.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "net", RegistrationPrice = 11.99m, RenewalPrice = 14.99m, TransferPrice = 11.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "org", RegistrationPrice = 10.99m, RenewalPrice = 13.99m, TransferPrice = 10.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "io", RegistrationPrice = 39.99m, RenewalPrice = 49.99m, TransferPrice = 39.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 5, SupportsPrivacy = true, IsGeneric = false, IsCountryCode = true, IsAvailable = true },
                new() { Name = "dev", RegistrationPrice = 14.99m, RenewalPrice = 16.99m, TransferPrice = 14.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "co", RegistrationPrice = 24.99m, RenewalPrice = 29.99m, TransferPrice = 24.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 5, SupportsPrivacy = true, IsGeneric = false, IsCountryCode = true, IsAvailable = true },
                new() { Name = "info", RegistrationPrice = 3.99m, RenewalPrice = 18.99m, TransferPrice = 12.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "biz", RegistrationPrice = 12.99m, RenewalPrice = 16.99m, TransferPrice = 12.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "xyz", RegistrationPrice = 1.99m, RenewalPrice = 12.99m, TransferPrice = 9.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true },
                new() { Name = "app", RegistrationPrice = 14.99m, RenewalPrice = 18.99m, TransferPrice = 14.99m, Currency = "USD", MinRegistrationYears = 1, MaxRegistrationYears = 10, SupportsPrivacy = true, IsGeneric = true, IsAvailable = true }
            };

            return Task.FromResult(tlds);
        }

        /// <summary>
        /// Returns a simulated list of registered domains.
        /// </summary>
        public override Task<RegisteredDomainsResult> GetRegisteredDomainsAsync()
        {
            _logger.Information("[SANDBOX] Returning simulated registered domains list");

            var result = new RegisteredDomainsResult
            {
                Success = true,
                Message = "[SANDBOX] Registered domains retrieved (simulated)",
                TotalCount = 2,
                Domains =
                [
                    new RegisteredDomainInfo
                    {
                        DomainName = "sandbox-example.com",
                        Status = "ACTIVE",
                        RegistrationDate = DateTime.UtcNow.AddYears(-1),
                        ExpirationDate = DateTime.UtcNow.AddYears(1),
                        AutoRenew = true,
                        Locked = true,
                        PrivacyProtection = true,
                        Nameservers = ["ns1.sandbox.local", "ns2.sandbox.local"]
                    },
                    new RegisteredDomainInfo
                    {
                        DomainName = "sandbox-test.net",
                        Status = "ACTIVE",
                        RegistrationDate = DateTime.UtcNow.AddMonths(-6),
                        ExpirationDate = DateTime.UtcNow.AddMonths(18),
                        AutoRenew = false,
                        Locked = false,
                        PrivacyProtection = false,
                        Nameservers = ["ns1.sandbox.local", "ns2.sandbox.local"]
                    }
                ]
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Extracts the TLD from a fully qualified domain name.
        /// </summary>
        private static string ExtractTld(string domainName)
        {
            var parts = domainName.Split('.');
            return parts.Length >= 2 ? parts[^1] : domainName;
        }
    }
}
