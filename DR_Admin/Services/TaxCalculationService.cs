using System.Text.Json;
using System.Net;
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
    public Task<TaxQuoteResultDto> QuoteTaxAsync(TaxQuoteRequestDto request)
    {
        return CalculateCoreAsync(request, persistSnapshot: false);
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

        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            throw new InvalidOperationException("IdempotencyKey is required for tax finalization.");
        }

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.OrderId.Value);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.OrderId.Value} not found.");
        }

        if (request.CustomerId.HasValue && request.CustomerId.Value != order.CustomerId)
        {
            throw new InvalidOperationException("CustomerId does not match the order owner.");
        }

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
                    ExchangeRate = existing.ExchangeRate,
                    ExchangeRateDate = existing.ExchangeRateDate,
                    ExchangeRateSource = existing.ExchangeRateSource,
                NetAmount = existing.NetAmount,
                TaxAmount = existing.TaxAmount,
                GrossAmount = existing.GrossAmount,
                BuyerTaxIdValidated = existing.BuyerTaxIdValidated,
                    TaxDeterminationEvidenceId = existing.TaxDeterminationEvidenceId,
                LegalNote = existing.ReverseChargeApplied ? "Reverse charge applied" : string.Empty,
                Lines = new List<TaxQuoteLineResultDto>()
            };
        }

        return await CalculateCoreAsync(request, persistSnapshot: true);
    }

    private async Task<TaxQuoteResultDto> CalculateCoreAsync(TaxQuoteRequestDto request, bool persistSnapshot)
    {
        if (request.Lines.Count == 0)
        {
            throw new InvalidOperationException("At least one line is required for tax calculation.");
        }

        if (persistSnapshot)
        {
            if (string.IsNullOrWhiteSpace(request.BillingCountryCode))
            {
                throw new InvalidOperationException("BillingCountryCode is required for tax finalization evidence.");
            }

            if (string.IsNullOrWhiteSpace(request.IpAddress) || !IPAddress.TryParse(request.IpAddress, out _))
            {
                throw new InvalidOperationException("A valid IpAddress is required for tax finalization evidence.");
            }
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

        var vatValidationResult = !string.IsNullOrWhiteSpace(buyerTaxId) && request.ValidateBuyerTaxId
            ? await _vatValidationService.ValidateDetailedAsync(buyerCountry, buyerTaxId)
            : new VatValidationResult
            {
                IsValid = !string.IsNullOrWhiteSpace(buyerTaxId),
                ProviderName = "NotValidated",
                RawResponse = "Validation bypassed"
            };

        var validatedTaxId = vatValidationResult.IsValid;

        var transactionDate = request.TransactionDate == default ? DateTime.UtcNow : request.TransactionDate;
        var decimals = GetCurrencyDecimals(string.IsNullOrWhiteSpace(request.DisplayCurrencyCode)
            ? request.TaxCurrencyCode
            : request.DisplayCurrencyCode);

        var lineCalculations = new List<(TaxQuoteLineResultDto LineResult, TaxRule? Rule, bool ReverseChargeApplied)>();

        foreach (var line in request.Lines)
        {
            var taxRule = await GetApplicableTaxRuleAsync(
                buyerCountry,
                buyerState,
                transactionDate,
                NormalizeTaxCategory(line.TaxCategory));

            var ruleTaxRate = taxRule?.TaxRate ?? 0m;
            var reverseChargeApplied = taxRule?.ReverseCharge == true && buyerType == CustomerType.B2B && validatedTaxId;
            var effectiveRate = reverseChargeApplied ? 0m : ruleTaxRate;

            var net = Math.Max(0m, line.NetAmount);
            var tax = Round(net * effectiveRate, decimals);
            var gross = Round(net + tax, decimals);

            lineCalculations.Add((
                new TaxQuoteLineResultDto
                {
                    LineId = line.LineId,
                    Description = line.Description,
                    NetAmount = Round(net, decimals),
                    TaxRate = effectiveRate,
                    TaxAmount = tax,
                    GrossAmount = gross
                },
                taxRule,
                reverseChargeApplied));
        }

        var lineResults = lineCalculations.Select(x => x.LineResult).ToList();
        var reverseChargeAny = lineCalculations.Any(x => x.ReverseChargeApplied);

        var netAmount = Round(lineResults.Sum(x => x.NetAmount), decimals);
        var taxAmount = Round(lineResults.Sum(x => x.TaxAmount), decimals);
        var grossAmount = Round(netAmount + taxAmount, decimals);

        var taxNames = lineCalculations.Select(x => x.Rule?.TaxName ?? "VAT").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var taxRates = lineResults.Select(x => x.TaxRate).Distinct().ToList();

        var taxName = taxNames.Count == 1 ? taxNames[0] : "Mixed";
        var summaryTaxRate = taxRates.Count == 1 ? taxRates[0] : 0m;

        var selectedRule = lineCalculations.Select(x => x.Rule).FirstOrDefault(x => x != null);

        var taxJurisdictionId = selectedRule?.TaxJurisdictionId;
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

        var ruleVersionParts = lineCalculations
            .Where(x => x.Rule != null)
            .Select(x => $"{x.Rule!.Id}:{x.Rule.UpdatedAt:yyyyMMddHHmmss}")
            .Distinct()
            .ToList();

        var ruleVersion = ruleVersionParts.Count == 0
            ? "none"
            : $"taxrules:{string.Join("|", ruleVersionParts)}";

        var (resolvedExchangeRate, resolvedExchangeRateDate, resolvedExchangeRateSource) = await ResolveExchangeRateAsync(
            NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR"),
            NormalizeCurrencyCode(request.DisplayCurrencyCode, NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR")),
            transactionDate,
            request,
            persistSnapshot);

        var result = new TaxQuoteResultDto
        {
            OrderId = request.OrderId,
            TaxJurisdictionId = taxJurisdictionId,
            TaxName = taxName,
            TaxRate = summaryTaxRate,
            ReverseChargeApplied = reverseChargeAny,
            RuleVersion = ruleVersion,
            TaxCurrencyCode = NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR"),
            DisplayCurrencyCode = NormalizeCurrencyCode(request.DisplayCurrencyCode, NormalizeCurrencyCode(request.TaxCurrencyCode, "EUR")),
            ExchangeRate = resolvedExchangeRate,
            ExchangeRateDate = resolvedExchangeRateDate,
            ExchangeRateSource = resolvedExchangeRateSource,
            NetAmount = netAmount,
            TaxAmount = taxAmount,
            GrossAmount = grossAmount,
            BuyerTaxIdValidated = validatedTaxId,
            LegalNote = reverseChargeAny ? "Reverse charge applied" : string.Empty,
            Lines = lineResults
        };

        if (persistSnapshot)
        {
            var evidence = new TaxDeterminationEvidence
            {
                CustomerId = request.CustomerId,
                OrderId = request.OrderId,
                BuyerCountryCode = buyerCountry,
                BuyerStateCode = buyerState,
                BillingCountryCode = string.IsNullOrWhiteSpace(request.BillingCountryCode)
                    ? buyerCountry
                    : NormalizeCountryCode(request.BillingCountryCode),
                IpAddress = request.IpAddress,
                BuyerTaxId = buyerTaxId,
                BuyerTaxIdValidated = validatedTaxId,
                VatValidationProvider = vatValidationResult.ProviderName,
                VatValidationRawResponse = vatValidationResult.RawResponse,
                ExchangeRateSource = resolvedExchangeRateSource,
                CapturedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaxDeterminationEvidences.Add(evidence);
            await _context.SaveChangesAsync();

            var snapshot = new OrderTaxSnapshot
            {
                OrderId = request.OrderId!.Value,
                TaxJurisdictionId = taxJurisdictionId,
                TaxDeterminationEvidenceId = evidence.Id,
                BuyerCountryCode = buyerCountry,
                BuyerStateCode = buyerState,
                BuyerType = buyerType,
                BuyerTaxId = buyerTaxId,
                BuyerTaxIdValidated = validatedTaxId,
                TaxCurrencyCode = result.TaxCurrencyCode,
                DisplayCurrencyCode = result.DisplayCurrencyCode,
                ExchangeRate = resolvedExchangeRate,
                ExchangeRateDate = resolvedExchangeRateDate,
                ExchangeRateSource = resolvedExchangeRateSource,
                NetAmount = netAmount,
                TaxAmount = taxAmount,
                GrossAmount = grossAmount,
                AppliedTaxRate = summaryTaxRate,
                AppliedTaxName = result.TaxName,
                ReverseChargeApplied = reverseChargeAny,
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
                    AppliedTaxRuleIds = lineCalculations.Where(x => x.Rule != null).Select(x => x.Rule!.Id).Distinct().ToList()
                }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OrderTaxSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();
            result.SnapshotId = snapshot.Id;
            result.TaxDeterminationEvidenceId = evidence.Id;
        }

        return result;
    }

    private async Task<TaxRule?> GetApplicableTaxRuleAsync(string countryCode, string? stateCode, DateTime when, string taxCategory)
    {
        return await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null
                        && x.IsActive
                        && x.CountryCode == countryCode
                        && (x.StateCode == stateCode || x.StateCode == null)
                        && x.TaxCategory == taxCategory
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

    private static string NormalizeTaxCategory(string? taxCategory)
    {
        return string.IsNullOrWhiteSpace(taxCategory)
            ? "STANDARD"
            : taxCategory.Trim().ToUpperInvariant();
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

    private async Task<(decimal? rate, DateTime? rateDate, CurrencyRateSource? source)> ResolveExchangeRateAsync(
        string taxCurrencyCode,
        string displayCurrencyCode,
        DateTime transactionDate,
        TaxQuoteRequestDto request,
        bool persistSnapshot)
    {
        if (taxCurrencyCode == displayCurrencyCode)
        {
            return (1m, transactionDate, CurrencyRateSource.Manual);
        }

        var trustedRate = await _context.CurrencyExchangeRates
            .AsNoTracking()
            .Where(x => x.IsActive
                        && x.BaseCurrency == taxCurrencyCode
                        && x.TargetCurrency == displayCurrencyCode
                        && x.EffectiveDate <= transactionDate
                        && (x.ExpiryDate == null || x.ExpiryDate >= transactionDate))
            .OrderByDescending(x => x.EffectiveDate)
            .FirstOrDefaultAsync();

        if (trustedRate != null)
        {
            var effective = trustedRate.EffectiveRate > 0 ? trustedRate.EffectiveRate : trustedRate.Rate;
            return (effective, trustedRate.EffectiveDate, trustedRate.Source);
        }

        if (request.RequireTrustedExchangeRate || persistSnapshot)
        {
            throw new InvalidOperationException($"No trusted exchange rate found for {taxCurrencyCode}/{displayCurrencyCode} at {transactionDate:O}.");
        }

        if (request.ExchangeRate.HasValue && request.ExchangeRate.Value > 0)
        {
            return (request.ExchangeRate.Value, request.ExchangeRateDate ?? transactionDate, CurrencyRateSource.Other);
        }

        return (null, null, null);
    }
}
