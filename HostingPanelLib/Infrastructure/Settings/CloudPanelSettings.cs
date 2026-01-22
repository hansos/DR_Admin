namespace HostingPanelLib.Infrastructure.Settings
{
    public class CloudPanelSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int Port { get; set; } = 8443;
        public bool UseHttps { get; set; } = true;
    }
}
