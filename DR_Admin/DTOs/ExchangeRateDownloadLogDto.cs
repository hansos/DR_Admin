using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for exchange rate download log
/// </summary>
public class ExchangeRateDownloadLogDto
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string? TargetCurrency { get; set; }
    public CurrencyRateSource Source { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime DownloadTimestamp { get; set; }
    public int RatesDownloaded { get; set; }
    public int RatesAdded { get; set; }
    public int RatesUpdated { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public long DurationMs { get; set; }
    public string? Notes { get; set; }
    public bool IsStartupDownload { get; set; }
    public bool IsScheduledDownload { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
