namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores each registrar price download run to support daily checks and historical traceability.
/// </summary>
public class RegistrarTldPriceDownloadSession : EntityBase
{
    /// <summary>
    /// Foreign key to registrar.
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// UTC timestamp when this download started.
    /// </summary>
    public DateTime StartedAtUtc { get; set; }

    /// <summary>
    /// UTC timestamp when this download finished.
    /// </summary>
    public DateTime? CompletedAtUtc { get; set; }

    /// <summary>
    /// Trigger source (e.g. Startup, ScheduledHourlyCheck, Manual).
    /// </summary>
    public string TriggerSource { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the download session completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of registrar/TLD combinations evaluated in this run.
    /// </summary>
    public int TldsProcessed { get; set; }

    /// <summary>
    /// Number of price changes detected and persisted.
    /// </summary>
    public int PriceChangesDetected { get; set; }

    /// <summary>
    /// Optional status/message details.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Error message when Success = false.
    /// </summary>
    public string? ErrorMessage { get; set; }

    public Registrar Registrar { get; set; } = null!;
    public ICollection<RegistrarTldPriceChangeLog> PriceChangeLogs { get; set; } = new List<RegistrarTldPriceChangeLog>();
}
