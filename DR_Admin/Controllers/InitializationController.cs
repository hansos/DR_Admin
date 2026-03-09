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
    private readonly ISystemService _systemService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationController>();
#if DEBUG
    private const bool IsDebugBuild = true;
#else
    private const bool IsDebugBuild = false;
#endif

    public InitializationController(IInitializationService initializationService, ITldService tldService, ISystemService systemService)
    {
        _initializationService = initializationService;
        _tldService = tldService;
        _systemService = systemService;
    }

    /// <summary>
    /// Imports a customer user snapshot from a debug snapshot file.
    /// </summary>
    /// <param name="request">Import request containing the snapshot file name.</param>
    /// <returns>Summary of the import operation.</returns>
    [HttpPost("import-customer-snapshot")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminUserMyCompanyImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserMyCompanyImportResultDto>> ImportCustomerSnapshot([FromBody] AdminUserMyCompanyImportRequestDto request)
    {
        try
        {
            if (!IsDebugBuild)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This endpoint is only available in Debug mode." });
            }

            if (request == null || string.IsNullOrWhiteSpace(request.FileName))
            {
                return BadRequest(new { message = "FileName is required." });
            }

            var result = await _systemService.ImportCustomerUserSnapshotAsync(request);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error importing debug customer snapshot during initialization");
            return StatusCode(500, new { message = "An error occurred while importing customer snapshot." });
        }
    }

    /// <summary>
    /// Gets whether the database has already been initialized.
    /// </summary>
    /// <returns>Initialization status</returns>
    /// <response code="200">Returns the initialization status</response>
    [HttpGet("status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(InitializationStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<InitializationStatusDto>> Status()
    {
        var isInitialized = await _initializationService.IsInitializedAsync();
        return Ok(new InitializationStatusDto { IsInitialized = isInitialized });
    }

    /// <summary>
    /// Gets whether the API runs in Debug mode for initialization-time debug tooling.
    /// </summary>
    /// <returns>Build mode details including mode name and debug flag.</returns>
    /// <response code="200">Returns current build mode information</response>
    [HttpGet("build-mode")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> GetBuildMode()
    {
        var mode = IsDebugBuild ? "Debug" : "Release";
        return Ok(new
        {
            Mode = mode,
            IsDebug = IsDebugBuild
        });
    }

    /// <summary>
    /// Imports admin user and MyCompany profile from a debug snapshot file.
    /// </summary>
    /// <param name="request">Import request containing the snapshot file name.</param>
    /// <returns>Summary of the import operation.</returns>
    /// <response code="200">Returns import result details</response>
    /// <response code="400">If file name is missing</response>
    /// <response code="403">If API is not running in debug mode</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("import-admin-mycompany-snapshot")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminUserMyCompanyImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserMyCompanyImportResultDto>> ImportAdminMyCompanySnapshot([FromBody] AdminUserMyCompanyImportRequestDto request)
    {
        try
        {
            if (!IsDebugBuild)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This endpoint is only available in Debug mode." });
            }

            if (request == null || string.IsNullOrWhiteSpace(request.FileName))
            {
                return BadRequest(new { message = "FileName is required." });
            }

            var result = await _systemService.ImportAdminUserAndMyCompanyAsync(request);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error importing debug admin/MyCompany snapshot during initialization");
            return StatusCode(500, new { message = "An error occurred while importing admin/MyCompany snapshot." });
        }
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
    [HttpPost("initialize-admin")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(InitializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InitializationResponseDto>> InitializeAdmin([FromBody] InitializationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password) || 
            string.IsNullOrWhiteSpace(request.Email))
        {
            _log.Warning("Initialization attempt with missing required fields");
            return BadRequest(new { message = "Username, password, and email are required" });
        }

        var result = await _initializationService.InitializeAdminAsync(request);

        if (result == null)
        {
            _log.Warning("Initialization failed - users may already exist or invalid input");
            return BadRequest(new { message = "Initialization failed. Users may already exist in the system or invalid input provided." });
        }

        _log.Information("System initialized successfully with user: {Username}", result.Username);
        return Ok(result);
    }


    /// <summary>
    /// Initializes the user panel with the first customer user, company and primary contact person.
    /// </summary>
    /// <param name="request">Initialization request for the first user-panel account.</param>
    /// <returns>Initialization result for user-panel onboarding.</returns>
    /// <response code="200">Returns the initialization result if successful</response>
    /// <response code="400">If required fields are missing or initialization cannot proceed</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("initialize-customer")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserPanelInitializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserPanelInitializationResponseDto>> InitializeCustomer([FromBody] UserPanelInitializationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(request.CompanyName) ||
            string.IsNullOrWhiteSpace(request.ContactFirstName) ||
            string.IsNullOrWhiteSpace(request.ContactLastName))
        {
            _log.Warning("User panel initialization attempt with missing required fields");
            return BadRequest(new { message = "Username, email, password, company name, and contact person names are required" });
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            _log.Warning("User panel initialization attempt with mismatched passwords");
            return BadRequest(new { message = "Password and confirm password must match" });
        }

        var result = await _initializationService.InitializeUserPanelAsync(request);
        if (result == null)
        {
            _log.Warning("User panel initialization failed - customer users may already exist or input is invalid");
            return BadRequest(new { message = "Initialization failed. Customer users may already exist in the system or invalid input provided." });
        }

        _log.Information("User panel initialized successfully with user: {Username}", result.Username);
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

    /// <summary>
    /// Checks and updates code tables (Roles, CustomerStatuses, DnsRecordTypes, ServiceTypes)
    /// </summary>
    /// <returns>Result with statistics about code tables</returns>
    /// <response code="200">Returns the code tables update result with statistics</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// Ensures all standard code tables have their default values.
    /// This includes:
    /// - Roles (ADMIN, SUPPORT, SALES, FINANCE, CUSTOMER)
    /// - Customer Statuses (ACTIVE, PENDING, SUSPENDED, INACTIVE)
    /// - DNS Record Types (A, AAAA, CNAME, MX, NS, TXT, SRV, PTR, SOA, CAA)
    /// - Service Types (DOMAIN, HOSTING)
    /// 
    /// Only adds missing entries; existing entries are preserved.
    /// </remarks>
    [HttpPost("check-code-tables")]
    //[Authorize]
    [ProducesResponseType(typeof(CodeTablesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CodeTablesResponseDto>> CheckCodeTables()
    {
        try
        {
            _log.Information("Code tables check initiated by user: {User}", User.Identity?.Name);

            var result = await _initializationService.CheckAndUpdateCodeTablesAsync();

            if (result.Success)
            {
                _log.Information("Code tables check completed successfully");
                return Ok(result);
            }
            else
            {
                _log.Warning("Code tables check completed with errors: {Message}", result.Message);
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Unexpected error during code tables check");
            return StatusCode(500, new CodeTablesResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during code tables check"
            });
        }
    }

    /// <summary>
    /// Synchronizes second-level domains from the Public Suffix List into the database
    /// </summary>
    /// <returns>Synchronization result with statistics</returns>
    /// <response code="200">Returns the synchronization result with statistics</response>
    /// <response code="400">If TLD table is empty</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// Downloads the Public Suffix List from https://publicsuffix.org/list/public_suffix_list.dat
    /// and extracts second-level domains from the ICANN DOMAINS section.
    /// Only processes second-level domains for TLDs that already exist in the database.
    /// 
    /// Prerequisites:
    /// - TLD table must not be empty (sync TLDs first using sync-tlds endpoint)
    /// 
    /// Example:
    /// For TLD 'ac' with second-level domains 'com.ac', 'edu.ac', etc., 
    /// these will be added to the TLD table with IsSecondLevel=true if 'ac' exists.
    /// </remarks>
    [HttpPost("sync-secondlevel-domains")]
    //[Authorize]
    [ProducesResponseType(typeof(SecondLevelDomainSyncResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SecondLevelDomainSyncResponseDto>> SyncSecondLevelDomains()
    {
        try
        {
            _log.Information("Second-level domain sync initiated by user: {User}", User.Identity?.Name);

            var result = await _tldService.SyncSecondLevelDomainsAsync();

            if (result.Success)
            {
                _log.Information("Second-level domain sync completed successfully. Added: {Added}, Processed: {Processed}, Skipped: {Skipped}", 
                    result.SecondLevelDomainsAdded, result.ParentTldsProcessed, result.ParentTldsSkipped);
                return Ok(result);
            }
            else
            {
                // Check if the error is due to empty TLD table
                if (result.Message.Contains("TLD table is empty"))
                {
                    _log.Warning("Second-level domain sync failed: TLD table is empty");
                    return BadRequest(result);
                }

                _log.Warning("Second-level domain sync completed with errors: {Message}", result.Message);
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Unexpected error during second-level domain synchronization");
            return StatusCode(500, new SecondLevelDomainSyncResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during second-level domain synchronization",
                SyncTimestamp = DateTime.UtcNow
            });
        }
    }
}
