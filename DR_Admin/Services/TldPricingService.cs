using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Infrastructure.Settings;
using ISPAdmin.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for comprehensive TLD pricing management
/// Handles cost pricing, sales pricing, discounts, and margin analysis
/// </summary>
public partial class TldPricingService : ITldPricingService
{
    private readonly ApplicationDbContext _context;
    private readonly TldPricingSettings _settings;
    private readonly MarginValidator _marginValidator;
    private readonly FuturePricingManager _futurePricingManager;
    private readonly ICurrencyService _currencyService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldPricingService>();

    public TldPricingService(
        ApplicationDbContext context,
        IOptions<TldPricingSettings> settings,
        MarginValidator marginValidator,
        FuturePricingManager futurePricingManager,
        ICurrencyService currencyService)
    {
        _context = context;
        _settings = settings.Value;
        _marginValidator = marginValidator;
        _futurePricingManager = futurePricingManager;
        _currencyService = currencyService;
    }

    // ==================== RegistrarTldCostPricing Methods ====================

    public async Task<List<RegistrarTldCostPricingDto>> GetCostPricingHistoryAsync(int registrarTldId, bool includeArchived = false)
    {
        try
        {
            _log.Information("Fetching cost pricing history for RegistrarTld {RegistrarTldId}, IncludeArchived: {IncludeArchived}",
                registrarTldId, includeArchived);

            var query = _context.RegistrarTldCostPricing
                .AsNoTracking()
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Registrar)
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Tld)
                .Where(c => c.RegistrarTldId == registrarTldId);

            if (!includeArchived)
            {
                query = query.Where(c => c.IsActive);
            }

            var costPricing = await query
                .OrderByDescending(c => c.EffectiveFrom)
                .ToListAsync();

            var dtos = costPricing.Select(MapCostPricingToDto).ToList();

