namespace HostingPanelLib.Models
{
    public class MailAccountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public string? EmailAddress { get; set; }
        public string? Domain { get; set; }
        public int? QuotaMB { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
