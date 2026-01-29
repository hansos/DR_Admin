using DomainRegistrationLib.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// AWS Route 53 Registrar Implementation
    /// API Documentation: https://docs.aws.amazon.com/Route53/latest/APIReference/
    /// Uses AWS SDK-style REST API with AWS Signature Version 4
    /// </summary>
    public class AwsRegistrar : BaseRegistrar
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _region;
        private readonly string _hostedZoneId;

        public AwsRegistrar(string accessKeyId, string secretAccessKey, string region, string hostedZoneId)
            : base($"https://route53.{region}.amazonaws.com")
        {
            _accessKeyId = accessKeyId;
            _secretAccessKey = secretAccessKey;
            _region = region;
            _hostedZoneId = hostedZoneId;

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var endpoint = "/2013-04-01/domain/availability";
                var payload = new { DomainName = domainName };
                var json = JsonSerializer.Serialize(payload);

                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var availability = result.GetProperty("Availability").GetString();
                var isAvailable = availability?.Equals("AVAILABLE", StringComparison.OrdinalIgnoreCase) ?? false;

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    Message = availability ?? "Unknown"
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
                var endpoint = "/2013-04-01/domain";
                var payload = new
                {
                    DomainName = request.DomainName,
                    DurationInYears = request.Years,
                    AutoRenew = request.AutoRenew,
                    PrivacyProtectAdminContact = request.PrivacyProtection,
                    PrivacyProtectRegistrantContact = request.PrivacyProtection,
                    PrivacyProtectTechContact = request.PrivacyProtection,
                    AdminContact = MapAwsContact(request.AdminContact ?? request.RegistrantContact),
                    RegistrantContact = MapAwsContact(request.RegistrantContact),
                    TechContact = MapAwsContact(request.TechContact ?? request.RegistrantContact)
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var operationId = result.GetProperty("OperationId").GetString();

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registration initiated via AWS Route 53",
                    OrderId = operationId,
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
                var endpoint = "/2013-04-01/domain/renew";
                var payload = new
                {
                    DomainName = request.DomainName,
                    DurationInYears = request.Years,
                    CurrentExpiryYear = DateTime.UtcNow.Year
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var operationId = result.GetProperty("OperationId").GetString();

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewal initiated via AWS Route 53",
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
                var endpoint = "/2013-04-01/domain/transfer";
                var payload = new
                {
                    DomainName = request.DomainName,
                    AuthCode = request.AuthCode,
                    AutoRenew = request.AutoRenew,
                    DurationInYears = 1,
                    PrivacyProtectAdminContact = request.PrivacyProtection,
                    PrivacyProtectRegistrantContact = request.PrivacyProtection,
                    PrivacyProtectTechContact = request.PrivacyProtection,
                    AdminContact = request.AdminContact != null ? MapAwsContact(request.AdminContact) : null,
                    RegistrantContact = request.RegistrantContact != null ? MapAwsContact(request.RegistrantContact) : null,
                    TechContact = request.TechContact != null ? MapAwsContact(request.TechContact) : null
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var operationId = result.GetProperty("OperationId").GetString();

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via AWS Route 53",
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
                var endpoint = $"/2013-04-01/hostedzone/{_hostedZoneId}/rrset";
                var response = await MakeAwsRequestAsync(HttpMethod.Get, endpoint);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var records = new List<DnsRecordModel>();
                var recordSets = result.GetProperty("ResourceRecordSets");

                int idCounter = 1;
                foreach (var recordSet in recordSets.EnumerateArray())
                {
                    var name = recordSet.GetProperty("Name").GetString()?.TrimEnd('.') ?? "";
                    var type = recordSet.GetProperty("Type").GetString() ?? "";
                    var ttl = recordSet.TryGetProperty("TTL", out var ttlProp) ? ttlProp.GetInt32() : 300;

                    var resourceRecords = recordSet.GetProperty("ResourceRecords");
                    foreach (var rr in resourceRecords.EnumerateArray())
                    {
                        var value = rr.GetProperty("Value").GetString() ?? "";
                        
                        records.Add(new DnsRecordModel
                        {
                            Id = idCounter++,
                            Name = name,
                            Type = type,
                            Value = value,
                            TTL = ttl,
                            Priority = null
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
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            try
            {
                // AWS Route 53 requires individual record updates via change batches
                var changes = new List<object>();

                foreach (var record in dnsZone.Records)
                {
                    changes.Add(new
                    {
                        Action = "UPSERT",
                        ResourceRecordSet = new
                        {
                            Name = record.Name,
                            Type = record.Type,
                            TTL = record.TTL,
                            ResourceRecords = new[] { new { Value = record.Value } }
                        }
                    });
                }

                var endpoint = $"/2013-04-01/hostedzone/{_hostedZoneId}/rrset/";
                var payload = new
                {
                    ChangeBatch = new
                    {
                        Changes = changes,
                        Comment = "Bulk DNS zone update"
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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
                var endpoint = $"/2013-04-01/hostedzone/{_hostedZoneId}/rrset/";
                var payload = new
                {
                    ChangeBatch = new
                    {
                        Changes = new[]
                        {
                            new
                            {
                                Action = "CREATE",
                                ResourceRecordSet = new
                                {
                                    Name = record.Name,
                                    Type = record.Type,
                                    TTL = record.TTL,
                                    ResourceRecords = new[] { new { Value = record.Value } }
                                }
                            }
                        },
                        Comment = "Add DNS record"
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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
                var endpoint = $"/2013-04-01/hostedzone/{_hostedZoneId}/rrset/";
                var payload = new
                {
                    ChangeBatch = new
                    {
                        Changes = new[]
                        {
                            new
                            {
                                Action = "UPSERT",
                                ResourceRecordSet = new
                                {
                                    Name = record.Name,
                                    Type = record.Type,
                                    TTL = record.TTL,
                                    ResourceRecords = new[] { new { Value = record.Value } }
                                }
                            }
                        },
                        Comment = "Update DNS record"
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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
                // Note: In real implementation, you'd need to fetch the record details first
                // This is a simplified version
                return new DnsUpdateResult
                {
                    Success = false,
                    DomainName = domainName,
                    Message = "Delete operation requires fetching record details first"
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
                var endpoint = $"/2013-04-01/domain/{domainName}";
                var response = await MakeAwsRequestAsync(HttpMethod.Get, endpoint);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var status = result.TryGetProperty("StatusList", out var statusList) && statusList.GetArrayLength() > 0
                    ? statusList[0].GetString() ?? "active"
                    : "active";

                DateTime? expirationDate = null;
                if (result.TryGetProperty("ExpirationDate", out var expDateProp))
                {
                    var timestamp = expDateProp.GetInt64();
                    expirationDate = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                }

                DateTime? registrationDate = null;
                if (result.TryGetProperty("CreationDate", out var regDateProp))
                {
                    var timestamp = regDateProp.GetInt64();
                    registrationDate = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                }

                var autoRenew = result.TryGetProperty("AutoRenew", out var autoRenewProp) && autoRenewProp.GetBoolean();

                var nameservers = new List<string>();
                if (result.TryGetProperty("Nameservers", out var nsArray))
                {
                    foreach (var ns in nsArray.EnumerateArray())
                    {
                        var nsName = ns.GetProperty("Name").GetString();
                        if (!string.IsNullOrEmpty(nsName))
                        {
                            nameservers.Add(nsName);
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
                var endpoint = $"/2013-04-01/domain/{domainName}/nameservers";
                var payload = new
                {
                    Nameservers = nameservers.Select(ns => new { Name = ns }).ToList()
                };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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
                var endpoint = $"/2013-04-01/domain/{domainName}/privacy";
                var payload = new { PrivacyProtection = enable };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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
                var endpoint = $"/2013-04-01/domain/{domainName}/autorenew";
                var payload = new { AutoRenew = enable };

                var json = JsonSerializer.Serialize(payload);
                var response = await MakeAwsRequestAsync(HttpMethod.Post, endpoint, json);

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

        private async Task<HttpResponseMessage> MakeAwsRequestAsync(HttpMethod method, string endpoint, string? jsonPayload = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            
            if (!string.IsNullOrEmpty(jsonPayload))
            {
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            // Add AWS Signature Version 4 authentication
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
            
            request.Headers.Add("X-Amz-Date", timestamp);
            request.Headers.Add("Host", $"route53.{_region}.amazonaws.com");

            // Generate AWS Signature V4
            var signature = GenerateAwsSignatureV4(method.Method, endpoint, timestamp, dateStamp, jsonPayload ?? "");
            request.Headers.Authorization = new AuthenticationHeaderValue("AWS4-HMAC-SHA256", signature);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            return response;
        }

        private string GenerateAwsSignatureV4(string httpMethod, string endpoint, string timestamp, string dateStamp, string payload)
        {
            // Simplified AWS Signature V4 - In production, use AWS SDK
            var credentialScope = $"{dateStamp}/{_region}/route53/aws4_request";
            
            var canonicalRequest = $"{httpMethod}\n{endpoint}\n\n" +
                                  $"host:route53.{_region}.amazonaws.com\n" +
                                  $"x-amz-date:{timestamp}\n\n" +
                                  $"host;x-amz-date\n" +
                                  ComputeSha256Hash(payload);

            var stringToSign = $"AWS4-HMAC-SHA256\n{timestamp}\n{credentialScope}\n{ComputeSha256Hash(canonicalRequest)}";
            
            var signingKey = GetSigningKey(dateStamp);
            var signature = ComputeHmacSha256(stringToSign, signingKey);

            return $"Credential={_accessKeyId}/{credentialScope}, SignedHeaders=host;x-amz-date, Signature={ToHex(signature)}";
        }

        private byte[] GetSigningKey(string dateStamp)
        {
            var kSecret = Encoding.UTF8.GetBytes($"AWS4{_secretAccessKey}");
            var kDate = ComputeHmacSha256(dateStamp, kSecret);
            var kRegion = ComputeHmacSha256(_region, kDate);
            var kService = ComputeHmacSha256("route53", kRegion);
            return ComputeHmacSha256("aws4_request", kService);
        }

        private byte[] ComputeHmacSha256(string data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private string ComputeSha256Hash(string data)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return ToHex(hashBytes);
        }

        private string ToHex(byte[] bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var endpoint = "/2013-04-01/domains/tlds";
                var response = await MakeAwsRequestAsync(HttpMethod.Get, endpoint);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                var tlds = new List<TldInfo>();
                if (result.TryGetProperty("Prices", out var pricesArray))
                {
                    foreach (var tld in pricesArray.EnumerateArray())
                    {
                        var name = tld.GetProperty("Name").GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            var tldInfo = new TldInfo
                            {
                                Name = name,
                                Currency = "USD"
                            };

                            if (tld.TryGetProperty("RegistrationPrice", out var regPrice))
                                tldInfo.RegistrationPrice = regPrice.GetDecimal();
                            if (tld.TryGetProperty("RenewalPrice", out var renewPrice))
                                tldInfo.RenewalPrice = renewPrice.GetDecimal();
                            if (tld.TryGetProperty("TransferPrice", out var transPrice))
                                tldInfo.TransferPrice = transPrice.GetDecimal();
                            if (tld.TryGetProperty("Type", out var typeProp))
                                tldInfo.Type = typeProp.GetString();

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

        private object MapAwsContact(ContactInformation contact)
        {
            return new
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                OrganizationName = contact.Organization ?? "",
                Email = contact.Email,
                PhoneNumber = contact.Phone,
                AddressLine1 = contact.Address1,
                AddressLine2 = contact.Address2 ?? "",
                City = contact.City,
                State = contact.State,
                ZipCode = contact.PostalCode,
                CountryCode = contact.Country
            };
        }
    }
}
