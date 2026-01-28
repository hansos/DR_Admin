using ExchangeRateLib.Factories;
using ExchangeRateLib.Infrastructure.Settings;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;
using System.Diagnostics;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that periodically updates currency exchange rates from configured provider
/// </summary>
public class ExchangeRateUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExchangeRateSettings _exchangeRateSettings;
    private readonly ILogger _log = Log.ForContext<ExchangeRateUpdateService>();
    private DateTime _lastUpdateTime = DateTime.MinValue;
    private int _updateCountToday = 0;
    private DateTime _lastResetDate = DateTime.UtcNow.Date;

    public ExchangeRateUpdateService(
        IServiceProvider serviceProvider,
        ExchangeRateSettings exchangeRateSettings)
    {
        _serviceProvider = serviceProvider;
        _exchangeRateSettings = exchangeRateSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Exchange Rate Update Service starting");

        // Update on startup if configured
        if (_exchangeRateSettings.UpdateOnStartup)
        {
            _log.Information("UpdateOnStartup is enabled, updating exchange rates immediately");
            await UpdateExchangeRatesAsync(stoppingToken);
        }

        // Continue with periodic updates
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                // Reset daily counter if it's a new day
                if (now.Date > _lastResetDate)
                {
                    _log.Information("New day started, resetting update counter");
                    _updateCountToday = 0;
                    _lastResetDate = now.Date;
                }

                // Check if we should update
                var timeSinceLastUpdate = now - _lastUpdateTime;
                var shouldUpdate = false;
                var reason = "";

                // Check if enough time has passed since last update
                if (_lastUpdateTime == DateTime.MinValue || 
                    timeSinceLastUpdate.TotalHours >= _exchangeRateSettings.HoursBetweenUpdates)
                {
                    // Check if we haven't exceeded max updates per day (0 = unlimited)
                    if (_exchangeRateSettings.MaxUpdatesPerDay == 0 || 
                        _updateCountToday < _exchangeRateSettings.MaxUpdatesPerDay)
                    {
                        shouldUpdate = true;
                        reason = $"Scheduled update (hours since last: {timeSinceLastUpdate.TotalHours:F2}, updates today: {_updateCountToday}/{_exchangeRateSettings.MaxUpdatesPerDay})";
                    }
                    else
                    {
                        _log.Debug("Max updates per day ({MaxUpdates}) reached, skipping update", 
                            _exchangeRateSettings.MaxUpdatesPerDay);
                    }
                }

                if (shouldUpdate)
                {
                    _log.Information("Triggering exchange rate update: {Reason}", reason);
                    await UpdateExchangeRatesAsync(stoppingToken);
                }

                // Wait before checking again (check every 30 minutes)
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _log.Information("Exchange Rate Update Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in Exchange Rate Update Service main loop");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _log.Information("Exchange Rate Update Service stopped");
    }

    private async Task UpdateExchangeRatesAsync(CancellationToken stoppingToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var source = MapProviderToSource(_exchangeRateSettings.Provider);
        var isStartup = _lastUpdateTime == DateTime.MinValue;
        var downloadTimestamp = DateTime.UtcNow;
        var addedCount = 0;
        var updatedCount = 0;
        var ratesDownloadedCount = 0;
        string? errorMessage = null;
        string? errorCode = null;
        bool success = false;

        try
        {
            _log.Information("Starting exchange rate update from provider: {Provider}", 
                _exchangeRateSettings.Provider);

            // Create a scope for scoped services
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logService = scope.ServiceProvider.GetRequiredService<IExchangeRateDownloadLogService>();

            // Create exchange rate provider
            var factory = new ExchangeRateFactory(_exchangeRateSettings);
            var provider = factory.CreateProvider();

            // Load base currency and target currencies from database
            var existingRates = await dbContext.CurrencyExchangeRates
                .Where(r => r.IsActive)
                .Select(r => new { r.BaseCurrency, r.TargetCurrency })
                .Distinct()
                .ToListAsync(stoppingToken);

            if (!existingRates.Any())
            {
                _log.Warning("No existing exchange rates found in database. Please manually add initial rates or configure currencies.");
                return;
            }

            // Group by BaseCurrency to handle multiple base currencies
            var currencyGroups = existingRates.GroupBy(r => r.BaseCurrency);

            foreach (var group in currencyGroups)
            {
                var baseCurrency = group.Key;
                var targetCurrenciesList = group.Select(r => r.TargetCurrency).Distinct().ToList();

                _log.Information("Updating {Count} currencies for base currency {Base}: {Currencies}", 
                    targetCurrenciesList.Count, baseCurrency, string.Join(", ", targetCurrenciesList.Take(10)));

                // Fetch exchange rates for this base currency
                var result = await provider.GetExchangeRatesAsync(baseCurrency, targetCurrenciesList);

                if (!result.Success)
                {
                    errorMessage = result.Message;
                    errorCode = result.ErrorCode;
                    _log.Error("Failed to fetch exchange rates for {Base}: {Message} (Error Code: {ErrorCode})", 
                        baseCurrency, result.Message, result.ErrorCode);
                    
                    // Log failure for each target currency
                    foreach (var targetCurrency in targetCurrenciesList)
                    {
                        await logService.CreateLogAsync(new CreateExchangeRateDownloadLogDto
                        {
                            BaseCurrency = baseCurrency,
                            TargetCurrency = targetCurrency,
                            Source = source,
                            Success = false,
                            DownloadTimestamp = downloadTimestamp,
                            RatesDownloaded = 0,
                            RatesAdded = 0,
                            RatesUpdated = 0,
                            ErrorMessage = errorMessage,
                            ErrorCode = errorCode,
                            DurationMs = stopwatch.ElapsedMilliseconds,
                            IsStartupDownload = isStartup,
                            IsScheduledDownload = !isStartup
                        });
                    }
                    
                    continue; // Continue with next base currency
                }

                if (result.Rates == null || !result.Rates.Any())
                {
                    errorMessage = "No exchange rates returned from provider";
                    _log.Warning("{Message} for base currency {Base}", errorMessage, baseCurrency);
                    
                    await logService.CreateLogAsync(new CreateExchangeRateDownloadLogDto
                    {
                        BaseCurrency = baseCurrency,
                        TargetCurrency = null,
                        Source = source,
                        Success = false,
                        DownloadTimestamp = downloadTimestamp,
                        RatesDownloaded = 0,
                        RatesAdded = 0,
                        RatesUpdated = 0,
                        ErrorMessage = errorMessage,
                        ErrorCode = "NO_RATES",
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        IsStartupDownload = isStartup,
                        IsScheduledDownload = !isStartup
                    });
                    
                    continue; // Continue with next base currency
                }

                ratesDownloadedCount += result.Rates.Count;
                _log.Information("Fetched {Count} exchange rates for {Base} from {Provider}", 
                    result.Rates.Count, baseCurrency, _exchangeRateSettings.Provider);

                // Update database
                var now = DateTime.UtcNow;

                foreach (var rate in result.Rates)
                {
                    var targetCurrency = rate.Key;
                    var exchangeRate = rate.Value;

                    // Find existing active rate for this currency pair
                    var existingRate = await dbContext.CurrencyExchangeRates
                        .FirstOrDefaultAsync(r => 
                            r.BaseCurrency == baseCurrency &&
                            r.TargetCurrency == targetCurrency &&
                            r.IsActive &&
                            r.Source == source,
                            stoppingToken);

                    if (existingRate != null)
                    {
                        // Update existing rate in place (no historical records)
                        if (existingRate.Rate != exchangeRate)
                        {
                            existingRate.Rate = exchangeRate;
                            existingRate.EffectiveDate = now;
                            updatedCount++;

                            _log.Debug("Updated rate for {Base}/{Target}: {OldRate} -> {NewRate}", 
                                baseCurrency, targetCurrency, 
                                existingRate.Rate, exchangeRate);
                        }
                        // If rate hasn't changed, do nothing
                    }
                    else
                    {
                        // No existing rate, create new one
                        var newRate = new CurrencyExchangeRate
                        {
                            BaseCurrency = baseCurrency,
                            TargetCurrency = targetCurrency,
                            Rate = exchangeRate,
                            EffectiveDate = now,
                            ExpiryDate = null,
                            Source = source,
                            IsActive = true,
                            Markup = 0m
                        };

                        dbContext.CurrencyExchangeRates.Add(newRate);
                        addedCount++;

                        _log.Debug("Added new rate for {Base}/{Target}: {Rate}", 
                            baseCurrency, targetCurrency, exchangeRate);
                    }
                }

                // Create download log for each currency
                foreach (var rate in result.Rates)
                {
                    await logService.CreateLogAsync(new CreateExchangeRateDownloadLogDto
                    {
                        BaseCurrency = baseCurrency,
                        TargetCurrency = rate.Key,
                        Source = source,
                        Success = true,
                        DownloadTimestamp = downloadTimestamp,
                        RatesDownloaded = 1,
                        RatesAdded = 0, // Per-currency logs don't track this
                        RatesUpdated = 0, // Per-currency logs don't track this
                        ErrorMessage = null,
                        ErrorCode = null,
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        IsStartupDownload = isStartup,
                        IsScheduledDownload = !isStartup,
                        Notes = $"Base: {baseCurrency}"
                    });
                }

                // Create a summary log for this base currency
                await logService.CreateLogAsync(new CreateExchangeRateDownloadLogDto
                {
                    BaseCurrency = baseCurrency,
                    TargetCurrency = null, // null indicates bulk download
                    Source = source,
                    Success = true,
                    DownloadTimestamp = downloadTimestamp,
                    RatesDownloaded = result.Rates.Count,
                    RatesAdded = addedCount,
                    RatesUpdated = updatedCount,
                    ErrorMessage = null,
                    ErrorCode = null,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    IsStartupDownload = isStartup,
                    IsScheduledDownload = !isStartup,
                    Notes = $"Summary for {baseCurrency}"
                });
            }

            // Save all changes
            var savedCount = await dbContext.SaveChangesAsync(stoppingToken);

            success = true;
            _log.Information(
                "Exchange rate update completed: {Added} added, {Updated} updated, {Total} total changes saved", 
                addedCount, updatedCount, savedCount);

            // Update tracking
            _lastUpdateTime = DateTime.UtcNow;
            _updateCountToday++;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            errorMessage = ex.Message;
            errorCode = "UNEXPECTED_ERROR";
            _log.Error(ex, "Error updating exchange rates");

            // Log the failure
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var logService = scope.ServiceProvider.GetRequiredService<IExchangeRateDownloadLogService>();
                
                await logService.CreateLogAsync(new CreateExchangeRateDownloadLogDto
                {
                    BaseCurrency = "UNKNOWN", // We don't have base currency in exception context
                    TargetCurrency = null,
                    Source = source,
                    Success = false,
                    DownloadTimestamp = downloadTimestamp,
                    RatesDownloaded = ratesDownloadedCount,
                    RatesAdded = addedCount,
                    RatesUpdated = updatedCount,
                    ErrorMessage = errorMessage,
                    ErrorCode = errorCode,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    IsStartupDownload = isStartup,
                    IsScheduledDownload = !isStartup,
                    Notes = "Exception during update process"
                });
            }
            catch (Exception logEx)
            {
                _log.Error(logEx, "Failed to log exchange rate download failure");
            }
        }
    }

    private CurrencyRateSource MapProviderToSource(string provider)
    {
        return provider.ToLower() switch
        {
            "exchangeratehost" => CurrencyRateSource.ExchangeRateHost,
            "frankfurter" => CurrencyRateSource.Frankfurter,
            "openexchangerates" => CurrencyRateSource.OpenExchangeRates,
            "currencylayer" => CurrencyRateSource.CurrencyLayer,
            "fixer" => CurrencyRateSource.Fixer,
            "xe" => CurrencyRateSource.XE,
            "oanda" => CurrencyRateSource.OANDA,
            _ => CurrencyRateSource.Other
        };
    }
}
