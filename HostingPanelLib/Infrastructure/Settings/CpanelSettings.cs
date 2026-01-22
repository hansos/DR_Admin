namespace HostingPanelLib.Infrastructure.Settings
{
    public class CpanelSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Port { get; set; } = 2087;
        public bool UseHttps { get; set; } = true;
    }
}
