using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Route53Domains;
using Amazon.Route53Domains.Model;
using DomainRegistrationLib.Models;
using Serilog;
using ContactDetail = Amazon.Route53Domains.Model.ContactDetail;
using Nameserver = Amazon.Route53Domains.Model.Nameserver;
using ResourceRecord = Amazon.Route53.Model.ResourceRecord;
using ResourceRecordSet = Amazon.Route53.Model.ResourceRecordSet;

namespace DomainRegistrationLib.Implementations
{
    /// <summary>
    /// AWS Route 53 Registrar Implementation
    /// API Documentation: https://docs.aws.amazon.com/Route53/latest/APIReference/
    /// Uses AWS SDK for .NET (AWSSDK.Route53 and AWSSDK.Route53Domains)
    /// </summary>
    public class AwsRegistrar : BaseRegistrar
    {
        private readonly ILogger _logger;
        private readonly IAmazonRoute53 _route53Client;
        private readonly IAmazonRoute53Domains _route53DomainsClient;
        private readonly Dictionary<string, string> _hostedZoneCache = new();

        public AwsRegistrar(string accessKeyId, string secretAccessKey, string region)
            : base($"https://route53.{region}.amazonaws.com")
        {
            _logger = Log.ForContext<AwsRegistrar>();
            
            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretAccessKey);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
            
            _route53Client = new AmazonRoute53Client(credentials, regionEndpoint);
            _route53DomainsClient = new AmazonRoute53DomainsClient(credentials, Amazon.RegionEndpoint.USEast1); // Route53Domains is only available in us-east-1
        }

