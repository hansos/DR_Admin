using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing domain registration operations through registrars
/// </summary>
public class DomainManagerService : IDomainManagerService
{
    private readonly ApplicationDbContext _context;
    private readonly DomainRegistrarFactory _domainRegistrarFactory;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainManagerService>();

    public DomainManagerService(
        ApplicationDbContext context,
        DomainRegistrarFactory domainRegistrarFactory)
    {
        _context = context;
        _domainRegistrarFactory = domainRegistrarFactory;
    }

    /// <summary>
    /// Registers a domain with the specified registrar
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="registeredDomainId">The ID of the registered domain in the database</param>
    /// <returns>Domain registration result from the registrar</returns>
    public async Task<DomainRegistrationResult> RegisterDomainAsync(string registrarCode, int registeredDomainId)
    {
        try
        {
            _log.Information("Registering domain {DomainId} with registrar {RegistrarCode}", 
                registeredDomainId, registrarCode);

            // Get the registered domain from database
            var registeredDomain = await _context.RegisteredDomains
                .Include(d => d.Registrar)
                .Include(d => d.Customer)
                .Include(d => d.DomainContacts)
                .Include(d => d.NameServers)
                .FirstOrDefaultAsync(d => d.Id == registeredDomainId);

            if (registeredDomain == null)
            {
                _log.Warning("Registered domain with ID {DomainId} not found", registeredDomainId);
                throw new InvalidOperationException($"Registered domain with ID {registeredDomainId} not found");
            }

            // Verify the registrar code matches the domain's registrar
            if (!string.Equals(registeredDomain.Registrar.Code, registrarCode, StringComparison.OrdinalIgnoreCase))
            {
                _log.Warning("Registrar code mismatch. Expected {ExpectedCode}, got {ActualCode}", 
                    registeredDomain.Registrar.Code, registrarCode);
                throw new InvalidOperationException(
                    $"Registrar code mismatch. Domain is configured for registrar '{registeredDomain.Registrar.Code}', but '{registrarCode}' was specified");
            }

            // Build DomainRegistrationRequest from RegisteredDomain entity
            var request = new DomainRegistrationRequest
            {
                DomainName = registeredDomain.Name,
                Years = CalculateYears(registeredDomain),
                PrivacyProtection = registeredDomain.PrivacyProtection,
                AutoRenew = registeredDomain.AutoRenew,
                Nameservers = registeredDomain.NameServers
                    .OrderBy(ns => ns.SortOrder)
                    .Select(ns => ns.Hostname)
                    .ToList(),
                RegistrantContact = GetContactInformation(registeredDomain, "Registrant"),
                AdminContact = GetContactInformation(registeredDomain, "Administrative"),
                TechContact = GetContactInformation(registeredDomain, "Technical"),
                BillingContact = GetContactInformation(registeredDomain, "Billing")
            };

            // Create registrar instance and register domain
            var registrar = _domainRegistrarFactory.CreateRegistrar(registrarCode);
            var result = await registrar.RegisterDomainAsync(request);

            _log.Information("Domain registration {Status} for {DomainName}. Message: {Message}", 
                result.Success ? "succeeded" : "failed", 
                registeredDomain.Name, 
                result.Message);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while registering domain {DomainId} with registrar {RegistrarCode}", 
                registeredDomainId, registrarCode);
            throw;
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration based on registered domain ID
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="registeredDomainId">The ID of the registered domain in the database</param>
    /// <returns>Domain availability result from the registrar</returns>
    public async Task<DomainAvailabilityResult> CheckDomainAvailabilityByIdAsync(string registrarCode, int registeredDomainId)
    {
        try
        {
            _log.Information("Checking domain availability for domain {DomainId} with registrar {RegistrarCode}", 
                registeredDomainId, registrarCode);

            // Get the registered domain from database
            var registeredDomain = await _context.RegisteredDomains
                .Include(d => d.Registrar)
                .FirstOrDefaultAsync(d => d.Id == registeredDomainId);

            if (registeredDomain == null)
            {
                _log.Warning("Registered domain with ID {DomainId} not found", registeredDomainId);
                throw new InvalidOperationException($"Registered domain with ID {registeredDomainId} not found");
            }

            // Verify the registrar code matches the domain's registrar
            if (!string.Equals(registeredDomain.Registrar.Code, registrarCode, StringComparison.OrdinalIgnoreCase))
            {
                _log.Warning("Registrar code mismatch. Expected {ExpectedCode}, got {ActualCode}", 
                    registeredDomain.Registrar.Code, registrarCode);
                throw new InvalidOperationException(
                    $"Registrar code mismatch. Domain is configured for registrar '{registeredDomain.Registrar.Code}', but '{registrarCode}' was specified");
            }

            // Create registrar instance and check availability
            var registrar = _domainRegistrarFactory.CreateRegistrar(registrarCode);
            var result = await registrar.CheckAvailabilityAsync(registeredDomain.Name);

            _log.Information("Domain availability check completed for {DomainName}. Available: {Available}", 
                registeredDomain.Name, result.IsAvailable);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking domain availability for domain {DomainId} with registrar {RegistrarCode}", 
                registeredDomainId, registrarCode);
            throw;
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration based on domain name
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="domainName">The domain name to check</param>
    /// <returns>Domain availability result from the registrar</returns>
    public async Task<DomainAvailabilityResult> CheckDomainAvailabilityByNameAsync(string registrarCode, string domainName)
    {
        try
        {
            _log.Information("Checking domain availability for {DomainName} with registrar {RegistrarCode}", 
                domainName, registrarCode);

            // Verify the registrar exists in database
            var registrar = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Code.ToLower() == registrarCode.ToLower());

            if (registrar == null)
            {
                _log.Warning("Registrar with code {RegistrarCode} not found", registrarCode);
                throw new InvalidOperationException($"Registrar with code '{registrarCode}' not found");
            }

            if (!registrar.IsActive)
            {
                _log.Warning("Registrar {RegistrarCode} is not active", registrarCode);
                throw new InvalidOperationException($"Registrar '{registrarCode}' is not active");
            }

            // Create registrar instance and check availability
            var domainRegistrar = _domainRegistrarFactory.CreateRegistrar(registrarCode);
            var result = await domainRegistrar.CheckAvailabilityAsync(domainName);

            _log.Information("Domain availability check completed for {DomainName}. Available: {Available}", 
                domainName, result.IsAvailable);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking domain availability for {DomainName} with registrar {RegistrarCode}", 
                domainName, registrarCode);
            throw;
        }
    }

