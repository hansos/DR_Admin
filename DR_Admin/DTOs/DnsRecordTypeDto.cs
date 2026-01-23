namespace ISPAdmin.DTOs;

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
