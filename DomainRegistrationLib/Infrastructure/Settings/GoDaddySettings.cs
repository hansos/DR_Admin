namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class GoDaddySettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public bool UseProduction { get; set; } = true;
        public string ProductionApiUrl { get; set; } = "https://api.godaddy.com";
        public string TestApiUrl { get; set; } = "https://api.ote-godaddy.com";
    }
}
