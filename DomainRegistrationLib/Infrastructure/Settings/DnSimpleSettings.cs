namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class DnSimpleSettings
    {
        public string AccountId { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = true;
        public string ApiUrl { get; set; } = "https://api.dnsimple.com/v2";
    }
}
