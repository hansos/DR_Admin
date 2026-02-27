namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores historical price changes for each registrar/TLD combination.
/// Includes downloaded and manually changed prices.
/// </summary>
public class RegistrarTldPriceChangeLog : EntityBase
{
    /// <summary>
    /// Foreign key to RegistrarTld.
    /// </summary>
    public int RegistrarTldId { get; set; }

    /// <summary>
    /// Optional foreign key to a download session. Null for manual changes.
    /// </summary>
    public int? DownloadSessionId { get; set; }

    /// <summary>
    /// Source of change: Download or Manual.
    /// </summary>
    public string ChangeSource { get; set; } = string.Empty;

    /// <summary>
    /// User/system identity that performed the change.
    /// </summary>
    public string? ChangedBy { get; set; }

    /// <summary>
    /// UTC timestamp for when the change was recorded.
    /// </summary>
    public DateTime ChangedAtUtc { get; set; }

    public decimal? OldRegistrationCost { get; set; }
    public decimal? NewRegistrationCost { get; set; }
    public decimal? OldRenewalCost { get; set; }
    public decimal? NewRenewalCost { get; set; }
    public decimal? OldTransferCost { get; set; }
    public decimal? NewTransferCost { get; set; }

    public string? OldCurrency { get; set; }
    public string? NewCurrency { get; set; }

    /// <summary>
    /// Optional description/context for this change.
    /// </summary>
    public string? Notes { get; set; }

    public RegistrarTld RegistrarTld { get; set; } = null!;
    public RegistrarTldPriceDownloadSession? DownloadSession { get; set; }
}
