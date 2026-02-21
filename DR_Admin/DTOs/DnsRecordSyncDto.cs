namespace ISPAdmin.DTOs;

/// <summary>
/// Result of a DNS record sync operation for a single domain
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
/// Result of a bulk DNS record sync operation across all domains for a registrar
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
