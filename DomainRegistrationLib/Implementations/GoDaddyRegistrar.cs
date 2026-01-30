using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    public class GoDaddyRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly bool _useProduction;

        public GoDaddyRegistrar(string apiKey, string apiSecret, bool useProduction)
            : base(useProduction ? "https://api.godaddy.com" : "https://api.ote-godaddy.com")
        {
            _logger = Log.ForContext<GoDaddyRegistrar>();
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _useProduction = useProduction;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("sso-key", $"{_apiKey}:{_apiSecret}");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v1/domains/available?domain={domainName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var isAvailable = result.GetProperty("available").GetBoolean();
                var price = result.TryGetProperty("price", out var priceElement) 
                    ? priceElement.GetDecimal() 
                    : (decimal?)null;

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    PremiumPrice = price,
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
                    domain = request.DomainName,
                    period = request.Years,
                    nameServers = request.Nameservers,
                    renewAuto = request.AutoRenew,
                    privacy = request.PrivacyProtection,
                    consent = new
                    {
                        agreementKeys = new[] { "DNRA" },
                        agreedBy = request.RegistrantContact.Email,
                        agreedAt = DateTime.UtcNow.ToString("o")
                    },
                    contactRegistrant = MapContact(request.RegistrantContact),
                    contactAdmin = MapContact(request.AdminContact ?? request.RegistrantContact),
                    contactTech = MapContact(request.TechContact ?? request.RegistrantContact),
                    contactBilling = MapContact(request.BillingContact ?? request.RegistrantContact)
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v1/domains/purchase", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully",
                    OrderId = result.TryGetProperty("orderId", out var orderId) ? orderId.GetString() : null,
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
                var payload = new { period = request.Years };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/v1/domains/{request.DomainName}/renew", content);
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
                    authCode = request.AuthCode,
                    privacy = request.PrivacyProtection,
                    renewAuto = request.AutoRenew,
                    consent = new
                    {
                        agreementKeys = new[] { "DNTA" },
                        agreedBy = request.RegistrantContact?.Email ?? "",
                        agreedAt = DateTime.UtcNow.ToString("o")
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/v1/domains/{request.DomainName}/transfer", content);
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
                var response = await _httpClient.GetAsync($"/v1/domains/{domainName}/records");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var records = JsonSerializer.Deserialize<List<JsonElement>>(content);

                var dnsRecords = records?.Select(r => new DnsRecordModel
                {
                    Type = r.GetProperty("type").GetString() ?? "",
                    Name = r.GetProperty("name").GetString() ?? "",
                    Value = r.GetProperty("data").GetString() ?? "",
                    TTL = r.GetProperty("ttl").GetInt32(),
                    Priority = r.TryGetProperty("priority", out var priority) ? priority.GetInt32() : null
                }).ToList() ?? new List<DnsRecordModel>();

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
            try
            {
                var records = dnsZone.Records.Select(r => new
                {
                    type = r.Type,
                    name = r.Name,
                    data = r.Value,
                    ttl = r.TTL,
                    priority = r.Priority
                }).ToList();

                var json = JsonSerializer.Serialize(records);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/v1/domains/{domainName}/records", content);
                response.EnsureSuccessStatusCode();

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS zone updated successfully"
                };
            }
            catch (Exception ex)
            {
                return CreateDnsErrorResult($"Error updating DNS zone: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            try
            {
                var payload = new[]
                {
                    new
                    {
                        type = record.Type,
                        name = record.Name,
                        data = record.Value,
                        ttl = record.TTL,
                        priority = record.Priority
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/v1/domains/{domainName}/records", content);
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
                var payload = new
                {
                    data = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"/v1/domains/{domainName}/records/{record.Type}/{record.Name}", 
                    content);
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
            // GoDaddy doesn't use numeric IDs, would need type and name
            return CreateDnsErrorResult("Delete by ID not supported - use UpdateDnsZoneAsync instead");
        }

        public override async Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v1/domains/{domainName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = result.GetProperty("status").GetString(),
                    RegistrationDate = result.TryGetProperty("createdAt", out var created) 
                        ? DateTime.Parse(created.GetString() ?? "") 
                        : null,
                    ExpirationDate = result.TryGetProperty("expires", out var expires) 
                        ? DateTime.Parse(expires.GetString() ?? "") 
                        : null,
                    AutoRenew = result.GetProperty("renewAuto").GetBoolean(),
                    PrivacyProtection = result.GetProperty("privacy").GetBoolean(),
                    Locked = result.GetProperty("locked").GetBoolean(),
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
                var json = JsonSerializer.Serialize(nameservers);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/v1/domains/{domainName}/nameservers", content);
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
                var endpoint = enable 
                    ? $"/v1/domains/{domainName}/privacy/purchase" 
                    : $"/v1/domains/{domainName}/privacy";

                HttpResponseMessage response;
                if (enable)
                {
                    var content = new StringContent("{}", Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync(endpoint, content);
                }
                else
                {
                    response = await _httpClient.DeleteAsync(endpoint);
                }

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
                var payload = new { renewAuto = enable };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/v1/domains/{domainName}", content);
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
                var response = await _httpClient.GetAsync("/v1/domains/tlds");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var tlds = new List<TldInfo>();
                foreach (var tld in result.EnumerateArray())
                {
                    var name = tld.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var tldInfo = new TldInfo
                        {
                            Name = name,
                            Currency = "USD"
                        };

                        if (tld.TryGetProperty("type", out var typeProp))
                            tldInfo.Type = typeProp.GetString();
                        if (tld.TryGetProperty("minRegistrationYears", out var minYears))
                            tldInfo.MinRegistrationYears = minYears.GetInt32();
                        if (tld.TryGetProperty("maxRegistrationYears", out var maxYears))
                            tldInfo.MaxRegistrationYears = maxYears.GetInt32();
                        if (tld.TryGetProperty("supportsPrivacy", out var privacy))
                            tldInfo.SupportsPrivacy = privacy.GetBoolean();

                        tlds.Add(tldInfo);
                    }
                }

                return tlds;
            }
            catch (Exception)
            {
                return [];
            }
        }

        private object MapContact(ContactInformation contact)
        {
            return new
            {
                nameFirst = contact.FirstName,
                nameLast = contact.LastName,
                organization = contact.Organization,
                email = contact.Email,
                phone = contact.Phone,
                addressMailing = new
                {
                    address1 = contact.Address1,
                    address2 = contact.Address2,
                    city = contact.City,
                    state = contact.State,
                    postalCode = contact.PostalCode,
                    country = contact.Country
                }
            };
        }
    }
}