        public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
        {
            _logger.Information("Checking availability for domain: {DomainName}", domainName);
            try
            {
                var request = new CheckDomainAvailabilityRequest
                {
                    DomainName = domainName
                };

                var response = await _route53DomainsClient.CheckDomainAvailabilityAsync(request);
                var isAvailable = response.Availability == DomainAvailability.AVAILABLE;

                return new DomainAvailabilityResult
                {
                    Success = true,
                    DomainName = domainName,
                    IsAvailable = isAvailable,
                    Message = response.Availability.Value
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking availability for domain: {DomainName}", domainName);
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
            _logger.Information("Registering domain: {DomainName} for {Years} years", request.DomainName, request.Years);
            try
            {
                var registerRequest = new RegisterDomainRequest
                {
                    DomainName = request.DomainName,
                    DurationInYears = request.Years,
                    AutoRenew = request.AutoRenew,
                    PrivacyProtectAdminContact = request.PrivacyProtection,
                    PrivacyProtectRegistrantContact = request.PrivacyProtection,
                    PrivacyProtectTechContact = request.PrivacyProtection,
                    AdminContact = MapToAwsContact(request.AdminContact ?? request.RegistrantContact),
                    RegistrantContact = MapToAwsContact(request.RegistrantContact),
                    TechContact = MapToAwsContact(request.TechContact ?? request.RegistrantContact)
                };

                var response = await _route53DomainsClient.RegisterDomainAsync(registerRequest);

                return new DomainRegistrationResult
                {
                    Success = true,
                    DomainName = request.DomainName,
                    Message = "Domain registration initiated via AWS Route 53",
                    OrderId = response.OperationId,
                    RegistrationDate = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddYears(request.Years)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering domain: {DomainName}", request.DomainName);
                return CreateErrorResult($"Error registering domain: {ex.Message}");
            }
        }

        public override async Task<DomainRenewalResult> RenewDomainAsync(DomainRenewalRequest request)
        {
            _logger.Information("Renewing domain: {DomainName} for {Years} years", request.DomainName, request.Years);
            try
            {
                var renewRequest = new RenewDomainRequest
                {
                    DomainName = request.DomainName,
                    DurationInYears = request.Years,
                    CurrentExpiryYear = DateTime.UtcNow.Year
                };

                var response = await _route53DomainsClient.RenewDomainAsync(renewRequest);

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
                _logger.Error(ex, "Error renewing domain: {DomainName}", request.DomainName);
                return CreateRenewalErrorResult($"Error renewing domain: {ex.Message}");
            }
        }

        public override async Task<DomainTransferResult> TransferDomainAsync(DomainTransferRequest request)
        {
            _logger.Information("Transferring domain: {DomainName}", request.DomainName);
            try
            {
                var transferRequest = new TransferDomainRequest
                {
                    DomainName = request.DomainName,
                    AuthCode = request.AuthCode,
                    AutoRenew = request.AutoRenew,
                    DurationInYears = 1,
                    PrivacyProtectAdminContact = request.PrivacyProtection,
                    PrivacyProtectRegistrantContact = request.PrivacyProtection,
                    PrivacyProtectTechContact = request.PrivacyProtection
                };

                if (request.AdminContact != null)
                    transferRequest.AdminContact = MapToAwsContact(request.AdminContact);
                if (request.RegistrantContact != null)
                    transferRequest.RegistrantContact = MapToAwsContact(request.RegistrantContact);
                if (request.TechContact != null)
                    transferRequest.TechContact = MapToAwsContact(request.TechContact);

                var response = await _route53DomainsClient.TransferDomainAsync(transferRequest);

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
                _logger.Error(ex, "Error transferring domain: {DomainName}", request.DomainName);
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
            _logger.Information("Getting DNS zone for domain: {DomainName}", domainName);
            try
            {
                var hostedZoneId = await GetHostedZoneIdAsync(domainName);
                if (string.IsNullOrEmpty(hostedZoneId))
                {
                    _logger.Warning("No hosted zone found for domain: {DomainName}", domainName);
                    return new DnsZone { DomainName = domainName };
                }

                var request = new ListResourceRecordSetsRequest
                {
                    HostedZoneId = hostedZoneId
                };

                var response = await _route53Client.ListResourceRecordSetsAsync(request);
                var records = new List<DnsRecordModel>();

                int idCounter = 1;
                foreach (var recordSet in response.ResourceRecordSets)
                {
                    var name = recordSet.Name.TrimEnd('.');
                    var type = recordSet.Type.Value;
                    var ttl = (int)(recordSet.TTL ?? 300);

                    foreach (var rr in recordSet.ResourceRecords)
                    {
                        records.Add(new DnsRecordModel
                        {
                            Id = idCounter++,
                            Name = name,
                            Type = type,
                            Value = rr.Value,
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
                _logger.Error(ex, "Error retrieving DNS zone for domain: {DomainName}", domainName);
                return new DnsZone { DomainName = domainName };
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsZoneAsync(string domainName, DnsZone dnsZone)
        {
            _logger.Information("Updating DNS zone for domain: {DomainName} with {RecordCount} records", domainName, dnsZone.Records.Count);
            try
            {
                var hostedZoneId = await GetHostedZoneIdAsync(domainName);
                if (string.IsNullOrEmpty(hostedZoneId))
                {
                    return CreateDnsErrorResult($"No hosted zone found for domain: {domainName}");
                }

                var changes = new List<Change>();

                foreach (var record in dnsZone.Records)
                {
                    changes.Add(new Change
                    {
                        Action = ChangeAction.UPSERT,
                        ResourceRecordSet = new ResourceRecordSet
                        {
                            Name = record.Name,
                            Type = record.Type,
                            TTL = record.TTL,
                            ResourceRecords = new List<ResourceRecord>
                            {
                                new ResourceRecord { Value = record.Value }
                            }
                        }
                    });
                }

                var changeBatchRequest = new ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = hostedZoneId,
                    ChangeBatch = new ChangeBatch
                    {
                        Changes = changes,
                        Comment = "Bulk DNS zone update"
                    }
                };

                await _route53Client.ChangeResourceRecordSetsAsync(changeBatchRequest);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS zone updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating DNS zone for domain: {DomainName}", domainName);
                return CreateDnsErrorResult($"Error updating DNS zone: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> AddDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            _logger.Information("Adding DNS record {RecordName} ({RecordType}) for domain: {DomainName}", record.Name, record.Type, domainName);
            try
            {
                var hostedZoneId = await GetHostedZoneIdAsync(domainName);
                if (string.IsNullOrEmpty(hostedZoneId))
                {
                    return CreateDnsErrorResult($"No hosted zone found for domain: {domainName}");
                }

                var changeRequest = new ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = hostedZoneId,
                    ChangeBatch = new ChangeBatch
                    {
                        Changes = new List<Change>
                        {
                            new Change
                            {
                                Action = ChangeAction.CREATE,
                                ResourceRecordSet = new ResourceRecordSet
                                {
                                    Name = record.Name,
                                    Type = record.Type,
                                    TTL = record.TTL,
                                    ResourceRecords = new List<ResourceRecord>
                                    {
                                        new ResourceRecord { Value = record.Value }
                                    }
                                }
                            }
                        },
                        Comment = "Add DNS record"
                    }
                };

                await _route53Client.ChangeResourceRecordSetsAsync(changeRequest);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record added successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding DNS record {RecordName} ({RecordType}) for domain: {DomainName}", record.Name, record.Type, domainName);
                return CreateDnsErrorResult($"Error adding DNS record: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> UpdateDnsRecordAsync(string domainName, DnsRecordModel record)
        {
            _logger.Information("Updating DNS record {RecordName} ({RecordType}) for domain: {DomainName}", record.Name, record.Type, domainName);
            try
            {
                var hostedZoneId = await GetHostedZoneIdAsync(domainName);
                if (string.IsNullOrEmpty(hostedZoneId))
                {
                    return CreateDnsErrorResult($"No hosted zone found for domain: {domainName}");
                }

                var changeRequest = new ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = hostedZoneId,
                    ChangeBatch = new ChangeBatch
                    {
                        Changes = new List<Change>
                        {
                            new Change
                            {
                                Action = ChangeAction.UPSERT,
                                ResourceRecordSet = new ResourceRecordSet
                                {
                                    Name = record.Name,
                                    Type = record.Type,
                                    TTL = record.TTL,
                                    ResourceRecords = new List<ResourceRecord>
                                    {
                                        new ResourceRecord { Value = record.Value }
                                    }
                                }
                            }
                        },
                        Comment = "Update DNS record"
                    }
                };

                await _route53Client.ChangeResourceRecordSetsAsync(changeRequest);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating DNS record {RecordName} ({RecordType}) for domain: {DomainName}", record.Name, record.Type, domainName);
                return CreateDnsErrorResult($"Error updating DNS record: {ex.Message}");
            }
        }

        public override async Task<DnsUpdateResult> DeleteDnsRecordAsync(string domainName, int recordId)
        {
            _logger.Information("Deleting DNS record {RecordId} for domain: {DomainName}", recordId, domainName);
            try
            {
                var hostedZoneId = await GetHostedZoneIdAsync(domainName);
                if (string.IsNullOrEmpty(hostedZoneId))
                {
                    return CreateDnsErrorResult($"No hosted zone found for domain: {domainName}");
                }

                // First, fetch the current DNS zone to find the record
                var dnsZone = await GetDnsZoneAsync(domainName);
                var recordToDelete = dnsZone.Records.FirstOrDefault(r => r.Id == recordId);

                if (recordToDelete == null)
                {
                    return CreateDnsErrorResult($"Record with ID {recordId} not found");
                }

                var changeRequest = new ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = hostedZoneId,
                    ChangeBatch = new ChangeBatch
                    {
                        Changes = new List<Change>
                        {
                            new Change
                            {
                                Action = ChangeAction.DELETE,
                                ResourceRecordSet = new ResourceRecordSet
                                {
                                    Name = recordToDelete.Name,
                                    Type = recordToDelete.Type,
                                    TTL = recordToDelete.TTL,
                                    ResourceRecords = new List<ResourceRecord>
                                    {
                                        new ResourceRecord { Value = recordToDelete.Value }
                                    }
                                }
                            }
                        },
                        Comment = "Delete DNS record"
                    }
                };

                await _route53Client.ChangeResourceRecordSetsAsync(changeRequest);

                return new DnsUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "DNS record deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting DNS record {RecordId} for domain: {DomainName}", recordId, domainName);
                return CreateDnsErrorResult($"Error deleting DNS record: {ex.Message}");
            }
        }

        public override async Task<DomainInfoResult> GetDomainInfoAsync(string domainName)
        {
            _logger.Information("Getting domain info for: {DomainName}", domainName);
            try
            {
                var request = new GetDomainDetailRequest
                {
                    DomainName = domainName
                };

                var response = await _route53DomainsClient.GetDomainDetailAsync(request);

                var status = response.StatusList.Count > 0 ? response.StatusList[0] : "active";
                var nameservers = response.Nameservers?.Select(ns => ns.Name).ToList() ?? new List<string>();

                return new DomainInfoResult
                {
                    Success = true,
                    DomainName = domainName,
                    Status = status,
                    RegistrationDate = response.CreationDate,
                    ExpirationDate = response.ExpirationDate,
                    AutoRenew = response.AutoRenew ?? false,
                    Nameservers = nameservers,
                    Message = "Domain information retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting domain info for: {DomainName}", domainName);
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
            _logger.Information("Updating nameservers for domain: {DomainName} with {Count} nameservers", domainName, nameservers.Count);
            try
            {
                var request = new UpdateDomainNameserversRequest
                {
                    DomainName = domainName,
                    Nameservers = nameservers.Select(ns => new Nameserver { Name = ns }).ToList()
                };

                await _route53DomainsClient.UpdateDomainNameserversAsync(request);

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = "Nameservers updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating nameservers for domain: {DomainName}", domainName);
                return CreateUpdateErrorResult($"Error updating nameservers: {ex.Message}");
            }
        }

        public override async Task<DomainUpdateResult> SetPrivacyProtectionAsync(string domainName, bool enable)
        {
            _logger.Information("Setting privacy protection to {Enable} for domain: {DomainName}", enable, domainName);
            try
            {
                var request = new UpdateDomainContactPrivacyRequest
                {
                    DomainName = domainName,
                    AdminPrivacy = enable,
                    RegistrantPrivacy = enable,
                    TechPrivacy = enable
                };

                await _route53DomainsClient.UpdateDomainContactPrivacyAsync(request);

                return new DomainUpdateResult
                {
                    Success = true,
                    DomainName = domainName,
                    Message = $"Privacy protection {(enable ? "enabled" : "disabled")} successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error setting privacy protection for domain: {DomainName}", domainName);
                return CreateUpdateErrorResult($"Error setting privacy protection: {ex.Message}");
            }
        }

        public override async Task<DomainUpdateResult> SetAutoRenewAsync(string domainName, bool enable)
        {
            _logger.Information("Setting auto-renew to {Enable} for domain: {DomainName}", enable, domainName);
            try
            {
                if (enable)
                {
                    var request = new EnableDomainAutoRenewRequest
                    {
                        DomainName = domainName
                    };
                    await _route53DomainsClient.EnableDomainAutoRenewAsync(request);
                }
                else
                {
                    var request = new DisableDomainAutoRenewRequest
                    {
                        DomainName = domainName
                    };
                    await _route53DomainsClient.DisableDomainAutoRenewAsync(request);
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
                _logger.Error(ex, "Error setting auto-renew for domain: {DomainName}", domainName);
                return CreateUpdateErrorResult($"Error setting auto-renew: {ex.Message}");
            }
        }


        public override async Task<List<TldInfo>> GetSupportedTldsAsync()
        {
            _logger.Information("Getting supported TLDs from AWS Route 53");
            try
            {
                var request = new ListPricesRequest();
                var response = await _route53DomainsClient.ListPricesAsync(request);

                var tlds = new List<TldInfo>();
                foreach (var price in response.Prices)
                {
                    var tldInfo = new TldInfo
                    {
                        Name = price.Name,
                        Currency = "USD",
                        RegistrationPrice = (decimal?)(price.RegistrationPrice?.Price ?? 0),
                        RenewalPrice = (decimal?)(price.RenewalPrice?.Price ?? 0),
                        TransferPrice = (decimal?)(price.TransferPrice?.Price ?? 0)
                    };

                    tlds.Add(tldInfo);
                }

                return tlds;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting supported TLDs from AWS Route 53");
                return new List<TldInfo>();
            }
        }

        private async Task<string?> GetHostedZoneIdAsync(string domainName)
        {
            // Check cache first
            if (_hostedZoneCache.TryGetValue(domainName, out var cachedId))
            {
                return cachedId;
            }

            try
            {
                var request = new ListHostedZonesByNameRequest
                {
                    DNSName = domainName,
                    MaxItems = "1"
                };

                var response = await _route53Client.ListHostedZonesByNameAsync(request);
                
                if (response.HostedZones.Count > 0)
                {
                    var zone = response.HostedZones[0];
                    // Check if the zone name matches (AWS includes trailing dot)
                    if (zone.Name.TrimEnd('.').Equals(domainName, StringComparison.OrdinalIgnoreCase))
                    {
                        var hostedZoneId = zone.Id;
                        _hostedZoneCache[domainName] = hostedZoneId;
                        return hostedZoneId;
                    }
                }

                _logger.Warning("No hosted zone found for domain: {DomainName}", domainName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting hosted zone ID for domain: {DomainName}", domainName);
                return null;
            }
        }

        private ContactDetail MapToAwsContact(ContactInformation contact)
        {
            return new ContactDetail
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                OrganizationName = contact.Organization,
                Email = contact.Email,
                PhoneNumber = contact.Phone,
                AddressLine1 = contact.Address1,
                AddressLine2 = contact.Address2,
                City = contact.City,
                State = contact.State,
                ZipCode = contact.PostalCode,
                CountryCode = contact.Country switch
                {
                    "US" => Amazon.Route53Domains.CountryCode.US,
                    "CA" => Amazon.Route53Domains.CountryCode.CA,
                    "GB" => Amazon.Route53Domains.CountryCode.GB,
                    _ => Amazon.Route53Domains.CountryCode.FindValue(contact.Country)
                }
            };
        }
    }
}
