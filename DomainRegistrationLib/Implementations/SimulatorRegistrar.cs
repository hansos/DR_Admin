using DomainRegistrationLib.Models;
using Serilog;
using System.Net;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Simulator registrar implementation used for testing without placing real registrar orders.
    /// Availability checks for .com, .net and .org are performed against official RDAP endpoints.
    /// </summary>
    public class SimulatorRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private static readonly SemaphoreSlim _storageLock = new(1, 1);
        private static readonly JsonSerializerOptions StorageJsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static readonly string StorageDirectoryPath = Path.Combine(AppContext.BaseDirectory, "simulator-data");
        private static readonly string StorageFilePath = Path.Combine(StorageDirectoryPath, "");

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
            EnsureStorageFile();
            _logger.Information("[SIMULATOR] SimulatorRegistrar initialized");
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            _logger.Information("[SIMULATOR] Checking availability for {DomainName}", normalizedDomain);

            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = domainName,
                    IsAvailable = false,
                    IsTldSupported = false,
                    Message = "Domain name is required",
                    Errors = new List<string> { "Domain name is required" }
                };
            }

            var isRegisteredInSimulator = await IsRegisteredInSimulatorAsync(normalizedDomain);
            if (isRegisteredInSimulator)
            {
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    IsAvailable = false,
                    IsTldSupported = true,
                    Message = "Domain is registered in simulator"
                };
            }

            var tld = ExtractTld(normalizedDomain);
            if (!OfficialRdapEndpoints.TryGetValue(tld, out var rdapBaseUrl))
            {
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    IsAvailable = false,
                    IsTldSupported = false,
                    Message = $"TLD '.{tld}' is unsupported by SimulatorRegistrar. Only .com, .net and .org are supported."
                };
            }

            try
            {
                var response = await _rdapClient.GetAsync($"{rdapBaseUrl}{normalizedDomain}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new DomainAvailabilityResult
                    {
                        Success = true,
                        DomainName = normalizedDomain,
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
                        DomainName = normalizedDomain,
                        IsAvailable = false,
                        IsTldSupported = true,
                        Message = "Domain is registered"
                    };
                }

                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    IsAvailable = false,
                    IsTldSupported = true,
                    Message = $"Availability check failed with status {(int)response.StatusCode}",
                    Errors = new List<string> { $"Unexpected RDAP status code: {(int)response.StatusCode}" }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error checking availability for {DomainName}", normalizedDomain);
                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    IsAvailable = false,
                    IsTldSupported = true,
                    Message = $"Availability check failed: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public override async Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request)
        {
            var normalizedDomain = NormalizeDomain(request.DomainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            if (request.Years <= 0)
            {
                return CreateErrorResult("Registration years must be greater than 0", "INVALID_PERIOD");
            }

            var availability = await CheckAvailabilityAsync(normalizedDomain);
            if (!availability.Success)
            {
                return CreateErrorResult(availability.Message, availability.ErrorCode ?? "AVAILABILITY_CHECK_FAILED");
            }

            if (!availability.IsAvailable)
            {
                return CreateErrorResult("Domain is not available for registration", "DOMAIN_UNAVAILABLE");
            }

            var now = DateTime.UtcNow;

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                if (store.Domains.Any(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase)))
                {
                    return CreateErrorResult("Domain is already registered in simulator", "DOMAIN_ALREADY_REGISTERED");
                }

                var nameservers = request.Nameservers?.Where(ns => !string.IsNullOrWhiteSpace(ns)).Select(ns => ns.Trim()).ToList();
                if (nameservers == null || nameservers.Count == 0)
                {
                    nameservers = GetDefaultNameservers();
                }

                var dnsRecords = new List<DnsRecordModel>
                {
                    new() { Id = 1, Type = "A", Name = "@", Value = "192.0.2.10", TTL = 3600 },
                    new() { Id = 2, Type = "CNAME", Name = "www", Value = normalizedDomain, TTL = 3600 }
                };

                store.Domains.Add(new SimulatorDomainEntry
                {
                    DomainName = normalizedDomain,
                    RegistrationDate = now,
                    ExpirationDate = now.AddYears(request.Years),
                    Status = "ACTIVE",
                    AutoRenew = request.AutoRenew,
                    PrivacyProtection = request.PrivacyProtection,
                    Locked = true,
                    Nameservers = nameservers,
                    RegistrantContact = request.RegistrantContact,
                    AdminContact = request.AdminContact,
                    TechContact = request.TechContact,
                    BillingContact = request.BillingContact,
                    DnsRecords = dnsRecords,
                    NextDnsRecordId = 3,
                    LastTransferStatus = null
                });

                await SaveStoreInternalAsync(store);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    OrderId = $"SIM-REG-{Guid.NewGuid():N}",
                    TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                    RegistrationDate = now,
                    ExpirationDate = now.AddYears(request.Years),
                    TotalCost = 0m,
                    Message = "Domain registration simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error registering domain {DomainName}", normalizedDomain);
                return CreateErrorResult($"Domain registration failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            var normalizedDomain = NormalizeDomain(request.DomainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateRenewalErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            if (request.Years <= 0)
            {
                return CreateRenewalErrorResult("Renewal years must be greater than 0", "INVALID_PERIOD");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateRenewalErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                var renewalBase = domain.ExpirationDate > DateTime.UtcNow ? domain.ExpirationDate : DateTime.UtcNow;
                domain.ExpirationDate = renewalBase.AddYears(request.Years);

                await SaveStoreInternalAsync(store);

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    OrderId = $"SIM-REN-{Guid.NewGuid():N}",
                    TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                    NewExpirationDate = domain.ExpirationDate,
                    TotalCost = 0m,
                    Message = "Domain renewal simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error renewing domain {DomainName}", normalizedDomain);
                return CreateRenewalErrorResult($"Domain renewal failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            var normalizedDomain = NormalizeDomain(request.DomainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return new DomainTransferResult
                {
                    Success = false,
                    DomainName = request.DomainName,
                    Message = "Domain name is required",
                    ErrorCode = "INVALID_DOMAIN",
                    Errors = new List<string> { "Domain name is required" }
                };
            }

            var tld = ExtractTld(normalizedDomain);
            if (!OfficialRdapEndpoints.ContainsKey(tld))
            {
                return new DomainTransferResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    Message = $"TLD '.{tld}' is unsupported by SimulatorRegistrar. Only .com, .net and .org are supported.",
                    ErrorCode = "UNSUPPORTED_TLD",
                    Errors = new List<string> { $"Unsupported TLD: .{tld}" }
                };
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                if (store.Domains.Any(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase)))
                {
                    return new DomainTransferResult
                    {
                        Success = false,
                        DomainName = normalizedDomain,
                        Message = "Domain is already managed by simulator",
                        ErrorCode = "DOMAIN_ALREADY_MANAGED",
                        Errors = new List<string> { "Domain is already managed by simulator" }
                    };
                }
            }
            finally
            {
                _storageLock.Release();
            }

            var availability = await CheckAvailabilityAsync(normalizedDomain);
            if (!availability.Success)
            {
                return new DomainTransferResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    Message = availability.Message,
                    ErrorCode = availability.ErrorCode ?? "AVAILABILITY_CHECK_FAILED",
                    Errors = availability.Errors
                };
            }

            if (availability.IsAvailable)
            {
                return new DomainTransferResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    Message = "Domain appears unregistered and cannot be transferred",
                    ErrorCode = "DOMAIN_NOT_REGISTERED",
                    Errors = new List<string> { "Domain appears unregistered and cannot be transferred" }
                };
            }

            var now = DateTime.UtcNow;
            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();

                var nameservers = GetDefaultNameservers();
                var dnsRecords = new List<DnsRecordModel>
                {
                    new() { Id = 1, Type = "A", Name = "@", Value = "192.0.2.10", TTL = 3600 },
                    new() { Id = 2, Type = "CNAME", Name = "www", Value = normalizedDomain, TTL = 3600 }
                };

                store.Domains.Add(new SimulatorDomainEntry
                {
                    DomainName = normalizedDomain,
                    RegistrationDate = now,
                    ExpirationDate = now.AddYears(1),
                    Status = "ACTIVE",
                    AutoRenew = request.AutoRenew,
                    PrivacyProtection = request.PrivacyProtection,
                    Locked = true,
                    Nameservers = nameservers,
                    RegistrantContact = request.RegistrantContact,
                    AdminContact = request.AdminContact,
                    TechContact = request.TechContact,
                    BillingContact = request.BillingContact,
                    DnsRecords = dnsRecords,
                    NextDnsRecordId = 3,
                    LastTransferStatus = "SIMULATED_COMPLETE"
                });

                await SaveStoreInternalAsync(store);

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    OrderId = $"SIM-TRF-{Guid.NewGuid():N}",
                    TransactionId = $"SIM-TXN-{Guid.NewGuid():N}",
                    TransferStatus = "SIMULATED_COMPLETE",
                    TotalCost = 0m,
                    Message = "Domain transfer simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error transferring domain {DomainName}", normalizedDomain);
                return new DomainTransferResult
                {
                    Success = false,
                    DomainName = normalizedDomain,
                    Message = $"Domain transfer failed: {ex.Message}",
                    ErrorCode = "SIMULATOR_STORAGE_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DnsZone> GetDnsZoneAsync(string domainName)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return new DnsZone
                {
                    DomainName = domainName,
                    Nameservers = GetDefaultNameservers(),
                    Records = new List<DnsRecordModel>()
                };
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return new DnsZone
                    {
                        DomainName = normalizedDomain,
                        Nameservers = GetDefaultNameservers(),
                        Records = new List<DnsRecordModel>()
                    };
                }

                return new DnsZone
                {
                    DomainName = normalizedDomain,
                    Nameservers = domain.Nameservers?.ToList() ?? GetDefaultNameservers(),
                    Records = domain.DnsRecords.Select(CloneDnsRecord).ToList()
                };
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateDnsErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateDnsErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                var records = dnsZone.Records?.Select(CloneDnsRecord).ToList() ?? new List<DnsRecordModel>();
                var nextId = 1;
                foreach (var item in records)
                {
                    if (item.Id is null or <= 0)
                    {
                        item.Id = nextId++;
                    }
                    else
                    {
                        nextId = Math.Max(nextId, item.Id.Value + 1);
                    }
                }

                domain.DnsRecords = records;
                domain.NextDnsRecordId = nextId;
                if (dnsZone.Nameservers != null && dnsZone.Nameservers.Count > 0)
                {
                    domain.Nameservers = dnsZone.Nameservers.Where(ns => !string.IsNullOrWhiteSpace(ns)).Select(ns => ns.Trim()).ToList();
                }

                await SaveStoreInternalAsync(store);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = "DNS zone update simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error updating DNS zone for {DomainName}", normalizedDomain);
                return CreateDnsErrorResult($"DNS zone update failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateDnsErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateDnsErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                var recordToAdd = CloneDnsRecord(record);
                recordToAdd.Id = domain.NextDnsRecordId++;
                domain.DnsRecords.Add(recordToAdd);

                await SaveStoreInternalAsync(store);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = "DNS record add simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error adding DNS record for {DomainName}", normalizedDomain);
                return CreateDnsErrorResult($"DNS record add failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateDnsErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            if (record.Id is null or <= 0)
            {
                return CreateDnsErrorResult("DNS record id is required", "INVALID_RECORD_ID");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateDnsErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                var existingIndex = domain.DnsRecords.FindIndex(r => r.Id == record.Id);
                if (existingIndex < 0)
                {
                    return CreateDnsErrorResult("DNS record was not found", "DNS_RECORD_NOT_FOUND");
                }

                domain.DnsRecords[existingIndex] = CloneDnsRecord(record);

                await SaveStoreInternalAsync(store);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = "DNS record update simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error updating DNS record for {DomainName}", normalizedDomain);
                return CreateDnsErrorResult($"DNS record update failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateDnsErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateDnsErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                var removed = domain.DnsRecords.RemoveAll(r => r.Id == recordId) > 0;
                if (!removed)
                {
                    return CreateDnsErrorResult("DNS record was not found", "DNS_RECORD_NOT_FOUND");
                }

                await SaveStoreInternalAsync(store);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = "DNS record delete simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error deleting DNS record for {DomainName}", normalizedDomain);
                return CreateDnsErrorResult($"DNS record delete failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return new DomainInfoResult
                {
                    Success = false,
                    DomainName = domainName,
                    Message = "Domain name is required",
                    ErrorCode = "INVALID_DOMAIN",
                    Errors = new List<string> { "Domain name is required" }
                };
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return new DomainInfoResult
                    {
                        Success = false,
                        DomainName = normalizedDomain,
                        Message = "Domain is not managed by simulator",
                        ErrorCode = "DOMAIN_NOT_FOUND",
                        Errors = new List<string> { "Domain is not managed by simulator" }
                    };
                }

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Status = domain.Status,
                    RegistrationDate = domain.RegistrationDate,
                    ExpirationDate = domain.ExpirationDate,
                    UpdatedDate = DateTime.UtcNow,
                    AutoRenew = domain.AutoRenew,
                    PrivacyProtection = domain.PrivacyProtection,
                    Locked = domain.Locked,
                    Nameservers = domain.Nameservers?.ToList() ?? GetDefaultNameservers(),
                    RegistrantContact = domain.RegistrantContact,
                    AdminContact = domain.AdminContact,
                    TechContact = domain.TechContact,
                    BillingContact = domain.BillingContact,
                    Message = "Domain info retrieved (simulated)"
                };
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateUpdateErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateUpdateErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                domain.Nameservers = nameservers?.Where(ns => !string.IsNullOrWhiteSpace(ns)).Select(ns => ns.Trim()).ToList() ?? new List<string>();
                if (domain.Nameservers.Count == 0)
                {
                    domain.Nameservers = GetDefaultNameservers();
                }

                await SaveStoreInternalAsync(store);

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = "Nameserver update simulated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error updating nameservers for {DomainName}", normalizedDomain);
                return CreateUpdateErrorResult($"Nameserver update failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateUpdateErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateUpdateErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                domain.PrivacyProtection = enable;
                await SaveStoreInternalAsync(store);

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = $"Privacy protection {(enable ? "enabled" : "disabled")} (simulated)"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error setting privacy for {DomainName}", normalizedDomain);
                return CreateUpdateErrorResult($"Privacy update failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
        }

        public override async Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            var normalizedDomain = NormalizeDomain(domainName);
            if (string.IsNullOrWhiteSpace(normalizedDomain))
            {
                return CreateUpdateErrorResult("Domain name is required", "INVALID_DOMAIN");
            }

            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domain = store.Domains.FirstOrDefault(d => string.Equals(d.DomainName, normalizedDomain, StringComparison.OrdinalIgnoreCase));
                if (domain == null)
                {
                    return CreateUpdateErrorResult("Domain is not managed by simulator", "DOMAIN_NOT_FOUND");
                }

                domain.AutoRenew = enable;
                await SaveStoreInternalAsync(store);

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = normalizedDomain,
                    Message = $"Auto-renew {(enable ? "enabled" : "disabled")} (simulated)"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error setting auto-renew for {DomainName}", normalizedDomain);
                return CreateUpdateErrorResult($"Auto-renew update failed: {ex.Message}", "SIMULATOR_STORAGE_ERROR");
            }
            finally
            {
                _storageLock.Release();
            }
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

        public override async Task<RegisteredDomainsResult> GetRegisteredDomainsAsync()
        {
            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                var domains = store.Domains.Select(d => new RegisteredDomainInfo
                {
                    DomainName = d.DomainName,
                    Status = d.Status,
                    RegistrationDate = d.RegistrationDate,
                    ExpirationDate = d.ExpirationDate,
                    AutoRenew = d.AutoRenew,
                    Locked = d.Locked,
                    PrivacyProtection = d.PrivacyProtection,
                    Nameservers = d.Nameservers?.ToList() ?? new List<string>(),
                    Contacts = BuildContacts(d)
                }).ToList();

                return new RegisteredDomainsResult
                {
                    Success = true,
                    Message = "Registered domains retrieved (simulated)",
                    Domains = domains,
                    TotalCount = domains.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[SIMULATOR] Error reading registered domains");
                return new RegisteredDomainsResult
                {
                    Success = false,
                    Message = $"Failed to read registered domains: {ex.Message}",
                    ErrorCode = "SIMULATOR_STORAGE_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
            finally
            {
                _storageLock.Release();
            }
        }

        private static async Task<bool> IsRegisteredInSimulatorAsync(string domainName)
        {
            await _storageLock.WaitAsync();
            try
            {
                var store = await LoadStoreInternalAsync();
                return store.Domains.Any(d => string.Equals(d.DomainName, domainName, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _storageLock.Release();
            }
        }

        private static string NormalizeDomain(string domainName)
        {
            return domainName.Trim().ToLowerInvariant();
        }

        private static DnsRecordModel CloneDnsRecord(DnsRecordModel source)
        {
            return new DnsRecordModel
            {
                Id = source.Id,
                Type = source.Type,
                Name = source.Name,
                Value = source.Value,
                TTL = source.TTL,
                Priority = source.Priority,
                Weight = source.Weight,
                Port = source.Port
            };
        }

        private static List<string> GetDefaultNameservers()
        {
            return new List<string> { "ns1.simulator.local", "ns2.simulator.local" };
        }

        private static List<DomainContactInfo> BuildContacts(SimulatorDomainEntry domain)
        {
            var contacts = new List<DomainContactInfo>();

            AddContactIfPresent(contacts, "registrant", domain.RegistrantContact);
            AddContactIfPresent(contacts, "admin", domain.AdminContact);
            AddContactIfPresent(contacts, "tech", domain.TechContact);
            AddContactIfPresent(contacts, "billing", domain.BillingContact);

            return contacts;
        }

        private static void AddContactIfPresent(List<DomainContactInfo> contacts, string type, ContactInformation? source)
        {
            if (source == null)
            {
                return;
            }

            contacts.Add(new DomainContactInfo
            {
                ContactType = type,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Organization = source.Organization,
                Email = source.Email,
                Phone = source.Phone,
                Address1 = source.Address1,
                Address2 = source.Address2,
                City = source.City,
                State = source.State,
                PostalCode = source.PostalCode,
                CountryCode = source.Country
            });
        }

        private static async Task<SimulatorStore> LoadStoreInternalAsync()
        {
            EnsureStorageFile();

            await using var stream = File.OpenRead(StorageFilePath);
            var store = await JsonSerializer.DeserializeAsync<SimulatorStore>(stream, StorageJsonOptions);
            return store ?? new SimulatorStore();
        }

        private static async Task SaveStoreInternalAsync(SimulatorStore store)
        {
            Directory.CreateDirectory(StorageDirectoryPath);

            await using var stream = File.Create(StorageFilePath);
            await JsonSerializer.SerializeAsync(stream, store, StorageJsonOptions);
        }

        private static void EnsureStorageFile()
        {
            Directory.CreateDirectory(StorageDirectoryPath);
            if (File.Exists(StorageFilePath))
            {
                return;
            }

            var initial = JsonSerializer.Serialize(new SimulatorStore(), StorageJsonOptions);
            File.WriteAllText(StorageFilePath, initial);
        }

        private static string ExtractTld(string domainName)
        {
            var parts = domainName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length >= 2 ? parts[^1] : string.Empty;
        }

        private sealed class SimulatorStore
        {
            public List<SimulatorDomainEntry> Domains { get; set; } = [];
        }

        private sealed class SimulatorDomainEntry
        {
            public string DomainName { get; set; } = string.Empty;
            public string Status { get; set; } = "ACTIVE";
            public DateTime RegistrationDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public bool AutoRenew { get; set; }
            public bool PrivacyProtection { get; set; }
            public bool Locked { get; set; } = true;
            public List<string> Nameservers { get; set; } = [];
            public ContactInformation? RegistrantContact { get; set; }
            public ContactInformation? AdminContact { get; set; }
            public ContactInformation? TechContact { get; set; }
            public ContactInformation? BillingContact { get; set; }
            public List<DnsRecordModel> DnsRecords { get; set; } = [];
            public int NextDnsRecordId { get; set; } = 1;
            public string? LastTransferStatus { get; set; }
        }
    }
}
