namespace ExchangeRateLib.Infrastructure.Settings
{
    public class ExchangeRateHostSettings
    {
        public string ApiUrl { get; set; } = "https://api.exchangerate.host";
        public string? ApiKey { get; set; }
        public bool UseHttps { get; set; } = true;
    }
}
