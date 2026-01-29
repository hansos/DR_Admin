using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Controller for managing exchange rate download logs
/// </summary>
[ApiController]
[Route("api/exchange-rate-download-logs")]
[Authorize]
public class ExchangeRateDownloadLogsController : ControllerBase
{
    private readonly IExchangeRateDownloadLogService _logService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ExchangeRateDownloadLogsController>();

    public ExchangeRateDownloadLogsController(IExchangeRateDownloadLogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// Gets all exchange rate download logs
    /// </summary>
    /// <returns>List of all download logs</returns>
    [HttpGet]
    [Authorize(Policy = "ExchangeRateDownloadLog.Read")]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDownloadLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExchangeRateDownloadLogDto>>> GetAllLogs()
    {
        try
        {
            _log.Information("API request: Get all exchange rate download logs");
            var logs = await _logService.GetAllLogsAsync();
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting all exchange rate download logs");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving download logs", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets exchange rate download logs for a specific period
    /// </summary>
    /// <param name="startDate">Start date of the period (ISO 8601 format)</param>
    /// <param name="endDate">End date of the period (ISO 8601 format)</param>
    /// <returns>List of download logs for the period</returns>
    [HttpGet("period")]
    [Authorize(Policy = "ExchangeRateDownloadLog.Read")]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDownloadLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExchangeRateDownloadLogDto>>> GetLogsByPeriod(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                _log.Warning("Invalid date range: start date {StartDate} is after end date {EndDate}", startDate, endDate);
                return BadRequest(new { message = "Start date must be before or equal to end date" });
            }

            _log.Information("API request: Get exchange rate download logs for period {StartDate} to {EndDate}", startDate, endDate);
            var logs = await _logService.GetLogsByPeriodAsync(startDate, endDate);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting exchange rate download logs for period");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving download logs for the period", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a specific exchange rate download log by ID
    /// </summary>
    /// <param name="id">The log ID</param>
    /// <returns>The download log</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "ExchangeRateDownloadLog.Read")]
    [ProducesResponseType(typeof(ExchangeRateDownloadLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateDownloadLogDto>> GetLogById(int id)
    {
        try
        {
            _log.Information("API request: Get exchange rate download log with ID: {LogId}", id);
            var log = await _logService.GetLogByIdAsync(id);

            if (log == null)
            {
                _log.Warning("Exchange rate download log with ID {LogId} not found", id);
                return NotFound(new { message = $"Download log with ID {id} not found" });
            }

            return Ok(log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting exchange rate download log with ID: {LogId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving the download log", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets exchange rate download logs by source/provider
    /// </summary>
    /// <param name="source">The currency rate source</param>
    /// <returns>List of download logs for the source</returns>
    [HttpGet("by-source/{source}")]
    [Authorize(Policy = "ExchangeRateDownloadLog.Read")]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDownloadLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExchangeRateDownloadLogDto>>> GetLogsBySource(CurrencyRateSource source)
    {
        try
        {
            _log.Information("API request: Get exchange rate download logs for source {Source}", source);
            var logs = await _logService.GetLogsBySourceAsync(source);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting exchange rate download logs for source {Source}", source);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving download logs for the source", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets exchange rate download logs by currency pair
    /// </summary>
    /// <param name="baseCurrency">The base currency code (e.g., EUR)</param>
    /// <param name="targetCurrency">The target currency code (optional, e.g., USD)</param>
    /// <returns>List of download logs for the currency pair</returns>
    [HttpGet("by-currency/{baseCurrency}")]
    [Authorize(Policy = "ExchangeRateDownloadLog.Read")]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDownloadLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExchangeRateDownloadLogDto>>> GetLogsByCurrency(
        string baseCurrency,
        [FromQuery] string? targetCurrency = null)
    {
        try
        {
            _log.Information("API request: Get exchange rate download logs for {Base}/{Target}", 
                baseCurrency, targetCurrency ?? "ALL");
            var logs = await _logService.GetLogsByCurrencyAsync(baseCurrency, targetCurrency);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting exchange rate download logs for currency");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving download logs for the currency", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets summary statistics for exchange rate downloads
    /// </summary>
    /// <param name="startDate">Optional start date for the summary period</param>
    /// <param name="endDate">Optional end date for the summary period</param>
    /// <returns>Summary statistics</returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ExchangeRateDownloadSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateDownloadSummaryDto>> GetSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _log.Information("API request: Get exchange rate download summary");
            var summary = await _logService.GetDownloadSummaryAsync(startDate, endDate);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting exchange rate download summary");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving download summary", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets failed exchange rate download logs
    /// </summary>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    /// <returns>List of failed download logs</returns>
    [HttpGet("failed")]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDownloadLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExchangeRateDownloadLogDto>>> GetFailedLogs(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _log.Information("API request: Get failed exchange rate download logs");
            var logs = await _logService.GetFailedLogsAsync(startDate, endDate);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting failed exchange rate download logs");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving failed download logs", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new exchange rate download log entry (for manual logging)
    /// </summary>
    /// <param name="dto">The download log data</param>
    /// <returns>The created download log</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExchangeRateDownloadLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateDownloadLogDto>> CreateLog([FromBody] CreateExchangeRateDownloadLogDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("Invalid model state for creating exchange rate download log");
                return BadRequest(ModelState);
            }

            _log.Information("API request: Create exchange rate download log");
            var log = await _logService.CreateLogAsync(dto);
            return CreatedAtAction(nameof(GetLogById), new { id = log.Id }, log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating exchange rate download log");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while creating the download log", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the last successful download for a specific currency pair and source
    /// </summary>
    /// <param name="baseCurrency">The base currency code</param>
    /// <param name="source">The currency rate source</param>
    /// <param name="targetCurrency">Optional target currency code</param>
    /// <returns>The last successful download log or 404 if not found</returns>
    [HttpGet("last-download/{baseCurrency}/{source}")]
    [ProducesResponseType(typeof(ExchangeRateDownloadLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateDownloadLogDto>> GetLastDownload(
        string baseCurrency,
        CurrencyRateSource source,
        [FromQuery] string? targetCurrency = null)
    {
        try
        {
            _log.Information("API request: Get last download for {Base}/{Target} from {Source}", 
                baseCurrency, targetCurrency ?? "ALL", source);
            var log = await _logService.GetLastDownloadAsync(baseCurrency, targetCurrency, source);

            if (log == null)
            {
                return NotFound(new { message = "No previous download found" });
            }

            return Ok(log);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting last download");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving the last download", error = ex.Message });
        }
    }
}
