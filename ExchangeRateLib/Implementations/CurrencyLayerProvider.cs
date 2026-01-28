using ExchangeRateLib.Models;

namespace ExchangeRateLib.Implementations
{
    public class CurrencyLayerProvider : BaseExchangeRateProvider
    {
        private readonly string _apiKey;

        public CurrencyLayerProvider(string apiUrl, string apiKey)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public override Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateConversionErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(CreateTimeSeriesErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync()
        {
            return Task.FromResult(CreateSupportedCurrenciesErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "CurrencyLayer provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }
    }
}
