using ISPAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Runs hourly checks and executes registrar/TLD price synchronization once per registrar per day.
/// Executes at startup if today's sync is missing.
/// </summary>
public class RegistrarTldPriceSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldPriceSyncBackgroundService>();
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public RegistrarTldPriceSyncBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Registrar TLD Price Sync Background Service starting");

        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        await RunSyncAsync("Startup", stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
                await RunSyncAsync("ScheduledHourlyCheck", stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in registrar TLD price sync background loop");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _log.Information("Registrar TLD Price Sync Background Service stopping");
    }

    private async Task RunSyncAsync(string triggerSource, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<IRegistrarTldPriceSyncService>();

        var synced = await syncService.SyncRegistrarsMissingTodayAsync(triggerSource, cancellationToken);
        _log.Information("Registrar TLD price sync check completed. Trigger: {TriggerSource}, Registrars synced: {SyncedCount}",
            triggerSource, synced);
    }
}
