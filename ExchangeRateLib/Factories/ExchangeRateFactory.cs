using ExchangeRateLib.Implementations;
using ExchangeRateLib.Infrastructure.Settings;
using ExchangeRateLib.Interfaces;

namespace ExchangeRateLib.Factories
{
    public class ExchangeRateFactory
    {
        private readonly ExchangeRateSettings _settings;

        public ExchangeRateFactory(ExchangeRateSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IExchangeRateProvider CreateProvider()
        {
            return _settings.Provider.ToLower() switch
            {
                "exchangeratehost" => _settings.ExchangeRateHost is not null
                    ? new ExchangeRateHostProvider(
                        _settings.ExchangeRateHost.ApiUrl,
                        _settings.ExchangeRateHost.ApiKey
                    )
                    : throw new InvalidOperationException("ExchangeRate.host settings are not configured"),

                "frankfurter" => _settings.Frankfurter is not null
                    ? new FrankfurterProvider(
                        _settings.Frankfurter.ApiUrl
                    )
                    : throw new InvalidOperationException("Frankfurter settings are not configured"),

                "openexchangerates" => _settings.OpenExchangeRates is not null
                    ? new OpenExchangeRatesProvider(
                        _settings.OpenExchangeRates.ApiUrl,
                        _settings.OpenExchangeRates.ApiKey
                    )
                    : throw new InvalidOperationException("Open Exchange Rates settings are not configured"),

                "currencylayer" => _settings.CurrencyLayer is not null
                    ? new CurrencyLayerProvider(
                        _settings.CurrencyLayer.ApiUrl,
                        _settings.CurrencyLayer.ApiKey
                    )
                    : throw new InvalidOperationException("CurrencyLayer settings are not configured"),

                "fixer" => _settings.Fixer is not null
                    ? new FixerProvider(
                        _settings.Fixer.ApiUrl,
                        _settings.Fixer.ApiKey
                    )
                    : throw new InvalidOperationException("Fixer.io settings are not configured"),

                "xe" => _settings.Xe is not null
                    ? new XeProvider(
                        _settings.Xe.ApiUrl,
                        _settings.Xe.ApiKey,
                        _settings.Xe.AccountId
                    )
                    : throw new InvalidOperationException("XE settings are not configured"),

                "oanda" => _settings.Oanda is not null
                    ? new OandaProvider(
                        _settings.Oanda.ApiUrl,
                        _settings.Oanda.ApiKey,
                        _settings.Oanda.AccountId
                    )
                    : throw new InvalidOperationException("OANDA settings are not configured"),

                _ => throw new NotSupportedException($"Exchange rate provider '{_settings.Provider}' is not supported")
            };
        }

        public IExchangeRateProvider CreateProvider(string providerCode)
        {
            var originalProvider = _settings.Provider;
            try
            {
                _settings.Provider = providerCode;
                return CreateProvider();
            }
            finally
            {
                _settings.Provider = originalProvider;
            }
        }
    }
}
