namespace ISPAdmin.DTOs;

public class DnsRecordDto
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Type { get; set; } = string.Empty; // For convenience, populated from DnsRecordType
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }
    public int? Priority { get; set; }
    public int? Weight { get; set; }
    public int? Port { get; set; }
    public bool IsEditableByUser { get; set; } // Populated from DnsRecordType
    public bool HasPriority { get; set; } // Populated from DnsRecordType
    public bool HasWeight { get; set; } // Populated from DnsRecordType
    public bool HasPort { get; set; } // Populated from DnsRecordType
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateDnsRecordDto
{
    public int DomainId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; } = 3600;
    public int? Priority { get; set; }
    public int? Weight { get; set; }
    public int? Port { get; set; }
}

public class UpdateDnsRecordDto
{
    public int DomainId { get; set; }
    public int DnsRecordTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }
    public int? Priority { get; set; }
    public int? Weight { get; set; }
    public int? Port { get; set; }
}
