namespace ExchangeRateLib.Infrastructure.Settings
{
    public class ExchangeRateSettings
    {
        public string Provider { get; set; } = string.Empty;
        
        public ExchangeRateHostSettings? ExchangeRateHost { get; set; }
        public FrankfurterSettings? Frankfurter { get; set; }
        public OpenExchangeRatesSettings? OpenExchangeRates { get; set; }
        public CurrencyLayerSettings? CurrencyLayer { get; set; }
        public FixerSettings? Fixer { get; set; }
        public XeSettings? Xe { get; set; }
        public OandaSettings? Oanda { get; set; }
    }
}
