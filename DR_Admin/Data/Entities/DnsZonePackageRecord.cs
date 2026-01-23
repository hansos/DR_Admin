namespace ISPAdmin.Data.Entities;

public class DnsZonePackageRecord : EntityBase
{
    public int DnsZonePackageId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; } = 3600;
    public int? Priority { get; set; } // For MX and SRV records
    public int? Weight { get; set; } // For SRV records
    public int? Port { get; set; } // For SRV records
    public string? Notes { get; set; }

    public DnsZonePackage DnsZonePackage { get; set; } = null!;
    public DnsRecordType DnsRecordType { get; set; } = null!;
}
