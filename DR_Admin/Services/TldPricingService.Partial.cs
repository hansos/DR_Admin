using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Partial class containing Discount, Preference, Calculation, and Archive methods for TldPricingService
/// </summary>
public partial class TldPricingService
{
    // ==================== ResellerTldDiscount Methods ====================

    public async Task<List<ResellerTldDiscountDto>> GetResellerDiscountsAsync(int resellerCompanyId, bool includeArchived = false)
    {
        try
        {
            _log.Information("Fetching discounts for ResellerCompany {ResellerCompanyId}", resellerCompanyId);

            var query = _context.ResellerTldDiscounts
                .AsNoTracking()
                .Include(d => d.ResellerCompany)
                .Include(d => d.Tld)
                .Where(d => d.ResellerCompanyId == resellerCompanyId);

            if (!includeArchived)
            {
                query = query.Where(d => d.IsActive);
            }

            var discounts = await query
                .OrderByDescending(d => d.EffectiveFrom)
                .ToListAsync();

            var dtos = discounts.Select(MapDiscountToDto).ToList();

            _log.Information("Successfully fetched {Count} discount records", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching discounts for ResellerCompany {ResellerCompanyId}", resellerCompanyId);
            throw;
        }
    }

    public async Task<ResellerTldDiscountDto?> GetCurrentDiscountAsync(int resellerCompanyId, int tldId, DateTime? effectiveDate = null)
    {
        try
        {
            var checkDate = effectiveDate ?? DateTime.UtcNow;
            _log.Information("Fetching current discount for ResellerCompany {ResellerCompanyId} and TLD {TldId}",
                resellerCompanyId, tldId);

            var discount = await _context.ResellerTldDiscounts
                .AsNoTracking()
                .Include(d => d.ResellerCompany)
                .Include(d => d.Tld)
                .Where(d => d.ResellerCompanyId == resellerCompanyId &&
                           d.TldId == tldId &&
                           d.IsActive &&
                           d.EffectiveFrom <= checkDate &&
                           (d.EffectiveTo == null || d.EffectiveTo > checkDate))
                .OrderByDescending(d => d.EffectiveFrom)
                .FirstOrDefaultAsync();

            return discount == null ? null : MapDiscountToDto(discount);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching current discount");
            throw;
        }
    }

    public async Task<ResellerTldDiscountDto> CreateDiscountAsync(CreateResellerTldDiscountDto createDto, string? createdBy)
    {
        try
        {
            _log.Information("Creating discount for ResellerCompany {ResellerCompanyId} and TLD {TldId}",
                createDto.ResellerCompanyId, createDto.TldId);

            // Validate that either percentage or amount is provided, not both
            if (createDto.DiscountPercentage.HasValue && createDto.DiscountAmount.HasValue)
            {
                throw new InvalidOperationException("Cannot specify both DiscountPercentage and DiscountAmount");
            }

            if (!createDto.DiscountPercentage.HasValue && !createDto.DiscountAmount.HasValue)
            {
                throw new InvalidOperationException("Must specify either DiscountPercentage or DiscountAmount");
            }

            var discount = new ResellerTldDiscount
            {
                ResellerCompanyId = createDto.ResellerCompanyId,
                TldId = createDto.TldId,
                EffectiveFrom = createDto.EffectiveFrom,
                EffectiveTo = createDto.EffectiveTo,
                DiscountPercentage = createDto.DiscountPercentage,
                DiscountAmount = createDto.DiscountAmount,
                DiscountCurrency = createDto.DiscountCurrency,
                ApplyToRegistration = createDto.ApplyToRegistration,
                ApplyToRenewal = createDto.ApplyToRenewal,
                ApplyToTransfer = createDto.ApplyToTransfer,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes,
                CreatedBy = createdBy
            };

            _context.ResellerTldDiscounts.Add(discount);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created discount {DiscountId}", discount.Id);

            var created = await _context.ResellerTldDiscounts
                .Include(d => d.ResellerCompany)
                .Include(d => d.Tld)
                .FirstAsync(d => d.Id == discount.Id);

            return MapDiscountToDto(created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating discount");
            throw;
        }
    }

    public async Task<ResellerTldDiscountDto?> UpdateDiscountAsync(int id, UpdateResellerTldDiscountDto updateDto, string? modifiedBy)
    {
        try
        {
            _log.Information("Updating discount {DiscountId}", id);

            var discount = await _context.ResellerTldDiscounts.FindAsync(id);

            if (discount == null)
            {
                _log.Warning("Discount {DiscountId} not found", id);
                return null;
            }

            // Validate that either percentage or amount is provided, not both
            if (updateDto.DiscountPercentage.HasValue && updateDto.DiscountAmount.HasValue)
            {
                throw new InvalidOperationException("Cannot specify both DiscountPercentage and DiscountAmount");
            }

            discount.EffectiveFrom = updateDto.EffectiveFrom;
            discount.EffectiveTo = updateDto.EffectiveTo;
            discount.DiscountPercentage = updateDto.DiscountPercentage;
            discount.DiscountAmount = updateDto.DiscountAmount;
            discount.DiscountCurrency = updateDto.DiscountCurrency;
            discount.ApplyToRegistration = updateDto.ApplyToRegistration;
            discount.ApplyToRenewal = updateDto.ApplyToRenewal;
            discount.ApplyToTransfer = updateDto.ApplyToTransfer;
            discount.IsActive = updateDto.IsActive;
            discount.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated discount {DiscountId}", id);

            var updated = await _context.ResellerTldDiscounts
                .Include(d => d.ResellerCompany)
                .Include(d => d.Tld)
                .FirstAsync(d => d.Id == id);

            return MapDiscountToDto(updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating discount {DiscountId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        try
        {
            _log.Information("Deleting discount {DiscountId}", id);

            var discount = await _context.ResellerTldDiscounts.FindAsync(id);

            if (discount == null)
            {
                _log.Warning("Discount {DiscountId} not found", id);
                return false;
            }

            _context.ResellerTldDiscounts.Remove(discount);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted discount {DiscountId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting discount {DiscountId}", id);
            throw;
        }
    }

    // ==================== RegistrarSelectionPreference Methods ====================

    public async Task<List<RegistrarSelectionPreferenceDto>> GetAllSelectionPreferencesAsync()
    {
        try
        {
            _log.Information("Fetching all selection preferences");

            var preferences = await _context.RegistrarSelectionPreferences
                .AsNoTracking()
                .Include(p => p.Registrar)
                .OrderBy(p => p.Priority)
                .ToListAsync();

            var dtos = preferences.Select(MapPreferenceToDto).ToList();

            _log.Information("Successfully fetched {Count} selection preferences", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching selection preferences");
            throw;
        }
    }

    public async Task<RegistrarSelectionPreferenceDto?> GetSelectionPreferenceAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching selection preference for Registrar {RegistrarId}", registrarId);

            var preference = await _context.RegistrarSelectionPreferences
                .AsNoTracking()
                .Include(p => p.Registrar)
                .FirstOrDefaultAsync(p => p.RegistrarId == registrarId && p.IsActive);

            return preference == null ? null : MapPreferenceToDto(preference);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching selection preference for Registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<RegistrarSelectionPreferenceDto> CreateSelectionPreferenceAsync(CreateRegistrarSelectionPreferenceDto createDto)
    {
        try
        {
            _log.Information("Creating selection preference for Registrar {RegistrarId}", createDto.RegistrarId);

            var preference = new RegistrarSelectionPreference
            {
                RegistrarId = createDto.RegistrarId,
                Priority = createDto.Priority,
                OffersHosting = createDto.OffersHosting,
                OffersEmail = createDto.OffersEmail,
                OffersSsl = createDto.OffersSsl,
                MaxCostDifferenceThreshold = createDto.MaxCostDifferenceThreshold,
                PreferForHostingCustomers = createDto.PreferForHostingCustomers,
                PreferForEmailCustomers = createDto.PreferForEmailCustomers,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes
            };

            _context.RegistrarSelectionPreferences.Add(preference);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created selection preference {PreferenceId}", preference.Id);

            var created = await _context.RegistrarSelectionPreferences
                .Include(p => p.Registrar)
                .FirstAsync(p => p.Id == preference.Id);

            return MapPreferenceToDto(created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating selection preference");
            throw;
        }
    }

    public async Task<RegistrarSelectionPreferenceDto?> UpdateSelectionPreferenceAsync(int id, UpdateRegistrarSelectionPreferenceDto updateDto)
    {
        try
        {
            _log.Information("Updating selection preference {PreferenceId}", id);

            var preference = await _context.RegistrarSelectionPreferences.FindAsync(id);

            if (preference == null)
            {
                _log.Warning("Selection preference {PreferenceId} not found", id);
                return null;
            }

            preference.Priority = updateDto.Priority;
            preference.OffersHosting = updateDto.OffersHosting;
            preference.OffersEmail = updateDto.OffersEmail;
            preference.OffersSsl = updateDto.OffersSsl;
            preference.MaxCostDifferenceThreshold = updateDto.MaxCostDifferenceThreshold;
            preference.PreferForHostingCustomers = updateDto.PreferForHostingCustomers;
            preference.PreferForEmailCustomers = updateDto.PreferForEmailCustomers;
            preference.IsActive = updateDto.IsActive;
            preference.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated selection preference {PreferenceId}", id);

            var updated = await _context.RegistrarSelectionPreferences
                .Include(p => p.Registrar)
                .FirstAsync(p => p.Id == id);

            return MapPreferenceToDto(updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating selection preference {PreferenceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteSelectionPreferenceAsync(int id)
    {
        try
        {
            _log.Information("Deleting selection preference {PreferenceId}", id);

            var preference = await _context.RegistrarSelectionPreferences.FindAsync(id);

            if (preference == null)
            {
                _log.Warning("Selection preference {PreferenceId} not found", id);
                return false;
            }

            _context.RegistrarSelectionPreferences.Remove(preference);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted selection preference {PreferenceId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting selection preference {PreferenceId}", id);
            throw;
        }
    }

    // ==================== Pricing Calculation Methods ====================

    public async Task<CalculatePricingResponse> CalculatePricingAsync(CalculatePricingRequest request)
    {
        try
        {
            _log.Information("Calculating pricing for TLD {TldId}, Operation: {OperationType}",
                request.TldId, request.OperationType);

            // Get current sales pricing
            var salesPricing = await GetCurrentSalesPricingAsync(request.TldId);
            if (salesPricing == null)
            {
                throw new InvalidOperationException($"No pricing found for TLD {request.TldId}");
            }

            // Get base price based on operation type
            decimal basePrice = request.OperationType.ToLower() switch
            {
                "registration" => request.IsFirstYear && salesPricing.FirstYearRegistrationPrice.HasValue
                    ? salesPricing.FirstYearRegistrationPrice.Value
                    : salesPricing.RegistrationPrice,
                "renewal" => salesPricing.RenewalPrice,
                "transfer" => salesPricing.TransferPrice,
                _ => salesPricing.RegistrationPrice
            };

            // Multiply by years
            basePrice *= request.Years;

            decimal discountAmount = 0;
            bool isDiscountApplied = false;
            string? discountDescription = null;

            // Apply discount if reseller company is specified
            if (request.ResellerCompanyId.HasValue)
            {
                var discount = await GetCurrentDiscountAsync(request.ResellerCompanyId.Value, request.TldId);
                if (discount != null && discount.IsActive)
                {
                    // Check if discount applies to this operation
                    bool applies = request.OperationType.ToLower() switch
                    {
                        "registration" => discount.ApplyToRegistration,
                        "renewal" => discount.ApplyToRenewal,
                        "transfer" => discount.ApplyToTransfer,
                        _ => false
                    };

                    if (applies)
                    {
                        // Check if we should stack discounts
                        bool canApplyDiscount = !salesPricing.IsPromotional || _settings.AllowDiscountStacking;

                        if (canApplyDiscount)
                        {
                            if (discount.DiscountPercentage.HasValue)
                            {
                                discountAmount = basePrice * (discount.DiscountPercentage.Value / 100);
                                discountDescription = $"{discount.DiscountPercentage.Value}% discount";
                            }
                            else if (discount.DiscountAmount.HasValue)
                            {
                                discountAmount = discount.DiscountAmount.Value * request.Years;
                                discountDescription = $"{discount.DiscountAmount.Value} {discount.DiscountCurrency} discount per year";
                            }

                            isDiscountApplied = true;
                        }
                    }
                }
            }

            var finalPrice = basePrice - discountAmount;

            // Select optimal registrar
            var selectedRegistrarId = await SelectOptimalRegistrarAsync(request.TldId);

            // Get TLD info
            var tld = await _context.Tlds.FindAsync(request.TldId);

            var response = new CalculatePricingResponse
            {
                TldExtension = tld?.Extension,
                BasePrice = basePrice,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice,
                Currency = salesPricing.Currency,
                IsPromotionalPricing = salesPricing.IsPromotional,
                PromotionName = salesPricing.PromotionName,
                IsDiscountApplied = isDiscountApplied,
                DiscountDescription = discountDescription,
                SelectedRegistrarId = selectedRegistrarId
            };

            // Get registrar name if selected
            if (selectedRegistrarId.HasValue)
            {
                var registrar = await _context.Registrars.FindAsync(selectedRegistrarId.Value);
                response.SelectedRegistrarName = registrar?.Name;
            }

            _log.Information("Pricing calculated: Base={BasePrice}, Discount={DiscountAmount}, Final={FinalPrice}",
                basePrice, discountAmount, finalPrice);

            return response;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error calculating pricing");
            throw;
        }
    }

    public async Task<int?> SelectOptimalRegistrarAsync(int tldId, int? customerId = null)
    {
        try
        {
            _log.Information("Selecting optimal registrar for TLD {TldId}", tldId);

            // Get all registrars offering this TLD with current cost pricing
            var registrarCosts = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.CostPricingHistory)
                .Where(rt => rt.TldId == tldId && rt.IsActive && rt.Registrar.IsActive)
                .ToListAsync();

            if (!registrarCosts.Any())
            {
                _log.Warning("No active registrars found for TLD {TldId}", tldId);
                return null;
            }

            // Get current cost pricing for each
            var now = DateTime.UtcNow;
            var registrarWithCosts = new List<(int RegistrarId, decimal RegistrationCost)>();

            foreach (var rt in registrarCosts)
            {
                var currentCost = rt.CostPricingHistory
                    .Where(c => c.IsActive &&
                               c.EffectiveFrom <= now &&
                               (c.EffectiveTo == null || c.EffectiveTo > now))
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefault();

                if (currentCost != null)
                {
                    registrarWithCosts.Add((rt.RegistrarId, currentCost.RegistrationCost));
                }
            }

            if (!registrarWithCosts.Any())
            {
                _log.Warning("No registrars with current cost pricing for TLD {TldId}", tldId);
                return registrarCosts.First().RegistrarId; // Fallback to first available
            }

            // Find cheapest
            var cheapest = registrarWithCosts.OrderBy(r => r.RegistrationCost).First();

            _log.Information("Selected registrar {RegistrarId} with cost {Cost} for TLD {TldId}",
                cheapest.RegistrarId, cheapest.RegistrationCost, tldId);

            return cheapest.RegistrarId;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error selecting optimal registrar for TLD {TldId}", tldId);
            throw;
        }
    }

    // ==================== Margin Analysis Methods ====================

    public async Task<MarginAnalysisResult> CalculateMarginAsync(int tldId, string operationType, int? registrarId = null)
    {
        try
        {
            _log.Information("Calculating margin for TLD {TldId}, Operation: {OperationType}", tldId, operationType);

            // Get sales pricing
            var salesPricing = await GetCurrentSalesPricingAsync(tldId);
            if (salesPricing == null)
            {
                throw new InvalidOperationException($"No sales pricing found for TLD {tldId}");
            }

            // Get price based on operation type
            decimal price = operationType.ToLower() switch
            {
                "registration" => salesPricing.RegistrationPrice,
                "renewal" => salesPricing.RenewalPrice,
                "transfer" => salesPricing.TransferPrice,
                _ => salesPricing.RegistrationPrice
            };

            // Select registrar if not specified
            var selectedRegistrarId = registrarId ?? await SelectOptimalRegistrarAsync(tldId);
            if (!selectedRegistrarId.HasValue)
            {
                throw new InvalidOperationException($"No registrar available for TLD {tldId}");
            }

            // Get cost pricing
            var registrarTld = await _context.RegistrarTlds
                .FirstOrDefaultAsync(rt => rt.TldId == tldId && rt.RegistrarId == selectedRegistrarId.Value);

            if (registrarTld == null)
            {
                throw new InvalidOperationException($"No registrar-TLD relationship found");
            }

            var costPricing = await GetCurrentCostPricingAsync(registrarTld.Id);
            if (costPricing == null)
            {
                throw new InvalidOperationException($"No cost pricing found for registrar-TLD {registrarTld.Id}");
            }

            // Get cost based on operation type
            decimal cost = operationType.ToLower() switch
            {
                "registration" => costPricing.RegistrationCost,
                "renewal" => costPricing.RenewalCost,
                "transfer" => costPricing.TransferCost,
                _ => costPricing.RegistrationCost
            };

            // Validate margin
            var marginValidation = _marginValidator.ValidateMargin(cost, price, costPricing.Currency, salesPricing.Currency);

            var result = new MarginAnalysisResult
            {
                TldId = tldId,
                TldExtension = salesPricing.TldExtension,
                RegistrarId = selectedRegistrarId.Value,
                RegistrarName = costPricing.RegistrarName,
                OperationType = operationType,
                Cost = cost,
                Price = price,
                MarginAmount = marginValidation.MarginAmount,
                MarginPercentage = marginValidation.MarginPercentage,
                CostCurrency = costPricing.Currency,
                PriceCurrency = salesPricing.Currency,
                IsNegativeMargin = marginValidation.IsNegativeMargin,
                IsLowMargin = marginValidation.IsLowMargin,
                AlertMessage = marginValidation.AlertMessage
            };

            _log.Information("Margin calculated: {MarginPercentage}% ({MarginAmount})", 
                result.MarginPercentage, result.MarginAmount);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error calculating margin");
            throw;
        }
    }

    public async Task<List<MarginAnalysisResult>> GetNegativeMarginReportAsync()
    {
        try
        {
            _log.Information("Generating negative margin report");

            var results = new List<MarginAnalysisResult>();
            var tlds = await _context.Tlds.Where(t => t.IsActive).ToListAsync();

            foreach (var tld in tlds)
            {
                try
                {
                    var margin = await CalculateMarginAsync(tld.Id, "Registration");
                    if (margin.IsNegativeMargin)
                    {
                        results.Add(margin);
                    }
                }
                catch
                {
                    // Skip TLDs with no pricing
                }
            }

            _log.Information("Found {Count} TLDs with negative margins", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error generating negative margin report");
            throw;
        }
    }

    public async Task<List<MarginAnalysisResult>> GetLowMarginReportAsync()
    {
        try
        {
            _log.Information("Generating low margin report");

            var results = new List<MarginAnalysisResult>();
            var tlds = await _context.Tlds.Where(t => t.IsActive).ToListAsync();

            foreach (var tld in tlds)
            {
                try
                {
                    var margin = await CalculateMarginAsync(tld.Id, "Registration");
                    if (margin.IsLowMargin || margin.IsNegativeMargin)
                    {
                        results.Add(margin);
                    }
                }
                catch
                {
                    // Skip TLDs with no pricing
                }
            }

            _log.Information("Found {Count} TLDs with low/negative margins", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error generating low margin report");
            throw;
        }
    }

    // ==================== Currency Conversion Methods ====================

    public async Task<decimal?> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, DateTime? effectiveDate = null)
    {
        try
        {
            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            _log.Information("Converting {Amount} from {FromCurrency} to {ToCurrency}",
                amount, fromCurrency, toCurrency);

            var checkDate = effectiveDate ?? DateTime.UtcNow;

            // Get exchange rate
            var rate = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(r => r.BaseCurrency == fromCurrency &&
                           r.TargetCurrency == toCurrency &&
                           r.IsActive &&
                           r.EffectiveDate <= checkDate &&
                           (r.ExpiryDate == null || r.ExpiryDate > checkDate))
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync();

            if (rate == null)
            {
                _log.Warning("No exchange rate found from {FromCurrency} to {ToCurrency}", fromCurrency, toCurrency);
                return null;
            }

            var converted = amount * rate.Rate;

            // Apply markup if configured
            if (_settings.CurrencyConversionMarkup > 0)
            {
                converted *= (1 + (_settings.CurrencyConversionMarkup / 100));
            }

            _log.Information("Converted {Amount} {FromCurrency} to {Converted} {ToCurrency}",
                amount, fromCurrency, converted, toCurrency);

            return converted;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error converting currency");
            throw;
        }
    }

    // ==================== Archive Management Methods ====================

    public async Task<int> ArchiveOldCostPricingAsync()
    {
        try
        {
            _log.Information("Archiving old cost pricing data");

            var cutoffDate = DateTime.UtcNow.AddYears(-_settings.CostPricingRetentionYears);

            var oldRecords = await _context.RegistrarTldCostPricing
                .Where(c => c.IsActive &&
                           c.EffectiveTo.HasValue &&
                           c.EffectiveTo.Value < cutoffDate)
                .ToListAsync();

            foreach (var record in oldRecords)
            {
                record.IsActive = false;
            }

            await _context.SaveChangesAsync();

            _log.Information("Archived {Count} old cost pricing records", oldRecords.Count);
            return oldRecords.Count;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error archiving old cost pricing");
            throw;
        }
    }

    public async Task<int> ArchiveOldSalesPricingAsync()
    {
        try
        {
            _log.Information("Archiving old sales pricing data");

            var cutoffDate = DateTime.UtcNow.AddYears(-_settings.SalesPricingRetentionYears);

            var oldRecords = await _context.TldSalesPricing
                .Where(s => s.IsActive &&
                           s.EffectiveTo.HasValue &&
                           s.EffectiveTo.Value < cutoffDate)
                .ToListAsync();

            foreach (var record in oldRecords)
            {
                record.IsActive = false;
            }

            await _context.SaveChangesAsync();

            _log.Information("Archived {Count} old sales pricing records", oldRecords.Count);
            return oldRecords.Count;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error archiving old sales pricing");
            throw;
        }
    }

    public async Task<int> ArchiveOldDiscountsAsync()
    {
        try
        {
            _log.Information("Archiving old discount data");

            var cutoffDate = DateTime.UtcNow.AddYears(-_settings.DiscountHistoryRetentionYears);

            var oldRecords = await _context.ResellerTldDiscounts
                .Where(d => d.IsActive &&
                           d.EffectiveTo.HasValue &&
                           d.EffectiveTo.Value < cutoffDate)
                .ToListAsync();

            foreach (var record in oldRecords)
            {
                record.IsActive = false;
            }

            await _context.SaveChangesAsync();

            _log.Information("Archived {Count} old discount records", oldRecords.Count);
            return oldRecords.Count;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error archiving old discounts");
            throw;
        }
    }

    // ==================== Helper Methods for Mapping ====================

    private static ResellerTldDiscountDto MapDiscountToDto(ResellerTldDiscount entity)
    {
        return new ResellerTldDiscountDto
        {
            Id = entity.Id,
            ResellerCompanyId = entity.ResellerCompanyId,
            ResellerCompanyName = entity.ResellerCompany?.Name,
            TldId = entity.TldId,
            TldExtension = entity.Tld?.Extension,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveTo = entity.EffectiveTo,
            DiscountPercentage = entity.DiscountPercentage,
            DiscountAmount = entity.DiscountAmount,
            DiscountCurrency = entity.DiscountCurrency,
            ApplyToRegistration = entity.ApplyToRegistration,
            ApplyToRenewal = entity.ApplyToRenewal,
            ApplyToTransfer = entity.ApplyToTransfer,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static RegistrarSelectionPreferenceDto MapPreferenceToDto(RegistrarSelectionPreference entity)
    {
        return new RegistrarSelectionPreferenceDto
        {
            Id = entity.Id,
            RegistrarId = entity.RegistrarId,
            RegistrarName = entity.Registrar?.Name,
            Priority = entity.Priority,
            OffersHosting = entity.OffersHosting,
            OffersEmail = entity.OffersEmail,
            OffersSsl = entity.OffersSsl,
            MaxCostDifferenceThreshold = entity.MaxCostDifferenceThreshold,
            PreferForHostingCustomers = entity.PreferForHostingCustomers,
            PreferForEmailCustomers = entity.PreferForEmailCustomers,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