            _log.Information("Successfully fetched {Count} cost pricing records", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching cost pricing history for RegistrarTld {RegistrarTldId}", registrarTldId);
            throw;
        }
    }

    public async Task<RegistrarTldCostPricingDto?> GetCurrentCostPricingAsync(int registrarTldId, DateTime? effectiveDate = null)
    {
        try
        {
            var checkDate = effectiveDate ?? DateTime.UtcNow;
            _log.Information("Fetching current cost pricing for RegistrarTld {RegistrarTldId} at {EffectiveDate}",
                registrarTldId, checkDate);

            var costPricing = await _context.RegistrarTldCostPricing
                .AsNoTracking()
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Registrar)
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Tld)
                .Where(c => c.RegistrarTldId == registrarTldId &&
                           c.IsActive &&
                           c.EffectiveFrom <= checkDate &&
                           (c.EffectiveTo == null || c.EffectiveTo > checkDate))
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (costPricing == null)
            {
                _log.Warning("No current cost pricing found for RegistrarTld {RegistrarTldId}", registrarTldId);
                return null;
            }

            return MapCostPricingToDto(costPricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching current cost pricing for RegistrarTld {RegistrarTldId}", registrarTldId);
            throw;
        }
    }

    public async Task<List<RegistrarTldCostPricingDto>> GetFutureCostPricingAsync(int registrarTldId)
    {
        try
        {
            _log.Information("Fetching future cost pricing for RegistrarTld {RegistrarTldId}", registrarTldId);

            var now = DateTime.UtcNow;
            var futurePricing = await _context.RegistrarTldCostPricing
                .AsNoTracking()
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Registrar)
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Tld)
                .Where(c => c.RegistrarTldId == registrarTldId &&
                           c.IsActive &&
                           c.EffectiveFrom > now)
                .OrderBy(c => c.EffectiveFrom)
                .ToListAsync();

            var dtos = futurePricing.Select(MapCostPricingToDto).ToList();

            _log.Information("Successfully fetched {Count} future cost pricing records", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching future cost pricing for RegistrarTld {RegistrarTldId}", registrarTldId);
            throw;
        }
    }

    public async Task<RegistrarTldCostPricingDto> CreateCostPricingAsync(CreateRegistrarTldCostPricingDto createDto, string? createdBy)
    {
        try
        {
            _log.Information("Creating cost pricing for RegistrarTld {RegistrarTldId}", createDto.RegistrarTldId);

            // Validate schedule date
            var (isValid, errorMessage) = _futurePricingManager.ValidateScheduleDate(createDto.EffectiveFrom);
            if (!isValid)
            {
                throw new InvalidOperationException(errorMessage);
            }

            // If EffectiveTo is null, close any existing open pricing
            if (createDto.EffectiveTo == null)
            {
                var existingOpen = await _context.RegistrarTldCostPricing
                    .Where(c => c.RegistrarTldId == createDto.RegistrarTldId &&
                               c.EffectiveTo == null &&
                               c.IsActive)
                    .ToListAsync();

                foreach (var existing in existingOpen)
                {
                    existing.EffectiveTo = createDto.EffectiveFrom.AddSeconds(-1);
                }
            }

            var costPricing = new RegistrarTldCostPricing
            {
                RegistrarTldId = createDto.RegistrarTldId,
                EffectiveFrom = createDto.EffectiveFrom,
                EffectiveTo = createDto.EffectiveTo,
                RegistrationCost = createDto.RegistrationCost,
                RenewalCost = createDto.RenewalCost,
                TransferCost = createDto.TransferCost,
                PrivacyCost = createDto.PrivacyCost,
                FirstYearRegistrationCost = createDto.FirstYearRegistrationCost,
                Currency = createDto.Currency,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes,
                CreatedBy = createdBy
            };

            _context.RegistrarTldCostPricing.Add(costPricing);
            await _context.SaveChangesAsync();

            await CreateManualPriceChangeLogAsync(
                createDto.RegistrarTldId,
                null,
                costPricing,
                createdBy,
                "Manual cost pricing created");

            _log.Information("Successfully created cost pricing {CostPricingId} for RegistrarTld {RegistrarTldId}",
                costPricing.Id, createDto.RegistrarTldId);

            // Reload with navigation properties
            var created = await _context.RegistrarTldCostPricing
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Registrar)
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Tld)
                .FirstAsync(c => c.Id == costPricing.Id);

            return MapCostPricingToDto(created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating cost pricing for RegistrarTld {RegistrarTldId}", createDto.RegistrarTldId);
            throw;
        }
    }

    public async Task<RegistrarTldCostPricingDto?> UpdateCostPricingAsync(int id, UpdateRegistrarTldCostPricingDto updateDto, string? modifiedBy)
    {
        try
        {
            _log.Information("Updating cost pricing {CostPricingId}", id);

            var costPricing = await _context.RegistrarTldCostPricing
                .FindAsync(id);

            if (costPricing == null)
            {
                _log.Warning("Cost pricing {CostPricingId} not found", id);
                return null;
            }

            // Validate can edit (only future prices)
            if (!_futurePricingManager.CanEdit(costPricing.EffectiveFrom))
            {
                throw new InvalidOperationException("Cannot edit pricing that is already effective or past");
            }

            // Validate new schedule date
            var (isValid, errorMessage) = _futurePricingManager.ValidateScheduleDate(updateDto.EffectiveFrom);
            if (!isValid)
            {
                throw new InvalidOperationException(errorMessage);
            }

            // Update fields
            var previousSnapshot = new RegistrarTldCostPricing
            {
                RegistrarTldId = costPricing.RegistrarTldId,
                RegistrationCost = costPricing.RegistrationCost,
                RenewalCost = costPricing.RenewalCost,
                TransferCost = costPricing.TransferCost,
                Currency = costPricing.Currency
            };

            costPricing.EffectiveFrom = updateDto.EffectiveFrom;
            costPricing.EffectiveTo = updateDto.EffectiveTo;
            costPricing.RegistrationCost = updateDto.RegistrationCost;
            costPricing.RenewalCost = updateDto.RenewalCost;
            costPricing.TransferCost = updateDto.TransferCost;
            costPricing.PrivacyCost = updateDto.PrivacyCost;
            costPricing.FirstYearRegistrationCost = updateDto.FirstYearRegistrationCost;
            costPricing.Currency = updateDto.Currency;
            costPricing.IsActive = updateDto.IsActive;
            costPricing.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            await CreateManualPriceChangeLogAsync(
                costPricing.RegistrarTldId,
                previousSnapshot,
                costPricing,
                modifiedBy,
                "Manual cost pricing updated");

            _log.Information("Successfully updated cost pricing {CostPricingId}", id);

            // Reload with navigation properties
            var updated = await _context.RegistrarTldCostPricing
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Registrar)
                .Include(c => c.RegistrarTld)
                    .ThenInclude(rt => rt.Tld)
                .FirstAsync(c => c.Id == id);

            return MapCostPricingToDto(updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating cost pricing {CostPricingId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteFutureCostPricingAsync(int id)
    {
        try
        {
            _log.Information("Deleting future cost pricing {CostPricingId}", id);

            var costPricing = await _context.RegistrarTldCostPricing.FindAsync(id);

            if (costPricing == null)
            {
                _log.Warning("Cost pricing {CostPricingId} not found", id);
                return false;
            }

            // Validate can delete (only future prices)
            if (!_futurePricingManager.CanDelete(costPricing.EffectiveFrom))
            {
                throw new InvalidOperationException("Cannot delete pricing that is already effective or past");
            }

            _context.RegistrarTldCostPricing.Remove(costPricing);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted cost pricing {CostPricingId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting cost pricing {CostPricingId}", id);
            throw;
        }
    }

    // ==================== TldSalesPricing Methods ====================

    private async Task CreateManualPriceChangeLogAsync(
        int registrarTldId,
        RegistrarTldCostPricing? previous,
        RegistrarTldCostPricing current,
        string? changedBy,
        string notes)
    {
        _context.RegistrarTldPriceChangeLogs.Add(new RegistrarTldPriceChangeLog
        {
            RegistrarTldId = registrarTldId,
            ChangeSource = "Manual",
            ChangedBy = string.IsNullOrWhiteSpace(changedBy) ? "manual" : changedBy,
            ChangedAtUtc = DateTime.UtcNow,
            OldRegistrationCost = previous?.RegistrationCost,
            NewRegistrationCost = current.RegistrationCost,
            OldRenewalCost = previous?.RenewalCost,
            NewRenewalCost = current.RenewalCost,
            OldTransferCost = previous?.TransferCost,
            NewTransferCost = current.TransferCost,
            OldCurrency = previous?.Currency,
            NewCurrency = current.Currency,
            Notes = notes
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<TldSalesPricingDto>> GetSalesPricingHistoryAsync(int tldId, bool includeArchived = false)
    {
        try
        {
            _log.Information("Fetching sales pricing history for TLD {TldId}, IncludeArchived: {IncludeArchived}",
                tldId, includeArchived);

            var query = _context.TldSalesPricing
                .AsNoTracking()
                .Include(s => s.Tld)
                .Where(s => s.TldId == tldId);

            if (!includeArchived)
            {
                query = query.Where(s => s.IsActive);
            }

            var salesPricing = await query
                .OrderByDescending(s => s.EffectiveFrom)
                .ToListAsync();

            var dtos = salesPricing.Select(MapSalesPricingToDto).ToList();

            _log.Information("Successfully fetched {Count} sales pricing records", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching sales pricing history for TLD {TldId}", tldId);
            throw;
        }
    }

    public async Task<TldSalesPricingDto?> GetCurrentSalesPricingAsync(int tldId, DateTime? effectiveDate = null)
    {
        try
        {
            var checkDate = effectiveDate ?? DateTime.UtcNow;
            _log.Information("Fetching current sales pricing for TLD {TldId} at {EffectiveDate}",
                tldId, checkDate);

            var salesPricing = await _context.TldSalesPricing
                .AsNoTracking()
                .Include(s => s.Tld)
                .Where(s => s.TldId == tldId &&
                           s.IsActive &&
                           s.EffectiveFrom <= checkDate &&
                           (s.EffectiveTo == null || s.EffectiveTo > checkDate))
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (salesPricing == null)
            {
                _log.Warning("No current sales pricing found for TLD {TldId}", tldId);
                return null;
            }

            return MapSalesPricingToDto(salesPricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching current sales pricing for TLD {TldId}", tldId);
            throw;
        }
    }

    public async Task<List<TldSalesPricingDto>> GetFutureSalesPricingAsync(int tldId)
    {
        try
        {
            _log.Information("Fetching future sales pricing for TLD {TldId}", tldId);

            var now = DateTime.UtcNow;
            var futurePricing = await _context.TldSalesPricing
                .AsNoTracking()
                .Include(s => s.Tld)
                .Where(s => s.TldId == tldId &&
                           s.IsActive &&
                           s.EffectiveFrom > now)
                .OrderBy(s => s.EffectiveFrom)
                .ToListAsync();

            var dtos = futurePricing.Select(MapSalesPricingToDto).ToList();

            _log.Information("Successfully fetched {Count} future sales pricing records", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching future sales pricing for TLD {TldId}", tldId);
            throw;
        }
    }

    public async Task<TldSalesPricingDto> CreateSalesPricingAsync(CreateTldSalesPricingDto createDto, string? createdBy)
    {
        try
        {
            _log.Information("Creating sales pricing for TLD {TldId}", createDto.TldId);

            // Validate schedule date
            var (isValid, errorMessage) = _futurePricingManager.ValidateScheduleDate(createDto.EffectiveFrom);
            if (!isValid)
            {
                throw new InvalidOperationException(errorMessage);
            }

            // If EffectiveTo is null, close any existing open pricing
            if (createDto.EffectiveTo == null)
            {
                var existingOpen = await _context.TldSalesPricing
                    .Where(s => s.TldId == createDto.TldId &&
                               s.EffectiveTo == null &&
                               s.IsActive)
                    .ToListAsync();

                foreach (var existing in existingOpen)
                {
                    existing.EffectiveTo = createDto.EffectiveFrom.AddSeconds(-1);
                }
            }

            var salesPricing = new TldSalesPricing
            {
                TldId = createDto.TldId,
                EffectiveFrom = createDto.EffectiveFrom,
                EffectiveTo = createDto.EffectiveTo,
                RegistrationPrice = createDto.RegistrationPrice,
                RenewalPrice = createDto.RenewalPrice,
                TransferPrice = createDto.TransferPrice,
                PrivacyPrice = createDto.PrivacyPrice,
                FirstYearRegistrationPrice = createDto.FirstYearRegistrationPrice,
                Currency = createDto.Currency,
                IsPromotional = createDto.IsPromotional,
                PromotionName = createDto.PromotionName,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes,
                CreatedBy = createdBy
            };

            _context.TldSalesPricing.Add(salesPricing);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created sales pricing {SalesPricingId} for TLD {TldId}",
                salesPricing.Id, createDto.TldId);

            // Reload with navigation properties
            var created = await _context.TldSalesPricing
                .Include(s => s.Tld)
                .FirstAsync(s => s.Id == salesPricing.Id);

            return MapSalesPricingToDto(created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating sales pricing for TLD {TldId}", createDto.TldId);
            throw;
        }
    }

    public async Task<TldSalesPricingDto?> UpdateSalesPricingAsync(int id, UpdateTldSalesPricingDto updateDto, string? modifiedBy)
    {
        try
        {
            _log.Information("Updating sales pricing {SalesPricingId}", id);

            var salesPricing = await _context.TldSalesPricing.FindAsync(id);

            if (salesPricing == null)
            {
                _log.Warning("Sales pricing {SalesPricingId} not found", id);
                return null;
            }

            // Validate can edit (only future prices)
            if (!_futurePricingManager.CanEdit(salesPricing.EffectiveFrom))
            {
                throw new InvalidOperationException("Cannot edit pricing that is already effective or past");
            }

            // Validate new schedule date
            var (isValid, errorMessage) = _futurePricingManager.ValidateScheduleDate(updateDto.EffectiveFrom);
            if (!isValid)
            {
                throw new InvalidOperationException(errorMessage);
            }

            // Update fields
            salesPricing.EffectiveFrom = updateDto.EffectiveFrom;
            salesPricing.EffectiveTo = updateDto.EffectiveTo;
            salesPricing.RegistrationPrice = updateDto.RegistrationPrice;
            salesPricing.RenewalPrice = updateDto.RenewalPrice;
            salesPricing.TransferPrice = updateDto.TransferPrice;
            salesPricing.PrivacyPrice = updateDto.PrivacyPrice;
            salesPricing.FirstYearRegistrationPrice = updateDto.FirstYearRegistrationPrice;
            salesPricing.Currency = updateDto.Currency;
            salesPricing.IsPromotional = updateDto.IsPromotional;
            salesPricing.PromotionName = updateDto.PromotionName;
            salesPricing.IsActive = updateDto.IsActive;
            salesPricing.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated sales pricing {SalesPricingId}", id);

            // Reload with navigation properties
            var updated = await _context.TldSalesPricing
                .Include(s => s.Tld)
                .FirstAsync(s => s.Id == id);

            return MapSalesPricingToDto(updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating sales pricing {SalesPricingId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteFutureSalesPricingAsync(int id)
    {
        try
        {
            _log.Information("Deleting future sales pricing {SalesPricingId}", id);

            var salesPricing = await _context.TldSalesPricing.FindAsync(id);

            if (salesPricing == null)
            {
                _log.Warning("Sales pricing {SalesPricingId} not found", id);
                return false;
            }

            // Validate can delete (only future prices)
            if (!_futurePricingManager.CanDelete(salesPricing.EffectiveFrom))
            {
                throw new InvalidOperationException("Cannot delete pricing that is already effective or past");
            }

            _context.TldSalesPricing.Remove(salesPricing);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted sales pricing {SalesPricingId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting sales pricing {SalesPricingId}", id);
            throw;
        }
    }

    // ==================== Helper Methods for Mapping ====================

    private static RegistrarTldCostPricingDto MapCostPricingToDto(RegistrarTldCostPricing entity)
    {
        return new RegistrarTldCostPricingDto
        {
            Id = entity.Id,
            RegistrarTldId = entity.RegistrarTldId,
            RegistrarName = entity.RegistrarTld?.Registrar?.Name,
            TldExtension = entity.RegistrarTld?.Tld?.Extension,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            RegistrationCost = entity.RegistrationCost,
            RenewalCost = entity.RenewalCost,
            TransferCost = entity.TransferCost,
            PrivacyCost = entity.PrivacyCost,
            FirstYearRegistrationCost = entity.FirstYearRegistrationCost,
            Currency = entity.Currency,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static TldSalesPricingDto MapSalesPricingToDto(TldSalesPricing entity)
    {
        return new TldSalesPricingDto
        {
            Id = entity.Id,
            TldId = entity.TldId,
            TldExtension = entity.Tld?.Extension,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            RegistrationPrice = entity.RegistrationPrice,
            RenewalPrice = entity.RenewalPrice,
            TransferPrice = entity.TransferPrice,
            PrivacyPrice = entity.PrivacyPrice,
            FirstYearRegistrationPrice = entity.FirstYearRegistrationPrice,
            Currency = entity.Currency,
            IsPromotional = entity.IsPromotional,
            PromotionName = entity.PromotionName,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Note: Continuing in next part due to file size...
    // The remaining methods will be implemented for:
    // - ResellerTldDiscount operations
    // - RegistrarSelectionPreference operations
    // - Pricing calculations
    // - Margin analysis
    // - Currency conversion
    // - Archive management
}