    private int CalculateYears(Data.Entities.RegisteredDomain domain)
    {
        // If expiration date is in the future, calculate years from now to expiration
        // Otherwise default to 1 year
        if (domain.ExpirationDate > DateTime.UtcNow)
        {
            var years = (domain.ExpirationDate - DateTime.UtcNow).TotalDays / 365.25;
            return Math.Max(1, (int)Math.Ceiling(years));
        }
        
        return 1;
    }

    private ContactInformation? GetContactInformation(Data.Entities.RegisteredDomain domain, string contactType)
    {
        // Parse string to enum
        if (!Enum.TryParse<ContactRoleType>(contactType, true, out var roleType))
        {
            return null;
        }

        var contact = domain.DomainContacts.FirstOrDefault(c => c.RoleType == roleType);

        if (contact == null)
            return null;

        return new ContactInformation
        {
            ContactType = string.IsNullOrWhiteSpace(contact.Organization) ? "PERSON" : "COMPANY",
            FirstName = contact.FirstName ?? string.Empty,
            LastName = contact.LastName ?? string.Empty,
            Organization = contact.Organization ?? string.Empty,
            Email = contact.Email ?? string.Empty,
            Phone = contact.Phone ?? string.Empty,
            Address1 = contact.Address1 ?? string.Empty,
            Address2 = contact.Address2 ?? string.Empty,
            City = contact.City ?? string.Empty,
            State = contact.State ?? string.Empty,
            PostalCode = contact.PostalCode ?? string.Empty,
            Country = contact.CountryCode ?? string.Empty
        };
    }

