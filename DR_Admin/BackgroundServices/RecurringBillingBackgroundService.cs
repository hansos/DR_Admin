using ISPAdmin.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that processes recurring billing for active subscriptions
/// Runs daily to check for due subscriptions and process billing
/// </summary>
public class RecurringBillingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Serilog.ILogger _log = Log.ForContext<RecurringBillingBackgroundService>();

    // Configuration
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour
    private readonly TimeSpan _processingDelay = TimeSpan.FromSeconds(30); // Delay between processing each subscription
    private readonly int _batchSize = 50; // Process 50 subscriptions at a time

    public RecurringBillingBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Main execution method that runs continuously while the service is active
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Recurring Billing Background Service starting");

        // Wait for 1 minute on startup to ensure all services are initialized
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueSubscriptionsAsync(stoppingToken);

                // Wait for the next check interval
                _log.Information("Recurring Billing Background Service sleeping for {Interval}", _checkInterval);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _log.Information("Recurring Billing Background Service cancelled");
                break;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in Recurring Billing Background Service main loop");
                // Wait a bit before retrying to avoid tight error loops
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _log.Information("Recurring Billing Background Service stopping");
    }

    /// <summary>
    /// Processes all subscriptions that are due for billing
    /// </summary>
    private async Task ProcessDueSubscriptionsAsync(CancellationToken stoppingToken)
    {
        try
        {
            _log.Information("Starting recurring billing process");

            using var scope = _serviceProvider.CreateScope();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

            // Get all subscriptions due for billing
            var dueSubscriptions = await subscriptionService.GetDueSubscriptionsAsync();
            var subscriptionList = dueSubscriptions.ToList();

            if (!subscriptionList.Any())
            {
                _log.Information("No subscriptions due for billing at this time");
                return;
            }

            _log.Information("Found {Count} subscriptions due for billing", subscriptionList.Count);

            var successCount = 0;
            var failureCount = 0;
            var processedCount = 0;

            // Process subscriptions in batches
            foreach (var subscription in subscriptionList)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _log.Warning("Recurring billing process cancelled by shutdown signal");
                    break;
                }

                try
                {
                    _log.Information("Processing billing for subscription {SubscriptionId} (Customer: {CustomerId}, Amount: {Amount})",
                        subscription.Id, subscription.CustomerId, subscription.Amount);

                    var result = await subscriptionService.ProcessSubscriptionBillingAsync(subscription.Id);

                    if (result)
                    {
                        successCount++;
                        _log.Information("Successfully billed subscription {SubscriptionId}", subscription.Id);
                    }
                    else
                    {
                        failureCount++;
                        _log.Warning("Failed to bill subscription {SubscriptionId}", subscription.Id);
                    }

                    processedCount++;

                    // Add a small delay between processing to avoid overwhelming the system
                    if (processedCount % _batchSize == 0)
                    {
                        _log.Information("Processed {ProcessedCount} subscriptions, pausing briefly", processedCount);
                        await Task.Delay(_processingDelay, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _log.Error(ex, "Error processing billing for subscription {SubscriptionId}", subscription.Id);
                }
            }

            _log.Information("Recurring billing process completed. Processed: {ProcessedCount}, Success: {SuccessCount}, Failed: {FailureCount}",
                processedCount, successCount, failureCount);

            // Log warning if failure rate is high
            if (processedCount > 0 && (failureCount / (double)processedCount) > 0.2)
            {
                _log.Warning("High failure rate detected: {FailureRate:P} ({FailureCount}/{ProcessedCount})",
                    failureCount / (double)processedCount, failureCount, processedCount);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Critical error in ProcessDueSubscriptionsAsync");
            throw;
        }
    }

    /// <summary>
    /// Called when the service is stopping
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _log.Information("Recurring Billing Background Service is stopping gracefully");
        await base.StopAsync(cancellationToken);
    }
}
