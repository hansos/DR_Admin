namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class CloudflareSettings
    {
        public string ApiToken { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://api.cloudflare.com/client/v4";
    }
}
