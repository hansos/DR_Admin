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

        /// <summary>
        /// Maximum number of updates per day (0 = unlimited)
        /// </summary>
        public int MaxUpdatesPerDay { get; set; } = 24;

        /// <summary>
        /// Hours between each update
        /// </summary>
        public int HoursBetweenUpdates { get; set; } = 1;

        /// <summary>
        /// Whether to download exchange rates on application startup
        /// </summary>
        public bool UpdateOnStartup { get; set; } = true;
    }
}
