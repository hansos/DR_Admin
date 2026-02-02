namespace DomainRegistrationLib.Models
{
    /// <summary>
    /// Result containing a list of domains registered with a registrar
    /// </summary>
    public class RegisteredDomainsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<RegisteredDomainInfo> Domains { get; set; } = [];
        public int TotalCount { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = [];
    }

    /// <summary>
    /// Basic information about a registered domain
    /// </summary>
    public class RegisteredDomainInfo
    {
        public string DomainName { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool AutoRenew { get; set; }
        public bool Locked { get; set; }
        public bool PrivacyProtection { get; set; }
        public List<string> Nameservers { get; set; } = [];
    }
}
