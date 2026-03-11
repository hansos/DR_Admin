using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing tax rules and baseline tax calculations.
/// </summary>
public class TaxService : ITaxService
{
    private readonly ApplicationDbContext _context;
    private readonly IVatValidationService _vatValidationService;

    public TaxService(ApplicationDbContext context, IVatValidationService vatValidationService)
    {
        _context = context;
        _vatValidationService = vatValidationService;
    }

    /// <summary>
    /// Retrieves all tax rules.
    /// </summary>
    /// <returns>Collection of tax rule DTOs.</returns>
    public async Task<IEnumerable<TaxRuleDto>> GetAllTaxRulesAsync()
    {
        var rules = await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.CountryCode)
            .ThenBy(x => x.StateCode)
            .ThenByDescending(x => x.Priority)
            .ToListAsync();

        return rules.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves active tax rules.
    /// </summary>
    /// <returns>Collection of active tax rule DTOs.</returns>
    public async Task<IEnumerable<TaxRuleDto>> GetActiveTaxRulesAsync()
    {
        var now = DateTime.UtcNow;

        var rules = await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null
                        && x.IsActive
                        && x.EffectiveFrom <= now
                        && (x.EffectiveUntil == null || x.EffectiveUntil >= now))
            .OrderBy(x => x.CountryCode)
            .ThenBy(x => x.StateCode)
            .ThenByDescending(x => x.Priority)
            .ToListAsync();

        return rules.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a tax rule by identifier.
    /// </summary>
    /// <param name="id">Tax rule identifier.</param>
    /// <returns>Tax rule DTO if found; otherwise null.</returns>
    public async Task<TaxRuleDto?> GetTaxRuleByIdAsync(int id)
    {
        var rule = await _context.TaxRules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

        return rule == null ? null : MapToDto(rule);
    }

    /// <summary>
    /// Retrieves tax rules by country and optional state.
    /// </summary>
    /// <param name="countryCode">Country code.</param>
    /// <param name="stateCode">Optional state code.</param>
    /// <returns>Collection of matching tax rule DTOs.</returns>
    public async Task<IEnumerable<TaxRuleDto>> GetTaxRulesByLocationAsync(string countryCode, string? stateCode = null)
    {
        var normalizedCountry = NormalizeCountryCode(countryCode);
        var normalizedState = NormalizeStateCode(stateCode);

        var rules = await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null
                        && x.CountryCode == normalizedCountry
                        && (normalizedState == null || x.StateCode == normalizedState || x.StateCode == null))
            .OrderByDescending(x => x.StateCode == normalizedState)
            .ThenByDescending(x => x.Priority)
            .ThenByDescending(x => x.EffectiveFrom)
            .ToListAsync();

