namespace DomainRegistrationLib.Models
{
    public class DomainUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? DomainName { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
