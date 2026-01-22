namespace HostingPanelLib.Infrastructure.Settings
{
    public class PleskSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int Port { get; set; } = 8443;
        public bool UseHttps { get; set; } = true;
    }
}
