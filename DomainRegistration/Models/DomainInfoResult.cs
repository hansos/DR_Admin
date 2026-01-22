namespace DomainRegistrationLib.Models
{
    public class DomainInfoResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? DomainName { get; set; }
        public string? Status { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool AutoRenew { get; set; }
        public bool PrivacyProtection { get; set; }
        public bool Locked { get; set; }
        public List<string> Nameservers { get; set; } = new();
        public ContactInformation? RegistrantContact { get; set; }
        public ContactInformation? AdminContact { get; set; }
        public ContactInformation? TechContact { get; set; }
        public ContactInformation? BillingContact { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
