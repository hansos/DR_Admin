namespace ISPAdmin.Data.Entities;

public class DnsRecord : EntityBase
{
    public int DomainId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; } = 3600;
    public int? Priority { get; set; } // For MX and SRV records
    public int? Weight { get; set; } // For SRV records
    public int? Port { get; set; } // For SRV records

    /// <summary>
    /// When true, this record has local changes that have not yet been pushed to the DNS server.
    /// Set to true on create/update via the API; set to false after a successful server sync.
    /// </summary>
    public bool IsPendingSync { get; set; } = true;

    /// <summary>
    /// When true, this record has been soft-deleted locally and is awaiting hard-deletion
    /// once the removal is confirmed on the DNS server.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when the record was soft-deleted. Null when the record is not deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    public RegisteredDomain Domain { get; set; } = null!;
    public DnsRecordType DnsRecordType { get; set; } = null!;
}
