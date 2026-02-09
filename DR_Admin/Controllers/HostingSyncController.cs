using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages synchronization between database and hosting panel servers
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class HostingSyncController : ControllerBase
{
    private readonly IHostingManagerService _hostingManagerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingSyncController>();

    public HostingSyncController(IHostingManagerService hostingManagerService)
    {
        _hostingManagerService = hostingManagerService;
    }

    /// <summary>
    /// Imports a hosting account from the server to the database
    /// </summary>
    /// <param name="serverControlPanelId">The server control panel ID</param>
    /// <param name="externalAccountId">The account username/ID on the server</param>
    /// <returns>Sync operation result</returns>
    /// <response code="200">Returns the sync result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("import")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncResultDto>> ImportAccountFromServer(
        [FromQuery] int serverControlPanelId,
        [FromQuery] string externalAccountId)
    {
        try
        {
            _log.Information("API: ImportAccountFromServer called for panel {PanelId}, account {AccountId} by user {User}", 
                serverControlPanelId, externalAccountId, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(externalAccountId))
            {
                return BadRequest("External account ID is required");
            }

            var result = await _hostingManagerService.SyncAccountFromServerAsync(serverControlPanelId, externalAccountId);
            
            if (result.Success)
            {
                _log.Information("API: Successfully imported account {AccountId} from server", externalAccountId);
            }
            else
            {
                _log.Warning("API: Failed to import account {AccountId}: {Message}", externalAccountId, result.Message);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ImportAccountFromServer for account {AccountId}", externalAccountId);
            return StatusCode(500, "An error occurred while importing the account");
        }
    }

    /// <summary>
    /// Exports a hosting account from the database to the server
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID in the database</param>
    /// <returns>Sync operation result</returns>
    /// <response code="200">Returns the sync result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("export/{hostingAccountId}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncResultDto>> ExportAccountToServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: ExportAccountToServer called for account {AccountId} by user {User}", 
                hostingAccountId, User.Identity?.Name);

            var result = await _hostingManagerService.SyncAccountToServerAsync(hostingAccountId);
            
            if (result.Success)
            {
                _log.Information("API: Successfully exported account {AccountId} to server", hostingAccountId);
            }
            else
            {
                _log.Warning("API: Failed to export account {AccountId}: {Message}", hostingAccountId, result.Message);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ExportAccountToServer for account {AccountId}", hostingAccountId);
            return StatusCode(500, "An error occurred while exporting the account");
        }
    }

    /// <summary>
    /// Imports all hosting accounts from a server to the database
    /// </summary>
    /// <param name="serverControlPanelId">The server control panel ID</param>
    /// <returns>Bulk sync operation result</returns>
    /// <response code="200">Returns the bulk sync result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("import-all/{serverControlPanelId}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncResultDto>> ImportAllAccountsFromServer(int serverControlPanelId)
    {
        try
        {
            _log.Information("API: ImportAllAccountsFromServer called for panel {PanelId} by user {User}", 
                serverControlPanelId, User.Identity?.Name);

            var result = await _hostingManagerService.SyncAllAccountsFromServerAsync(serverControlPanelId);
            
            _log.Information("API: Bulk import completed for panel {PanelId}. Synced: {Count}", 
                serverControlPanelId, result.RecordsSynced);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ImportAllAccountsFromServer for panel {PanelId}", serverControlPanelId);
            return StatusCode(500, "An error occurred while importing accounts");
        }
    }

    /// <summary>
    /// Compares a hosting account in the database with its state on the server
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <returns>Comparison result showing differences</returns>
    /// <response code="200">Returns the comparison result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("compare/{hostingAccountId}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(SyncComparisonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncComparisonDto>> CompareWithServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: CompareWithServer called for account {AccountId} by user {User}", 
                hostingAccountId, User.Identity?.Name);

            var comparison = await _hostingManagerService.CompareDatabaseWithServerAsync(hostingAccountId);
            
            _log.Information("API: Comparison completed for account {AccountId}. In sync: {InSync}", 
                hostingAccountId, comparison.InSync);

            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CompareWithServer for account {AccountId}", hostingAccountId);
            return StatusCode(500, "An error occurred while comparing the account");
        }
    }
}
