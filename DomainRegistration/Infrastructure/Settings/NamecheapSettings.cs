namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class NamecheapSettings
    {
        public string ApiUser { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ClientIp { get; set; } = string.Empty;
        public bool UseSandbox { get; set; }
        public string ApiUrl { get; set; } = "https://api.namecheap.com/xml.response";
        public string SandboxApiUrl { get; set; } = "https://api.sandbox.namecheap.com/xml.response";
    }
}
