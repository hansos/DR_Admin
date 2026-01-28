using ExchangeRateLib.Models;

namespace ExchangeRateLib.Implementations
{
    public class OpenExchangeRatesProvider : BaseExchangeRateProvider
    {
        private readonly string _apiKey;

        public OpenExchangeRatesProvider(string apiUrl, string apiKey)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public override Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateConversionErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(CreateTimeSeriesErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync()
        {
            return Task.FromResult(CreateSupportedCurrenciesErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "Open Exchange Rates provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }
    }
}
