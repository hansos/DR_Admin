using ExchangeRateLib.Models;

namespace ExchangeRateLib.Implementations
{
    public class OandaProvider : BaseExchangeRateProvider
    {
        private readonly string _apiKey;
        private readonly string _accountId;

        public OandaProvider(string apiUrl, string apiKey, string accountId)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        }

        public override Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetExchangeRatesAsync(string baseCurrency, List<string> targetCurrencies)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ConversionResult> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            return Task.FromResult(CreateConversionErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRateResult> GetHistoricalRateAsync(string fromCurrency, string toCurrency, DateTime date)
        {
            return Task.FromResult(CreateExchangeRateErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<TimeSeriesResult> GetTimeSeriesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(CreateTimeSeriesErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<SupportedCurrenciesResult> GetSupportedCurrenciesAsync()
        {
            return Task.FromResult(CreateSupportedCurrenciesErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }

        public override Task<ExchangeRatesResult> GetLatestRatesAsync(string baseCurrency)
        {
            return Task.FromResult(CreateExchangeRatesErrorResult(
                "OANDA provider not yet implemented",
                "NOT_IMPLEMENTED"
            ));
        }
    }
}
