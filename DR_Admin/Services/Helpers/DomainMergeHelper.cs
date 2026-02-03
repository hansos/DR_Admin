using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
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
                
                var normalizedName = domainInfo.DomainName.ToLowerInvariant();
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

                // Find or create TLD
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

                // Find or create RegistrarTld relationship
                _log.Debug("Checking if RegistrarTld exists for registrar {RegistrarId} and TLD {TldId}", registrarId, tld.Id);
                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);

                if (registrarTld == null)
                {
                    _log.Information("RegistrarTld not found, creating new relationship for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, tldExtension);
                    
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
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

                // Find existing domain
                _log.Debug("Checking if domain {DomainName} exists in database (normalized: {NormalizedName})", 
                    domainInfo.DomainName, normalizedName);
                var domain = await _context.Domains
                    .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

                if (domain != null)
                {
                    // Update existing domain
                    _log.Debug("Domain {DomainName} found with ID {DomainId}, updating existing record", 
                        domainInfo.DomainName, domain.Id);
                    _log.Debug("Updating domain: Status={Status}, ExpirationDate={ExpirationDate}, AutoRenew={AutoRenew}, PrivacyProtection={PrivacyProtection}",
                        domainInfo.Status, domainInfo.ExpirationDate, domainInfo.AutoRenew, domainInfo.PrivacyProtection);
                    
                    domain.Status = domainInfo.Status ?? domain.Status;
                    domain.ExpirationDate = domainInfo.ExpirationDate ?? domain.ExpirationDate;
                    domain.AutoRenew = domainInfo.AutoRenew;
                    domain.PrivacyProtection = domainInfo.PrivacyProtection;
                    domain.RegistrarTldId = registrarTld.Id;
                    domain.RegistrarId = registrarId;
                    domain.UpdatedAt = DateTime.UtcNow;

                    // Merge contact information if available
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
                    
                    // Save changes immediately for this domain update
                    await _context.SaveChangesAsync();
                    result.DomainsUpdated++;
                }
                else
                {
                    _log.Debug("Domain {DomainName} not found in database, attempting to create new record", domainInfo.DomainName);
                    
                    // Try to find customer based on registrant email
                    int? customerId = null;
                    
                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                    {
                        _log.Debug("Processing {ContactCount} contacts to identify domain owner for {DomainName}", 
                            domainInfo.Contacts.Count, domainInfo.DomainName);
                        
                        // Look for registrant contact first, then admin contact
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
                                customerId = customer.Id;
                                _log.Information("Found customer {CustomerId} ({CustomerName}) for domain {DomainName} using email {Email}", 
                                    customerId, customer.Name, domainInfo.DomainName, ownerContact.Email);
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
                    }
                    else
                    {
                        var warning = $"No contacts available for domain {domainInfo.DomainName}, cannot identify customer";
                        _log.Debug(warning);
                        result.Warnings.Add(warning);
                    }

                    if (!customerId.HasValue)
                    {
                        var warning = $"Domain {domainInfo.DomainName} skipped: no customer could be identified";
                        _log.Debug(warning);
                        result.Warnings.Add(warning);
                        result.DomainsSkipped++;
                    }
                    else
                    {
                        try
                        {
                            // Validate required dates FIRST before creating anything
                            if (!domainInfo.ExpirationDate.HasValue || !domainInfo.RegistrationDate.HasValue)
                            {
                                var error = $"Domain {domainInfo.DomainName} missing required dates (Registration: {domainInfo.RegistrationDate}, Expiration: {domainInfo.ExpirationDate})";
                                _log.Error(error);
                                result.Errors.Add(error);
                                result.DomainsSkipped++;
                                continue;
                            }
                            
                            _log.Information("Creating new domain {DomainName} for customer {CustomerId}", 
                                domainInfo.DomainName, customerId);

                            // Double-check domain doesn't exist before creating service
                            var doubleCheckDomain = await _context.Domains
                                .AsNoTracking()
                                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);
                            
                            if (doubleCheckDomain != null)
                            {
                                _log.Warning("Domain {DomainName} found during double-check, updating instead of creating", domainInfo.DomainName);
                                
                                // Reload with tracking for update
                                var existingDomain = await _context.Domains
                                    .FirstOrDefaultAsync(d => d.Id == doubleCheckDomain.Id);
                                
                                if (existingDomain != null)
                                {
                                    existingDomain.Status = domainInfo.Status ?? existingDomain.Status;
                                    existingDomain.ExpirationDate = domainInfo.ExpirationDate ?? existingDomain.ExpirationDate;
                                    existingDomain.AutoRenew = domainInfo.AutoRenew;
                                    existingDomain.PrivacyProtection = domainInfo.PrivacyProtection;
                                    existingDomain.RegistrarTldId = registrarTld.Id;
                                    existingDomain.RegistrarId = registrarId;
                                    existingDomain.UpdatedAt = DateTime.UtcNow;
                                    
                                    // Merge contact information if available
                                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                                    {
                                        _log.Debug("Merging {ContactCount} contacts for existing domain {DomainId}", 
                                            domainInfo.Contacts.Count, existingDomain.Id);
                                        var contactStats = await MergeDomainContactsAsync(existingDomain.Id, domainInfo.Contacts);
                                        result.ContactsCreated += contactStats.Created;
                                        result.ContactsUpdated += contactStats.Updated;
                                    }
                                    
                                    await _context.SaveChangesAsync();
                                    result.DomainsUpdated++;
                                    _log.Information("Updated existing domain {DomainName} with ID {DomainId}", 
                                        domainInfo.DomainName, existingDomain.Id);
                                }
                                continue;
                            }

                            _log.Debug("Looking up DOMAIN service type");
                            var serviceType = await _serviceTypeService.GetServiceTypeByNameAsync("DOMAIN");
                            if (serviceType == null)
                            {
                                var error = "Service type 'DOMAIN' not found in database";
                                _log.Error(error);
                                result.Errors.Add(error);
                                result.DomainsSkipped++;
                                continue;
                            }
                            _log.Debug("Found service type ID: {ServiceTypeId}", serviceType.Id);
                            
                            _log.Debug("Getting default reseller company");
                            var resellerCompany = await _resellerCompanyService.GetDefaultResellerCompanyAsync();
                            _log.Debug("Default reseller company ID: {ResellerCompanyId}", resellerCompany?.Id ?? 0);
                            
                            var createServiceDto = new CreateServiceDto
                            {
                                ServiceTypeId = serviceType.Id,
                                Name = domainInfo.DomainName + " Domain registration",
                                ResellerCompanyId = resellerCompany?.Id,
                            };

                            _log.Debug("Creating service for domain {DomainName}", domainInfo.DomainName);
                            var service = await _serviceService.CreateServiceAsync(createServiceDto);
                            _log.Debug("Created service with ID {ServiceId}", service.Id);

                            var createDomainDto = new CreateDomainDto
                            {
                                ServiceId = service.Id,
                                CustomerId = customerId.Value,
                                ExpirationDate = domainInfo.ExpirationDate.Value,
                                RegistrationDate = domainInfo.RegistrationDate.Value,
                                ProviderId = registrarId,
                                Name = domainInfo.DomainName,
                                Status = "Imported",
                            };
                            
                            _log.Debug("Creating domain record: ServiceId={ServiceId}, CustomerId={CustomerId}, RegDate={RegistrationDate}, ExpDate={ExpirationDate}",
                                service.Id, customerId, domainInfo.RegistrationDate, domainInfo.ExpirationDate);
                            
                            var domainDto = await _domainService.CreateDomainAsync(createDomainDto);
                            _log.Information("Successfully created domain {DomainName} with ID {DomainId}", 
                                domainInfo.DomainName, domainDto.Id);
                            result.DomainsCreated++;

                            // Merge contact information if available
                            if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                            {
                                _log.Debug("Merging {ContactCount} contacts for domain {DomainId}", 
                                    domainInfo.Contacts.Count, domainDto.Id);
                                var contactStats = await MergeDomainContactsAsync(domainDto.Id, domainInfo.Contacts);
                                result.ContactsCreated += contactStats.Created;
                                result.ContactsUpdated += contactStats.Updated;
                                _log.Debug("Contact merge complete: {Created} created, {Updated} updated", 
                                    contactStats.Created, contactStats.Updated);
                            }
                            
                            // Save changes immediately after processing this domain
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var error = $"Failed to create/update domain {domainInfo.DomainName}: {ex.Message}";
                            _log.Error(ex, error);
                            result.Errors.Add(error);
                            result.DomainsSkipped++;
                        }
                    }
                }
            }

            _log.Information("Database merge completed for registrar {RegistrarId}:", registrarId);
            _log.Information("  - Domains processed: {Processed}", result.DomainsProcessed);
            _log.Information("  - Domains created: {Created}", result.DomainsCreated);
            _log.Information("  - Domains updated: {Updated}", result.DomainsUpdated);
            _log.Information("  - Domains skipped: {Skipped}", result.DomainsSkipped);
            _log.Information("  - TLDs created: {TldsCreated}", result.TldsCreated);
            _log.Information("  - RegistrarTlds created: {RegistrarTldsCreated}", result.RegistrarTldsCreated);
            _log.Information("  - Contacts created: {ContactsCreated}", result.ContactsCreated);
            _log.Information("  - Contacts updated: {ContactsUpdated}", result.ContactsUpdated);
            
            if (result.Errors.Any())
            {
                _log.Error("  - Errors encountered: {ErrorCount}", result.Errors.Count);
            }
            if (result.Warnings.Any())
            {
                _log.Warning("  - Warnings: {WarningCount}", result.Warnings.Count);
            }
            
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
