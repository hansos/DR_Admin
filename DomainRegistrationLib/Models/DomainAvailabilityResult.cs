namespace DomainRegistrationLib.Models
{
    public class DomainAvailabilityResult
    {
        public bool Success { get; set; }
        public string DomainName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool IsTldSupported { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal? PremiumPrice { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
