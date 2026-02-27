using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Synchronizes registrar/TLD prices from registrar APIs and writes session/change logs.
/// </summary>
public class RegistrarTldPriceSyncService : IRegistrarTldPriceSyncService
{
    private readonly ApplicationDbContext _context;
    private readonly DomainRegistrarFactory _registrarFactory;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldPriceSyncService>();

    public RegistrarTldPriceSyncService(ApplicationDbContext context, DomainRegistrarFactory registrarFactory)
    {
        _context = context;
        _registrarFactory = registrarFactory;
    }

    public async Task<int> SyncRegistrarsMissingTodayAsync(string triggerSource, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var activeRegistrars = await _context.Registrars
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        var syncedCount = 0;

        foreach (var registrar in activeRegistrars)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hasSuccessfulSyncToday = await _context.RegistrarTldPriceDownloadSessions
                .AsNoTracking()
                .AnyAsync(s => s.RegistrarId == registrar.Id && s.Success && s.StartedAtUtc >= today, cancellationToken);

            if (hasSuccessfulSyncToday)
            {
                continue;
            }

            await SyncRegistrarAsync(registrar, triggerSource, cancellationToken);
            syncedCount++;
        }

        return syncedCount;
    }

    public async Task<List<RegistrarCurrentCostByTldDto>> PreviewRegistrarCostsByExtensionAsync(string extension, CancellationToken cancellationToken = default)
    {
        var normalizedExtension = extension.Trim().TrimStart('.').ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedExtension))
        {
            return [];
        }

        var registrars = await _context.Registrars
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        var rows = new List<RegistrarCurrentCostByTldDto>();

        foreach (var registrar in registrars)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);
                var supported = await registrarClient.GetSupportedTldsAsync([normalizedExtension]);
                var tldInfo = supported
                    .FirstOrDefault(s => string.Equals(
                        s.Name?.TrimStart('.'),
                        normalizedExtension,
                        StringComparison.OrdinalIgnoreCase));

                if (tldInfo == null || !HasAnyPrice(tldInfo))
                {
                    continue;
                }

                rows.Add(new RegistrarCurrentCostByTldDto
                {
                    RegistrarTldId = 0,
                    RegistrarId = registrar.Id,
                    RegistrarName = registrar.Name,
                    RegistrationCost = tldInfo.RegistrationPrice,
                    RenewalCost = tldInfo.RenewalPrice,
                    TransferCost = tldInfo.TransferPrice,
                    Currency = string.IsNullOrWhiteSpace(tldInfo.Currency) ? "USD" : tldInfo.Currency
                });
            }
            catch (Exception ex)
            {
                _log.Warning(ex, "Failed previewing registrar prices for registrar {RegistrarId} and extension {Extension}",
                    registrar.Id, normalizedExtension);
            }
        }

        return rows
            .OrderBy(r => r.RegistrarName)
            .ToList();
    }

    public async Task<int> SyncRegistrarsForTldAsync(int tldId, string triggerSource, CancellationToken cancellationToken = default)
    {
        var tld = await _context.Tlds
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tldId && t.IsActive, cancellationToken);

        if (tld == null || string.IsNullOrWhiteSpace(tld.Extension))
        {
            return 0;
        }

        var registrars = await _context.Registrars
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        var normalizedExtension = tld.Extension.TrimStart('.');
        var now = DateTime.UtcNow;
        var syncedCount = 0;

        foreach (var registrar in registrars)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);
                var supported = await registrarClient.GetSupportedTldsAsync([normalizedExtension]);
                var tldInfo = supported
                    .FirstOrDefault(s => string.Equals(
                        s.Name?.TrimStart('.'),
                        normalizedExtension,
                        StringComparison.OrdinalIgnoreCase));

                if (tldInfo == null || !HasAnyPrice(tldInfo))
                {
                    continue;
                }

                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrar.Id && rt.TldId == tldId, cancellationToken);

                if (registrarTld == null)
                {
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrar.Id,
                        TldId = tldId,
                        IsActive = true,
                        AutoRenew = false,
                        MinRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        Notes = "Auto-created from registrar price sync",
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    _context.RegistrarTlds.Add(registrarTld);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var targetRegistration = tldInfo.RegistrationPrice ?? 0m;
                var targetRenewal = tldInfo.RenewalPrice ?? 0m;
                var targetTransfer = tldInfo.TransferPrice ?? 0m;
                var targetCurrency = string.IsNullOrWhiteSpace(tldInfo.Currency) ? "USD" : tldInfo.Currency!;

                var currentPricing = await _context.RegistrarTldCostPricing
                    .Where(c => c.RegistrarTldId == registrarTld.Id &&
                                c.IsActive &&
                                c.EffectiveFrom <= now &&
                                (c.EffectiveTo == null || c.EffectiveTo > now))
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentPricing != null &&
                    currentPricing.RegistrationCost == targetRegistration &&
                    currentPricing.RenewalCost == targetRenewal &&
                    currentPricing.TransferCost == targetTransfer &&
                    string.Equals(currentPricing.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    syncedCount++;
                    continue;
                }

                if (currentPricing != null && currentPricing.EffectiveTo == null)
                {
                    currentPricing.EffectiveTo = now.AddSeconds(-1);
                }

                var newPricing = new RegistrarTldCostPricing
                {
                    RegistrarTldId = registrarTld.Id,
                    EffectiveFrom = now,
                    EffectiveTo = null,
                    RegistrationCost = targetRegistration,
                    RenewalCost = targetRenewal,
                    TransferCost = targetTransfer,
                    Currency = targetCurrency,
                    IsActive = true,
                    Notes = $"Downloaded from registrar API ({registrar.Code}) via {triggerSource}",
                    CreatedBy = "system:registrar-price-sync"
                };

                _context.RegistrarTldCostPricing.Add(newPricing);
                await _context.SaveChangesAsync(cancellationToken);
                syncedCount++;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error syncing registrar {RegistrarId} for TLD {TldId}", registrar.Id, tldId);
            }
        }

        return syncedCount;
    }

    private async Task SyncRegistrarAsync(Registrar registrar, string triggerSource, CancellationToken cancellationToken)
    {
        var session = new RegistrarTldPriceDownloadSession
        {
            RegistrarId = registrar.Id,
            StartedAtUtc = DateTime.UtcNow,
            TriggerSource = triggerSource,
            Success = false
        };

        _context.RegistrarTldPriceDownloadSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var registrarTlds = await _context.RegistrarTlds
                .Where(rt => rt.RegistrarId == registrar.Id && rt.IsActive && rt.Tld.IsActive)
                .Include(rt => rt.Tld)
                .ToListAsync(cancellationToken);

            if (registrarTlds.Count == 0)
            {
                session.Success = true;
                session.CompletedAtUtc = DateTime.UtcNow;
                session.Message = "No active registrar/TLD combinations found";
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            var tldFilters = registrarTlds
                .Select(rt => rt.Tld.Extension)
                .Where(ext => !string.IsNullOrWhiteSpace(ext))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);
            var supportedTlds = await registrarClient.GetSupportedTldsAsync(tldFilters);

            var supportedByExtension = supportedTlds
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .GroupBy(t => t.Name.TrimStart('.'), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var now = DateTime.UtcNow;
            var changedCount = 0;

            foreach (var registrarTld in registrarTlds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!supportedByExtension.TryGetValue(registrarTld.Tld.Extension, out var tldInfo))
                {
                    continue;
                }

                if (!HasAnyPrice(tldInfo))
                {
                    continue;
                }

                var targetRegistration = tldInfo.RegistrationPrice ?? 0m;
                var targetRenewal = tldInfo.RenewalPrice ?? 0m;
                var targetTransfer = tldInfo.TransferPrice ?? 0m;
                var targetCurrency = string.IsNullOrWhiteSpace(tldInfo.Currency) ? "USD" : tldInfo.Currency!;

                var currentPricing = await _context.RegistrarTldCostPricing
                    .Where(c => c.RegistrarTldId == registrarTld.Id &&
                                c.IsActive &&
                                c.EffectiveFrom <= now &&
                                (c.EffectiveTo == null || c.EffectiveTo > now))
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentPricing != null &&
                    currentPricing.RegistrationCost == targetRegistration &&
                    currentPricing.RenewalCost == targetRenewal &&
                    currentPricing.TransferCost == targetTransfer &&
                    string.Equals(currentPricing.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (currentPricing != null && currentPricing.EffectiveTo == null)
                {
                    currentPricing.EffectiveTo = now.AddSeconds(-1);
                }

                var newPricing = new RegistrarTldCostPricing
                {
                    RegistrarTldId = registrarTld.Id,
                    EffectiveFrom = now,
                    EffectiveTo = null,
                    RegistrationCost = targetRegistration,
                    RenewalCost = targetRenewal,
                    TransferCost = targetTransfer,
                    Currency = targetCurrency,
                    IsActive = true,
                    Notes = $"Downloaded from registrar API ({registrar.Code})",
                    CreatedBy = "system:registrar-price-sync"
                };

                _context.RegistrarTldCostPricing.Add(newPricing);

                _context.RegistrarTldPriceChangeLogs.Add(new RegistrarTldPriceChangeLog
                {
                    RegistrarTldId = registrarTld.Id,
                    DownloadSessionId = session.Id,
                    ChangeSource = "Download",
                    ChangedBy = "system:registrar-price-sync",
                    ChangedAtUtc = now,
                    OldRegistrationCost = currentPricing?.RegistrationCost,
                    NewRegistrationCost = targetRegistration,
                    OldRenewalCost = currentPricing?.RenewalCost,
                    NewRenewalCost = targetRenewal,
                    OldTransferCost = currentPricing?.TransferCost,
                    NewTransferCost = targetTransfer,
                    OldCurrency = currentPricing?.Currency,
                    NewCurrency = targetCurrency,
                    Notes = currentPricing == null
                        ? "Initial price created from registrar API"
                        : "Price updated from registrar API"
                });

                changedCount++;
            }

            session.TldsProcessed = registrarTlds.Count;
            session.PriceChangesDetected = changedCount;
            session.Success = true;
            session.CompletedAtUtc = DateTime.UtcNow;
            session.Message = $"Processed {registrarTlds.Count} registrar/TLD combinations";

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error syncing registrar prices for registrar {RegistrarId}", registrar.Id);

            session.Success = false;
            session.CompletedAtUtc = DateTime.UtcNow;
            session.ErrorMessage = ex.Message;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static bool HasAnyPrice(TldInfo tldInfo)
    {
        return tldInfo.RegistrationPrice.HasValue || tldInfo.RenewalPrice.HasValue || tldInfo.TransferPrice.HasValue;
    }
}
