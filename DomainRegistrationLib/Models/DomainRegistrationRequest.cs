namespace DomainRegistrationLib.Models
{
    public class DomainRegistrationRequest
    {
        public string DomainName { get; set; } = string.Empty;
        public int Years { get; set; } = 1;
        public bool PrivacyProtection { get; set; }
        public bool AutoRenew { get; set; }
        public List<string>? Nameservers { get; set; }
        public ContactInformation RegistrantContact { get; set; } = new();
        public ContactInformation? AdminContact { get; set; }
        public ContactInformation? TechContact { get; set; }
        public ContactInformation? BillingContact { get; set; }
    }

    public class ContactInformation
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
