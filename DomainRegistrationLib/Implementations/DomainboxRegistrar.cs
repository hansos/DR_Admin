using DomainRegistrationLib.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// DomainBox Registrar Implementation
    /// API Documentation: https://www.domainbox.com/api/docs
    /// </summary>
    public class DomainboxRegistrar : BaseRegistrar
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public DomainboxRegistrar(string apiKey, string apiSecret, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.domainbox.com/v1" 
                : "https://sandbox.domainbox.com/v1")
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Get, 
                    $"/domains/check?domain={domainName}");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var isAvailable = result.GetProperty("available").GetBoolean();
                var isPremium = result.TryGetProperty("premium", out var premiumElement) && premiumElement.GetBoolean();
                
                decimal? price = null;
                if (result.TryGetProperty("price", out var priceElement))
                {
                    price = priceElement.GetDecimal();
                }

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    PremiumPrice = isPremium ? price : null,
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
                    nameservers = request.Nameservers,
                    auto_renew = request.AutoRenew,
                    privacy = request.PrivacyProtection,
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

                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Post, "/domains/register", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via DomainBox",
                    OrderId = result.TryGetProperty("order_id", out var orderId) ? orderId.GetString() : null,
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
                var payload = new 
                { 
                    domain = request.DomainName,
                    period = request.Years 
                };
                
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Post, "/domains/renew", content);

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via DomainBox",
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
                    auth_code = request.AuthCode,
                    privacy = request.PrivacyProtection,
                    auto_renew = request.AutoRenew,
                    contacts = request.RegistrantContact != null ? new
                    {
                        registrant = MapContact(request.RegistrantContact),
                        admin = MapContact(request.AdminContact ?? request.RegistrantContact),
                        tech = MapContact(request.TechContact ?? request.RegistrantContact),
                        billing = MapContact(request.BillingContact ?? request.RegistrantContact)
                    } : null
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Post, "/domains/transfer", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via DomainBox",
                    TransferStatus = result.TryGetProperty("status", out var status) 
                        ? status.GetString() ?? "Pending" 
                        : "Pending"
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
                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Get, 
                    $"/dns/zones/{domainName}/records");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var records = result.GetProperty("records");

                var dnsRecords = new List<DnsRecordModel>();
                foreach (var record in records.EnumerateArray())
                {
                    dnsRecords.Add(new DnsRecordModel
                    {
                        Id = record.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0,
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
                var payload = new
                {
                    records = dnsZone.Records.Select(r => new
                    {
                        name = r.Name,
                        type = r.Type,
                        content = r.Value,
                        ttl = r.TTL,
                        priority = r.Priority
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Put, 
                    $"/dns/zones/{domainName}/records", 
                    content);

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

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Post, 
                    $"/dns/zones/{domainName}/records", 
                    content);

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
                    type = record.Type,
                    content = record.Value,
                    ttl = record.TTL,
                    priority = record.Priority
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Put, 
                    $"/dns/zones/{domainName}/records/{record.Id}", 
                    content);

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
                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Delete, 
                    $"/dns/zones/{domainName}/records/{recordId}");

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
                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Get, 
                    $"/domains/{domainName}");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                var data = result.GetProperty("domain");

                var expiresAt = data.TryGetProperty("expires_at", out var expiresAtProp) 
                    ? DateTime.Parse(expiresAtProp.GetString() ?? DateTime.UtcNow.ToString())
                    : (DateTime?)null;

                var createdAt = data.TryGetProperty("created_at", out var createdAtProp) 
                    ? DateTime.Parse(createdAtProp.GetString() ?? DateTime.UtcNow.ToString())
                    : (DateTime?)null;

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = data.TryGetProperty("status", out var status) ? status.GetString() ?? "active" : "active",
                    RegistrationDate = createdAt,
                    ExpirationDate = expiresAt,
                    AutoRenew = data.TryGetProperty("auto_renew", out var autoRenew) && autoRenew.GetBoolean(),
                    PrivacyProtection = data.TryGetProperty("privacy", out var privacy) && privacy.GetBoolean(),
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

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Put, 
                    $"/domains/{domainName}/nameservers", 
                    content);

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

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Patch, 
                    $"/domains/{domainName}/privacy", 
                    content);

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

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Patch, 
                    $"/domains/{domainName}/auto-renew", 
                    content);

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

        private async Task<HttpResponseMessage> MakeAuthenticatedRequestAsync(
            HttpMethod method, 
            string endpoint, 
            HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            
            if (content != null)
            {
                request.Content = content;
            }

            // Add timestamp for request signing
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            request.Headers.Add("X-Timestamp", timestamp);

            // Generate signature: HMAC-SHA256(secret, method + endpoint + timestamp)
            var signatureData = $"{method.Method}{endpoint}{timestamp}";
            var signature = GenerateSignature(signatureData, _apiSecret);
            request.Headers.Add("X-Signature", signature);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            return response;
        }

        private string GenerateSignature(string data, string secret)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var dataBytes = encoding.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }

        private object MapContact(ContactInformation contact)
        {
            return new
            {
                first_name = contact.FirstName,
                last_name = contact.LastName,
                organization = contact.Organization,
                email = contact.Email,
                phone = contact.Phone,
                address1 = contact.Address1,
                address2 = contact.Address2,
                city = contact.City,
                state = contact.State,
                postal_code = contact.PostalCode,
                country = contact.Country
            };
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Get, "/tlds");
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
                                Currency = "GBP"
                            };

                            if (tld.TryGetProperty("registration_price", out var regPrice))
                                tldInfo.RegistrationPrice = regPrice.GetDecimal();
                            if (tld.TryGetProperty("renewal_price", out var renewPrice))
                                tldInfo.RenewalPrice = renewPrice.GetDecimal();
                            if (tld.TryGetProperty("transfer_price", out var transPrice))
                                tldInfo.TransferPrice = transPrice.GetDecimal();
                            if (tld.TryGetProperty("currency", out var currencyProp))
                                tldInfo.Currency = currencyProp.GetString();
                            if (tld.TryGetProperty("type", out var typeProp))
                                tldInfo.Type = typeProp.GetString();
                            if (tld.TryGetProperty("privacy_available", out var privacy))
                                tldInfo.SupportsPrivacy = privacy.GetBoolean();

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
