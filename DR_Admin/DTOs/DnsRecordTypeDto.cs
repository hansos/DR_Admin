namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a DNS record type (e.g., A, AAAA, CNAME, MX, TXT)
/// </summary>
public class DnsRecordTypeDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasPriority { get; set; }
    public bool HasWeight { get; set; }
    public bool HasPort { get; set; }
    public bool IsEditableByUser { get; set; }
    public bool IsActive { get; set; }
    public int DefaultTTL { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new DNS record type
/// </summary>
public class CreateDnsRecordTypeDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasPriority { get; set; } = false;
    public bool HasWeight { get; set; } = false;
    public bool HasPort { get; set; } = false;
    public bool IsEditableByUser { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int DefaultTTL { get; set; } = 3600;
}


/// <summary>
/// Data transfer object for updating an existing DNS record type
/// </summary>
public class UpdateDnsRecordTypeDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasPriority { get; set; }
    public bool HasWeight { get; set; }
    public bool HasPort { get; set; }
    public bool IsEditableByUser { get; set; }
    public bool IsActive { get; set; }
    public int DefaultTTL { get; set; }
}
