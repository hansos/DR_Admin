using ExchangeRateLib.Models;

namespace ExchangeRateLib.Interfaces
{
    public interface IExchangeRateProvider
    {
        /// <summary>
        /// Gets the current exchange rate between two currencies
        /// </summary>
        Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency);

        /// <summary>
        /// Gets exchange rates from base currency to multiple target currencies
        /// </summary>
        Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies);

        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);

        /// <summary>
        /// Gets historical exchange rate for a specific date
        /// </summary>
        Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date);

        /// <summary>
        /// Gets time series data for exchange rates
        /// </summary>
        Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets list of supported currencies
        /// </summary>
        Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync();

        /// <summary>
        /// Gets the latest exchange rates for all available currencies
        /// </summary>
        Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency);
    }
}
