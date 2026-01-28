using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a log entry for an exchange rate download operation
/// </summary>
public class ExchangeRateDownloadLog : EntityBase
{
    /// <summary>
    /// The base currency code (ISO 4217, e.g., "EUR")
    /// </summary>
    public string BaseCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The target currency code (ISO 4217, e.g., "USD", "GBP")
    /// If null or empty, represents a bulk download of all currencies
    /// </summary>
    public string? TargetCurrency { get; set; }

    /// <summary>
    /// The source/provider of this exchange rate download
    /// </summary>
    public CurrencyRateSource Source { get; set; }

    /// <summary>
    /// Whether the download was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The date and time when the download occurred
    /// </summary>
    public DateTime DownloadTimestamp { get; set; }

    /// <summary>
    /// Number of rates successfully downloaded in this operation
    /// </summary>
    public int RatesDownloaded { get; set; }

    /// <summary>
    /// Number of rates that were added to the database
    /// </summary>
    public int RatesAdded { get; set; }

    /// <summary>
    /// Number of rates that were updated in the database
    /// </summary>
    public int RatesUpdated { get; set; }

    /// <summary>
    /// Error message if the download failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code if the download failed
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Duration of the download operation in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Additional metadata or notes about the download
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this was triggered by application startup
    /// </summary>
    public bool IsStartupDownload { get; set; }

    /// <summary>
    /// Whether this was a scheduled/automatic download
    /// </summary>
    public bool IsScheduledDownload { get; set; }
}
