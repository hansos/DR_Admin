namespace ISPAdmin.DTOs;

/// <summary>
/// Result of a DNS record sync operation for a single domain.
/// </summary>
public class DnsRecordSyncResult
{
    public string DomainName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
}

/// <summary>
/// Result of a bulk DNS record sync operation across all domains for a registrar.
/// </summary>
public class DnsBulkSyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int DomainsProcessed { get; set; }
    public int DomainsSucceeded { get; set; }
    public int DomainsFailed { get; set; }
    public int TotalCreated { get; set; }
    public int TotalUpdated { get; set; }
    public int TotalSkipped { get; set; }
    public List<DnsRecordSyncResult> DomainResults { get; set; } = new();
}

/// <summary>
/// Result of pushing a single DNS record to the registrar's DNS server.
/// </summary>
public class DnsPushRecordResult
{
    /// <summary>Whether the push succeeded.</summary>
    public bool Success { get; set; }

    /// <summary>The ID of the DNS record that was pushed.</summary>
    public int DnsRecordId { get; set; }

    /// <summary>Human-readable message describing the outcome.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>The action performed on the registrar (Upserted or Deleted).</summary>
    public string Action { get; set; } = string.Empty;
}

/// <summary>
/// Result of pushing all pending-sync DNS records for a domain to the registrar's DNS server.
/// </summary>
public class DnsPushPendingResult
{
    /// <summary>Whether the overall operation succeeded (no failures).</summary>
    public bool Success { get; set; }

    /// <summary>The domain name that was processed.</summary>
    public string DomainName { get; set; } = string.Empty;

    /// <summary>Human-readable summary of the operation outcome.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Number of records successfully upserted on the registrar.</summary>
    public int Upserted { get; set; }

    /// <summary>Number of records successfully deleted on the registrar.</summary>
    public int Deleted { get; set; }

    /// <summary>Number of records that failed to push.</summary>
    public int Failed { get; set; }

    /// <summary>Per-record details for each push attempt.</summary>
    public List<DnsPushRecordResult> RecordResults { get; set; } = new();
}

