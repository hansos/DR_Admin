using DomainRegistrationLib.Models;
using System.Text;
using System.Xml.Linq;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// OXXA Domain API Registrar Implementation
    /// API Documentation: https://www.oxxa.com/api-documentation
    /// Uses XML-based API with username/password authentication
    /// </summary>
    public class OxxaRegistrar : BaseRegistrar
    {
        private readonly string _username;
        private readonly string _password;

        public OxxaRegistrar(string username, string password, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.oxxa.com" 
                : "https://api-ote.oxxa.com")
        {
            _username = username;
            _password = password;
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var xml = BuildCommand("CheckDomain", new XElement("domain", domainName));
                var response = await MakeApiCallAsync(xml);
                
                var resultElement = response.Descendants("result").FirstOrDefault();
                var isAvailable = resultElement?.Attribute("code")?.Value == "210";
                
                var message = resultElement?.Element("msg")?.Value ?? "Unknown status";
                
                decimal? price = null;
                var priceElement = resultElement?.Element("price");
                if (priceElement != null && decimal.TryParse(priceElement.Value, out var priceValue))
                {
                    price = priceValue;
                }

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    PremiumPrice = price,
                    Message = message
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
                var commandData = new XElement("domain", request.DomainName);
                commandData.Add(new XElement("period", request.Years));
                
                // Add nameservers
                if (request.Nameservers != null && request.Nameservers.Any())
                {
                    var nsElement = new XElement("nameservers");
                    foreach (var ns in request.Nameservers)
                    {
                        nsElement.Add(new XElement("ns", ns));
                    }
                    commandData.Add(nsElement);
                }

                // Add contacts
                commandData.Add(BuildContactElement("registrant", request.RegistrantContact));
                commandData.Add(BuildContactElement("admin", request.AdminContact ?? request.RegistrantContact));
                commandData.Add(BuildContactElement("tech", request.TechContact ?? request.RegistrantContact));
                commandData.Add(BuildContactElement("billing", request.BillingContact ?? request.RegistrantContact));

                // Add auto-renew and privacy settings
                commandData.Add(new XElement("autorenew", request.AutoRenew ? "1" : "0"));
                commandData.Add(new XElement("privacy", request.PrivacyProtection ? "1" : "0"));

                var xml = BuildCommand("CreateDomain", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200" || code == "201";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Registration failed";
                    return CreateErrorResult(errorMsg, code);
                }

                var orderId = resultElement?.Element("order-id")?.Value;
                var expiryDate = resultElement?.Element("expiry-date")?.Value;

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via OXXA",
                    OrderId = orderId,
                    RegistrationDate = DateTime.UtcNow,
                    ExpirationDate = !string.IsNullOrEmpty(expiryDate) 
                        ? DateTime.Parse(expiryDate) 
                        : DateTime.UtcNow.AddYears(request.Years)
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
                var commandData = new XElement("domain", request.DomainName);
                commandData.Add(new XElement("period", request.Years));
                commandData.Add(new XElement("currentexpirationyear", DateTime.UtcNow.Year));

                var xml = BuildCommand("RenewDomain", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Renewal failed";
                    return CreateRenewalErrorResult(errorMsg, code);
                }

                var expiryDate = resultElement?.Element("expiry-date")?.Value;

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via OXXA",
                    NewExpirationDate = !string.IsNullOrEmpty(expiryDate) 
                        ? DateTime.Parse(expiryDate) 
                        : DateTime.UtcNow.AddYears(request.Years)
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
                var commandData = new XElement("domain", request.DomainName);
                commandData.Add(new XElement("authcode", request.AuthCode));

                if (request.RegistrantContact != null)
                {
                    commandData.Add(BuildContactElement("registrant", request.RegistrantContact));
                    commandData.Add(BuildContactElement("admin", request.AdminContact ?? request.RegistrantContact));
                    commandData.Add(BuildContactElement("tech", request.TechContact ?? request.RegistrantContact));
                    commandData.Add(BuildContactElement("billing", request.BillingContact ?? request.RegistrantContact));
                }

                commandData.Add(new XElement("autorenew", request.AutoRenew ? "1" : "0"));
                commandData.Add(new XElement("privacy", request.PrivacyProtection ? "1" : "0"));

                var xml = BuildCommand("TransferDomain", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200" || code == "201";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Transfer failed";
                    return new DomainTransferResult
                    {
                        Success = false,
                        Message = errorMsg,
                        ErrorCode = code,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var status = resultElement?.Element("status")?.Value ?? "Pending";

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via OXXA",
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
                var commandData = new XElement("domain", domainName);
                var xml = BuildCommand("QueryDNS", commandData);
                var response = await MakeApiCallAsync(xml);

                var records = new List<DnsRecordModel>();
                var recordElements = response.Descendants("record");

                foreach (var recordElement in recordElements)
                {
                    var record = new DnsRecordModel
                    {
                        Id = int.TryParse(recordElement.Attribute("id")?.Value, out var id) ? id : 0,
                        Name = recordElement.Element("name")?.Value ?? "",
                        Type = recordElement.Element("type")?.Value ?? "",
                        Value = recordElement.Element("content")?.Value ?? "",
                        TTL = int.TryParse(recordElement.Element("ttl")?.Value, out var ttl) ? ttl : 3600,
                        Priority = int.TryParse(recordElement.Element("priority")?.Value, out var priority) ? priority : (int?)null
                    };
                    records.Add(record);
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
                var commandData = new XElement("domain", domainName);
                var recordsElement = new XElement("records");

                foreach (var record in dnsZone.Records)
                {
                    var recordElement = new XElement("record",
                        new XElement("name", record.Name),
                        new XElement("type", record.Type),
                        new XElement("content", record.Value),
                        new XElement("ttl", record.TTL)
                    );

                    if (record.Priority.HasValue)
                    {
                        recordElement.Add(new XElement("priority", record.Priority.Value));
                    }

                    recordsElement.Add(recordElement);
                }

                commandData.Add(recordsElement);
                var xml = BuildCommand("ModifyDNS", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "DNS update failed";
                    return CreateDnsErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                var recordElement = new XElement("record",
                    new XElement("name", record.Name),
                    new XElement("type", record.Type),
                    new XElement("content", record.Value),
                    new XElement("ttl", record.TTL)
                );

                if (record.Priority.HasValue)
                {
                    recordElement.Add(new XElement("priority", record.Priority.Value));
                }

                commandData.Add(recordElement);
                var xml = BuildCommand("AddDNSRecord", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Add DNS record failed";
                    return CreateDnsErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                var recordElement = new XElement("record",
                    new XAttribute("id", record.Id),
                    new XElement("name", record.Name),
                    new XElement("type", record.Type),
                    new XElement("content", record.Value),
                    new XElement("ttl", record.TTL)
                );

                if (record.Priority.HasValue)
                {
                    recordElement.Add(new XElement("priority", record.Priority.Value));
                }

                commandData.Add(recordElement);
                var xml = BuildCommand("ModifyDNSRecord", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Update DNS record failed";
                    return CreateDnsErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                commandData.Add(new XElement("record-id", recordId));

                var xml = BuildCommand("DeleteDNSRecord", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Delete DNS record failed";
                    return CreateDnsErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                var xml = BuildCommand("QueryDomain", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Query failed";
                    return new DomainInfoResult
                    {
                        Success = false,
                        Message = errorMsg,
                        ErrorCode = code,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var domainElement = resultElement?.Element("domain");
                var status = domainElement?.Element("status")?.Value ?? "active";
                
                DateTime? registrationDate = null;
                if (DateTime.TryParse(domainElement?.Element("create-date")?.Value, out var regDate))
                {
                    registrationDate = regDate;
                }

                DateTime? expirationDate = null;
                if (DateTime.TryParse(domainElement?.Element("expiry-date")?.Value, out var expDate))
                {
                    expirationDate = expDate;
                }

                var autoRenew = domainElement?.Element("autorenew")?.Value == "1";
                var privacy = domainElement?.Element("privacy")?.Value == "1";

                var nameservers = domainElement?.Element("nameservers")?.Elements("ns")
                    .Select(ns => ns.Value)
                    .ToList() ?? new List<string>();

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
                var commandData = new XElement("domain", domainName);
                var nsElement = new XElement("nameservers");
                
                foreach (var ns in nameservers)
                {
                    nsElement.Add(new XElement("ns", ns));
                }
                
                commandData.Add(nsElement);
                var xml = BuildCommand("ModifyNS", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Nameserver update failed";
                    return CreateUpdateErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                commandData.Add(new XElement("privacy", enable ? "1" : "0"));

                var xml = BuildCommand("ModifyPrivacy", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Privacy update failed";
                    return CreateUpdateErrorResult(errorMsg, code);
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
                var commandData = new XElement("domain", domainName);
                commandData.Add(new XElement("autorenew", enable ? "1" : "0"));

                var xml = BuildCommand("ModifyAutoRenew", commandData);
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    var errorMsg = resultElement?.Element("msg")?.Value ?? "Auto-renew update failed";
                    return CreateUpdateErrorResult(errorMsg, code);
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

        private XDocument BuildCommand(string command, XElement data)
        {
            return new XDocument(
                new XElement("request",
                    new XElement("auth",
                        new XElement("username", _username),
                        new XElement("password", _password)
                    ),
                    new XElement("command",
                        new XAttribute("name", command),
                        data
                    )
                )
            );
        }

        private async Task<XDocument> MakeApiCallAsync(XDocument request)
        {
            var content = new StringContent(request.ToString(), Encoding.UTF8, "text/xml");
            
            var response = await _httpClient.PostAsync("/command.php", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(responseContent);
        }

        private XElement BuildContactElement(string type, ContactInformation contact)
        {
            return new XElement(type,
                new XElement("firstname", contact.FirstName),
                new XElement("lastname", contact.LastName),
                new XElement("organization", contact.Organization ?? ""),
                new XElement("email", contact.Email),
                new XElement("phone", contact.Phone),
                new XElement("address1", contact.Address1),
                new XElement("address2", contact.Address2 ?? ""),
                new XElement("city", contact.City),
                new XElement("state", contact.State),
                new XElement("postalcode", contact.PostalCode),
                new XElement("country", contact.Country)
            );
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var xml = BuildCommand("GetTldList", new XElement("request"));
                var response = await MakeApiCallAsync(xml);

                var resultElement = response.Descendants("result").FirstOrDefault();
                var code = resultElement?.Attribute("code")?.Value;
                var success = code == "200";

                if (!success)
                {
                    return [];
                }

                var tlds = resultElement?.Descendants("tld")
                    .Select(t => 
                    {
                        var name = t.Element("name")?.Value;
                        if (string.IsNullOrEmpty(name)) return null;

                        return new TldInfo
                        {
                            Name = name,
                            Currency = t.Element("currency")?.Value ?? "EUR",
                            RegistrationPrice = decimal.TryParse(t.Element("register-price")?.Value, out var regPrice) ? regPrice : null,
                            RenewalPrice = decimal.TryParse(t.Element("renew-price")?.Value, out var renewPrice) ? renewPrice : null,
                            TransferPrice = decimal.TryParse(t.Element("transfer-price")?.Value, out var transPrice) ? transPrice : null,
                            MinRegistrationYears = int.TryParse(t.Element("min-period")?.Value, out var minYears) ? minYears : null,
                            MaxRegistrationYears = int.TryParse(t.Element("max-period")?.Value, out var maxYears) ? maxYears : null,
                            Type = t.Element("type")?.Value,
                            SupportsPrivacy = t.Element("privacy")?.Value == "1",
                            SupportsDnssec = t.Element("dnssec")?.Value == "1"
                        };
                    })
                    .Where(t => t != null)
                    .Cast<TldInfo>()
                    .ToList() ?? [];

                return tlds;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
