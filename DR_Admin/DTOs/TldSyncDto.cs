namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for TLD synchronization request
/// </summary>
public class TldSyncRequestDto
{
    /// <summary>
    /// Whether to mark all TLDs as inactive before syncing (default: false)
    /// </summary>
    public bool MarkAllInactiveBeforeSync { get; set; } = false;

    /// <summary>
    /// Whether to activate newly added TLDs automatically (default: false)
    /// </summary>
    public bool ActivateNewTlds { get; set; } = false;
}

/// <summary>
/// Data transfer object for TLD synchronization response
/// </summary>
public class TldSyncResponseDto
{
    /// <summary>
    /// Indicates whether the synchronization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message describing the result of the synchronization
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Number of TLDs added during synchronization
    /// </summary>
    public int TldsAdded { get; set; }

    /// <summary>
    /// Number of TLDs updated during synchronization
    /// </summary>
    public int TldsUpdated { get; set; }

    /// <summary>
    /// Total number of TLDs found in the IANA source
    /// </summary>
    public int TotalTldsInSource { get; set; }

    /// <summary>
    /// Timestamp when the synchronization was performed
    /// </summary>
    public DateTime SyncTimestamp { get; set; }
}
