using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing currency exchange rates and conversions
/// </summary>
public class CurrencyService : ICurrencyService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CurrencyService>();

    public CurrencyService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all currency exchange rates
    /// </summary>
    /// <returns>A collection of all currency exchange rates</returns>
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetAllRatesAsync()
    {
        try
        {
            _log.Information("Fetching all currency exchange rates");
            
            var rates = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .OrderByDescending(r => r.EffectiveDate)
                .ToListAsync();

            var rateDtos = rates.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} currency exchange rates", rates.Count);
            return rateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all currency exchange rates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active currency exchange rates
    /// </summary>
    /// <returns>A collection of active currency exchange rates</returns>
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetActiveRatesAsync()
    {
        try
        {
            _log.Information("Fetching active currency exchange rates");
            
            var rates = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(r => r.IsActive && 
                           (r.ExpiryDate == null || r.ExpiryDate > DateTime.UtcNow))
                .OrderBy(r => r.BaseCurrency)
                .ThenBy(r => r.TargetCurrency)
                .ToListAsync();

            var rateDtos = rates.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active currency exchange rates", rates.Count);
            return rateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active currency exchange rates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific currency exchange rate by ID
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate</param>
    /// <returns>The currency exchange rate if found, otherwise null</returns>
    public async Task<CurrencyExchangeRateDto?> GetRateByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching currency exchange rate with ID: {RateId}", id);
            
            var rate = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rate == null)
            {
                _log.Warning("Currency exchange rate with ID {RateId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched currency exchange rate with ID: {RateId}", id);
            return MapToDto(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching currency exchange rate with ID: {RateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the current exchange rate between two currencies
    /// </summary>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <param name="effectiveDate">Optional date for the exchange rate (defaults to current date)</param>
    /// <returns>The exchange rate if found, otherwise null</returns>
    public async Task<CurrencyExchangeRateDto?> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime? effectiveDate = null)
    {
        try
        {
            var date = effectiveDate ?? DateTime.UtcNow;
            _log.Information("Fetching exchange rate from {FromCurrency} to {ToCurrency} for date {Date}", 
                fromCurrency, toCurrency, date);

            // If same currency, return rate of 1
            if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                _log.Information("Same currency conversion requested, returning rate of 1");
                return new CurrencyExchangeRateDto
                {
                    BaseCurrency = fromCurrency,
                    TargetCurrency = toCurrency,
                    Rate = 1.0m,
                    EffectiveRate = 1.0m,
                    EffectiveDate = date,
                    IsActive = true
                };
            }

            var rate = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(r => r.BaseCurrency == fromCurrency && 
                           r.TargetCurrency == toCurrency &&
                           r.IsActive &&
                           r.EffectiveDate <= date &&
                           (r.ExpiryDate == null || r.ExpiryDate > date))
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync();

            if (rate == null)
            {
                _log.Warning("No exchange rate found from {FromCurrency} to {ToCurrency} for date {Date}", 
                    fromCurrency, toCurrency, date);
                return null;
            }

            _log.Information("Successfully fetched exchange rate from {FromCurrency} to {ToCurrency}", 
                fromCurrency, toCurrency);
            return MapToDto(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate from {FromCurrency} to {ToCurrency}", 
                fromCurrency, toCurrency);
            throw;
        }
    }

    /// <summary>
    /// Creates a new currency exchange rate
    /// </summary>
    /// <param name="createDto">The data for creating the exchange rate</param>
    /// <returns>The created currency exchange rate</returns>
    public async Task<CurrencyExchangeRateDto> CreateRateAsync(CreateCurrencyExchangeRateDto createDto)
    {
        try
        {
            _log.Information("Creating new currency exchange rate from {FromCurrency} to {ToCurrency}", 
                createDto.BaseCurrency, createDto.TargetCurrency);

            var effectiveRate = createDto.Rate * (1 + createDto.Markup / 100);

            var rate = new CurrencyExchangeRate
            {
                BaseCurrency = createDto.BaseCurrency.ToUpper(),
                TargetCurrency = createDto.TargetCurrency.ToUpper(),
                Rate = createDto.Rate,
                EffectiveDate = createDto.EffectiveDate,
                ExpiryDate = createDto.ExpiryDate,
                Source = createDto.Source,
                IsActive = createDto.IsActive,
                Markup = createDto.Markup,
                EffectiveRate = effectiveRate,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CurrencyExchangeRates.Add(rate);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created currency exchange rate with ID: {RateId}", rate.Id);
            return MapToDto(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating currency exchange rate from {FromCurrency} to {ToCurrency}", 
                createDto.BaseCurrency, createDto.TargetCurrency);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to update</param>
    /// <param name="updateDto">The updated data</param>
    /// <returns>The updated currency exchange rate if found, otherwise null</returns>
    public async Task<CurrencyExchangeRateDto?> UpdateRateAsync(int id, UpdateCurrencyExchangeRateDto updateDto)
    {
        try
        {
            _log.Information("Updating currency exchange rate with ID: {RateId}", id);

            var rate = await _context.CurrencyExchangeRates.FindAsync(id);

            if (rate == null)
            {
                _log.Warning("Currency exchange rate with ID {RateId} not found for update", id);
                return null;
            }

            rate.Rate = updateDto.Rate;
            rate.EffectiveDate = updateDto.EffectiveDate;
            rate.ExpiryDate = updateDto.ExpiryDate;
            rate.Source = updateDto.Source;
            rate.IsActive = updateDto.IsActive;
            rate.Markup = updateDto.Markup;
            rate.EffectiveRate = updateDto.Rate * (1 + updateDto.Markup / 100);
            rate.Notes = updateDto.Notes;
            rate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated currency exchange rate with ID: {RateId}", id);
            return MapToDto(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating currency exchange rate with ID: {RateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to delete</param>
    /// <returns>True if the rate was deleted, false if not found</returns>
    public async Task<bool> DeleteRateAsync(int id)
    {
        try
        {
            _log.Information("Deleting currency exchange rate with ID: {RateId}", id);

            var rate = await _context.CurrencyExchangeRates.FindAsync(id);

            if (rate == null)
            {
                _log.Warning("Currency exchange rate with ID {RateId} not found for deletion", id);
                return false;
            }

            _context.CurrencyExchangeRates.Remove(rate);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted currency exchange rate with ID: {RateId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting currency exchange rate with ID: {RateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Converts an amount from one currency to another
    /// </summary>
    /// <param name="amount">The amount to convert</param>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <param name="effectiveDate">Optional date for the exchange rate (defaults to current date)</param>
    /// <returns>The conversion result including the converted amount and exchange rate used</returns>
    public async Task<CurrencyConversionResultDto> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, DateTime? effectiveDate = null)
    {
        try
        {
            _log.Information("Converting {Amount} from {FromCurrency} to {ToCurrency}", 
                amount, fromCurrency, toCurrency);

            var date = effectiveDate ?? DateTime.UtcNow;
            var exchangeRate = await GetExchangeRateAsync(fromCurrency, toCurrency, date);

            if (exchangeRate == null)
            {
                var errorMessage = $"No exchange rate found from {fromCurrency} to {toCurrency} for date {date:yyyy-MM-dd}";
                _log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            var convertedAmount = amount * exchangeRate.EffectiveRate;

            var result = new CurrencyConversionResultDto
            {
                OriginalAmount = amount,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeRate = exchangeRate.EffectiveRate,
                ConvertedAmount = Math.Round(convertedAmount, 2),
                RateDate = exchangeRate.EffectiveDate
            };

            _log.Information("Successfully converted {Amount} {FromCurrency} to {ConvertedAmount} {ToCurrency}", 
                amount, fromCurrency, result.ConvertedAmount, toCurrency);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while converting {Amount} from {FromCurrency} to {ToCurrency}", 
                amount, fromCurrency, toCurrency);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all exchange rates for a specific currency pair
    /// </summary>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <returns>A collection of exchange rates for the currency pair</returns>
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetRatesForCurrencyPairAsync(string fromCurrency, string toCurrency)
    {
        try
        {
            _log.Information("Fetching all exchange rates from {FromCurrency} to {ToCurrency}", 
                fromCurrency, toCurrency);
            
            var rates = await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(r => r.BaseCurrency == fromCurrency && r.TargetCurrency == toCurrency)
                .OrderByDescending(r => r.EffectiveDate)
                .ToListAsync();

            var rateDtos = rates.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} exchange rates from {FromCurrency} to {ToCurrency}", 
                rates.Count, fromCurrency, toCurrency);
            return rateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rates from {FromCurrency} to {ToCurrency}", 
                fromCurrency, toCurrency);
            throw;
        }
    }

    /// <summary>
    /// Deactivates expired exchange rates
    /// </summary>
    /// <returns>The number of rates that were deactivated</returns>
    public async Task<int> DeactivateExpiredRatesAsync()
    {
        try
        {
            _log.Information("Deactivating expired currency exchange rates");
            
            var now = DateTime.UtcNow;
            var expiredRates = await _context.CurrencyExchangeRates
                .Where(r => r.IsActive && r.ExpiryDate != null && r.ExpiryDate <= now)
                .ToListAsync();

            foreach (var rate in expiredRates)
            {
                rate.IsActive = false;
                rate.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully deactivated {Count} expired currency exchange rates", expiredRates.Count);
            return expiredRates.Count;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deactivating expired currency exchange rates");
            throw;
        }
    }

    private static CurrencyExchangeRateDto MapToDto(CurrencyExchangeRate rate)
    {
        return new CurrencyExchangeRateDto
        {
            Id = rate.Id,
            BaseCurrency = rate.BaseCurrency,
            TargetCurrency = rate.TargetCurrency,
            Rate = rate.Rate,
            EffectiveDate = rate.EffectiveDate,
            ExpiryDate = rate.ExpiryDate,
            Source = rate.Source,
            IsActive = rate.IsActive,
            Markup = rate.Markup,
            EffectiveRate = rate.EffectiveRate,
            Notes = rate.Notes,
            CreatedAt = rate.CreatedAt,
            UpdatedAt = rate.UpdatedAt
        };
    }
}
