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
}
