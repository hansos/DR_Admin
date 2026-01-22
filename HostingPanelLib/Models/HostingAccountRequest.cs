namespace HostingPanelLib.Models
{
    public class HostingAccountRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Plan { get; set; }
        public int? DiskQuotaMB { get; set; }
        public int? BandwidthLimitMB { get; set; }
        public int? MaxEmailAccounts { get; set; }
        public int? MaxDatabases { get; set; }
        public int? MaxFtpAccounts { get; set; }
        public int? MaxSubdomains { get; set; }
        public bool? EnableCgi { get; set; }
        public bool? EnableShellAccess { get; set; }
        public bool? EnableBackup { get; set; }
        public Dictionary<string, string>? AdditionalSettings { get; set; }
    }
}
