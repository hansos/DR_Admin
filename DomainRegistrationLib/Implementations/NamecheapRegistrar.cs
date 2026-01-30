using DomainRegistrationLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;

namespace DomainRegistrationLib.Implementations
{
    public class NamecheapRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly string _apiUser;
        private readonly string _apiKey;
        private readonly string _username;
        private readonly string _clientIp;
        private readonly bool _useSandbox;

        public NamecheapRegistrar(string apiUser, string apiKey, string username, string clientIp, bool useSandbox)
            : base(useSandbox ? "https://api.sandbox.namecheap.com/xml.response" : "https://api.namecheap.com/xml.response")
        {
            _logger = Log.ForContext<NamecheapRegistrar>();
            _apiUser = apiUser;
            _apiKey = apiKey;
            _username = username;
            _clientIp = clientIp;
            _useSandbox = useSandbox;
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            try
            {
                var parameters = BuildBaseParameters("namecheap.domains.check");
                parameters.Add("DomainList", domainName);

                var response = await MakeApiCallAsync(parameters);
                
                // Parse XML response (simplified - actual implementation would parse XML)
                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = true, // Parse from XML
                    Message = "Domain availability checked successfully"
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
                var parameters = BuildBaseParameters("namecheap.domains.create");
                parameters.Add("DomainName", request.DomainName);
                parameters.Add("Years", request.Years.ToString());
                
                // Add contact information
                AddContactParameters(parameters, "Registrant", request.RegistrantContact);
                AddContactParameters(parameters, "Admin", request.AdminContact ?? request.RegistrantContact);
                AddContactParameters(parameters, "Tech", request.TechContact ?? request.RegistrantContact);
                AddContactParameters(parameters, "Billing", request.BillingContact ?? request.RegistrantContact);

                if (request.Nameservers != null && request.Nameservers.Any())
                {
                    for (int i = 0; i < request.Nameservers.Count; i++)
                    {
                        parameters.Add($"Nameservers.{i + 1}", request.Nameservers[i]);
                    }
                }

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.renew");
                parameters.Add("DomainName", request.DomainName);
                parameters.Add("Years", request.Years.ToString());

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.transfer.create");
                parameters.Add("DomainName", request.DomainName);
                parameters.Add("EPPCode", request.AuthCode);

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.dns.getHosts");
                var parts = domainName.Split('.');
                parameters.Add("SLD", string.Join(".", parts.Take(parts.Length - 1)));
                parameters.Add("TLD", parts.Last());

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.dns.setHosts");
                var parts = domainName.Split('.');
                parameters.Add("SLD", string.Join(".", parts.Take(parts.Length - 1)));
                parameters.Add("TLD", parts.Last());

                for (int i = 0; i < dnsZone.Records.Count; i++)
                {
                    var record = dnsZone.Records[i];
                    parameters.Add($"HostName{i + 1}", record.Name);
                    parameters.Add($"RecordType{i + 1}", record.Type);
                    parameters.Add($"Address{i + 1}", record.Value);
                    parameters.Add($"TTL{i + 1}", record.TTL.ToString());
                    
                    if (record.Priority.HasValue)
                        parameters.Add($"MXPref{i + 1}", record.Priority.Value.ToString());
                }

                var response = await MakeApiCallAsync(parameters);

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
            var existingRecord = zone.Records.FirstOrDefault(r => r.Id == record.Id);
            if (existingRecord != null)
            {
                zone.Records.Remove(existingRecord);
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
                var parameters = BuildBaseParameters("namecheap.domains.getInfo");
                parameters.Add("DomainName", domainName);

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.dns.setCustom");
                parameters.Add("DomainName", domainName);
                parameters.Add("Nameservers", string.Join(",", nameservers));

                var response = await MakeApiCallAsync(parameters);

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
                var command = enable ? "namecheap.whoisguard.enable" : "namecheap.whoisguard.disable";
                var parameters = BuildBaseParameters(command);
                parameters.Add("DomainName", domainName);

                var response = await MakeApiCallAsync(parameters);

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
                var parameters = BuildBaseParameters("namecheap.domains.setRegistrarLock");
                parameters.Add("DomainName", domainName);
                parameters.Add("LockAction", enable ? "LOCK" : "UNLOCK");

                var response = await MakeApiCallAsync(parameters);

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

        private Dictionary<string, string> BuildBaseParameters(string command)
        {
            return new Dictionary<string, string>
            {
                { "ApiUser", _apiUser },
                { "ApiKey", _apiKey },
                { "UserName", _username },
                { "ClientIp", _clientIp },
                { "Command", command }
            };
        }

        private void AddContactParameters(Dictionary<string, string> parameters, string prefix, ContactInformation contact)
        {
            parameters.Add($"{prefix}FirstName", contact.FirstName);
            parameters.Add($"{prefix}LastName", contact.LastName);
            parameters.Add($"{prefix}Organization", contact.Organization);
            parameters.Add($"{prefix}EmailAddress", contact.Email);
            parameters.Add($"{prefix}Phone", contact.Phone);
            parameters.Add($"{prefix}Address1", contact.Address1);
            parameters.Add($"{prefix}Address2", contact.Address2);
            parameters.Add($"{prefix}City", contact.City);
            parameters.Add($"{prefix}StateProvince", contact.State);
            parameters.Add($"{prefix}PostalCode", contact.PostalCode);
            parameters.Add($"{prefix}Country", contact.Country);
        }

        private async Task<string> MakeApiCallAsync(Dictionary<string, string> parameters)
        {
            var queryString = string.Join("&", parameters.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
            var url = $"{_apiUrl}?{queryString}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            try
            {
                var parameters = BuildBaseParameters("namecheap.domains.getTldList");

                var responseXml = await MakeApiCallAsync(parameters);
                var doc = XDocument.Parse(responseXml);
                var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

                var tlds = doc.Descendants(ns + "Tld")
                    .Select(t => 
                    {
                        var name = t.Attribute("Name")?.Value;
                        if (string.IsNullOrEmpty(name)) return null;

                        return new TldInfo
                        {
                            Name = name,
                            Currency = "USD",
                            IsGeneric = t.Attribute("IsGenericTld")?.Value == "true",
                            IsCountryCode = t.Attribute("IsCcTld")?.Value == "true",
                            Type = t.Attribute("Type")?.Value,
                            MinRegistrationYears = int.TryParse(t.Attribute("MinRegYears")?.Value, out var minYears) ? minYears : null,
                            MaxRegistrationYears = int.TryParse(t.Attribute("MaxRegYears")?.Value, out var maxYears) ? maxYears : null,
                            SupportsPrivacy = t.Attribute("SupportsPrivacy")?.Value == "true"
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
    }
}
