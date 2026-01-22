namespace HostingPanelLib.Infrastructure.Settings
{
    public class ISPConfigSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 8080;
        public bool UseHttps { get; set; } = true;
        public string? RemoteApiUrl { get; set; }
    }
}
