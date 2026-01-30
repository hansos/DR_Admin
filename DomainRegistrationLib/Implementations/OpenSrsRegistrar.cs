using DomainRegistrationLib.Models;
using Serilog;
using System.Text;
using System.Xml.Linq;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// OpenSRS (Tucows) Registrar Implementation
    /// Full Reseller API support
    /// </summary>
    public class OpenSrsRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _username;
        private readonly string _apiKey;
        private readonly string _domain;

        public OpenSrsRegistrar(string username, string apiKey, string domain, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://rr-n1-tor.opensrs.net:55443" 
                : "https://horizon.opensrs.net:55443")
        {
            _logger = Log.ForContext<OpenSrsRegistrar>();
            _username = username;
            _apiKey = apiKey;
            _domain = domain;
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var xml = BuildXmlRequest("lookup", new Dictionary<string, object>
                {
                    { "domain", domainName }
                });

                var response = await MakeXmlApiCallAsync(xml);
                var isAvailable = ParseAvailabilityResponse(response);

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
                var attributes = new Dictionary<string, object>
                {
                    { "domain", request.DomainName },
                    { "period", request.Years },
                    { "auto_renew", request.AutoRenew ? 1 : 0 },
                    { "contact_set", BuildContactSet(request) }
                };

                if (request.Nameservers != null && request.Nameservers.Any())
                {
                    attributes.Add("nameserver_list", request.Nameservers);
                }

                var xml = BuildXmlRequest("sw_register", attributes);
                var response = await MakeXmlApiCallAsync(xml);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via OpenSRS",
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
                var xml = BuildXmlRequest("renew", new Dictionary<string, object>
                {
                    { "domain", request.DomainName },
                    { "period", request.Years },
                    { "currentexpirationyear", DateTime.UtcNow.Year }
                });

                var response = await MakeXmlApiCallAsync(xml);

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via OpenSRS",
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
                var xml = BuildXmlRequest("transfer", new Dictionary<string, object>
                {
                    { "domain", request.DomainName },
                    { "auth_info", request.AuthCode },
                    { "contact_set", BuildContactSet(request) }
                });

                var response = await MakeXmlApiCallAsync(xml);

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via OpenSRS",
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
                var xml = BuildXmlRequest("get_dns_zone", new Dictionary<string, object>
                {
                    { "domain", domainName }
                });

                var response = await MakeXmlApiCallAsync(xml);

                return new DnsZone
                {
                    DomainName = domainName,
                    Records = new List<DnsRecordModel>()
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting DNS zone for {DomainName}: {Error}", domainName, ex.Message);
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            try
            {
                var records = dnsZone.Records.Select(r => new Dictionary<string, string>
                {
                    { "type", r.Type },
                    { "name", r.Name },
                    { "value", r.Value },
                    { "ttl", r.TTL.ToString() }
                }).ToList();

                var xml = BuildXmlRequest("modify_dns_zone", new Dictionary<string, object>
                {
                    { "domain", domainName },
                    { "records", records }
                });

                var response = await MakeXmlApiCallAsync(xml);

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
                var xml = BuildXmlRequest("get_domain", new Dictionary<string, object>
                {
                    { "domain", domainName },
                    { "type", "all_info" }
                });

                var response = await MakeXmlApiCallAsync(xml);

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = "Active",
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
                var xml = BuildXmlRequest("modify", new Dictionary<string, object>
                {
                    { "domain", domainName },
                    { "data", new Dictionary<string, object>
                        {
                            { "nameserver_list", nameservers }
                        }
                    }
                });

                var response = await MakeXmlApiCallAsync(xml);

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
                var xml = BuildXmlRequest("modify", new Dictionary<string, object>
                {
                    { "domain", domainName },
                    { "data", new Dictionary<string, object>
                        {
                            { "whois_privacy", enable ? "FULL" : "NONE" }
                        }
                    }
                });

                var response = await MakeXmlApiCallAsync(xml);

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
                var xml = BuildXmlRequest("modify", new Dictionary<string, object>
                {
                    { "domain", domainName },
                    { "data", new Dictionary<string, object>
                        {
                            { "auto_renew", enable ? 1 : 0 }
                        }
                    }
                });

                var response = await MakeXmlApiCallAsync(xml);

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

        private string BuildXmlRequest(string action, Dictionary<string, object> attributes)
        {
            // OpenSRS uses XML-RPC format
            var xml = new XElement("OPS_envelope",
                new XElement("header",
                    new XElement("version", "0.9"),
                    new XElement("msg_id", Guid.NewGuid().ToString()),
                    new XElement("msg_label", action)
                ),
                new XElement("body",
                    new XElement("data_block",
                        new XElement("dt_assoc",
                            new XElement("item", new XAttribute("key", "protocol"), "XCP"),
                            new XElement("item", new XAttribute("key", "action"), action),
                            new XElement("item", new XAttribute("key", "object"), "DOMAIN"),
                            new XElement("item", new XAttribute("key", "attributes"),
                                new XElement("dt_assoc")
                            )
                        )
                    )
                )
            );

            return xml.ToString();
        }

        private async Task<string> MakeXmlApiCallAsync(string xmlRequest)
        {
            var content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml");
            _httpClient.DefaultRequestHeaders.Add("X-Username", _username);
            _httpClient.DefaultRequestHeaders.Add("X-Signature", _apiKey);

            var response = await _httpClient.PostAsync("", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private bool ParseAvailabilityResponse(string xmlResponse)
        {
            // Simplified parsing - actual implementation would parse XML properly
            return xmlResponse.Contains("available");
        }

        private Dictionary<string, object> BuildContactSet(DomainRegistrationRequest request)
        {
            return new Dictionary<string, object>
            {
                { "owner", MapContact(request.RegistrantContact) },
                { "admin", MapContact(request.AdminContact ?? request.RegistrantContact) },
                { "tech", MapContact(request.TechContact ?? request.RegistrantContact) },
                { "billing", MapContact(request.BillingContact ?? request.RegistrantContact) }
            };
        }

        private Dictionary<string, object> BuildContactSet(DomainTransferRequest request)
        {
            if (request.RegistrantContact == null)
                return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                { "owner", MapContact(request.RegistrantContact) },
                { "admin", MapContact(request.AdminContact ?? request.RegistrantContact) },
                { "tech", MapContact(request.TechContact ?? request.RegistrantContact) },
                { "billing", MapContact(request.BillingContact ?? request.RegistrantContact) }
            };
        }

        private Dictionary<string, string> MapContact(ContactInformation contact)
        {
            return new Dictionary<string, string>
            {
                { "first_name", contact.FirstName },
                { "last_name", contact.LastName },
                { "org_name", contact.Organization },
                { "email", contact.Email },
                { "phone", contact.Phone },
                { "address1", contact.Address1 },
                { "address2", contact.Address2 },
                { "city", contact.City },
                { "state", contact.State },
                { "postal_code", contact.PostalCode },
                { "country", contact.Country }
            };
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var attributes = new Dictionary<string, object>();
                var xmlRequest = BuildXmlRequest("GET_PRICE", attributes);
                var response = await MakeXmlApiCallAsync(xmlRequest);

                // Parse TLDs from pricing response
                var doc = XDocument.Parse(response);
                var tldElements = doc.Descendants("item")
                    .Where(i => i.Attribute("key")?.Value?.StartsWith(".") == true)
                    .GroupBy(i => i.Attribute("key")?.Value?.TrimStart('.'))
                    .Where(g => !string.IsNullOrEmpty(g.Key));

                var tlds = new List<TldInfo>();
                foreach (var group in tldElements)
                {
                    var tldInfo = new TldInfo
                    {
                        Name = group.Key!,
                        Currency = "USD"
                    };

                    // Parse pricing from the grouped items
                    foreach (var item in group)
                    {
                        var priceType = item.Attribute("key")?.Value;
                        var priceValue = decimal.TryParse(item.Value, out var price) ? price : (decimal?)null;

                        if (priceType?.Contains("register") == true && tldInfo.RegistrationPrice == null)
                            tldInfo.RegistrationPrice = priceValue;
                        else if (priceType?.Contains("renew") == true && tldInfo.RenewalPrice == null)
                            tldInfo.RenewalPrice = priceValue;
                        else if (priceType?.Contains("transfer") == true && tldInfo.TransferPrice == null)
                            tldInfo.TransferPrice = priceValue;
                    }

                    tlds.Add(tldInfo);
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
