using DomainRegistrationLib.Models;
using Serilog;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Simulator registrar implementation used for testing without placing real registrar orders.
    /// Availability checks for .com, .net and .org are performed against official RDAP endpoints.
    /// </summary>
    public class SimulatorRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;

        private static readonly HttpClient _rdapClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly Dictionary<string, string> OfficialRdapEndpoints = new(StringComparer.OrdinalIgnoreCase)
        {
            ["com"] = "https://rdap.verisign.com/com/v1/domain/",
            ["net"] = "https://rdap.verisign.com/net/v1/domain/",
            ["org"] = "https://rdap.publicinterestregistry.org/rdap/domain/"
        };

        public SimulatorRegistrar()
            : base("https://simulator.local")
        {
            _logger = Log.ForContext<SimulatorRegistrar>();
            _logger.Information("[SIMULATOR] SimulatorRegistrar initialized");
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            _logger.Information("[SIMULATOR] Checking availability for {DomainName}", domainName);

            var tld = ExtractTld(domainName);
            if (!OfficialRdapEndpoints.TryGetValue(tld, out var rdapBaseUrl))
            {
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = false,
                    IsTldSupported = false,
                    Message = $"TLD '.{tld}' is unsupported by SimulatorRegistrar. Only .com, .net and .org are supported."
                };
            }

            try
            {
                var response = await _rdapClient.GetAsync($"{rdapBaseUrl}{domainName.ToLowerInvariant()}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = domainName,
                        IsAvailable = true,
                        IsTldSupported = true,
                        Message = "Domain is available"
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = domainName,
                        IsAvailable = false,
                        IsTldSupported = true,
                        Message = "Domain is registered"
                    };
                }

                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = domainName,
                    IsAvailable = false,
                    IsTldSupported = true,
                    Message = $"Availability check failed with status {(int)response.StatusCode}",
                    Errors = new List<string> { $"Unexpected RDAP status code: {(int)response.StatusCode}" }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error checking availability for {DomainName}", domainName);
                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = domainName,
                    IsAvailable = false,
                    IsTldSupported = true,
                    Message = $"Availability check failed: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public override Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request)
        {
            return Task.FromResult(new DomainRegistrationResult
            {
                Success = true,
                DomainName = request.DomainName,
                OrderId = $"SIM-REG-{Guid.NewGuid():N}",
                TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                RegistrationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(request.Years),
                TotalCost = 0m,
                Message = "Domain registration simulated successfully"
            });
        }

        public override Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            return Task.FromResult(new DomainRenewalResult
            {
                Success = true,
                DomainName = request.DomainName,
                OrderId = $"SIM-REN-{Guid.NewGuid():N}",
                TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                NewExpirationDate = DateTime.UtcNow.AddYears(request.Years),
                TotalCost = 0m,
                Message = "Domain renewal simulated successfully"
            });
        }

        public override Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            return Task.FromResult(new DomainTransferResult
            {
                Success = true,
                DomainName = request.DomainName,
                OrderId = $"SIM-TRF-{Guid.NewGuid():N}",
                TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                TransferStatus = "SIMULATED_COMPLETE",
                TotalCost = 0m,
                Message = "Domain transfer simulated successfully"
            });
        }

        public override Task<DnsZone> GetDnsZoneAsync(string domainName)
        {
            return Task.FromResult(new DnsZone
            {
                DomainName = domainName,
                Nameservers = new List<string> { "ns1.simulator.local", "ns2.simulator.local" },
                Records = new List<DnsRecordModel>
                {
                    new() { Id = 1, Type = "A", Name = "@", Value = "192.0.2.10", TTL = 3600 },
                    new() { Id = 2, Type = "CNAME", Name = "www", Value = domainName, TTL = 3600 }
                }
            });
        }

        public override Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "DNS zone update simulated successfully"
            });
        }

        public override Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "DNS record add simulated successfully"
            });
        }

        public override Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "DNS record update simulated successfully"
            });
        }

        public override Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            return Task.FromResult(new DnsUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "DNS record delete simulated successfully"
            });
        }

        public override Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            return Task.FromResult(new DomainInfoResult
            {
                Success = true,
                DomainName = domainName,
                Status = "ACTIVE",
                RegistrationDate = DateTime.UtcNow.AddYears(-1),
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                AutoRenew = true,
                PrivacyProtection = true,
                Locked = true,
                Nameservers = new List<string> { "ns1.simulator.local", "ns2.simulator.local" },
                Message = "Domain info retrieved (simulated)"
            });
        }

        public override Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers)
        {
            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = "Nameserver update simulated successfully"
            });
        }

        public override Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"Privacy protection {(enable ? "enabled" : "disabled")} (simulated)"
            });
        }

        public override Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            return Task.FromResult(new DomainUpdateResult
            {
                Success = true,
                DomainName = domainName,
                Message = $"Auto-renew {(enable ? "enabled" : "disabled")} (simulated)"
            });
        }

        public override Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            return Task.FromResult(new List<TldInfo>
            {
                new() { Name = "com", Currency = "USD", IsAvailable = true },
                new() { Name = "net", Currency = "USD", IsAvailable = true },
                new() { Name = "org", Currency = "USD", IsAvailable = true }
            });
        }

        public override Task<RegisteredDomainsResult> GetRegisteredDomainsAsync()
        {
            return Task.FromResult(new RegisteredDomainsResult
            {
                Success = true,
                Message = "Registered domains retrieved (simulated)",
                Domains = new List<RegisteredDomainInfo>(),
                TotalCount = 0
            });
        }

        private static string ExtractTld(string domainName)
        {
            var parts = domainName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length >= 2 ? parts[^1] : string.Empty;
        }
    }
}
