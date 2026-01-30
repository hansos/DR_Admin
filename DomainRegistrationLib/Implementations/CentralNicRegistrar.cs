using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    public class CentralNicRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _username;
        private readonly string _password;

        public CentralNicRegistrar(string username, string password, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.centralnic.com/v2" 
                : "https://api-ote.centralnic.com/v2")
        {
            _logger = Log.ForContext<CentralNicRegistrar>();
            _username = username;
            _password = password;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/domains/{domainName}/check");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var isAvailable = result.GetProperty("available").GetBoolean();

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
                    domain = request.DomainName,
                    period = request.Years,
                    contacts = new
                    {
                        registrant = MapContact(request.RegistrantContact),
                        admin = MapContact(request.AdminContact ?? request.RegistrantContact),
                        tech = MapContact(request.TechContact ?? request.RegistrantContact),
                        billing = MapContact(request.BillingContact ?? request.RegistrantContact)
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/domains", content);
                response.EnsureSuccessStatusCode();

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via CentralNic",
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

                var response = await _httpClient.PostAsync($"/domains/{request.DomainName}/renew", content);
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
                    domain = request.DomainName,
                    authCode = request.AuthCode
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/domains/transfer", content);
                response.EnsureSuccessStatusCode();

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated",
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
                var response = await _httpClient.GetAsync($"/domains/{domainName}/dns");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var records = JsonSerializer.Deserialize<List<JsonElement>>(content);

                var dnsRecords = records?.Select(r => new DnsRecordModel
                {
                    Type = r.GetProperty("type").GetString() ?? "",
                    Name = r.GetProperty("name").GetString() ?? "",
                    Value = r.GetProperty("content").GetString() ?? "",
                    TTL = r.GetProperty("ttl").GetInt32()
                }).ToList() ?? new List<DnsRecordModel>();

                return new DnsZone
                {
                    DomainName = domainName,
                    Records = dnsRecords
                };
            }
            catch (Exception ex)
            {
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
                    content = r.Value,
                    ttl = r.TTL
                }).ToList();

                var json = JsonSerializer.Serialize(records);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/domains/{domainName}/dns", content);
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
            var zone = await GetDnsZoneAsync(domainName);
            zone.Records.Add(record);
            return await UpdateDnsZoneAsync(domainName, zone);
        }

        public override async Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            var zone = await GetDnsZoneAsync(domainName);
            var existing = zone.Records.FirstOrDefault(r => r.Id == record.Id);
            if (existing != null)
            {
                zone.Records.Remove(existing);
                zone.Records.Add(record);
            }
            return await UpdateDnsZoneAsync(domainName, zone);
        }

        public override async Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            var zone = await GetDnsZoneAsync(domainName);
            var record = zone.Records.FirstOrDefault(r => r.Id == recordId);
            if (record != null)
            {
                zone.Records.Remove(record);
            }
            return await UpdateDnsZoneAsync(domainName, zone);
        }

        public override async Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/domains/{domainName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = result.GetProperty("status").GetString(),
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

                var response = await _httpClient.PutAsync($"/domains/{domainName}/nameservers", content);
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

                var response = await _httpClient.PatchAsync($"/domains/{domainName}", content);
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
                var payload = new { autoRenew = enable };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/domains/{domainName}", content);
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

        private object MapContact(ContactInformation contact)
        {
            return new
            {
                firstName = contact.FirstName,
                lastName = contact.LastName,
                organization = contact.Organization,
                email = contact.Email,
                phone = contact.Phone,
                address = new
                {
                    street1 = contact.Address1,
                    street2 = contact.Address2,
                    city = contact.City,
                    state = contact.State,
                    postalCode = contact.PostalCode,
                    country = contact.Country
                }
            };
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/v3/tlds");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var tlds = new List<TldInfo>();
                if (result.TryGetProperty("tlds", out var tldsArray))
                {
                    foreach (var tld in tldsArray.EnumerateArray())
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
                            if (tld.TryGetProperty("type", out var typeProp))
                                tldInfo.Type = typeProp.GetString();
                            if (tld.TryGetProperty("currency", out var currencyProp))
                                tldInfo.Currency = currencyProp.GetString();

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
    }
}
