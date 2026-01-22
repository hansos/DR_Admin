namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class CentralNicSettings
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://api.centralnic.com/v2";
        public string TestApiUrl { get; set; } = "https://api-ote.centralnic.com/v2";
    }
}
