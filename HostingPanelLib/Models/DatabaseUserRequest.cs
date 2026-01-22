namespace HostingPanelLib.Models
{
    public class DatabaseUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public List<string>? Privileges { get; set; }
        public List<string>? AllowedHosts { get; set; }
        public Dictionary<string, string>? AdditionalSettings { get; set; }
    }
}
