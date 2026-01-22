namespace HostingPanelLib.Infrastructure.Settings
{
    public class CyberPanelSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public int Port { get; set; } = 8090;
        public bool UseHttps { get; set; } = true;
    }
}
