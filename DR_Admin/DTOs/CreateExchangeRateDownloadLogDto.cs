using ISPAdmin.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for creating an exchange rate download log entry
/// </summary>
public class CreateExchangeRateDownloadLogDto
{
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string BaseCurrency { get; set; } = string.Empty;

    [StringLength(3, MinimumLength = 3)]
    public string? TargetCurrency { get; set; }

    [Required]
    public CurrencyRateSource Source { get; set; }

    [Required]
    public bool Success { get; set; }

    [Required]
    public DateTime DownloadTimestamp { get; set; } = DateTime.UtcNow;

    [Range(0, int.MaxValue)]
    public int RatesDownloaded { get; set; }

    [Range(0, int.MaxValue)]
    public int RatesAdded { get; set; }

    [Range(0, int.MaxValue)]
    public int RatesUpdated { get; set; }

    [StringLength(2000)]
    public string? ErrorMessage { get; set; }

    [StringLength(100)]
    public string? ErrorCode { get; set; }

    [Range(0, long.MaxValue)]
    public long DurationMs { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsStartupDownload { get; set; }

    public bool IsScheduledDownload { get; set; }
}
