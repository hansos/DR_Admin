namespace ISPAdmin.Data.Entities;

public class DnsRecordType : EntityBase
{
    public string Type { get; set; } = string.Empty; // A, AAAA, CNAME, MX, TXT, NS, SRV, etc.
    public string Description { get; set; } = string.Empty;
    public bool HasPriority { get; set; } = false; // MX, SRV
    public bool HasWeight { get; set; } = false; // SRV
    public bool HasPort { get; set; } = false; // SRV
    public bool IsEditableByUser { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int DefaultTTL { get; set; } = 3600;

    public ICollection<DnsRecord> DnsRecords { get; set; } = new List<DnsRecord>();
}
