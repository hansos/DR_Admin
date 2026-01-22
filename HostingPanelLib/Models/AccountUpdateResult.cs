namespace HostingPanelLib.Models
{
    public class AccountUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public string? UpdatedField { get; set; }
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
