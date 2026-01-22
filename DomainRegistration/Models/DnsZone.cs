namespace DomainRegistrationLib.Models
{
    public class DnsZone
    {
        public string DomainName { get; set; } = string.Empty;
        public List<DnsRecordModel> Records { get; set; } = new();
        public List<string> Nameservers { get; set; } = new();
    }
}
