namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a DNS record for a domain.
/// </summary>
public class DnsRecordDto
{
    /// <summary>Unique identifier of the DNS record.</summary>
    public int Id { get; set; }

    /// <summary>ID of the registered domain this record belongs to.</summary>
    public int DomainId { get; set; }

    /// <summary>ID of the DNS record type (A, AAAA, CNAME, MX, etc.).</summary>
    public int DnsRecordTypeId { get; set; }

    /// <summary>
    /// DNS record type string (e.g., A, AAAA, CNAME, MX, TXT, SRV).
    /// Populated from the associated DnsRecordType.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Record name (e.g., "@", "www", "mail").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Record value (e.g., IP address, hostname, or text content).</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Time-to-live in seconds.</summary>
    public int TTL { get; set; }

    /// <summary>Priority for MX and SRV records; null for all other types.</summary>
    public int? Priority { get; set; }

    /// <summary>Weight for SRV records; null for all other types.</summary>
    public int? Weight { get; set; }

    /// <summary>Port for SRV records; null for all other types.</summary>
    public int? Port { get; set; }

    /// <summary>
    /// Whether regular users are allowed to edit this record.
    /// Populated from the associated DnsRecordType.
    /// </summary>
    public bool IsEditableByUser { get; set; }

    /// <summary>Whether this record type uses the Priority field. Populated from DnsRecordType.</summary>
    public bool HasPriority { get; set; }

    /// <summary>Whether this record type uses the Weight field. Populated from DnsRecordType.</summary>
    public bool HasWeight { get; set; }

    /// <summary>Whether this record type uses the Port field. Populated from DnsRecordType.</summary>
    public bool HasPort { get; set; }

    /// <summary>
    /// When true, this record has local changes that have not yet been pushed to the DNS server.
    /// </summary>
    public bool IsPendingSync { get; set; }

    /// <summary>
    /// When true, this record has been soft-deleted locally and is awaiting
    /// hard-deletion once the removal is confirmed on the DNS server.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>Timestamp when the record was soft-deleted; null if not deleted.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>Timestamp when the record was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp when the record was last updated.</summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new DNS record.
/// </summary>
public class CreateDnsRecordDto
{
    /// <summary>ID of the registered domain this record belongs to.</summary>
    public int DomainId { get; set; }

    /// <summary>ID of the DNS record type.</summary>
    public int DnsRecordTypeId { get; set; }

    /// <summary>Record name (e.g., "@", "www", "mail").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Record value (e.g., IP address, hostname, or text content).</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Time-to-live in seconds. Defaults to the record type's DefaultTTL when set to 0.
    /// </summary>
    public int TTL { get; set; } = 3600;

    /// <summary>Priority for MX and SRV records; required when HasPriority is true on the record type.</summary>
    public int? Priority { get; set; }

    /// <summary>Weight for SRV records; required when HasWeight is true on the record type.</summary>
    public int? Weight { get; set; }

    /// <summary>Port for SRV records; required when HasPort is true on the record type.</summary>
    public int? Port { get; set; }

    /// <summary>
    /// When true, the record is flagged for synchronisation to the DNS server on the next sync operation.
    /// Defaults to true so all newly created records are included in the next push.
    /// </summary>
    public bool IsPendingSync { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing DNS record.
/// Updating a record automatically marks it as pending synchronisation.
/// </summary>
public class UpdateDnsRecordDto
{
    /// <summary>ID of the registered domain this record belongs to.</summary>
    public int DomainId { get; set; }

    /// <summary>ID of the DNS record type.</summary>
    public int DnsRecordTypeId { get; set; }

    /// <summary>Record name (e.g., "@", "www", "mail").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Record value (e.g., IP address, hostname, or text content).</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Time-to-live in seconds.</summary>
    public int TTL { get; set; }

    /// <summary>Priority for MX and SRV records; required when HasPriority is true on the record type.</summary>
    public int? Priority { get; set; }

    /// <summary>Weight for SRV records; required when HasWeight is true on the record type.</summary>
    public int? Weight { get; set; }

    /// <summary>Port for SRV records; required when HasPort is true on the record type.</summary>
    public int? Port { get; set; }
}

