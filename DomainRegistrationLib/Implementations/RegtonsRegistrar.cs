using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Regtons Reseller Program Implementation
    /// API Documentation: https://www.regtons.com/reseller/api
    /// Uses REST API with JSON and HMAC authentication
    /// </summary>
    public class RegtonsRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _username;

        public RegtonsRegistrar(string apiKey, string apiSecret, string username, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.regtons.com/v1" 
                : "https://sandbox.regtons.com/v1")
        {
            _logger = Log.ForContext<RegtonsRegistrar>();
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _username = username;

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Username", _username);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Get, 
                    $"/domains/availability?domain={domainName}");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var isAvailable = result.GetProperty("available").GetBoolean();
                var isPremium = result.TryGetProperty("premium", out var premiumElement) && premiumElement.GetBoolean();
                
                decimal? price = null;
                if (result.TryGetProperty("price", out var priceElement))
                {
                    price = priceElement.GetDecimal();
                }

                var message = result.TryGetProperty("message", out var msgElement) 
                    ? msgElement.GetString() 
                    : (isAvailable ? "Domain is available" : "Domain is not available");

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    PremiumPrice = isPremium ? price : null,
                    Message = message ?? "Status retrieved"
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
                    whois_privacy = request.PrivacyProtection,
                    contacts = new
                    {
                        registrant = MapContact(request.RegistrantContact),
                        admin = MapContact(request.AdminContact ?? request.RegistrantContact),
                        technical = MapContact(request.TechContact ?? request.RegistrantContact),
                        billing = MapContact(request.BillingContact ?? request.RegistrantContact)
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Post, "/domains/register", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Registration failed" 
                        : "Registration failed";
                    return CreateErrorResult(errorMsg);
                }

                var data = result.GetProperty("data");
                var orderId = data.TryGetProperty("order_id", out var orderIdProp) 
                    ? orderIdProp.GetString() 
                    : null;

                var expiresAt = data.TryGetProperty("expires_at", out var expiresAtProp) 
                    ? DateTime.Parse(expiresAtProp.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow.AddYears(request.Years);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via Regtons",
                    OrderId = orderId,
                    RegistrationDate = DateTime.UtcNow,
                    ExpirationDate = expiresAt
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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Renewal failed" 
                        : "Renewal failed";
                    return CreateRenewalErrorResult(errorMsg);
                }

                var data = result.GetProperty("data");
                var expiresAt = data.TryGetProperty("expires_at", out var expiresAtProp) 
                    ? DateTime.Parse(expiresAtProp.GetString() ?? DateTime.UtcNow.ToString())
                    : DateTime.UtcNow.AddYears(request.Years);

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via Regtons",
                    NewExpirationDate = expiresAt
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
                    auto_renew = request.AutoRenew,
                    whois_privacy = request.PrivacyProtection,
                    contacts = request.RegistrantContact != null ? new
                    {
                        registrant = MapContact(request.RegistrantContact),
                        admin = MapContact(request.AdminContact ?? request.RegistrantContact),
                        technical = MapContact(request.TechContact ?? request.RegistrantContact),
                        billing = MapContact(request.BillingContact ?? request.RegistrantContact)
                    } : null
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(HttpMethod.Post, "/domains/transfer", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Transfer failed" 
                        : "Transfer failed";
                    return new DomainTransferResult
                    {
                        Success = false,
                        Message = errorMsg,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var data = result.GetProperty("data");
                var status = data.TryGetProperty("status", out var statusProp) 
                    ? statusProp.GetString() ?? "pending" 
                    : "pending";

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via Regtons",
                    TransferStatus = status
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

                var records = new List<DnsRecordModel>();
                
                if (result.TryGetProperty("data", out var dataElement) && 
                    dataElement.TryGetProperty("records", out var recordsArray))
                {
                    foreach (var record in recordsArray.EnumerateArray())
                    {
                        records.Add(new DnsRecordModel
                        {
                            Id = record.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0,
                            Name = record.GetProperty("name").GetString() ?? "",
                            Type = record.GetProperty("type").GetString() ?? "",
                            Value = record.GetProperty("content").GetString() ?? "",
                            TTL = record.TryGetProperty("ttl", out var ttlProp) ? ttlProp.GetInt32() : 3600,
                            Priority = record.TryGetProperty("priority", out var priority) ? priority.GetInt32() : (int?)null
                        });
                    }
                }

                return new DnsZone
                {
                    DomainName = domainName,
                    Records = records
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting DNS zone for {DomainName}: {ErrorMessage}", domainName, ex.Message);
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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "DNS update failed" 
                        : "DNS update failed";
                    return CreateDnsErrorResult(errorMsg);
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

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Post, 
                    $"/dns/zones/{domainName}/records", 
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Add DNS record failed" 
                        : "Add DNS record failed";
                    return CreateDnsErrorResult(errorMsg);
                }

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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Update DNS record failed" 
                        : "Update DNS record failed";
                    return CreateDnsErrorResult(errorMsg);
                }

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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Delete DNS record failed" 
                        : "Delete DNS record failed";
                    return CreateDnsErrorResult(errorMsg);
                }

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

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Query failed" 
                        : "Query failed";
                    return new DomainInfoResult
                    {
                        Success = false,
                        Message = errorMsg,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var data = result.GetProperty("data");
                var domain = data.GetProperty("domain");

                var status = domain.TryGetProperty("status", out var statusProp) 
                    ? statusProp.GetString() ?? "active" 
                    : "active";
                
                DateTime? registrationDate = null;
                if (domain.TryGetProperty("created_at", out var createdAtProp))
                {
                    var dateStr = createdAtProp.GetString();
                    if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var regDate))
                    {
                        registrationDate = regDate;
                    }
                }

                DateTime? expirationDate = null;
                if (domain.TryGetProperty("expires_at", out var expiresAtProp))
                {
                    var dateStr = expiresAtProp.GetString();
                    if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var expDate))
                    {
                        expirationDate = expDate;
                    }
                }

                var autoRenew = domain.TryGetProperty("auto_renew", out var autoRenewProp) && autoRenewProp.GetBoolean();
                var privacy = domain.TryGetProperty("whois_privacy", out var privacyProp) && privacyProp.GetBoolean();

                var nameservers = new List<string>();
                if (domain.TryGetProperty("nameservers", out var nsArray))
                {
                    foreach (var ns in nsArray.EnumerateArray())
                    {
                        var nsValue = ns.GetString();
                        if (!string.IsNullOrEmpty(nsValue))
                        {
                            nameservers.Add(nsValue);
                        }
                    }
                }

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = status,
                    RegistrationDate = registrationDate,
                    ExpirationDate = expirationDate,
                    AutoRenew = autoRenew,
                    PrivacyProtection = privacy,
                    Nameservers = nameservers,
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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Nameserver update failed" 
                        : "Nameserver update failed";
                    return CreateUpdateErrorResult(errorMsg);
                }

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
                var payload = new { whois_privacy = enable };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await MakeAuthenticatedRequestAsync(
                    HttpMethod.Patch, 
                    $"/domains/{domainName}/privacy", 
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Privacy update failed" 
                        : "Privacy update failed";
                    return CreateUpdateErrorResult(errorMsg);
                }

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

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    var errorMsg = result.TryGetProperty("error", out var error) 
                        ? error.GetString() ?? "Auto-renew update failed" 
                        : "Auto-renew update failed";
                    return CreateUpdateErrorResult(errorMsg);
                }

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

            // Generate signature: HMAC-SHA256(secret, apiKey + method + endpoint + timestamp)
            var signatureData = $"{_apiKey}{method.Method}{endpoint}{timestamp}";
            var signature = GenerateHmacSignature(signatureData, _apiSecret);
            request.Headers.Add("X-Signature", signature);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            return response;
        }

        private string GenerateHmacSignature(string data, string secret)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var dataBytes = encoding.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private object MapContact(ContactInformation contact)
        {
            return new
            {
                first_name = contact.FirstName,
                last_name = contact.LastName,
                organization = contact.Organization ?? "",
                email = contact.Email,
                phone = contact.Phone,
                address_line_1 = contact.Address1,
                address_line_2 = contact.Address2 ?? "",
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
                var success = result.GetProperty("success").GetBoolean();
                if (!success)
                {
                    return tlds;
                }

                if (result.TryGetProperty("data", out var dataObj) && 
                    dataObj.TryGetProperty("tlds", out var tldsArray))
                {
                    foreach (var tld in tldsArray.EnumerateArray())
                    {
                        var name = tld.GetProperty("name").GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            var tldInfo = new TldInfo
                            {
                                Name = name,
                                Currency = "EUR"
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
                            if (tld.TryGetProperty("min_years", out var minYears))
                                tldInfo.MinRegistrationYears = minYears.GetInt32();
                            if (tld.TryGetProperty("max_years", out var maxYears))
                                tldInfo.MaxRegistrationYears = maxYears.GetInt32();
                            if (tld.TryGetProperty("privacy_available", out var privacy))
                                tldInfo.SupportsPrivacy = privacy.GetBoolean();
                            if (tld.TryGetProperty("dnssec_available", out var dnssec))
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
    }
}
