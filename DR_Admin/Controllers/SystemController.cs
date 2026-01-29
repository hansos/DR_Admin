using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// System-level operations including data normalization and maintenance tasks
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SystemController : ControllerBase
{
    private readonly ISystemService _systemService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemController>();

    public SystemController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    /// <summary>
    /// Normalizes all records in the database by updating normalized fields for exact searches
    /// </summary>
    /// <remarks>
    /// This endpoint updates all normalized name fields across all entities:
    /// - Country: NormalizedEnglishName, NormalizedLocalName
    /// - Coupon: NormalizedName
    /// - Customer: NormalizedName, NormalizedCompanyName, NormalizedContactPerson
    /// - Domain: NormalizedName
    /// - HostingPackage: NormalizedName
    /// - PaymentGateway: NormalizedName
    /// - PostalCode: NormalizedCode, NormalizedCountryCode, NormalizedCity, NormalizedState, NormalizedRegion, NormalizedDistrict
    /// - Registrar: NormalizedName
    /// - SalesAgent: NormalizedFirstName, NormalizedLastName
    /// - User: NormalizedUsername
    /// 
    /// This operation should be run:
    /// - After upgrading from a version without normalized fields
    /// - To fix any data inconsistencies
    /// - As part of data maintenance procedures
    /// </remarks>
    /// <returns>Summary of normalization results including count of records processed per entity</returns>
    /// <response code="200">Returns the normalization summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("normalize-all-records")]
    [Authorize(Policy = "Admin.Only")]
    [ProducesResponseType(typeof(NormalizationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NormalizationResultDto>> NormalizeAllRecords()
    {
        try
        {
            _log.Information("API: NormalizeAllRecords called by user {User}", User.Identity?.Name);
            
            var result = await _systemService.NormalizeAllRecordsAsync();

            if (!result.Success)
            {
                _log.Error("API: Normalization failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully normalized {TotalCount} records in {Duration}ms", 
                result.TotalRecordsProcessed, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in NormalizeAllRecords");
            return StatusCode(500, new NormalizationResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while normalizing records: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the system controller
    /// </summary>
    /// <returns>OK if the system is healthy</returns>
    /// <response code="200">System is healthy</response>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "SystemController"
        });
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <param name="backupFileName">Optional custom backup file name (without extension)</param>
    /// <remarks>
    /// This endpoint creates a backup of the database based on the configured database type:
    /// - SQLite: Copies the database file to the Backups directory
    /// - SQL Server: Creates a .bak file using SQL Server BACKUP DATABASE command
    /// - PostgreSQL: Uses pg_dump to create a backup file (requires pg_dump in PATH)
    /// 
    /// The backup file will be created in the Backups directory in the application root.
    /// A timestamp will be automatically appended to the filename.
    /// 
    /// This operation should be run:
    /// - Before major updates or data migrations
    /// - As part of regular backup procedures
    /// - Before restore operations
    /// </remarks>
    /// <returns>Summary of backup results including file path and size</returns>
    /// <response code="200">Returns the backup summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("backup")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BackupResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BackupResultDto>> CreateBackup([FromQuery] string? backupFileName = null)
    {
        try
        {
            _log.Information("API: CreateBackup called by user {User} with filename {FileName}", 
                User.Identity?.Name, backupFileName ?? "(auto-generated)");
            
            var result = await _systemService.CreateBackupAsync(backupFileName);

            if (!result.Success)
            {
                _log.Error("API: Backup failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully created backup at {BackupPath} ({Size} bytes) in {Duration}ms", 
                result.BackupFilePath, result.BackupFileSizeBytes, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateBackup");
            return StatusCode(500, new BackupResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while creating backup: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    /// <param name="backupFilePath">Full path to the backup file to restore</param>
    /// <remarks>
    /// This endpoint restores the database from a backup file based on the configured database type:
    /// - SQLite: Copies the backup file to replace the current database (creates pre-restore backup)
    /// - SQL Server: Uses SQL Server RESTORE DATABASE command
    /// - PostgreSQL: Uses pg_restore to restore the database (requires pg_restore in PATH)
    /// 
    /// WARNING: This operation will replace all current data with the backup data.
    /// For SQLite, a pre-restore backup of the current database is automatically created.
    /// 
    /// This operation should be run:
    /// - To recover from data corruption or loss
    /// - To restore a previous state of the database
    /// - Only when you are certain you want to replace current data
    /// </remarks>
    /// <returns>Summary of restore results</returns>
    /// <response code="200">Returns the restore summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If backup file is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("restore")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RestoreResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RestoreResultDto>> RestoreFromBackup([FromQuery] string backupFilePath)
    {
        try
        {
            _log.Information("API: RestoreFromBackup called by user {User} with file {FilePath}", 
                User.Identity?.Name, backupFilePath);

            if (string.IsNullOrWhiteSpace(backupFilePath))
            {
                return BadRequest(new RestoreResultDto
                {
                    Success = false,
                    ErrorMessage = "Backup file path is required"
                });
            }

            if (!System.IO.File.Exists(backupFilePath))
            {
                return NotFound(new RestoreResultDto
                {
                    Success = false,
                    ErrorMessage = $"Backup file not found: {backupFilePath}"
                });
            }
            
            var result = await _systemService.RestoreFromBackupAsync(backupFilePath);

            if (!result.Success)
            {
                _log.Error("API: Restore failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully restored database from {BackupPath} in {Duration}ms", 
                backupFilePath, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RestoreFromBackup");
            return StatusCode(500, new RestoreResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while restoring from backup: " + ex.Message
            });
        }
    }
}
