namespace DomainRegistrationLib.Models
{
    public class DomainTransferRequest
    {
        public string DomainName { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public bool PrivacyProtection { get; set; }
        public bool AutoRenew { get; set; }
        public ContactInformation? RegistrantContact { get; set; }
        public ContactInformation? AdminContact { get; set; }
        public ContactInformation? TechContact { get; set; }
        public ContactInformation? BillingContact { get; set; }
    }
}
