using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Handles system initialization with the first admin user
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class InitializationController : ControllerBase
{
    private readonly IInitializationService _initializationService;
    private readonly ITldService _tldService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationController>();

    public InitializationController(IInitializationService initializationService, ITldService tldService)
    {
        _initializationService = initializationService;
        _tldService = tldService;
    }

    /// <summary>
    /// Initializes the system with the first admin user (only works if no users exist)
    /// </summary>
    /// <param name="request">Initial admin user credentials and information</param>
    /// <returns>Initialization result with user information</returns>
    /// <response code="200">Returns the initialization result if successful</response>
    /// <response code="400">If required fields are missing, users already exist, or input is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// This endpoint can only be used once to create the first admin user. Subsequent calls will fail.
    /// </remarks>
    [HttpPost("initialize")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(InitializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InitializationResponseDto>> Initialize([FromBody] InitializationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password) || 
            string.IsNullOrWhiteSpace(request.Email))
        {
            _log.Warning("Initialization attempt with missing required fields");
            return BadRequest(new { message = "Username, password, and email are required" });
        }

        var result = await _initializationService.InitializeAsync(request);

        if (result == null)
        {
            _log.Warning("Initialization failed - users may already exist or invalid input");
            return BadRequest(new { message = "Initialization failed. Users may already exist in the system or invalid input provided." });
        }

        _log.Information("System initialized successfully with user: {Username}", result.Username);
        return Ok(result);
    }

    /// <summary>
    /// Synchronizes TLDs from IANA's official TLD list into the database
    /// </summary>
    /// <param name="request">Synchronization options (optional)</param>
    /// <returns>Synchronization result with statistics</returns>
    /// <response code="200">Returns the synchronization result with statistics</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// Downloads the latest TLD list from https://data.iana.org/TLD/tlds-alpha-by-domain.txt
    /// and merges it into the TLD table. Existing TLDs are preserved with their configurations.
    /// 
    /// Options:
    /// - MarkAllInactiveBeforeSync: Set all existing TLDs to inactive before sync, then reactivate only those in IANA list
    /// - ActivateNewTlds: Automatically activate newly discovered TLDs (default: false)
    /// </remarks>
    [HttpPost("sync-tlds")]
    //[Authorize]
    [ProducesResponseType(typeof(TldSyncResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldSyncResponseDto>> SyncTlds([FromBody] TldSyncRequestDto? request)
    {
        try
        {
            _log.Information("TLD sync initiated by user: {User}", User.Identity?.Name);

            var syncRequest = request ?? new TldSyncRequestDto();
            var result = await _tldService.SyncTldsFromIanaAsync(syncRequest);

            if (result.Success)
            {
                _log.Information("TLD sync completed successfully. Added: {Added}, Updated: {Updated}, Total: {Total}", 
                    result.TldsAdded, result.TldsUpdated, result.TotalTldsInSource);
                return Ok(result);
            }
            else
            {
                _log.Warning("TLD sync completed with errors: {Message}", result.Message);
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Unexpected error during TLD synchronization");
            return StatusCode(500, new TldSyncResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during TLD synchronization",
                SyncTimestamp = DateTime.UtcNow
            });
        }
    }
}
