namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class OpenSrsSettings
    {
        public string Username { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://rr-n1-tor.opensrs.net:55443";
        public string TestApiUrl { get; set; } = "https://horizon.opensrs.net:55443";
    }
}
