namespace HostingPanelLib.Models
{
    public class DatabaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? DatabaseId { get; set; }
        public string? DatabaseName { get; set; }
        public string? DatabaseType { get; set; }
        public string? Server { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? ConnectionString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
