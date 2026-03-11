using System.Text.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Services;

/// <summary>
/// Service that calculates and finalizes VAT and TAX values.
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    private readonly ApplicationDbContext _context;
    private readonly IVatValidationService _vatValidationService;

    public TaxCalculationService(ApplicationDbContext context, IVatValidationService vatValidationService)
    {
        _context = context;
        _vatValidationService = vatValidationService;
    }

    /// <summary>
    /// Calculates a tax quote without persisting final tax snapshot.
    /// </summary>
    /// <param name="request">Tax quote request payload.</param>
    /// <returns>Calculated tax quote result.</returns>
    public async Task<TaxQuoteResultDto> QuoteTaxAsync(TaxQuoteRequestDto request)
    {
        return await CalculateCoreAsync(request, persistSnapshot: false);
    }

    /// <summary>
    /// Finalizes a tax calculation and stores immutable tax snapshot for audit.
    /// </summary>
    /// <param name="request">Tax finalize request payload.</param>
    /// <returns>Finalized tax result with snapshot identifier.</returns>
    public async Task<TaxQuoteResultDto> FinalizeTaxAsync(TaxQuoteRequestDto request)
    {
        if (!request.OrderId.HasValue || request.OrderId.Value <= 0)
        {
            throw new InvalidOperationException("OrderId is required for tax finalization.");
        }

        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existing = await _context.OrderTaxSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderId == request.OrderId.Value && x.IdempotencyKey == request.IdempotencyKey);

            if (existing != null)
            {
                return new TaxQuoteResultDto
                {
                    SnapshotId = existing.Id,
                    OrderId = existing.OrderId,
                    TaxJurisdictionId = existing.TaxJurisdictionId,
                    TaxName = existing.AppliedTaxName,
                    TaxRate = existing.AppliedTaxRate,
                    ReverseChargeApplied = existing.ReverseChargeApplied,
                    RuleVersion = existing.RuleVersion,
                    TaxCurrencyCode = existing.TaxCurrencyCode,
                    DisplayCurrencyCode = existing.DisplayCurrencyCode,
                    NetAmount = existing.NetAmount,
                    TaxAmount = existing.TaxAmount,
                    GrossAmount = existing.GrossAmount,
                    BuyerTaxIdValidated = existing.BuyerTaxIdValidated,
                    LegalNote = existing.ReverseChargeApplied ? "Reverse charge applied" : string.Empty,
                    Lines = new List<TaxQuoteLineResultDto>()
                };
            }
        }

        return await CalculateCoreAsync(request, persistSnapshot: true);
    }

    private async Task<TaxQuoteResultDto> CalculateCoreAsync(TaxQuoteRequestDto request, bool persistSnapshot)
    {
        if (request.Lines.Count == 0)
        {
            throw new InvalidOperationException("At least one line is required for tax calculation.");
        }

        var buyerCountry = request.BuyerCountryCode;
        var buyerState = request.BuyerStateCode;
        var buyerType = request.BuyerType;
        var buyerTaxId = request.BuyerTaxId;

        if (request.CustomerId.HasValue)
        {
            var profile = await _context.CustomerTaxProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId.Value);

            if (profile != null)
            {
                if (string.IsNullOrWhiteSpace(buyerCountry))
                {
                    buyerCountry = profile.TaxResidenceCountry;
                }

                if (string.IsNullOrWhiteSpace(buyerTaxId))
                {
                    buyerTaxId = profile.TaxIdNumber ?? string.Empty;
                }

                buyerType = profile.CustomerType;
            }
        }

        buyerCountry = NormalizeCountryCode(buyerCountry);
        buyerState = NormalizeStateCode(buyerState);

        var validatedTaxId = !string.IsNullOrWhiteSpace(buyerTaxId)
            && (!request.ValidateBuyerTaxId || await _vatValidationService.ValidateAsync(buyerCountry, buyerTaxId));

        var transactionDate = request.TransactionDate == default ? DateTime.UtcNow : request.TransactionDate;
        var taxRule = await GetApplicableTaxRuleAsync(buyerCountry, buyerState, transactionDate);

        var ruleTaxRate = taxRule?.TaxRate ?? 0m;
        var reverseChargeApplied = taxRule?.ReverseCharge == true && buyerType == CustomerType.B2B && validatedTaxId;
        var effectiveRate = reverseChargeApplied ? 0m : ruleTaxRate;
        var decimals = GetCurrencyDecimals(string.IsNullOrWhiteSpace(request.DisplayCurrencyCode)
            ? request.TaxCurrencyCode
            : request.DisplayCurrencyCode);

        var lineResults = request.Lines.Select(line =>
        {
            var net = Math.Max(0m, line.NetAmount);
            var tax = Round(net * effectiveRate, decimals);
            var gross = Round(net + tax, decimals);

            return new TaxQuoteLineResultDto
            {
                LineId = line.LineId,
                Description = line.Description,
                NetAmount = Round(net, decimals),
                TaxRate = effectiveRate,
                TaxAmount = tax,
                GrossAmount = gross
            };
        }).ToList();

        var netAmount = Round(lineResults.Sum(x => x.NetAmount), decimals);
        var taxAmount = Round(lineResults.Sum(x => x.TaxAmount), decimals);
        var grossAmount = Round(netAmount + taxAmount, decimals);

        var taxJurisdictionId = taxRule?.TaxJurisdictionId;
        if (!taxJurisdictionId.HasValue)
        {
            taxJurisdictionId = await _context.TaxJurisdictions
                .AsNoTracking()
                .Where(x => x.IsActive && x.CountryCode == buyerCountry && (x.StateCode == buyerState || x.StateCode == null))
                .OrderByDescending(x => x.StateCode == buyerState)
                .ThenBy(x => x.Name)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
        }

        var ruleVersion = taxRule == null
            ? "none"
            : $"taxrule:{taxRule.Id}:{taxRule.UpdatedAt:yyyyMMddHHmmss}";

        var result = new TaxQuoteResultDto
        {
            OrderId = request.OrderId,
            TaxJurisdictionId = taxJurisdictionId,
            TaxName = taxRule?.TaxName ?? "VAT",
            TaxRate = effectiveRate,
            ReverseChargeApplied = reverseChargeApplied,
            RuleVersion = ruleVersion,
            TaxCurrencyCode = NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR"),
            DisplayCurrencyCode = NormalizeCurrencyCode(request.DisplayCurrencyCode, NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR")),
            NetAmount = netAmount,
            TaxAmount = taxAmount,
            GrossAmount = grossAmount,
            BuyerTaxIdValidated = validatedTaxId,
            LegalNote = reverseChargeApplied ? "Reverse charge applied" : string.Empty,
            Lines = lineResults
        };

        if (persistSnapshot)
        {
            var snapshot = new OrderTaxSnapshot
            {
                OrderId = request.OrderId!.Value,
                TaxJurisdictionId = taxJurisdictionId,
                BuyerCountryCode = buyerCountry,
                BuyerStateCode = buyerState,
                BuyerType = buyerType,
                BuyerTaxId = buyerTaxId,
                BuyerTaxIdValidated = validatedTaxId,
                TaxCurrencyCode = result.TaxCurrencyCode,
                DisplayCurrencyCode = result.DisplayCurrencyCode,
                ExchangeRate = request.ExchangeRate,
                ExchangeRateDate = request.ExchangeRateDate,
                NetAmount = netAmount,
                TaxAmount = taxAmount,
                GrossAmount = grossAmount,
                AppliedTaxRate = effectiveRate,
                AppliedTaxName = result.TaxName,
                ReverseChargeApplied = reverseChargeApplied,
                RuleVersion = ruleVersion,
                IdempotencyKey = request.IdempotencyKey,
                CalculationInputsJson = JsonSerializer.Serialize(new
                {
                    request.CustomerId,
                    request.BuyerCountryCode,
                    request.BuyerStateCode,
                    request.BuyerType,
                    request.BuyerTaxId,
                    request.ValidateBuyerTaxId,
                    request.TransactionDate,
                    request.TaxCurrencyCode,
                    request.DisplayCurrencyCode,
                    request.ExchangeRate,
                    request.ExchangeRateDate,
                    request.Lines,
                    AppliedTaxRuleId = taxRule?.Id
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OrderTaxSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();
            result.SnapshotId = snapshot.Id;
        }

        return result;
    }

    private async Task<TaxRule?> GetApplicableTaxRuleAsync(string countryCode, string? stateCode, DateTime when)
    {
        return await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null
                        && x.IsActive
                        && x.CountryCode == countryCode
                        && (x.StateCode == stateCode || x.StateCode == null)
                        && x.EffectiveFrom <= when
                        && (x.EffectiveUntil == null || x.EffectiveUntil >= when))
            .OrderByDescending(x => x.StateCode == stateCode)
            .ThenByDescending(x => x.Priority)
            .ThenByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync();
    }

    private static string NormalizeCountryCode(string countryCode)
    {
        return string.IsNullOrWhiteSpace(countryCode)
            ? string.Empty
            : countryCode.Trim().ToUpperInvariant();
    }

    private static string? NormalizeStateCode(string? stateCode)
    {
        return string.IsNullOrWhiteSpace(stateCode)
            ? null
            : stateCode.Trim().ToUpperInvariant();
    }

    private static string NormalizeCurrencyCode(string? currencyCode, string fallback)
    {
        return string.IsNullOrWhiteSpace(currencyCode)
            ? fallback
            : currencyCode.Trim().ToUpperInvariant();
    }

    private static int GetCurrencyDecimals(string currencyCode)
    {
        return currencyCode switch
        {
            "JPY" => 0,
            "KWD" => 3,
            _ => 2
        };
    }

    private static decimal Round(decimal value, int decimals)
    {
        return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
    }
}
