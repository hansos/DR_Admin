namespace ExchangeRateLib.Infrastructure.Settings
{
    public class XeSettings
    {
        public string ApiUrl { get; set; } = "https://xecdapi.xe.com/v1";
        public string ApiKey { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;
    }
}