    /// <inheritdoc/>
    public async Task<DnsBulkSyncResult> SyncDnsRecordsForAllDomainsAsync(string registrarCode)
    {
        var bulk = new DnsBulkSyncResult();

        try
        {
            _log.Information("Starting bulk DNS record sync for registrar {RegistrarCode}", registrarCode);

            var registrarEntity = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Code.ToLower() == registrarCode.ToLower());

            if (registrarEntity == null)
                throw new InvalidOperationException($"Registrar with code '{registrarCode}' not found");

            if (!registrarEntity.IsActive)
                throw new InvalidOperationException($"Registrar '{registrarCode}' is not active");

            var domains = await _context.RegisteredDomains
                .Where(d => d.RegistrarId == registrarEntity.Id)
                .ToListAsync();

            _log.Information("Found {DomainCount} domains for registrar {RegistrarCode}", domains.Count, registrarCode);

            var registrarClient = _domainRegistrarFactory.CreateRegistrar(registrarCode);

            foreach (var domain in domains)
            {
                bulk.DomainsProcessed++;
                var result = await SyncDnsRecordsForDomainAsync(registrarClient, domain);
                bulk.DomainResults.Add(result);

                if (result.Success)
                    bulk.DomainsSucceeded++;
                else
                    bulk.DomainsFailed++;

                bulk.TotalCreated += result.Created;
                bulk.TotalUpdated += result.Updated;
                bulk.TotalSkipped += result.Skipped;
            }

            bulk.Success = bulk.DomainsFailed == 0;
            bulk.Message = $"Processed {bulk.DomainsProcessed} domain(s): {bulk.DomainsSucceeded} succeeded, {bulk.DomainsFailed} failed. " +
                           $"Records: {bulk.TotalCreated} created, {bulk.TotalUpdated} updated, {bulk.TotalSkipped} skipped.";

            _log.Information("Bulk DNS sync complete for registrar {RegistrarCode}. {Message}", registrarCode, bulk.Message);
            return bulk;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during bulk DNS record sync for registrar {RegistrarCode}", registrarCode);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordSyncResult> SyncDnsRecordsByDomainNameAsync(string registrarCode, string domainName)
    {
        try
        {
            _log.Information("Syncing DNS records for domain {DomainName} via registrar {RegistrarCode}", domainName, registrarCode);

            var registrarEntity = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Code.ToLower() == registrarCode.ToLower());

            if (registrarEntity == null)
                throw new InvalidOperationException($"Registrar with code '{registrarCode}' not found");

            if (!registrarEntity.IsActive)
                throw new InvalidOperationException($"Registrar '{registrarCode}' is not active");

            var normalizedName = NormalizationHelper.Normalize(domainName);
            var domain = await _context.RegisteredDomains
                .Include(d => d.Registrar)
                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

            if (domain == null)
                throw new InvalidOperationException($"Domain '{domainName}' not found");

            if (!string.Equals(domain.Registrar.Code, registrarCode, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Domain '{domainName}' is assigned to registrar '{domain.Registrar.Code}', not '{registrarCode}'");

            var registrarClient = _domainRegistrarFactory.CreateRegistrar(registrarCode);
            return await SyncDnsRecordsForDomainAsync(registrarClient, domain);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error syncing DNS records for domain {DomainName} via registrar {RegistrarCode}", domainName, registrarCode);
            return new DnsRecordSyncResult
            {
                DomainName = domainName,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Downloads the DNS zone from the registrar for one domain and upserts records into the local DnsRecord table.
    /// Existing records matched by (type, name, value) are updated; unmatched incoming records are inserted.
    /// Local records not present in the remote zone are left untouched.
    /// </summary>
    private async Task<DnsRecordSyncResult> SyncDnsRecordsForDomainAsync(IDomainRegistrar registrarClient, Data.Entities.RegisteredDomain domain)
    {
        var result = new DnsRecordSyncResult { DomainName = domain.Name };

        try
        {
            _log.Information("Fetching DNS zone for {DomainName} from registrar", domain.Name);
            var zone = await registrarClient.GetDnsZoneAsync(domain.Name);

            if (zone?.Records == null || zone.Records.Count == 0)
            {
                _log.Information("No DNS records returned for {DomainName}", domain.Name);
                result.Success = true;
                return result;
            }

            // Build a lookup of active DNS record types keyed by uppercased type string
            var dnsRecordTypes = await _context.DnsRecordTypes
                .Where(t => t.IsActive)
                .ToDictionaryAsync(t => t.Type.ToUpper());

            // Load all existing records for this domain into memory for matching
            var existingRecords = await _context.DnsRecords
                .Where(r => r.DomainId == domain.Id)
                .ToListAsync();

            foreach (var incoming in zone.Records)
            {
                if (string.IsNullOrWhiteSpace(incoming.Type) ||
                    !dnsRecordTypes.TryGetValue(incoming.Type.ToUpper(), out var dnsRecordType))
                {
                    _log.Warning("Unknown DNS record type '{Type}' for domain {DomainName} — skipping", incoming.Type, domain.Name);
                    result.Skipped++;
                    continue;
                }

                // Match on type + name + value — all three must be equal for an update
                var existing = existingRecords.FirstOrDefault(r =>
                    r.DnsRecordTypeId == dnsRecordType.Id &&
                    r.Name == incoming.Name &&
                    r.Value == incoming.Value);

                if (existing != null)
                {
                    existing.TTL = incoming.TTL > 0 ? incoming.TTL : existing.TTL;
                    existing.Priority = incoming.Priority;
                    existing.Weight = incoming.Weight;
                    existing.Port = incoming.Port;
                    existing.UpdatedAt = DateTime.UtcNow;
                    result.Updated++;
                }
                else
                {
                    _context.DnsRecords.Add(new DnsRecord
                    {
                        DomainId = domain.Id,
                        DnsRecordTypeId = dnsRecordType.Id,
                        Name = incoming.Name,
                        Value = incoming.Value,
                        TTL = incoming.TTL > 0 ? incoming.TTL : dnsRecordType.DefaultTTL,
                        Priority = incoming.Priority,
                        Weight = incoming.Weight,
                        Port = incoming.Port,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    result.Created++;
                }
            }

            await _context.SaveChangesAsync();
            result.Success = true;

            _log.Information("DNS sync for {DomainName}: {Created} created, {Updated} updated, {Skipped} skipped",
                domain.Name, result.Created, result.Updated, result.Skipped);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error syncing DNS records for domain {DomainName}", domain.Name);
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }
}
