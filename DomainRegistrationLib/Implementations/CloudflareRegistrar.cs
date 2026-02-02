using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    public class CloudflareRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _apiToken;
        private readonly string _accountId;

        public CloudflareRegistrar(string apiToken, string accountId)
            : base("https://api.cloudflare.com/client/v4")
        {
            _logger = Log.ForContext<CloudflareRegistrar>();
            _apiToken = apiToken;
            _accountId = accountId;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/accounts/{_accountId}/registrar/domains/{domainName}/availability");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var isAvailable = result.GetProperty("result").GetProperty("available").GetBoolean();

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    Message = isAvailable ? "Domain is available" : "Domain is not available"
                };
            }
            catch (Exception ex)
            {
                return new DomainAvailabilityResult
                {
                    Success = false,
                    DomainName = domainName,
                    IsAvailable = false,
                    Message = $"Error checking availability: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public override async Task<DomainRegistrationResult> RegisterDomainAsync(DomainRegistrationRequest request)
        {
            try
            {
                var payload = new
                {
                    name = request.DomainName,
                    years = request.Years,
                    auto_renew = request.AutoRenew,
                    privacy = request.PrivacyProtection
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"/accounts/{_accountId}/registrar/domains", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully",
                    RegistrationDate = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddYears(request.Years)
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Error registering domain: {ex.Message}");
            }
        }

        public override async Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            try
            {
                var payload = new { years = request.Years };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"/accounts/{_accountId}/registrar/domains/{request.DomainName}/renew", content);
                response.EnsureSuccessStatusCode();

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully",
                    NewExpirationDate = DateTime.UtcNow.AddYears(request.Years)
                };
            }
            catch (Exception ex)
            {
                return CreateRenewalErrorResult($"Error renewing domain: {ex.Message}");
            }
        }

        public override async Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            try
            {
                var payload = new
                {
                    name = request.DomainName,
                    auth_code = request.AuthCode,
                    auto_renew = request.AutoRenew,
                    privacy = request.PrivacyProtection
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"/accounts/{_accountId}/registrar/domains/transfer", content);
                response.EnsureSuccessStatusCode();

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated successfully",
                    TransferStatus = "Pending"
                };
            }
            catch (Exception ex)
            {
                return new DomainTransferResult
                {
                    Success = false,
                    Message = $"Error transferring domain: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public override async Task<DnsZone> GetDnsZoneAsync(string domainName)
        {
            try
            {
                // First get the zone ID
                var zonesResponse = await _httpClient.GetAsync($"/zones?name={domainName}");
                zonesResponse.EnsureSuccessStatusCode();

                var zonesContent = await zonesResponse.Content.ReadAsStringAsync();
                var zonesResult = JsonSerializer.Deserialize<JsonElement>(zonesContent);
                var zones = zonesResult.GetProperty("result");

                if (zones.GetArrayLength() == 0)
                {
                    return new DnsZone { DomainName = domainName };
                }

                var zoneId = zones[0].GetProperty("id").GetString();

                // Get DNS records
                var response = await _httpClient.GetAsync($"/zones/{zoneId}/dns_records");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var records = result.GetProperty("result");

                var dnsRecords = new List<DnsRecordModel>();
                foreach (var record in records.EnumerateArray())
                {
                    dnsRecords.Add(new DnsRecordModel
                    {
                        Id = int.TryParse(record.GetProperty("id").GetString(), out var id) ? id : null,
                        Type = record.GetProperty("type").GetString() ?? "",
                        Name = record.GetProperty("name").GetString() ?? "",
                        Value = record.GetProperty("content").GetString() ?? "",
                        TTL = record.GetProperty("ttl").GetInt32(),
                        Priority = record.TryGetProperty("priority", out var priority) ? priority.GetInt32() : null
                    });
                }

                return new DnsZone
                {
                    DomainName = domainName,
                    Records = dnsRecords
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting DNS zone for {domainName}: {ex.Message}");
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            // Cloudflare doesn't support bulk update, would need to update records individually
            return CreateDnsErrorResult("Bulk DNS zone update not supported - use individual record methods");
        }

        public override async Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            try
            {
                var zonesResponse = await _httpClient.GetAsync($"/zones?name={domainName}");
                zonesResponse.EnsureSuccessStatusCode();

                var zonesContent = await zonesResponse.Content.ReadAsStringAsync();
                var zonesResult = JsonSerializer.Deserialize<JsonElement>(zonesContent);
                var zones = zonesResult.GetProperty("result");

                if (zones.GetArrayLength() == 0)
                {
                    return CreateDnsErrorResult("Zone not found");
                }

                var zoneId = zones[0].GetProperty("id").GetString();

                var payload = new
                {
                    type = record.Type,
                    name = record.Name,
                    content = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/zones/{zoneId}/dns_records", content);
                response.EnsureSuccessStatusCode();

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record added successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateDnsErrorResult($"Error adding DNS record: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            try
            {
                var zonesResponse = await _httpClient.GetAsync($"/zones?name={domainName}");
                zonesResponse.EnsureSuccessStatusCode();

                var zonesContent = await zonesResponse.Content.ReadAsStringAsync();
                var zonesResult = JsonSerializer.Deserialize<JsonElement>(zonesContent);
                var zones = zonesResult.GetProperty("result");

                if (zones.GetArrayLength() == 0)
                {
                    return CreateDnsErrorResult("Zone not found");
                }

                var zoneId = zones[0].GetProperty("id").GetString();

                var payload = new
                {
                    type = record.Type,
                    name = record.Name,
                    content = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/zones/{zoneId}/dns_records/{record.Id}", content);
                response.EnsureSuccessStatusCode();

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record updated successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateDnsErrorResult($"Error updating DNS record: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            try
            {
                var zonesResponse = await _httpClient.GetAsync($"/zones?name={domainName}");
                zonesResponse.EnsureSuccessStatusCode();

                var zonesContent = await zonesResponse.Content.ReadAsStringAsync();
                var zonesResult = JsonSerializer.Deserialize<JsonElement>(zonesContent);
                var zones = zonesResult.GetProperty("result");

                if (zones.GetArrayLength() == 0)
                {
                    return CreateDnsErrorResult("Zone not found");
                }

                var zoneId = zones[0].GetProperty("id").GetString();

                var response = await _httpClient.DeleteAsync($"/zones/{zoneId}/dns_records/{recordId}");
                response.EnsureSuccessStatusCode();

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateDnsErrorResult($"Error deleting DNS record: {ex.Message}");
            }
        }

        public override async Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/accounts/{_accountId}/registrar/domains/{domainName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var domain = result.GetProperty("result");

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = domain.GetProperty("status").GetString(),
                    RegistrationDate = domain.TryGetProperty("created_at", out var created) 
                        ? DateTime.Parse(created.GetString() ?? "") 
                        : null,
                    ExpirationDate = domain.TryGetProperty("expires_at", out var expires) 
                        ? DateTime.Parse(expires.GetString() ?? "") 
                        : null,
                    AutoRenew = domain.GetProperty("auto_renew").GetBoolean(),
                    PrivacyProtection = domain.GetProperty("privacy").GetBoolean(),
                    Locked = domain.GetProperty("locked").GetBoolean(),
                    Message = "Domain information retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new DomainInfoResult
                {
                    Success = false,
                    Message = $"Error getting domain info: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public override async Task<DomainUpdateResult> UpdateNameserversAsync(string domainName, List<string> nameservers)
        {
            try
            {
                var payload = new { nameservers };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"/accounts/{_accountId}/registrar/domains/{domainName}/nameservers", content);
                response.EnsureSuccessStatusCode();

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "Nameservers updated successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Error updating nameservers: {ex.Message}");
            }
        }

        public override async Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            try
            {
                var payload = new { privacy = enable };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(
                    $"/accounts/{_accountId}/registrar/domains/{domainName}", content);
                response.EnsureSuccessStatusCode();

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = $"Privacy protection {(enable ? "enabled" : "disabled")} successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Error setting privacy protection: {ex.Message}");
            }
        }

        public override async Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            try
            {
                var payload = new { auto_renew = enable };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(
                    $"/accounts/{_accountId}/registrar/domains/{domainName}", content);
                response.EnsureSuccessStatusCode();

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = $"Auto-renew {(enable ? "enabled" : "disabled")} successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Error setting auto-renew: {ex.Message}");
            }
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"/accounts/{_accountId}/registrar/tlds");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var tlds = new List<TldInfo>();
                if (result.TryGetProperty("result", out var resultArray))
                {
                    foreach (var tld in resultArray.EnumerateArray())
                    {
                        var name = tld.GetProperty("name").GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            var tldInfo = new TldInfo
                            {
                                Name = name,
                                Currency = "USD"
                            };

                            if (tld.TryGetProperty("registration_price", out var regPrice))
                                tldInfo.RegistrationPrice = regPrice.GetDecimal();
                            if (tld.TryGetProperty("renewal_price", out var renewPrice))
                                tldInfo.RenewalPrice = renewPrice.GetDecimal();
                            if (tld.TryGetProperty("transfer_price", out var transPrice))
                                tldInfo.TransferPrice = transPrice.GetDecimal();
                            if (tld.TryGetProperty("dnssec", out var dnssec))
                                tldInfo.SupportsDnssec = dnssec.GetBoolean();

                            tlds.Add(tldInfo);
                        }
                    }
                }

                return tlds;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public override async Task<RegisteredDomainsResult> GetRegisteredDomainsAsync()
        {
            try
            {
                _logger.Information("Getting registered domains from Cloudflare");

                // Note: Cloudflare's domain list API doesn't include contact information
                // Individual API calls per domain would be required to fetch contact details
                // Cloudflare uses account-level domain listing
                var response = await _httpClient.GetAsync($"/client/v4/accounts/{_accountId}/registrar/domains");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var domains = new List<RegisteredDomainInfo>();

                if (result.TryGetProperty("result", out var resultProp) && resultProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var domain in resultProp.EnumerateArray())
                    {
                        var domainInfo = new RegisteredDomainInfo
                        {
                            DomainName = domain.GetProperty("name").GetString() ?? "",
                            Status = domain.TryGetProperty("status", out var status) ? status.GetString() : null,
                            ExpirationDate = domain.TryGetProperty("expires_at", out var expires) 
                                ? DateTime.Parse(expires.GetString() ?? "") 
                                : null,
                            RegistrationDate = domain.TryGetProperty("created_at", out var created) 
                                ? DateTime.Parse(created.GetString() ?? "") 
                                : null,
                            AutoRenew = domain.TryGetProperty("auto_renew", out var autoRenew) && autoRenew.GetBoolean(),
                            Locked = domain.TryGetProperty("locked", out var locked) && locked.GetBoolean(),
                            PrivacyProtection = domain.TryGetProperty("privacy", out var privacy) && privacy.GetBoolean(),
                            Contacts = [] // Cloudflare list API doesn't include contacts
                        };

                        domains.Add(domainInfo);
                    }
                }

                _logger.Information("Successfully retrieved {Count} domains from Cloudflare", domains.Count);

                return new RegisteredDomainsResult
                {
                    Success = true,
                    Message = $"Successfully retrieved {domains.Count} domains",
                    Domains = domains,
                    TotalCount = domains.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting registered domains from Cloudflare");
                return new RegisteredDomainsResult
                {
                    Success = false,
                    Message = $"Error retrieving domains: {ex.Message}",
                    ErrorCode = "API_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
