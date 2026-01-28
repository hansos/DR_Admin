namespace ExchangeRateLib.Infrastructure.Settings
{
    public class CurrencyLayerSettings
    {
        public string ApiUrl { get; set; } = "https://api.currencylayer.com";
        public string ApiKey { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;
    }
}
