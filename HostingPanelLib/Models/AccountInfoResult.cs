namespace HostingPanelLib.Models
{
    public class AccountInfoResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public string? Username { get; set; }
        public string? Domain { get; set; }
        public string? Email { get; set; }
        public string? Plan { get; set; }
        public string? Status { get; set; }
        public int? DiskUsageMB { get; set; }
        public int? DiskQuotaMB { get; set; }
        public int? BandwidthUsageMB { get; set; }
        public int? BandwidthLimitMB { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public string? IpAddress { get; set; }
        public Dictionary<string, object>? AdditionalInfo { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? DatabaseName { get; internal set; }
        public string? DatabaseType { get; internal set; }
        public string? DatabaseUser { get; internal set; }
    }
}
