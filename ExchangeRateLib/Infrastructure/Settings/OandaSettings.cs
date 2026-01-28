namespace ExchangeRateLib.Infrastructure.Settings
{
    public class OandaSettings
    {
        public string ApiUrl { get; set; } = "https://api.oanda.com/v2";
        public string ApiKey { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;
    }
}
