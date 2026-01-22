namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class DomainboxSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://api.domainbox.com/v1";
        public string TestApiUrl { get; set; } = "https://sandbox.domainbox.com/v1";
    }
}
