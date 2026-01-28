namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for summary statistics of exchange rate downloads
/// </summary>
public class ExchangeRateDownloadSummaryDto
{
    public int TotalDownloads { get; set; }
    public int SuccessfulDownloads { get; set; }
    public int FailedDownloads { get; set; }
    public int TotalRatesDownloaded { get; set; }
    public int TotalRatesAdded { get; set; }
    public int TotalRatesUpdated { get; set; }
    public DateTime? LastSuccessfulDownload { get; set; }
    public DateTime? LastFailedDownload { get; set; }
    public long AverageDurationMs { get; set; }
    public double SuccessRate { get; set; }
}
