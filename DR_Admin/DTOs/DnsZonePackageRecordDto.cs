namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a DNS zone package record
/// </summary>
public class DnsZonePackageRecordDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the DNS zone package record
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the DNS zone package ID this record belongs to
    /// </summary>
    public int DnsZonePackageId { get; set; }
    
    /// <summary>
    /// Gets or sets the DNS record type ID
    /// </summary>
    public int DnsRecordTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the record name (e.g., @, www, mail)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the record value (e.g., IP address, hostname)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value source type (Manual, ServerIp, PanelIp, ServerHost, PanelHost)
    /// </summary>
    public string ValueSourceType { get; set; } = "Manual";

    /// <summary>
    /// Gets or sets optional structured value source reference data
    /// </summary>
    public string? ValueSourceReference { get; set; }
    
    /// <summary>
    /// Gets or sets the Time To Live in seconds
    /// </summary>
    public int TTL { get; set; } = 3600;
    
    /// <summary>
    /// Gets or sets the priority (for MX and SRV records)
    /// </summary>
    public int? Priority { get; set; }
    
    /// <summary>
    /// Gets or sets the weight (for SRV records)
    /// </summary>
    public int? Weight { get; set; }
    
    /// <summary>
    /// Gets or sets the port (for SRV records)
    /// </summary>
    public int? Port { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about this record
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new DNS zone package record
/// </summary>
public class CreateDnsZonePackageRecordDto
{
    /// <summary>
    /// Gets or sets the DNS zone package ID this record belongs to
    /// </summary>
    public int DnsZonePackageId { get; set; }
    
    /// <summary>
    /// Gets or sets the DNS record type ID
    /// </summary>
    public int DnsRecordTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the record name (e.g., @, www, mail)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the record value (e.g., IP address, hostname)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value source type (Manual, ServerIp, PanelIp, ServerHost, PanelHost)
    /// </summary>
    public string ValueSourceType { get; set; } = "Manual";

    /// <summary>
    /// Gets or sets optional structured value source reference data
    /// </summary>
    public string? ValueSourceReference { get; set; }
    
    /// <summary>
    /// Gets or sets the Time To Live in seconds
    /// </summary>
    public int TTL { get; set; } = 3600;
    
    /// <summary>
    /// Gets or sets the priority (for MX and SRV records)
    /// </summary>
    public int? Priority { get; set; }
    
    /// <summary>
    /// Gets or sets the weight (for SRV records)
    /// </summary>
    public int? Weight { get; set; }
    
    /// <summary>
    /// Gets or sets the port (for SRV records)
    /// </summary>
    public int? Port { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about this record
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing DNS zone package record
/// </summary>
public class UpdateDnsZonePackageRecordDto
{
    /// <summary>
    /// Gets or sets the DNS record type ID
    /// </summary>
    public int DnsRecordTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the record name (e.g., @, www, mail)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the record value (e.g., IP address, hostname)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value source type (Manual, ServerIp, PanelIp, ServerHost, PanelHost)
    /// </summary>
    public string ValueSourceType { get; set; } = "Manual";

    /// <summary>
    /// Gets or sets optional structured value source reference data
    /// </summary>
    public string? ValueSourceReference { get; set; }
    
    /// <summary>
    /// Gets or sets the Time To Live in seconds
    /// </summary>
    public int TTL { get; set; } = 3600;
    
    /// <summary>
    /// Gets or sets the priority (for MX and SRV records)
    /// </summary>
    public int? Priority { get; set; }
    
    /// <summary>
    /// Gets or sets the weight (for SRV records)
    /// </summary>
    public int? Weight { get; set; }
    
    /// <summary>
    /// Gets or sets the port (for SRV records)
    /// </summary>
    public int? Port { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about this record
    /// </summary>
    public string? Notes { get; set; }
}
