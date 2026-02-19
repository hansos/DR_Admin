using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
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
}
