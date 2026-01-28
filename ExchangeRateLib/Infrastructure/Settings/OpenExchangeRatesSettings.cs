namespace ExchangeRateLib.Infrastructure.Settings
{
    public class OpenExchangeRatesSettings
    {
        public string ApiUrl { get; set; } = "https://openexchangerates.org/api";
        public string ApiKey { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;
    }
}
