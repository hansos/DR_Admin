using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services.Helpers;

/// <summary>
/// Helper class for merging domain data from registrars into the database
/// </summary>
public class DomainMergeHelper
{
    private readonly ApplicationDbContext _context;
    private readonly ICustomerService _customerService;
    private readonly IServiceTypeService _serviceTypeService;
    private readonly IServiceService _serviceService;
    private readonly IResellerCompanyService _resellerCompanyService;
    private readonly IDomainService _domainService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainMergeHelper>();

    public DomainMergeHelper(
        ApplicationDbContext context,
        ICustomerService customerService,
        IServiceTypeService serviceTypeService,
        IServiceService serviceService,
        IResellerCompanyService resellerCompanyService,
        IDomainService domainService)
    {
        _context = context;
        _customerService = customerService;
        _serviceTypeService = serviceTypeService;
        _serviceService = serviceService;
        _resellerCompanyService = resellerCompanyService;
        _domainService = domainService;
    }

    /// <summary>
    /// Merges registered domains, their TLDs, and contact persons to the database
    /// </summary>
    public async Task<DomainMergeResult> MergeRegisteredDomainsToDatabase(int registrarId, List<RegisteredDomainInfo> domains)
    {
        var result = new DomainMergeResult();
        
        try
        {
            _log.Debug("Starting merge of {DomainCount} domains for registrar {RegistrarId}", domains.Count, registrarId);

            foreach (var domainInfo in domains)
            {
                result.DomainsProcessed++;
                _log.Debug("Processing domain {CurrentDomain}/{TotalDomains}: {DomainName}", 
                    result.DomainsProcessed, domains.Count, domainInfo.DomainName);
                
                var normalizedName = NormalizationHelper.Normalize(domainInfo.DomainName);
                _log.Debug("Domain {DomainName} normalized to {NormalizedName}", domainInfo.DomainName, normalizedName);
                
                // Extract TLD from domain name
                var tldExtension = ExtractTldFromDomain(domainInfo.DomainName);
                if (string.IsNullOrEmpty(tldExtension))
                {
                    var warning = $"Could not extract TLD from domain {domainInfo.DomainName}";
                    _log.Warning(warning);
                    result.Warnings.Add(warning);
                    result.DomainsSkipped++;
                    continue;
                }
                
                _log.Debug("Extracted TLD extension: {TldExtension} from domain {DomainName}", tldExtension, domainInfo.DomainName);

                // Ensure TLD exists
                var tld = await EnsureTldExistsAsync(tldExtension, result);
                if (tld == null)
                {
                    result.DomainsSkipped++;
                    continue;
                }

                // Ensure RegistrarTld relationship exists
                var registrarTld = await EnsureRegistrarTldExistsAsync(registrarId, tld.Id, tldExtension, result);
                if (registrarTld == null)
                {
                    result.DomainsSkipped++;
                    continue;
                }

                // Find existing domain
                _log.Debug("Checking if domain {DomainName} exists in database (normalized: {NormalizedName})", 
                    domainInfo.DomainName, normalizedName);
                var domain = await _context.RegisteredDomains
                    .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

                if (domain != null)
                {
                    await UpdateExistingDomainAsync(domain, domainInfo, registrarId, registrarTld.Id, result);
                }
                else
                {
                    await CreateNewDomainAsync(domainInfo, registrarId, registrarTld.Id, normalizedName!, result);
                }
            }

            LogMergeResults(registrarId, result);
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging registered domains to database for registrar {RegistrarId}", registrarId);
            result.Errors.Add($"Fatal error during merge: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// Ensures a TLD exists in the database, creating it if necessary
    /// </summary>
    private async Task<Tld?> EnsureTldExistsAsync(string tldExtension, DomainMergeResult result)
    {
        _log.Debug("Checking if TLD {TldExtension} exists in database", tldExtension);
        var tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == tldExtension);
        
        if (tld == null)
        {
            _log.Information("TLD {Extension} not found, creating new TLD record", tldExtension);
            tld = new Tld
            {
                Extension = tldExtension,
                Description = $"{tldExtension.ToUpper()} domain",
                IsActive = true,
                DefaultRegistrationYears = 1,
                MaxRegistrationYears = 10,
                RequiresPrivacy = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Tlds.Add(tld);
            await _context.SaveChangesAsync();
            _log.Debug("Created TLD {Extension} with ID {TldId}", tldExtension, tld.Id);
            result.TldsCreated++;
        }
        else
        {
            _log.Debug("TLD {Extension} already exists with ID {TldId}", tldExtension, tld.Id);
        }

        return tld;
    }

    /// <summary>
    /// Ensures a RegistrarTld relationship exists in the database, creating it if necessary
    /// </summary>
    private async Task<RegistrarTld?> EnsureRegistrarTldExistsAsync(int registrarId, int tldId, string tldExtension, DomainMergeResult result)
    {
        _log.Debug("Checking if RegistrarTld exists for registrar {RegistrarId} and TLD {TldId}", registrarId, tldId);
        var registrarTld = await _context.RegistrarTlds
            .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldId);

        if (registrarTld == null)
        {
            _log.Information("RegistrarTld not found, creating new relationship for registrar {RegistrarId} and TLD {TldExtension}", 
                registrarId, tldExtension);
            
            registrarTld = new RegistrarTld
            {
                RegistrarId = registrarId,
                TldId = tldId,
                RegistrationCost = 0,
                RegistrationPrice = 0,
                RenewalCost = 0,
                RenewalPrice = 0,
                TransferCost = 0,
                TransferPrice = 0,
                Currency = "USD",
                IsAvailable = true,
                AutoRenew = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();
            _log.Debug("Created RegistrarTld with ID {RegistrarTldId}", registrarTld.Id);
            result.RegistrarTldsCreated++;
        }
        else
        {
            _log.Debug("RegistrarTld already exists with ID {RegistrarTldId}", registrarTld.Id);
        }

        return registrarTld;
    }

    /// <summary>
    /// Updates an existing domain with information from the registrar
    /// </summary>
    private async Task UpdateExistingDomainAsync(ISPAdmin.Data.Entities.RegisteredDomain domain, RegisteredDomainInfo domainInfo, int registrarId, int registrarTldId, DomainMergeResult result)
    {
        _log.Debug("Domain {DomainName} found with ID {DomainId}, updating existing record", 
            domainInfo.DomainName, domain.Id);
        _log.Debug("Updating domain: Status={Status}, ExpirationDate={ExpirationDate}, AutoRenew={AutoRenew}, PrivacyProtection={PrivacyProtection}",
            domainInfo.Status, domainInfo.ExpirationDate, domainInfo.AutoRenew, domainInfo.PrivacyProtection);
        
        domain.Status = domainInfo.Status ?? domain.Status;
        domain.ExpirationDate = domainInfo.ExpirationDate ?? domain.ExpirationDate;
        domain.AutoRenew = domainInfo.AutoRenew;
        domain.PrivacyProtection = domainInfo.PrivacyProtection;
        domain.RegistrarTldId = registrarTldId;
        domain.RegistrarId = registrarId;
        domain.UpdatedAt = DateTime.UtcNow;

        if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
        {
            _log.Debug("Merging {ContactCount} contacts for existing domain {DomainId}", 
                domainInfo.Contacts.Count, domain.Id);
            var contactStats = await MergeDomainContactsAsync(domain.Id, domainInfo.Contacts);
            result.ContactsCreated += contactStats.Created;
            result.ContactsUpdated += contactStats.Updated;
            _log.Debug("Contact merge complete: {Created} created, {Updated} updated", 
                contactStats.Created, contactStats.Updated);
        }
        else
        {
            _log.Debug("No contacts to merge for domain {DomainId}", domain.Id);
        }

        if (domainInfo.Nameservers != null && domainInfo.Nameservers.Any())
        {
            _log.Debug("Merging {NameServerCount} name servers for existing domain {DomainId}", 
                domainInfo.Nameservers.Count, domain.Id);
            var nameServerStats = await MergeNameServersAsync(domain.Id, domainInfo.Nameservers);
            result.NameServersCreated += nameServerStats.Created;
            result.NameServersUpdated += nameServerStats.Updated;
            _log.Debug("Name server merge complete: {Created} created, {Updated} updated", 
                nameServerStats.Created, nameServerStats.Updated);
        }
        else
        {
            _log.Debug("No name servers to merge for domain {DomainId}", domain.Id);
        }
        
        await _context.SaveChangesAsync();
        result.DomainsUpdated++;
    }

    /// <summary>
    /// Creates a new domain in the database
    /// </summary>
    private async Task CreateNewDomainAsync(RegisteredDomainInfo domainInfo, int registrarId, int registrarTldId, string normalizedName, DomainMergeResult result)
    {
        _log.Debug("Domain {DomainName} not found in database, attempting to create new record", domainInfo.DomainName);
        
        var customerId = await FindCustomerIdFromContactsAsync(domainInfo, result);
        
        if (!customerId.HasValue)
        {
            var warning = $"Domain {domainInfo.DomainName} skipped: no customer could be identified";
            _log.Debug(warning);
            result.Warnings.Add(warning);
            result.DomainsSkipped++;
            return;
        }

        try
        {
            if (!ValidateRequiredDates(domainInfo, result))
            {
                result.DomainsSkipped++;
                return;
            }
            
            _log.Information("Creating new domain {DomainName} for customer {CustomerId}", 
                domainInfo.DomainName, customerId);

            // Double-check domain doesn't exist
            var doubleCheckDomain = await _context.RegisteredDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);
            
            if (doubleCheckDomain != null)
            {
                await HandleDuplicateDomainDuringCreation(doubleCheckDomain.Id, domainInfo, registrarId, registrarTldId, result);
                return;
            }

            var service = await CreateServiceForDomainAsync(domainInfo.DomainName, result);
            if (service == null)
            {
                result.DomainsSkipped++;
                return;
            }

            var domainDto = await CreateDomainRecordAsync(domainInfo, customerId.Value, registrarId, service.Id, result);
            if (domainDto != null)
            {
                await MergeContactsForNewDomainAsync(domainDto.Id, domainInfo, result);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var error = $"Failed to create/update domain {domainInfo.DomainName}: {ex.Message}";
            _log.Error(ex, error);
            result.Errors.Add(error);
            result.DomainsSkipped++;
        }
    }

    /// <summary>
    /// Finds customer ID from domain contact information
    /// </summary>
    private async Task<int?> FindCustomerIdFromContactsAsync(RegisteredDomainInfo domainInfo, DomainMergeResult result)
    {
        if (domainInfo.Contacts == null || !domainInfo.Contacts.Any())
        {
            var warning = $"No contacts available for domain {domainInfo.DomainName}, cannot identify customer";
            _log.Debug(warning);
            result.Warnings.Add(warning);
            return null;
        }

        _log.Debug("Processing {ContactCount} contacts to identify domain owner for {DomainName}", 
            domainInfo.Contacts.Count, domainInfo.DomainName);
        
        var registrantContact = domainInfo.Contacts
            .FirstOrDefault(c => c.ContactType.Equals("Registrant", StringComparison.OrdinalIgnoreCase));
        
        var ownerContact = registrantContact ?? 
            domainInfo.Contacts.FirstOrDefault(c => c.ContactType.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ??
            domainInfo.Contacts.FirstOrDefault();

        if (ownerContact != null)
        {
            _log.Debug("Selected {ContactType} contact as owner: {Email}", 
                ownerContact.ContactType, ownerContact.Email);
        }

        if (ownerContact != null && !string.IsNullOrWhiteSpace(ownerContact.Email))
        {
            _log.Debug("Looking up customer by email: {Email}", ownerContact.Email);
            var customer = await _customerService.GetCustomerByEmailAsync(ownerContact.Email);
            if (customer != null)
            {
                _log.Information("Found customer {CustomerId} ({CustomerName}) for domain {DomainName} using email {Email}", 
                    customer.Id, customer.Name, domainInfo.DomainName, ownerContact.Email);
                return customer.Id;
            }
            else
            {
                var warning = $"No customer found with email {ownerContact.Email} for domain {domainInfo.DomainName}";
                _log.Warning(warning);
                result.Warnings.Add(warning);
            }
        }
        else
        {
            var warning = $"No valid owner contact email found for domain {domainInfo.DomainName}";
            _log.Debug(warning);
            result.Warnings.Add(warning);
        }

        return null;
    }

    /// <summary>
    /// Validates that required dates are present for domain creation
    /// </summary>
    private bool ValidateRequiredDates(RegisteredDomainInfo domainInfo, DomainMergeResult result)
    {
        if (!domainInfo.ExpirationDate.HasValue || !domainInfo.RegistrationDate.HasValue)
        {
            var error = $"Domain {domainInfo.DomainName} missing required dates (Registration: {domainInfo.RegistrationDate}, Expiration: {domainInfo.ExpirationDate})";
            _log.Error(error);
            result.Errors.Add(error);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Handles the case where a domain is found during the double-check before creation
    /// </summary>
    private async Task HandleDuplicateDomainDuringCreation(int domainId, RegisteredDomainInfo domainInfo, int registrarId, int registrarTldId, DomainMergeResult result)
    {
        _log.Warning("Domain {DomainName} found during double-check, updating instead of creating", domainInfo.DomainName);
        
        var existingDomain = await _context.RegisteredDomains.FirstOrDefaultAsync(d => d.Id == domainId);
        
        if (existingDomain != null)
        {
            existingDomain.Status = domainInfo.Status ?? existingDomain.Status;
            existingDomain.ExpirationDate = domainInfo.ExpirationDate ?? existingDomain.ExpirationDate;
            existingDomain.AutoRenew = domainInfo.AutoRenew;
            existingDomain.PrivacyProtection = domainInfo.PrivacyProtection;
            existingDomain.RegistrarTldId = registrarTldId;
            existingDomain.RegistrarId = registrarId;
            existingDomain.UpdatedAt = DateTime.UtcNow;
            
            if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
            {
                _log.Debug("Merging {ContactCount} contacts for existing domain {DomainId}", 
                    domainInfo.Contacts.Count, existingDomain.Id);
                var contactStats = await MergeDomainContactsAsync(existingDomain.Id, domainInfo.Contacts);
                result.ContactsCreated += contactStats.Created;
                result.ContactsUpdated += contactStats.Updated;
            }

            if (domainInfo.Nameservers != null && domainInfo.Nameservers.Any())
            {
                _log.Debug("Merging {NameServerCount} name servers for existing domain {DomainId}", 
                    domainInfo.Nameservers.Count, existingDomain.Id);
                var nameServerStats = await MergeNameServersAsync(existingDomain.Id, domainInfo.Nameservers);
                result.NameServersCreated += nameServerStats.Created;
                result.NameServersUpdated += nameServerStats.Updated;
            }
            
            await _context.SaveChangesAsync();
            result.DomainsUpdated++;
            _log.Information("Updated existing domain {DomainName} with ID {DomainId}", 
                domainInfo.DomainName, existingDomain.Id);
        }
    }

    /// <summary>
    /// Creates a service record for a domain
    /// </summary>
    private async Task<ServiceDto?> CreateServiceForDomainAsync(string domainName, DomainMergeResult result)
    {
        _log.Debug("Looking up DOMAIN service type");
        var serviceType = await _serviceTypeService.GetServiceTypeByNameAsync("DOMAIN");
        if (serviceType == null)
        {
            var error = "Service type 'DOMAIN' not found in database";
            _log.Error(error);
            result.Errors.Add(error);
            return null;
        }
        _log.Debug("Found service type ID: {ServiceTypeId}", serviceType.Id);
        
        _log.Debug("Getting default reseller company");
        var resellerCompany = await _resellerCompanyService.GetDefaultResellerCompanyAsync();
        _log.Debug("Default reseller company ID: {ResellerCompanyId}", resellerCompany?.Id ?? 0);
        
        var createServiceDto = new CreateServiceDto
        {
            ServiceTypeId = serviceType.Id,
            Name = domainName + " Domain registration",
            ResellerCompanyId = resellerCompany?.Id,
        };

        _log.Debug("Creating service for domain {DomainName}", domainName);
        var service = await _serviceService.CreateServiceAsync(createServiceDto);
        _log.Debug("Created service with ID {ServiceId}", service.Id);
        
        return service;
    }

    /// <summary>
    /// Creates a domain record in the database
    /// </summary>
    private async Task<DomainDto?> CreateDomainRecordAsync(RegisteredDomainInfo domainInfo, int customerId, int registrarId, int serviceId, DomainMergeResult result)
    {
        var createDomainDto = new CreateDomainDto
        {
            ServiceId = serviceId,
            CustomerId = customerId,
            ExpirationDate = domainInfo.ExpirationDate!.Value,
            RegistrationDate = domainInfo.RegistrationDate!.Value,
            ProviderId = registrarId,
            Name = domainInfo.DomainName,
            Status = "Imported",
        };
        
        _log.Debug("Creating domain record: ServiceId={ServiceId}, CustomerId={CustomerId}, RegDate={RegistrationDate}, ExpDate={ExpirationDate}",
            serviceId, customerId, domainInfo.RegistrationDate, domainInfo.ExpirationDate);
        
        var domainDto = await _domainService.CreateDomainAsync(createDomainDto);
        _log.Information("Successfully created domain {DomainName} with ID {DomainId}", 
            domainInfo.DomainName, domainDto.Id);
        result.DomainsCreated++;
        
        return domainDto;
    }

    /// <summary>
    /// Merges contacts for a newly created domain
    /// </summary>
    private async Task MergeContactsForNewDomainAsync(int domainId, RegisteredDomainInfo domainInfo, DomainMergeResult result)
    {
        if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
        {
            _log.Debug("Merging {ContactCount} contacts for domain {DomainId}", 
                domainInfo.Contacts.Count, domainId);
            var contactStats = await MergeDomainContactsAsync(domainId, domainInfo.Contacts);
            result.ContactsCreated += contactStats.Created;
            result.ContactsUpdated += contactStats.Updated;
            _log.Debug("Contact merge complete: {Created} created, {Updated} updated", 
                contactStats.Created, contactStats.Updated);
        }

        if (domainInfo.Nameservers != null && domainInfo.Nameservers.Any())
        {
            _log.Debug("Merging {NameServerCount} name servers for domain {DomainId}", 
                domainInfo.Nameservers.Count, domainId);
            var nameServerStats = await MergeNameServersAsync(domainId, domainInfo.Nameservers);
            result.NameServersCreated += nameServerStats.Created;
            result.NameServersUpdated += nameServerStats.Updated;
            _log.Debug("Name server merge complete: {Created} created, {Updated} updated", 
                nameServerStats.Created, nameServerStats.Updated);
        }
    }

    /// <summary>
    /// Logs the final merge results
    /// </summary>
    private void LogMergeResults(int registrarId, DomainMergeResult result)
    {
        _log.Information("Database merge completed for registrar {RegistrarId}:", registrarId);
        _log.Information("  - Domains processed: {Processed}", result.DomainsProcessed);
        _log.Information("  - Domains created: {Created}", result.DomainsCreated);
        _log.Information("  - Domains updated: {Updated}", result.DomainsUpdated);
        _log.Information("  - Domains skipped: {Skipped}", result.DomainsSkipped);
        _log.Information("  - TLDs created: {TldsCreated}", result.TldsCreated);
        _log.Information("  - RegistrarTlds created: {RegistrarTldsCreated}", result.RegistrarTldsCreated);
        _log.Information("  - Contacts created: {ContactsCreated}", result.ContactsCreated);
        _log.Information("  - Contacts updated: {ContactsUpdated}", result.ContactsUpdated);
        _log.Information("  - NameServers created: {NameServersCreated}", result.NameServersCreated);
        _log.Information("  - NameServers updated: {NameServersUpdated}", result.NameServersUpdated);
        
        if (result.Errors.Any())
        {
            _log.Error("  - Errors encountered: {ErrorCount}", result.Errors.Count);
        }
        if (result.Warnings.Any())
        {
            _log.Warning("  - Warnings: {WarningCount}", result.Warnings.Count);
        }
    }

    /// <summary>
    /// Merges domain contact information from registrar into the DomainContacts table
    /// </summary>
    /// <param name="domainId">The domain ID to associate contacts with</param>
    /// <param name="contacts">List of contact information from the registrar</param>
    /// <returns>Statistics about created and updated contacts</returns>
    public async Task<(int Created, int Updated)> MergeDomainContactsAsync(int domainId, List<DomainRegistrationLib.Models.DomainContactInfo> contacts)
    {
        try
        {
            int created = 0;
            int updated = 0;

            _log.Debug("Merging {Count} contacts for domain ID {DomainId}", contacts.Count, domainId);

            foreach (var contactInfo in contacts)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(contactInfo.Email) || 
                    string.IsNullOrWhiteSpace(contactInfo.FirstName) || 
                    string.IsNullOrWhiteSpace(contactInfo.LastName))
                {
                    _log.Warning("Skipping contact for domain {DomainId} due to missing required fields (Email, FirstName, or LastName)", domainId);
                    continue;
                }

                // Try to find existing contact by type and email
                var existingContact = await _context.DomainContacts
                    .FirstOrDefaultAsync(dc => 
                        dc.DomainId == domainId && 
                        dc.ContactType == contactInfo.ContactType &&
                        dc.Email == contactInfo.Email);

                if (existingContact != null)
                {
                    // Update existing contact
                    _log.Debug("Updating existing {ContactType} contact for domain {DomainId}", contactInfo.ContactType, domainId);
                    
                    existingContact.FirstName = contactInfo.FirstName;
                    existingContact.LastName = contactInfo.LastName;
                    existingContact.Organization = contactInfo.Organization;
                    existingContact.Phone = contactInfo.Phone ?? existingContact.Phone;
                    existingContact.Fax = contactInfo.Fax;
                    existingContact.Address1 = contactInfo.Address1 ?? existingContact.Address1;
                    existingContact.Address2 = contactInfo.Address2;
                    existingContact.City = contactInfo.City ?? existingContact.City;
                    existingContact.State = contactInfo.State;
                    existingContact.PostalCode = contactInfo.PostalCode ?? existingContact.PostalCode;
                    existingContact.CountryCode = contactInfo.CountryCode ?? existingContact.CountryCode;
                    existingContact.IsActive = contactInfo.IsActive;
                    existingContact.Notes = contactInfo.Notes;
                    existingContact.UpdatedAt = DateTime.UtcNow;
                    
                    updated++;
                }
                else
                {
                    // Create new contact
                    _log.Debug("Creating new {ContactType} contact for domain {DomainId}", contactInfo.ContactType, domainId);
                    
                    var newContact = new DomainContact
                    {
                        DomainId = domainId,
                        ContactType = contactInfo.ContactType,
                        FirstName = contactInfo.FirstName,
                        LastName = contactInfo.LastName,
                        Organization = contactInfo.Organization,
                        Email = contactInfo.Email,
                        Phone = contactInfo.Phone ?? string.Empty,
                        Fax = contactInfo.Fax,
                        Address1 = contactInfo.Address1 ?? string.Empty,
                        Address2 = contactInfo.Address2,
                        City = contactInfo.City ?? string.Empty,
                        State = contactInfo.State,
                        PostalCode = contactInfo.PostalCode ?? string.Empty,
                        CountryCode = contactInfo.CountryCode ?? string.Empty,
                        IsActive = contactInfo.IsActive,
                        Notes = contactInfo.Notes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    _context.DomainContacts.Add(newContact);
                    created++;
                }
            }

            return (created, updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging domain contacts for domain ID {DomainId}", domainId);
            throw;
        }
    }

    /// <summary>
    /// Merges name server information from registrar into the NameServers table
    /// </summary>
    /// <param name="domainId">The domain ID to associate name servers with</param>
    /// <param name="nameservers">List of name server hostnames from the registrar</param>
    /// <returns>Statistics about created and updated name servers</returns>
    public async Task<(int Created, int Updated)> MergeNameServersAsync(int domainId, List<string> nameservers)
    {
        try
        {
            int created = 0;
            int updated = 0;

            _log.Debug("Merging {Count} name servers for domain ID {DomainId}", nameservers.Count, domainId);

            // Get existing name servers for this domain
            var existingNameServers = await _context.NameServers
                .Where(ns => ns.DomainId == domainId)
                .ToListAsync();

            // Process each nameserver from the registrar
            for (int i = 0; i < nameservers.Count; i++)
            {
                var hostname = nameservers[i];

                // Validate required fields
                if (string.IsNullOrWhiteSpace(hostname))
                {
                    _log.Warning("Skipping empty nameserver hostname for domain {DomainId}", domainId);
                    continue;
                }

                // Normalize hostname
                hostname = hostname.Trim().ToLowerInvariant();

                // Try to find existing nameserver by hostname
                var existingNameServer = existingNameServers
                    .FirstOrDefault(ns => ns.Hostname.Equals(hostname, StringComparison.OrdinalIgnoreCase));

                if (existingNameServer != null)
                {
                    // Update existing nameserver
                    _log.Debug("Updating existing nameserver {Hostname} for domain {DomainId}", hostname, domainId);
                    
                    // Update sort order and mark as primary if first in list
                    existingNameServer.SortOrder = i;
                    existingNameServer.IsPrimary = (i == 0);
                    existingNameServer.UpdatedAt = DateTime.UtcNow;
                    
                    updated++;
                }
                else
                {
                    // Create new nameserver
                    _log.Debug("Creating new nameserver {Hostname} for domain {DomainId}", hostname, domainId);
                    
                    var newNameServer = new NameServer
                    {
                        DomainId = domainId,
                        Hostname = hostname,
                        IpAddress = null, // IP address not provided by registrar list
                        IsPrimary = (i == 0), // First nameserver is primary
                        SortOrder = i,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    _context.NameServers.Add(newNameServer);
                    created++;
                }
            }

            // Remove nameservers that are no longer in the registrar list
            var nameserverHostnames = nameservers.Select(ns => ns.Trim().ToLowerInvariant()).ToList();
            var nameserversToRemove = existingNameServers
                .Where(ns => !nameserverHostnames.Contains(ns.Hostname.ToLowerInvariant()))
                .ToList();

            if (nameserversToRemove.Any())
            {
                _log.Debug("Removing {Count} obsolete nameservers for domain {DomainId}", 
                    nameserversToRemove.Count, domainId);
                _context.NameServers.RemoveRange(nameserversToRemove);
            }

            return (created, updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging nameservers for domain ID {DomainId}", domainId);
            throw;
        }
    }

    /// <summary>
    /// Extracts TLD extension from a domain name
    /// </summary>
    public static string ExtractTldFromDomain(string domainName)
    {
        if (string.IsNullOrEmpty(domainName))
            return string.Empty;

        var normalized = domainName.TrimEnd('.').ToLowerInvariant();
        var lastDotIndex = normalized.LastIndexOf('.');
        
        if (lastDotIndex <= 0 || lastDotIndex >= normalized.Length - 1)
            return string.Empty;

        return normalized.Substring(lastDotIndex + 1);
    }
}
