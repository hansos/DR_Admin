namespace ISPAdmin.Data.Entities;

public class DnsRecord
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; } = 3600;
    public int? Priority { get; set; } // For MX and SRV records
    public int? Weight { get; set; } // For SRV records
    public int? Port { get; set; } // For SRV records
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Domain Domain { get; set; } = null!;
    public DnsRecordType DnsRecordType { get; set; } = null!;
}
