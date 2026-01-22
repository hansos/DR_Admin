namespace HostingPanelLib.Models
{
    public class HostingAccountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public string? Username { get; set; }
        public string? Domain { get; set; }
        public string? PlanName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
