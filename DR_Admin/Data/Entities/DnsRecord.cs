namespace ISPAdmin.Data.Entities;

public class DnsRecord
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string Type { get; set; } = string.Empty; // A, AAAA, CNAME, MX, TXT, NS, SRV, etc.
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; } = 3600;
    public int? Priority { get; set; } // For MX and SRV records
    public int? Weight { get; set; } // For SRV records
    public int? Port { get; set; } // For SRV records
    public bool IsEditableByUser { get; set; } = true; // Flag to control if end users can edit this record
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Domain Domain { get; set; } = null!;
}
