namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class RegtonsSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://api.regtons.com/v1";
        public string TestApiUrl { get; set; } = "https://sandbox.regtons.com/v1";
    }
}