        return rules.Select(MapToDto);
    }

    /// <summary>
    /// Creates a new tax rule.
    /// </summary>
    /// <param name="createDto">Tax rule creation payload.</param>
    /// <returns>Created tax rule DTO.</returns>
    public async Task<TaxRuleDto> CreateTaxRuleAsync(CreateTaxRuleDto createDto)
    {
        var entity = new TaxRule
        {
            CountryCode = NormalizeCountryCode(createDto.CountryCode),
            StateCode = NormalizeStateCode(createDto.StateCode),
            TaxName = createDto.TaxName,
            TaxRate = createDto.TaxRate,
            IsActive = createDto.IsActive,
            EffectiveFrom = createDto.EffectiveFrom,
            EffectiveUntil = createDto.EffectiveUntil,
            AppliesToSetupFees = createDto.AppliesToSetupFees,
            AppliesToRecurring = createDto.AppliesToRecurring,
            ReverseCharge = createDto.ReverseCharge,
            TaxAuthority = createDto.TaxAuthority,
            TaxRegistrationNumber = createDto.TaxRegistrationNumber,
            Priority = createDto.Priority,
            InternalNotes = createDto.InternalNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaxRules.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    /// <summary>
    /// Updates an existing tax rule.
    /// </summary>
    /// <param name="id">Tax rule identifier.</param>
    /// <param name="updateDto">Tax rule update payload.</param>
    /// <returns>Updated tax rule DTO if found; otherwise null.</returns>
    public async Task<TaxRuleDto?> UpdateTaxRuleAsync(int id, UpdateTaxRuleDto updateDto)
    {
        var entity = await _context.TaxRules.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (entity == null)
        {
            return null;
        }

        entity.TaxName = updateDto.TaxName;
        entity.TaxRate = updateDto.TaxRate;
        entity.IsActive = updateDto.IsActive;
        entity.EffectiveFrom = updateDto.EffectiveFrom;
        entity.EffectiveUntil = updateDto.EffectiveUntil;
        entity.AppliesToSetupFees = updateDto.AppliesToSetupFees;
        entity.AppliesToRecurring = updateDto.AppliesToRecurring;
        entity.ReverseCharge = updateDto.ReverseCharge;
        entity.TaxAuthority = updateDto.TaxAuthority;
        entity.TaxRegistrationNumber = updateDto.TaxRegistrationNumber;
        entity.Priority = updateDto.Priority;
        entity.InternalNotes = updateDto.InternalNotes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    /// <summary>
    /// Soft-deletes a tax rule.
    /// </summary>
    /// <param name="id">Tax rule identifier.</param>
    /// <returns>True when successful; otherwise false.</returns>
    public async Task<bool> DeleteTaxRuleAsync(int id)
    {
        var entity = await _context.TaxRules.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (entity == null)
        {
            return false;
        }

        entity.DeletedAt = DateTime.UtcNow;
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Calculates tax for a customer and amount using active rules.
    /// </summary>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="amount">Taxable amount.</param>
    /// <param name="isSetupFee">Indicates whether setup fee rule should be used.</param>
    /// <returns>Tax amount, applied rate, and tax name.</returns>
    public async Task<(decimal taxAmount, decimal taxRate, string taxName)> CalculateTaxAsync(int customerId, decimal amount, bool isSetupFee = false)
    {
        var profile = await _context.CustomerTaxProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);

        if (profile == null || profile.TaxExempt)
        {
            return (0m, 0m, "VAT");
        }

        var countryCode = NormalizeCountryCode(profile.TaxResidenceCountry);
        var now = DateTime.UtcNow;

        var rule = await _context.TaxRules
            .AsNoTracking()
            .Where(x => x.DeletedAt == null
                        && x.IsActive
                        && x.CountryCode == countryCode
                        && x.EffectiveFrom <= now
                        && (x.EffectiveUntil == null || x.EffectiveUntil >= now)
                        && (isSetupFee ? x.AppliesToSetupFees : x.AppliesToRecurring))
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (rule == null)
        {
            return (0m, 0m, "VAT");
        }

        var taxRate = profile.CustomerType == Data.Enums.CustomerType.B2B && profile.TaxIdValidated && rule.ReverseCharge
            ? 0m
            : rule.TaxRate;

        var taxAmount = Math.Round(amount * taxRate, 2, MidpointRounding.AwayFromZero);
        return (taxAmount, taxRate, rule.TaxName);
    }

    /// <summary>
    /// Validates VAT number for country.
    /// </summary>
    /// <param name="vatNumber">VAT number.</param>
    /// <param name="countryCode">Country code.</param>
    /// <returns>True when valid; otherwise false.</returns>
    public Task<bool> ValidateVatNumberAsync(string vatNumber, string countryCode)
    {
        return _vatValidationService.ValidateAsync(NormalizeCountryCode(countryCode), vatNumber);
    }

    private static TaxRuleDto MapToDto(TaxRule entity)
    {
        return new TaxRuleDto
        {
            Id = entity.Id,
            CountryCode = entity.CountryCode,
            StateCode = entity.StateCode,
            TaxName = entity.TaxName,
            TaxRate = entity.TaxRate,
            IsActive = entity.IsActive,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveUntil = entity.EffectiveUntil,
            AppliesToSetupFees = entity.AppliesToSetupFees,
            AppliesToRecurring = entity.AppliesToRecurring,
            ReverseCharge = entity.ReverseCharge,
            TaxAuthority = entity.TaxAuthority,
            TaxRegistrationNumber = entity.TaxRegistrationNumber,
            Priority = entity.Priority,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static string NormalizeCountryCode(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeStateCode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }
}


