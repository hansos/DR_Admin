using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// Domain Name API Reseller Program Implementation
    /// API Documentation: https://www.domainnameapi.com/reseller-api
    /// Uses XML-based SOAP API with username/password authentication
    /// </summary>
    public class DomainNameApiRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _username;
        private readonly string _password;

        public DomainNameApiRegistrar(string username, string password, bool useLiveEnvironment)
            : base(useLiveEnvironment 
                ? "https://api.domainnameapi.com" 
                : "https://api-test.domainnameapi.com")
        {
            _logger = Log.ForContext<DomainNameApiRegistrar>();
            _username = username;
            _password = password;

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/xml"));
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var requestXml = BuildSoapRequest("CheckAvailability", 
                    new XElement("DomainName", domainName));

                var response = await MakeApiCallAsync(requestXml);
                
                var availabilityElement = response.Descendants("Available").FirstOrDefault();
                var isAvailable = availabilityElement?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
                
                var statusElement = response.Descendants("Status").FirstOrDefault();
                var status = statusElement?.Value ?? "Unknown";

                decimal? price = null;
                var priceElement = response.Descendants("Price").FirstOrDefault();
                if (priceElement != null && decimal.TryParse(priceElement.Value, out var priceValue))
                {
                    price = priceValue;
                }

                var isPremium = response.Descendants("IsPremium").FirstOrDefault()?.Value
                    .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    PremiumPrice = isPremium ? price : null,
                    Message = status
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", request.DomainName),
                    new XElement("Period", request.Years),
                    new XElement("AutoRenew", request.AutoRenew ? "true" : "false"),
                    new XElement("PrivacyProtection", request.PrivacyProtection ? "true" : "false")
                );

                // Add nameservers
                if (request.Nameservers != null && request.Nameservers.Any())
                {
                    var nsElement = new XElement("NameServers");
                    foreach (var ns in request.Nameservers)
                    {
                        nsElement.Add(new XElement("NameServer", ns));
                    }
                    domainData.Add(nsElement);
                }

                // Add contacts
                var contactsElement = new XElement("Contacts",
                    BuildContactElement("Registrant", request.RegistrantContact),
                    BuildContactElement("Administrative", request.AdminContact ?? request.RegistrantContact),
                    BuildContactElement("Technical", request.TechContact ?? request.RegistrantContact),
                    BuildContactElement("Billing", request.BillingContact ?? request.RegistrantContact)
                );
                domainData.Add(contactsElement);

                var requestXml = BuildSoapRequest("RegisterDomain", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Registration failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateErrorResult(errorMsg, errorCode);
                }

                var orderId = response.Descendants("OrderId").FirstOrDefault()?.Value;
                var expiryDateStr = response.Descendants("ExpirationDate").FirstOrDefault()?.Value;
                
                DateTime? expiryDate = null;
                if (!string.IsNullOrEmpty(expiryDateStr) && DateTime.TryParse(expiryDateStr, out var expDate))
                {
                    expiryDate = expDate;
                }

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registered successfully via Domain Name API",
                    OrderId = orderId,
                    RegistrationDate = DateTime.UtcNow,
                    ExpirationDate = expiryDate ?? DateTime.UtcNow.AddYears(request.Years)
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", request.DomainName),
                    new XElement("Period", request.Years)
                );

                var requestXml = BuildSoapRequest("RenewDomain", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Renewal failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateRenewalErrorResult(errorMsg, errorCode);
                }

                var expiryDateStr = response.Descendants("ExpirationDate").FirstOrDefault()?.Value;
                
                DateTime? expiryDate = null;
                if (!string.IsNullOrEmpty(expiryDateStr) && DateTime.TryParse(expiryDateStr, out var expDate))
                {
                    expiryDate = expDate;
                }

                return new DomainRenewalResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain renewed successfully via Domain Name API",
                    NewExpirationDate = expiryDate ?? DateTime.UtcNow.AddYears(request.Years)
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", request.DomainName),
                    new XElement("AuthCode", request.AuthCode),
                    new XElement("AutoRenew", request.AutoRenew ? "true" : "false"),
                    new XElement("PrivacyProtection", request.PrivacyProtection ? "true" : "false")
                );

                if (request.RegistrantContact != null)
                {
                    var contactsElement = new XElement("Contacts",
                        BuildContactElement("Registrant", request.RegistrantContact),
                        BuildContactElement("Administrative", request.AdminContact ?? request.RegistrantContact),
                        BuildContactElement("Technical", request.TechContact ?? request.RegistrantContact),
                        BuildContactElement("Billing", request.BillingContact ?? request.RegistrantContact)
                    );
                    domainData.Add(contactsElement);
                }

                var requestXml = BuildSoapRequest("TransferDomain", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Transfer failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return new DomainTransferResult
                    {
                        Success = false,
                        Message = errorMsg,
                        ErrorCode = errorCode,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var status = response.Descendants("TransferStatus").FirstOrDefault()?.Value ?? "Pending";

                return new DomainTransferResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain transfer initiated via Domain Name API",
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var requestXml = BuildSoapRequest("GetDnsRecords", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var records = new List<DnsRecordModel>();
                var recordElements = response.Descendants("DnsRecord");

                foreach (var recordElement in recordElements)
                {
                    var record = new DnsRecordModel
                    {
                        Id = int.TryParse(recordElement.Element("Id")?.Value, out var id) ? id : 0,
                        Name = recordElement.Element("Name")?.Value ?? "",
                        Type = recordElement.Element("Type")?.Value ?? "",
                        Value = recordElement.Element("Content")?.Value ?? "",
                        TTL = int.TryParse(recordElement.Element("TTL")?.Value, out var ttl) ? ttl : 3600,
                        Priority = int.TryParse(recordElement.Element("Priority")?.Value, out var priority) ? priority : (int?)null
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
                _logger.Error("Error getting DNS zone for {DomainName}: {Error}", domainName, ex.Message);
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            try
            {
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var recordsElement = new XElement("DnsRecords");
                foreach (var record in dnsZone.Records)
                {
                    var recordElement = new XElement("DnsRecord",
                        new XElement("Name", record.Name),
                        new XElement("Type", record.Type),
                        new XElement("Content", record.Value),
                        new XElement("TTL", record.TTL)
                    );

                    if (record.Priority.HasValue)
                    {
                        recordElement.Add(new XElement("Priority", record.Priority.Value));
                    }

                    recordsElement.Add(recordElement);
                }
                domainData.Add(recordsElement);

                var requestXml = BuildSoapRequest("ModifyDnsRecords", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "DNS update failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateDnsErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var recordElement = new XElement("DnsRecord",
                    new XElement("Name", record.Name),
                    new XElement("Type", record.Type),
                    new XElement("Content", record.Value),
                    new XElement("TTL", record.TTL)
                );

                if (record.Priority.HasValue)
                {
                    recordElement.Add(new XElement("Priority", record.Priority.Value));
                }

                domainData.Add(recordElement);

                var requestXml = BuildSoapRequest("AddDnsRecord", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Add DNS record failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateDnsErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var recordElement = new XElement("DnsRecord",
                    new XElement("Id", record.Id),
                    new XElement("Name", record.Name),
                    new XElement("Type", record.Type),
                    new XElement("Content", record.Value),
                    new XElement("TTL", record.TTL)
                );

                if (record.Priority.HasValue)
                {
                    recordElement.Add(new XElement("Priority", record.Priority.Value));
                }

                domainData.Add(recordElement);

                var requestXml = BuildSoapRequest("UpdateDnsRecord", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Update DNS record failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateDnsErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName),
                    new XElement("RecordId", recordId)
                );

                var requestXml = BuildSoapRequest("DeleteDnsRecord", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Delete DNS record failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateDnsErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var requestXml = BuildSoapRequest("GetDomainInfo", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Query failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return new DomainInfoResult
                    {
                        Success = false,
                        Message = errorMsg,
                        ErrorCode = errorCode,
                        Errors = new List<string> { errorMsg }
                    };
                }

                var domainElement = response.Descendants("DomainInfo").FirstOrDefault();
                var status = domainElement?.Element("Status")?.Value ?? "active";
                
                DateTime? registrationDate = null;
                var regDateStr = domainElement?.Element("CreationDate")?.Value;
                if (!string.IsNullOrEmpty(regDateStr) && DateTime.TryParse(regDateStr, out var regDate))
                {
                    registrationDate = regDate;
                }

                DateTime? expirationDate = null;
                var expDateStr = domainElement?.Element("ExpirationDate")?.Value;
                if (!string.IsNullOrEmpty(expDateStr) && DateTime.TryParse(expDateStr, out var expDate))
                {
                    expirationDate = expDate;
                }

                var autoRenew = domainElement?.Element("AutoRenew")?.Value
                    .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
                var privacy = domainElement?.Element("PrivacyProtection")?.Value
                    .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

                var nameservers = domainElement?.Element("NameServers")?.Elements("NameServer")
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName)
                );

                var nsElement = new XElement("NameServers");
                foreach (var ns in nameservers)
                {
                    nsElement.Add(new XElement("NameServer", ns));
                }
                domainData.Add(nsElement);

                var requestXml = BuildSoapRequest("ModifyNameServers", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Nameserver update failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateUpdateErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName),
                    new XElement("PrivacyProtection", enable ? "true" : "false")
                );

                var requestXml = BuildSoapRequest("ModifyPrivacyProtection", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Privacy update failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateUpdateErrorResult(errorMsg, errorCode);
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
                var domainData = new XElement("Domain",
                    new XElement("DomainName", domainName),
                    new XElement("AutoRenew", enable ? "true" : "false")
                );

                var requestXml = BuildSoapRequest("ModifyAutoRenew", domainData);
                var response = await MakeApiCallAsync(requestXml);

                var resultElement = response.Descendants("OperationResult").FirstOrDefault();
                var success = resultElement?.Descendants("Status").FirstOrDefault()?.Value
                    .Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ?? false;

                if (!success)
                {
                    var errorMsg = resultElement?.Descendants("Message").FirstOrDefault()?.Value 
                        ?? "Auto-renew update failed";
                    var errorCode = resultElement?.Descendants("Code").FirstOrDefault()?.Value;
                    return CreateUpdateErrorResult(errorMsg, errorCode);
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

        private XDocument BuildSoapRequest(string action, XElement data)
        {
            var soapNs = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
            var apiNs = XNamespace.Get("http://www.domainnameapi.com/");

            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(soapNs + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soap", soapNs),
                    new XAttribute(XNamespace.Xmlns + "api", apiNs),
                    new XElement(soapNs + "Header",
                        new XElement(apiNs + "Authentication",
                            new XElement(apiNs + "Username", _username),
                            new XElement(apiNs + "Password", _password)
                        )
                    ),
                    new XElement(soapNs + "Body",
                        new XElement(apiNs + action,
                            data
                        )
                    )
                )
            );
        }

        private async Task<XDocument> MakeApiCallAsync(XDocument request)
        {
            var content = new StringContent(request.ToString(), Encoding.UTF8, "text/xml");
            content.Headers.ContentType = new MediaTypeHeaderValue("text/xml")
            {
                CharSet = "utf-8"
            };

            var response = await _httpClient.PostAsync("/service", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(responseContent);
        }

        private XElement BuildContactElement(string type, ContactInformation contact)
        {
            return new XElement(type,
                new XElement("FirstName", contact.FirstName),
                new XElement("LastName", contact.LastName),
                new XElement("Organization", contact.Organization ?? ""),
                new XElement("Email", contact.Email),
                new XElement("Phone", contact.Phone),
                new XElement("AddressLine1", contact.Address1),
                new XElement("AddressLine2", contact.Address2 ?? ""),
                new XElement("City", contact.City),
                new XElement("State", contact.State),
                new XElement("PostalCode", contact.PostalCode),
                new XElement("Country", contact.Country)
            );
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var requestXml = BuildSoapRequest("GetTldList", new XElement("Request"));
                var response = await MakeApiCallAsync(requestXml);

                var tlds = response.Descendants("Tld")
                    .Select(t => 
                    {
                        var name = t.Element("Name")?.Value;
                        if (string.IsNullOrEmpty(name)) return null;

                        return new TldInfo
                        {
                            Name = name,
                            Currency = t.Element("Currency")?.Value ?? "USD",
                            RegistrationPrice = decimal.TryParse(t.Element("RegistrationPrice")?.Value, out var regPrice) ? regPrice : null,
                            RenewalPrice = decimal.TryParse(t.Element("RenewalPrice")?.Value, out var renewPrice) ? renewPrice : null,
                            TransferPrice = decimal.TryParse(t.Element("TransferPrice")?.Value, out var transPrice) ? transPrice : null,
                            MinRegistrationYears = int.TryParse(t.Element("MinPeriod")?.Value, out var minYears) ? minYears : null,
                            MaxRegistrationYears = int.TryParse(t.Element("MaxPeriod")?.Value, out var maxYears) ? maxYears : null,
                            Type = t.Element("Type")?.Value,
                            SupportsPrivacy = t.Element("PrivacyAvailable")?.Value == "true",
                            SupportsDnssec = t.Element("DnssecSupported")?.Value == "true"
                        };
                    })
                    .Where(t => t != null)
                    .Cast<TldInfo>()
                    .ToList();

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
                _logger.Information("Getting registered domains from DomainNameApi");

                var requestXml = BuildSoapRequest("GetList", new XElement("Empty"));
                var response = await MakeApiCallAsync(requestXml);

                var domains = new List<RegisteredDomainInfo>();

                var domainElements = response.Descendants("Domain");
                foreach (var domain in domainElements)
                {
                    var domainInfo = new RegisteredDomainInfo
                    {
                        DomainName = domain.Element("Name")?.Value ?? "",
                        Status = domain.Element("Status")?.Value,
                        ExpirationDate = DateTime.TryParse(domain.Element("ExpirationDate")?.Value, out var expires) 
                            ? expires 
                            : null,
                        RegistrationDate = DateTime.TryParse(domain.Element("RegistrationDate")?.Value, out var created) 
                            ? created 
                            : null,
                        AutoRenew = domain.Element("RenewalMode")?.Value == "AutoRenew",
                        Locked = domain.Element("LockStatus")?.Value == "true",
                        PrivacyProtection = domain.Element("PrivacyProtectionStatus")?.Value == "enabled"
                    };

                    var nsElements = domain.Descendants("Nameserver");
                    if (nsElements.Any())
                    {
                        domainInfo.Nameservers = nsElements
                            .Select(ns => ns.Value)
                            .Where(ns => !string.IsNullOrEmpty(ns))
                            .ToList();
                    }

                    domains.Add(domainInfo);
                }

                _logger.Information("Successfully retrieved {Count} domains from DomainNameApi", domains.Count);

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
                _logger.Error(ex, "Error getting registered domains from DomainNameApi");
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
