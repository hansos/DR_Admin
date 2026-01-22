namespace DomainRegistrationLib.Models
{
    public class DomainRenewalRequest
    {
        public string DomainName { get; set; } = string.Empty;
        public int Years { get; set; } = 1;
    }
}
