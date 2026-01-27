using ISPAdmin.Data;
using ISPAdmin.Data.Enums;
using ISPAdmin.Domain.Events.DomainEvents;
using ISPAdmin.Domain.Services;
using ISPAdmin.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that monitors domain expirations and triggers renewal workflows
/// </summary>
public class DomainExpirationMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainExpirationMonitorService>();
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    public DomainExpirationMonitorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Domain Expiration Monitor Service starting");

        // Wait a bit before starting to allow other services to initialize
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiringDomainsAsync(stoppingToken);
                await CheckExpiredDomainsAsync(stoppingToken);

                _log.Information("Domain expiration check completed. Next check in {Hours} hours", 
                    _checkInterval.TotalHours);

                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in domain expiration monitor");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _log.Information("Domain Expiration Monitor Service stopping");
    }

    private async Task CheckExpiringDomainsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var renewalWorkflow = scope.ServiceProvider.GetRequiredService<IDomainRenewalWorkflow>();

        // Find domains expiring within 30 days
        var expiringDomains = await context.Domains
            .Where(d => d.Status == DomainStatus.Active.ToString() &&
                       d.ExpirationDate <= DateTime.UtcNow.AddDays(30) &&
                       d.ExpirationDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _log.Information("Found {Count} domains expiring within 30 days", expiringDomains.Count);

        foreach (var domain in expiringDomains)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var daysUntilExpiration = (domain.ExpirationDate - DateTime.UtcNow).Days;
                
                _log.Information("Processing expiring domain {DomainName} (expires in {Days} days)", 
                    domain.Name, daysUntilExpiration);

                // Trigger renewal workflow
                await renewalWorkflow.ExecuteAsync(domain.Id);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error processing expiring domain {DomainName}", domain.Name);
            }
        }
    }

    private async Task CheckExpiredDomainsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IDomainEventPublisher>();

        // Find domains that have expired but status hasn't been updated
        var expiredDomains = await context.Domains
            .Where(d => d.Status == DomainStatus.Active.ToString() &&
                       d.ExpirationDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _log.Information("Found {Count} expired domains to update", expiredDomains.Count);

        foreach (var domain in expiredDomains)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                _log.Warning("Marking domain {DomainName} as expired (expired on {ExpirationDate})", 
                    domain.Name, domain.ExpirationDate);

                // Update domain status
                domain.Status = DomainStatus.Expired.ToString();
                domain.UpdatedAt = DateTime.UtcNow;

                // Publish domain expired event
                await eventPublisher.PublishAsync(new DomainExpiredEvent
                {
                    AggregateId = domain.Id,
                    DomainName = domain.Name,
                    ExpiredAt = domain.ExpirationDate,
                    CustomerId = domain.CustomerId,
                    AutoRenewEnabled = domain.AutoRenew
                });

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error marking domain {DomainName} as expired", domain.Name);
            }
        }
    }
}
