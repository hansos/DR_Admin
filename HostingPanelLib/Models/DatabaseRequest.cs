namespace HostingPanelLib.Models
{
    public class DatabaseRequest
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string DatabaseType { get; set; } = "mysql";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Server { get; set; }
        public string? Charset { get; set; }
        public string? Collation { get; set; }
        public int? QuotaMB { get; set; }
        public List<string>? AllowedHosts { get; set; }
        public Dictionary<string, string>? AdditionalSettings { get; set; }
    }
}
