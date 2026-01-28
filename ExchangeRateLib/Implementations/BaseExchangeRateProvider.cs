using ExchangeRateLib.Interfaces;
using ExchangeRateLib.Models;

namespace ExchangeRateLib.Implementations
{
    public abstract class BaseExchangeRateProvider : IExchangeRateProvider
    {
        protected readonly string _apiUrl;
        protected readonly HttpClient _httpClient;

        protected BaseExchangeRateProvider(string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
        }

        public abstract Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency);
        public abstract Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies);
        public abstract Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
        public abstract Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date);
        public abstract Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate);
        public abstract Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync();
        public abstract Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency);

        protected virtual ExchangeRateResult CreateExchangeRateErrorResult(string message, string? errorCode = null)
        {
            return new ExchangeRateResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual ExchangeRatesResult CreateExchangeRatesErrorResult(string message, string? errorCode = null)
        {
            return new ExchangeRatesResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual ConversionResult CreateConversionErrorResult(string message, string? errorCode = null)
        {
            return new ConversionResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual TimeSeriesResult CreateTimeSeriesErrorResult(string message, string? errorCode = null)
        {
            return new TimeSeriesResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual SupportedCurrenciesResult CreateSupportedCurrenciesErrorResult(string message, string? errorCode = null)
        {
            return new SupportedCurrenciesResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }
    }
}
