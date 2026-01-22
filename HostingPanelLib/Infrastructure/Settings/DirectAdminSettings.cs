namespace HostingPanelLib.Infrastructure.Settings
{
    public class DirectAdminSettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 2222;
        public bool UseHttps { get; set; } = true;
    }
}
