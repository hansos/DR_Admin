namespace ExchangeRateLib.Infrastructure.Settings
{
    public class FixerSettings
    {
        public string ApiUrl { get; set; } = "https://api.fixer.io";
        public string ApiKey { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;
    }
}
