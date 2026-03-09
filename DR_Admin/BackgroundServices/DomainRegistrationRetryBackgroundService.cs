using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that retries failed or pending paid domain registrations.
/// </summary>
public class DomainRegistrationRetryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainRegistrationRetryBackgroundService>();
    private readonly TimeSpan _pollInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _startupDelay = TimeSpan.FromMinutes(1);
    private const int BatchSize = 50;
    private static readonly ContactRoleType[] RequiredContactRoles =
    [
        ContactRoleType.Registrant,
        ContactRoleType.Administrative,
        ContactRoleType.Technical,
        ContactRoleType.Billing
    ];

    public DomainRegistrationRetryBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Domain Registration Retry Background Service starting");

        await Task.Delay(_startupDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueRetriesAsync(stoppingToken);
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in domain registration retry loop");
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }

        _log.Information("Domain Registration Retry Background Service stopping");
    }

    private async Task ProcessDueRetriesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registrarFactory = scope.ServiceProvider.GetRequiredService<DomainRegistrarFactory>();

        var now = DateTime.UtcNow;

        var dueDomains = await context.RegisteredDomains
            .Include(d => d.Registrar)
            .Include(d => d.Customer)
            .Include(d => d.NameServers)
            .Where(d => d.Status == DomainStatus.PendingRegistration.ToString() &&
                        (d.RegistrationStatus == DomainRegistrationStatus.PaidPendingRegistration ||
                         d.RegistrationStatus == DomainRegistrationStatus.RegistrationFailed) &&
                        (!d.NextRegistrationAttemptUtc.HasValue || d.NextRegistrationAttemptUtc <= now))
            .OrderBy(d => d.NextRegistrationAttemptUtc ?? DateTime.MinValue)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (dueDomains.Count == 0)
        {
            return;
        }

        _log.Information("Retrying domain registration for {Count} domains", dueDomains.Count);

        foreach (var domain in dueDomains)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await RetryDomainAsync(context, registrarFactory, domain, cancellationToken);
        }
    }

    private async Task RetryDomainAsync(
        ApplicationDbContext context,
        DomainRegistrarFactory registrarFactory,
        RegisteredDomain domain,
        CancellationToken cancellationToken)
    {
        var attemptAt = DateTime.UtcNow;
        domain.RegistrationAttemptCount += 1;
        domain.LastRegistrationAttemptUtc = attemptAt;
        domain.RegistrationStatus = DomainRegistrationStatus.RegistrationInProgress;
        domain.UpdatedAt = attemptAt;

        try
        {
            var registrarCode = domain.Registrar?.Code;
            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                throw new InvalidOperationException($"Registrar code missing for domain {domain.Name}");
            }

            var registrar = registrarFactory.CreateRegistrar(registrarCode);
            var fallbackContact = BuildContactFromCustomer(domain.Customer);

            var request = new DomainRegistrationRequest
            {
                DomainName = domain.Name,
                Years = ResolveRegistrationYears(domain),
                PrivacyProtection = domain.PrivacyProtection,
                AutoRenew = domain.AutoRenew,
                Nameservers = domain.NameServers
                    .OrderBy(ns => ns.SortOrder)
                    .Select(ns => ns.Hostname)
                    .Where(ns => !string.IsNullOrWhiteSpace(ns))
                    .ToList(),
                RegistrantContact = fallbackContact,
                AdminContact = fallbackContact,
                TechContact = fallbackContact,
                BillingContact = fallbackContact
            };

            var result = await registrar.RegisterDomainAsync(request);
            if (!result.Success)
            {
                domain.RegistrationStatus = DomainRegistrationStatus.RegistrationFailed;
                domain.RegistrationError = result.Message;
                domain.NextRegistrationAttemptUtc = DateTime.UtcNow.Add(ComputeRetryDelay(domain.RegistrationAttemptCount));
                domain.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(cancellationToken);
                _log.Warning("Domain registration retry failed for {DomainName}. Next retry at {NextRetry}", domain.Name, domain.NextRegistrationAttemptUtc);
                return;
            }

            domain.Status = DomainStatus.Active.ToString();
            domain.RegistrationStatus = DomainRegistrationStatus.Registered;
            domain.RegistrationDate = result.RegistrationDate ?? DateTime.UtcNow;
            domain.ExpirationDate = result.ExpirationDate ?? DateTime.UtcNow.AddYears(ResolveRegistrationYears(domain));
            domain.NextRegistrationAttemptUtc = null;
            domain.RegistrationError = null;
            domain.UpdatedAt = DateTime.UtcNow;

            await EnsurePostRegistrationDataAsync(context, domain, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            _log.Information("Domain registration retry succeeded for {DomainName}", domain.Name);
        }
        catch (Exception ex)
        {
            domain.RegistrationStatus = DomainRegistrationStatus.RegistrationFailed;
            domain.RegistrationError = ex.Message;
            domain.NextRegistrationAttemptUtc = DateTime.UtcNow.Add(ComputeRetryDelay(domain.RegistrationAttemptCount));
            domain.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
            _log.Error(ex, "Domain registration retry threw exception for {DomainName}. Next retry at {NextRetry}", domain.Name, domain.NextRegistrationAttemptUtc);
        }
    }

    private static TimeSpan ComputeRetryDelay(int attempt)
    {
        var exponent = Math.Clamp(attempt - 1, 0, 6);
        var minutes = 15 * Math.Pow(2, exponent);
        return TimeSpan.FromMinutes(minutes);
    }

    private static int ResolveRegistrationYears(RegisteredDomain domain)
    {
        if (domain.ExpirationDate.HasValue && domain.ExpirationDate.Value > DateTime.UtcNow)
        {
            var years = (int)Math.Ceiling((domain.ExpirationDate.Value - DateTime.UtcNow).TotalDays / 365d);
            return Math.Max(1, years);
        }

        return 1;
    }

    private static ContactInformation BuildContactFromCustomer(Customer? customer)
    {
        var fullName = customer?.Name?.Trim() ?? string.Empty;
        var split = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new ContactInformation
        {
            FirstName = split.Length > 0 ? split[0] : "Customer",
            LastName = split.Length > 1 ? string.Join(' ', split.Skip(1)) : "User",
            Email = customer?.Email ?? string.Empty,
            Phone = customer?.Phone ?? string.Empty,
            Country = "US"
        };
    }

    private static async Task EnsurePostRegistrationDataAsync(
        ApplicationDbContext context,
        RegisteredDomain domain,
        CancellationToken cancellationToken)
    {
        await EnsureDefaultDnsRecordsAsync(context, domain.Id, cancellationToken);
        await EnsureDomainContactsAndAssignmentsAsync(context, domain, cancellationToken);
    }

    private static async Task EnsureDefaultDnsRecordsAsync(
        ApplicationDbContext context,
        int domainId,
        CancellationToken cancellationToken)
    {
        var hasDnsRecords = await context.DnsRecords
            .AnyAsync(r => r.DomainId == domainId && !r.IsDeleted, cancellationToken);

        if (hasDnsRecords)
        {
            return;
        }

        var package = await context.DnsZonePackages
            .Include(p => p.Records)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.SortOrder)
            .ThenBy(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (package == null || package.Records.Count == 0)
        {
            return;
        }

        foreach (var templateRecord in package.Records)
        {
            context.DnsRecords.Add(new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = templateRecord.DnsRecordTypeId,
                Name = templateRecord.Name,
                Value = templateRecord.Value,
                TTL = templateRecord.TTL,
                Priority = templateRecord.Priority,
                Weight = templateRecord.Weight,
                Port = templateRecord.Port,
                IsPendingSync = true,
                IsDeleted = false
            });
        }
    }

    private static async Task EnsureDomainContactsAndAssignmentsAsync(
        ApplicationDbContext context,
        RegisteredDomain domain,
        CancellationToken cancellationToken)
    {
        var activeContactPeople = await context.ContactPersons
            .Where(cp => cp.CustomerId == domain.CustomerId && cp.IsActive)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Id)
            .ToListAsync(cancellationToken);

        if (activeContactPeople.Count == 0)
        {
            var fallbackContactPerson = await EnsureFallbackContactPersonAsync(context, domain, cancellationToken);
            activeContactPeople.Add(fallbackContactPerson);
        }

        var existingAssignmentRoles = await context.DomainContactAssignments
            .Where(a => a.RegisteredDomainId == domain.Id && a.IsActive)
            .Select(a => a.RoleType)
            .Distinct()
            .ToListAsync(cancellationToken);

        var existingContactRoles = await context.DomainContacts
            .Where(c => c.DomainId == domain.Id && c.IsCurrentVersion)
            .Select(c => c.RoleType)
            .Distinct()
            .ToListAsync(cancellationToken);

        var assignmentRoleSet = existingAssignmentRoles.ToHashSet();
        var contactRoleSet = existingContactRoles.ToHashSet();

        foreach (var role in RequiredContactRoles)
        {
            var selectedContactPerson = SelectContactPersonForRole(activeContactPeople, role);

            if (selectedContactPerson != null && !assignmentRoleSet.Contains(role))
            {
                context.DomainContactAssignments.Add(new DomainContactAssignment
                {
                    RegisteredDomainId = domain.Id,
                    ContactPersonId = selectedContactPerson.Id,
                    RoleType = role,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                });

                assignmentRoleSet.Add(role);
            }

            if (contactRoleSet.Contains(role))
            {
                continue;
            }

            context.DomainContacts.Add(BuildDomainContact(domain, selectedContactPerson, role));
            contactRoleSet.Add(role);
        }
    }

    private static async Task<ContactPerson> EnsureFallbackContactPersonAsync(
        ApplicationDbContext context,
        RegisteredDomain domain,
        CancellationToken cancellationToken)
    {
        var existingContactPerson = await context.ContactPersons
            .Where(cp => cp.CustomerId == domain.CustomerId)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingContactPerson != null)
        {
            if (!existingContactPerson.IsActive)
            {
                existingContactPerson.IsActive = true;
            }

            return existingContactPerson;
        }

        var fallback = BuildContactFromCustomer(domain.Customer);

        var contactPerson = new ContactPerson
        {
            CustomerId = domain.CustomerId,
            FirstName = fallback.FirstName,
            LastName = fallback.LastName,
            Email = fallback.Email,
            Phone = fallback.Phone,
            IsPrimary = true,
            IsActive = true,
            IsDefaultOwner = true,
            IsDefaultAdministrator = true,
            IsDefaultTech = true,
            IsDefaultBilling = true,
            IsDomainGlobal = true,
            Notes = $"Auto-created from domain registration for {domain.Name}"
        };

        context.ContactPersons.Add(contactPerson);
        await context.SaveChangesAsync(cancellationToken);

        return contactPerson;
    }

    private static ContactPerson? SelectContactPersonForRole(
        IReadOnlyList<ContactPerson> contactPeople,
        ContactRoleType role)
    {
        if (contactPeople.Count == 0)
        {
            return null;
        }

        return role switch
        {
            ContactRoleType.Registrant => contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Administrative => contactPeople.FirstOrDefault(cp => cp.IsDefaultAdministrator)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Technical => contactPeople.FirstOrDefault(cp => cp.IsDefaultTech)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultAdministrator)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Billing => contactPeople.FirstOrDefault(cp => cp.IsDefaultBilling)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            _ => contactPeople[0]
        };
    }

    private static DomainContact BuildDomainContact(
        RegisteredDomain domain,
        ContactPerson? selectedContactPerson,
        ContactRoleType role)
    {
        var firstName = selectedContactPerson?.FirstName;
        var lastName = selectedContactPerson?.LastName;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            var fallback = BuildContactFromCustomer(domain.Customer);
            firstName ??= fallback.FirstName;
            lastName ??= fallback.LastName;
        }

        return new DomainContact
        {
            DomainId = domain.Id,
            RoleType = role,
            FirstName = firstName,
            LastName = lastName,
            Email = selectedContactPerson?.Email ?? domain.Customer?.Email ?? string.Empty,
            Phone = selectedContactPerson?.Phone ?? domain.Customer?.Phone ?? string.Empty,
            Organization = selectedContactPerson?.Department,
            Address1 = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            CountryCode = "US",
            IsActive = selectedContactPerson?.IsActive ?? true,
            SourceContactPersonId = selectedContactPerson?.Id,
            NeedsSync = true,
            IsCurrentVersion = true,
            IsPrivacyProtected = domain.PrivacyProtection
        };
    }
}
