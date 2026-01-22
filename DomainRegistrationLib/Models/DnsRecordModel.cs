namespace DomainRegistrationLib.Models
{
    public class DnsRecordModel
    {
        public int? Id { get; set; }
        public string Type { get; set; } = string.Empty; // A, AAAA, CNAME, MX, TXT, NS, etc.
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int TTL { get; set; } = 3600;
        public int? Priority { get; set; } // For MX and SRV records
        public int? Weight { get; set; } // For SRV records
        public int? Port { get; set; } // For SRV records
    }
}
