using ISPAdmin.DTOs;
using ISPAdmin.Data.Enums;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing exchange rate download logs
/// </summary>
public interface IExchangeRateDownloadLogService
{
    /// <summary>
    /// Retrieves all exchange rate download logs
    /// </summary>
    Task<IEnumerable<ExchangeRateDownloadLogDto>> GetAllLogsAsync();

    /// <summary>
    /// Retrieves exchange rate download logs for a specific period
    /// </summary>
    Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsByPeriodAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves exchange rate download logs by source/provider
    /// </summary>
    Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsBySourceAsync(CurrencyRateSource source);

    /// <summary>
    /// Retrieves exchange rate download logs by currency pair
    /// </summary>
    Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsByCurrencyAsync(string baseCurrency, string? targetCurrency = null);

    /// <summary>
    /// Retrieves a specific exchange rate download log by ID
    /// </summary>
    Task<ExchangeRateDownloadLogDto?> GetLogByIdAsync(int id);

    /// <summary>
    /// Creates a new exchange rate download log entry
    /// </summary>
    Task<ExchangeRateDownloadLogDto> CreateLogAsync(CreateExchangeRateDownloadLogDto dto);

    /// <summary>
    /// Gets the most recent download log for a specific currency pair and source
    /// </summary>
    Task<ExchangeRateDownloadLogDto?> GetLastDownloadAsync(string baseCurrency, string? targetCurrency, CurrencyRateSource source);

    /// <summary>
    /// Gets summary statistics for exchange rate downloads
    /// </summary>
    Task<ExchangeRateDownloadSummaryDto> GetDownloadSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets logs for failed downloads only
    /// </summary>
    Task<IEnumerable<ExchangeRateDownloadLogDto>> GetFailedLogsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Checks if a download is needed based on last successful download time
    /// </summary>
    Task<bool> IsDownloadNeededAsync(string baseCurrency, string? targetCurrency, CurrencyRateSource source, TimeSpan minTimeBetweenDownloads);
}
