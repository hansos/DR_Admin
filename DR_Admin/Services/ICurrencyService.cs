using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing currency exchange rates and conversions
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Retrieves all currency exchange rates
    /// </summary>
    /// <returns>A collection of all currency exchange rates</returns>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetAllRatesAsync();

    /// <summary>
    /// Retrieves all active currency exchange rates
    /// </summary>
    /// <returns>A collection of active currency exchange rates</returns>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetActiveRatesAsync();

    /// <summary>
    /// Retrieves a specific currency exchange rate by ID
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate</param>
    /// <returns>The currency exchange rate if found, otherwise null</returns>
    Task<CurrencyExchangeRateDto?> GetRateByIdAsync(int id);

    /// <summary>
    /// Retrieves the current exchange rate between two currencies
    /// </summary>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <param name="effectiveDate">Optional date for the exchange rate (defaults to current date)</param>
    /// <returns>The exchange rate if found, otherwise null</returns>
    Task<CurrencyExchangeRateDto?> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime? effectiveDate = null);

    /// <summary>
    /// Creates a new currency exchange rate
    /// </summary>
    /// <param name="createDto">The data for creating the exchange rate</param>
    /// <returns>The created currency exchange rate</returns>
    Task<CurrencyExchangeRateDto> CreateRateAsync(CreateCurrencyExchangeRateDto createDto);

    /// <summary>
    /// Updates an existing currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to update</param>
    /// <param name="updateDto">The updated data</param>
    /// <returns>The updated currency exchange rate if found, otherwise null</returns>
    Task<CurrencyExchangeRateDto?> UpdateRateAsync(int id, UpdateCurrencyExchangeRateDto updateDto);

    /// <summary>
    /// Deletes a currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to delete</param>
    /// <returns>True if the rate was deleted, false if not found</returns>
    Task<bool> DeleteRateAsync(int id);

    /// <summary>
    /// Converts an amount from one currency to another
    /// </summary>
    /// <param name="amount">The amount to convert</param>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <param name="effectiveDate">Optional date for the exchange rate (defaults to current date)</param>
    /// <returns>The conversion result including the converted amount and exchange rate used</returns>
    Task<CurrencyConversionResultDto> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, DateTime? effectiveDate = null);

    /// <summary>
    /// Retrieves all exchange rates for a specific currency pair
    /// </summary>
    /// <param name="fromCurrency">The source currency code</param>
    /// <param name="toCurrency">The target currency code</param>
    /// <returns>A collection of exchange rates for the currency pair</returns>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetRatesForCurrencyPairAsync(string fromCurrency, string toCurrency);

    /// <summary>
    /// Deactivates expired exchange rates
    /// </summary>
    /// <returns>The number of rates that were deactivated</returns>
    Task<int> DeactivateExpiredRatesAsync();
}
