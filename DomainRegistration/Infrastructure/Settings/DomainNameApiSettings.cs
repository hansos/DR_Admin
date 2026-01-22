namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class DomainNameApiSettings
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://api.domainnameapi.com";
        public string TestApiUrl { get; set; } = "https://api-test.domainnameapi.com";
    }
}
