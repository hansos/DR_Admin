using DomainRegistrationLib.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// DNSimple Registrar Implementation
    /// API Documentation: https://developer.dnsimple.com/v2/
    /// </summary>
    public class DnSimpleRegistrar : BaseRegistrar
    {
        private readonly string _accountId;
        private readonly string _apiToken;

        public DnSimpleRegistrar(string accountId, string apiToken, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.dnsimple.com/v2" 
                : "https://api.sandbox.dnsimple.com/v2")
        {
            _accountId = accountId;
            _apiToken = apiToken;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/{_accountId}/registrar/domains/{domainName}/check");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var data = result.GetProperty("data");
                var isAvailable = data.GetProperty("available").GetBoolean();
                var isPremium = data.TryGetProperty("premium", out var premiumElement) && premiumElement.GetBoolean();
                
                decimal? price = null;
                if (isPremium && data.TryGetProperty("premium_price", out var priceElement))
                {
                    price = decimal.Parse(priceElement.GetString() ?? "0");
                }

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
                    registrant_id = await GetOrCreateContactAsync(request.RegistrantContact),
                    auto_renew = request.AutoRenew,
                    whois_privacy = request.PrivacyProtection,
                    extended_attributes = new { }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/{_accountId}/registrar/domains/{request.DomainName}/registrations", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var data = result.GetProperty("data");

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via DNSimple",
                    OrderId = data.TryGetProperty("id", out var idProp) ? idProp.GetInt32().ToString() : null,
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

                var response = await _httpClient.PostAsync($"/{_accountId}/registrar/domains/{request.DomainName}/renewals", content);
                response.EnsureSuccessStatusCode();

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via DNSimple",
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
                    registrant_id = await GetOrCreateContactAsync(request.RegistrantContact!),
                    auth_code = request.AuthCode,
                    auto_renew = request.AutoRenew,
                    whois_privacy = request.PrivacyProtection
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/{_accountId}/registrar/domains/{request.DomainName}/transfers", content);
                response.EnsureSuccessStatusCode();

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via DNSimple",
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
                var response = await _httpClient.GetAsync($"/{_accountId}/zones/{domainName}/records");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var records = result.GetProperty("data");

                var dnsRecords = new List<DnsRecordModel>();
                foreach (var record in records.EnumerateArray())
                {
                    dnsRecords.Add(new DnsRecordModel
                    {
                        Id = record.GetProperty("id").GetInt32(),
                        Name = record.GetProperty("name").GetString() ?? "",
                        Type = record.GetProperty("type").GetString() ?? "",
                        Value = record.GetProperty("content").GetString() ?? "",
                        TTL = record.GetProperty("ttl").GetInt32(),
                        Priority = record.TryGetProperty("priority", out var priority) ? priority.GetInt32() : (int?)null
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
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            try
            {
                // DNSimple doesn't support bulk update, so we need to update records individually
                foreach (var record in dnsZone.Records)
                {
                    if (record.Id > 0)
                    {
                        await UpdateDnsRecordAsync(domainName, record);
                    }
                    else
                    {
                        await AddDnsRecordAsync(domainName, record);
                    }
                }

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
                var payload = new
                {
                    name = record.Name,
                    type = record.Type,
                    content = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/{_accountId}/zones/{domainName}/records", content);
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
                    name = record.Name,
                    content = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/{_accountId}/zones/{domainName}/records/{record.Id}", content);
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
                var response = await _httpClient.DeleteAsync($"/{_accountId}/zones/{domainName}/records/{recordId}");
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
                var response = await _httpClient.GetAsync($"/{_accountId}/registrar/domains/{domainName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var data = result.GetProperty("data");

                var expiresAt = data.TryGetProperty("expires_at", out var expiresAtProp) 
                    ? DateTime.Parse(expiresAtProp.GetString() ?? DateTime.UtcNow.ToString())
                    : (DateTime?)null;

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = data.GetProperty("state").GetString() ?? "active",
                    ExpirationDate = expiresAt,
                    AutoRenew = data.TryGetProperty("auto_renew", out var autoRenew) && autoRenew.GetBoolean(),
                    PrivacyProtection = data.TryGetProperty("whois_privacy", out var privacy) && privacy.GetBoolean(),
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
                var payload = nameservers.Select(ns => new { name = ns }).ToArray();
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/{_accountId}/registrar/domains/{domainName}/delegation", content);
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
                HttpResponseMessage response;
                if (enable)
                {
                    response = await _httpClient.PutAsync($"/{_accountId}/registrar/domains/{domainName}/whois_privacy", null);
                }
                else
                {
                    response = await _httpClient.DeleteAsync($"/{_accountId}/registrar/domains/{domainName}/whois_privacy");
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
                HttpResponseMessage response;
                if (enable)
                {
                    response = await _httpClient.PutAsync($"/{_accountId}/registrar/domains/{domainName}/auto_renewal", null);
                }
                else
                {
                    response = await _httpClient.DeleteAsync($"/{_accountId}/registrar/domains/{domainName}/auto_renewal");
                }

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

        private async Task<int> GetOrCreateContactAsync(ContactInformation contact)
        {
            // Simplified - in production, you would search for existing contact or create new one
            // For now, return a placeholder ID
            // In real implementation, you would call:
            // POST /{accountId}/contacts with contact details
            // and return the contact ID from the response
            return 1;
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/tlds");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var tlds = new List<TldInfo>();
                if (result.TryGetProperty("data", out var dataArray))
                {
                    foreach (var tld in dataArray.EnumerateArray())
                    {
                        var name = tld.GetProperty("tld").GetString();
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
                            if (tld.TryGetProperty("minimum_registration", out var minYears))
                                tldInfo.MinRegistrationYears = minYears.GetInt32();
                            if (tld.TryGetProperty("maximum_registration", out var maxYears))
                                tldInfo.MaxRegistrationYears = maxYears.GetInt32();
                            if (tld.TryGetProperty("whois_privacy", out var privacy))
                                tldInfo.SupportsPrivacy = privacy.GetBoolean();
                            if (tld.TryGetProperty("dnssec_interface_type", out var dnssec))
                                tldInfo.SupportsDnssec = !string.IsNullOrEmpty(dnssec.GetString());

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
