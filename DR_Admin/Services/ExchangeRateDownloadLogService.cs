using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing exchange rate download logs
/// </summary>
public class ExchangeRateDownloadLogService : IExchangeRateDownloadLogService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ExchangeRateDownloadLogService>();

    public ExchangeRateDownloadLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ExchangeRateDownloadLogDto>> GetAllLogsAsync()
    {
        try
        {
            _log.Information("Fetching all exchange rate download logs");

            var logs = await _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .OrderByDescending(l => l.DownloadTimestamp)
                .ToListAsync();

            var logDtos = logs.Select(MapToDto);

            _log.Information("Successfully fetched {Count} exchange rate download logs", logs.Count);
            return logDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all exchange rate download logs");
            throw;
        }
    }

    public async Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _log.Information("Fetching exchange rate download logs for period {StartDate} to {EndDate}", startDate, endDate);

            var logs = await _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .Where(l => l.DownloadTimestamp >= startDate && l.DownloadTimestamp <= endDate)
                .OrderByDescending(l => l.DownloadTimestamp)
                .ToListAsync();

            var logDtos = logs.Select(MapToDto);

            _log.Information("Successfully fetched {Count} exchange rate download logs for the period", logs.Count);
            return logDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate download logs for period");
            throw;
        }
    }

    public async Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsBySourceAsync(CurrencyRateSource source)
    {
        try
        {
            _log.Information("Fetching exchange rate download logs for source {Source}", source);

            var logs = await _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .Where(l => l.Source == source)
                .OrderByDescending(l => l.DownloadTimestamp)
                .ToListAsync();

            var logDtos = logs.Select(MapToDto);

            _log.Information("Successfully fetched {Count} exchange rate download logs for source {Source}", logs.Count, source);
            return logDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate download logs for source {Source}", source);
            throw;
        }
    }

    public async Task<IEnumerable<ExchangeRateDownloadLogDto>> GetLogsByCurrencyAsync(string baseCurrency, string? targetCurrency = null)
    {
        try
        {
            _log.Information("Fetching exchange rate download logs for {Base}/{Target}", baseCurrency, targetCurrency ?? "ALL");

            var query = _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .Where(l => l.BaseCurrency == baseCurrency);

            if (!string.IsNullOrEmpty(targetCurrency))
            {
                query = query.Where(l => l.TargetCurrency == targetCurrency);
            }

            var logs = await query
                .OrderByDescending(l => l.DownloadTimestamp)
                .ToListAsync();

            var logDtos = logs.Select(MapToDto);

            _log.Information("Successfully fetched {Count} exchange rate download logs for currency", logs.Count);
            return logDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate download logs for currency");
            throw;
        }
    }

    public async Task<ExchangeRateDownloadLogDto?> GetLogByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching exchange rate download log with ID: {LogId}", id);

            var log = await _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (log == null)
            {
                _log.Warning("Exchange rate download log with ID {LogId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched exchange rate download log with ID: {LogId}", id);
            return MapToDto(log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate download log with ID: {LogId}", id);
            throw;
        }
    }

    public async Task<ExchangeRateDownloadLogDto> CreateLogAsync(CreateExchangeRateDownloadLogDto dto)
    {
        try
        {
            _log.Information("Creating exchange rate download log for {Base}/{Target} from {Source}", 
                dto.BaseCurrency, dto.TargetCurrency ?? "ALL", dto.Source);

            var log = new ExchangeRateDownloadLog
            {
                BaseCurrency = dto.BaseCurrency,
                TargetCurrency = dto.TargetCurrency,
                Source = dto.Source,
                Success = dto.Success,
                DownloadTimestamp = dto.DownloadTimestamp,
                RatesDownloaded = dto.RatesDownloaded,
                RatesAdded = dto.RatesAdded,
                RatesUpdated = dto.RatesUpdated,
                ErrorMessage = dto.ErrorMessage,
                ErrorCode = dto.ErrorCode,
                DurationMs = dto.DurationMs,
                Notes = dto.Notes,
                IsStartupDownload = dto.IsStartupDownload,
                IsScheduledDownload = dto.IsScheduledDownload
            };

            _context.ExchangeRateDownloadLogs.Add(log);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created exchange rate download log with ID: {LogId}", log.Id);
            return MapToDto(log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating exchange rate download log");
            throw;
        }
    }

    public async Task<ExchangeRateDownloadLogDto?> GetLastDownloadAsync(string baseCurrency, string? targetCurrency, CurrencyRateSource source)
    {
        try
        {
            _log.Debug("Fetching last download for {Base}/{Target} from {Source}", 
                baseCurrency, targetCurrency ?? "ALL", source);

            var query = _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .Where(l => l.BaseCurrency == baseCurrency && l.Source == source && l.Success);

            if (!string.IsNullOrEmpty(targetCurrency))
            {
                query = query.Where(l => l.TargetCurrency == targetCurrency);
            }
            else
            {
                query = query.Where(l => l.TargetCurrency == null || l.TargetCurrency == "");
            }

            var log = await query
                .OrderByDescending(l => l.DownloadTimestamp)
                .FirstOrDefaultAsync();

            if (log == null)
            {
                _log.Debug("No previous download found for {Base}/{Target} from {Source}", 
                    baseCurrency, targetCurrency ?? "ALL", source);
                return null;
            }

            return MapToDto(log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching last download");
            throw;
        }
    }

    public async Task<ExchangeRateDownloadSummaryDto> GetDownloadSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            _log.Information("Fetching exchange rate download summary");

            var query = _context.ExchangeRateDownloadLogs.AsNoTracking();

            if (startDate.HasValue)
                query = query.Where(l => l.DownloadTimestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.DownloadTimestamp <= endDate.Value);

            var logs = await query.ToListAsync();

            var summary = new ExchangeRateDownloadSummaryDto
            {
                TotalDownloads = logs.Count,
                SuccessfulDownloads = logs.Count(l => l.Success),
                FailedDownloads = logs.Count(l => !l.Success),
                TotalRatesDownloaded = logs.Sum(l => l.RatesDownloaded),
                TotalRatesAdded = logs.Sum(l => l.RatesAdded),
                TotalRatesUpdated = logs.Sum(l => l.RatesUpdated),
                LastSuccessfulDownload = logs.Where(l => l.Success).Max(l => (DateTime?)l.DownloadTimestamp),
                LastFailedDownload = logs.Where(l => !l.Success).Max(l => (DateTime?)l.DownloadTimestamp),
                AverageDurationMs = logs.Any() ? (long)logs.Average(l => l.DurationMs) : 0,
                SuccessRate = logs.Any() ? (double)logs.Count(l => l.Success) / logs.Count * 100 : 0
            };

            _log.Information("Successfully calculated exchange rate download summary");
            return summary;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching exchange rate download summary");
            throw;
        }
    }

    public async Task<IEnumerable<ExchangeRateDownloadLogDto>> GetFailedLogsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            _log.Information("Fetching failed exchange rate download logs");

            var query = _context.ExchangeRateDownloadLogs
                .AsNoTracking()
                .Where(l => !l.Success);

            if (startDate.HasValue)
                query = query.Where(l => l.DownloadTimestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.DownloadTimestamp <= endDate.Value);

            var logs = await query
                .OrderByDescending(l => l.DownloadTimestamp)
                .ToListAsync();

            var logDtos = logs.Select(MapToDto);

            _log.Information("Successfully fetched {Count} failed exchange rate download logs", logs.Count);
            return logDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching failed exchange rate download logs");
            throw;
        }
    }

    public async Task<bool> IsDownloadNeededAsync(string baseCurrency, string? targetCurrency, CurrencyRateSource source, TimeSpan minTimeBetweenDownloads)
    {
        try
        {
            var lastDownload = await GetLastDownloadAsync(baseCurrency, targetCurrency, source);

            if (lastDownload == null)
            {
                _log.Debug("No previous download found, download needed");
                return true;
            }

            var timeSinceLastDownload = DateTime.UtcNow - lastDownload.DownloadTimestamp;
            var isNeeded = timeSinceLastDownload >= minTimeBetweenDownloads;

            _log.Debug("Last download was {TimeSince} ago, minimum is {MinTime}, download needed: {IsNeeded}",
                timeSinceLastDownload, minTimeBetweenDownloads, isNeeded);

            return isNeeded;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking if download is needed");
            throw;
        }
    }

    private static ExchangeRateDownloadLogDto MapToDto(ExchangeRateDownloadLog log)
    {
        return new ExchangeRateDownloadLogDto
        {
            Id = log.Id,
            BaseCurrency = log.BaseCurrency,
            TargetCurrency = log.TargetCurrency,
            Source = log.Source,
            SourceName = log.Source.ToString(),
            Success = log.Success,
            DownloadTimestamp = log.DownloadTimestamp,
            RatesDownloaded = log.RatesDownloaded,
            RatesAdded = log.RatesAdded,
            RatesUpdated = log.RatesUpdated,
            ErrorMessage = log.ErrorMessage,
            ErrorCode = log.ErrorCode,
            DurationMs = log.DurationMs,
            Notes = log.Notes,
            IsStartupDownload = log.IsStartupDownload,
            IsScheduledDownload = log.IsScheduledDownload,
            CreatedAt = log.CreatedAt,
            UpdatedAt = log.UpdatedAt
        };
    }
}
