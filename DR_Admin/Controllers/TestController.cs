using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides admin-only testing endpoints.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "Admin.Only")]
public class TestController : ControllerBase
{
    private readonly ISystemService _systemService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TestController>();
#if DEBUG
    private const bool IsDebugBuild = true;
#else
    private const bool IsDebugBuild = false;
#endif

    public TestController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    /// <summary>
    /// Sends test emails with both plain text and HTML bodies.
    /// </summary>
    /// <param name="request">Test email request containing sender and receiver addresses.</param>
    /// <returns>Detailed test email execution report.</returns>
    /// <response code="200">Returns detailed test email result report</response>
    /// <response code="400">If sender or receiver is missing</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not an admin</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("email")]
    [ProducesResponseType(typeof(TestEmailResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TestEmailResultDto>> SendTestEmail([FromBody] TestEmailRequestDto request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Request body is required");
            }

            if (string.IsNullOrWhiteSpace(request.SenderEmail))
            {
                return BadRequest("SenderEmail is required");
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverEmail))
            {
                return BadRequest("ReceiverEmail is required");
            }

            _log.Information("API: Test/SendTestEmail called by user {User} from {Sender} to {Receiver}",
                User.Identity?.Name,
                request.SenderEmail,
                request.ReceiverEmail);

            var result = await _systemService.SendTestEmailAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in Test/SendTestEmail");
            return StatusCode(500, "An error occurred while sending test email");
        }
    }

    /// <summary>
    /// Seeds test catalog data into selected tables when those tables are empty.
    /// </summary>
    /// <returns>Summary of inserted records grouped by table.</returns>
    /// <response code="200">Returns seeding result summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not an admin</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("seed-data")]
    [ProducesResponseType(typeof(SeedTestDataResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SeedTestDataResultDto>> SeedTestData()
    {
        try
        {
            _log.Information("API: Test/SeedTestData called by user {User}", User.Identity?.Name);
            var result = await _systemService.SeedTestDataAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in Test/SeedTestData");
            return StatusCode(500, "An error occurred while seeding test data");
        }
    }

    /// <summary>
    /// Exports the current admin user and MyCompany profile to a debug snapshot file.
    /// </summary>
    /// <param name="fileName">Optional snapshot file name. If omitted, a timestamped file name is generated.</param>
    /// <returns>Summary and payload for the exported debug snapshot.</returns>
    /// <response code="200">Returns export result details</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not an admin or API is not running in debug mode</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("admin-mycompany/export")]
    [ProducesResponseType(typeof(AdminUserMyCompanyExportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserMyCompanyExportResultDto>> ExportAdminMyCompany([FromQuery] string? fileName = null)
    {
        try
        {
            if (!IsDebugBuild)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "This endpoint is only available in Debug mode." });
            }

            _log.Information("API: Test/ExportAdminMyCompany called by user {User}", User.Identity?.Name);
            var result = await _systemService.ExportAdminUserAndMyCompanyAsync(fileName);

            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in Test/ExportAdminMyCompany");
            return StatusCode(500, "An error occurred while exporting admin/MyCompany snapshot");
        }
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
    [HttpPost("admin-mycompany/import")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminUserMyCompanyImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminUserMyCompanyImportResultDto>> ImportAdminMyCompany([FromBody] AdminUserMyCompanyImportRequestDto request)
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

            _log.Information("API: Test/ImportAdminMyCompany called by user {User} with file {FileName}", User.Identity?.Name, request.FileName);
            var result = await _systemService.ImportAdminUserAndMyCompanyAsync(request);

            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in Test/ImportAdminMyCompany");
            return StatusCode(500, "An error occurred while importing admin/MyCompany snapshot");
        }
    }

    /// <summary>
    /// Returns whether the API is running as a Debug or Release build.
    /// </summary>
    /// <returns>Build mode details including mode name and debug flag.</returns>
    /// <response code="200">Returns current build mode information</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not an admin</response>
    [HttpGet("build-mode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<object> GetBuildMode()
    {
        var mode = IsDebugBuild ? "Debug" : "Release";
        _log.Information("API: Test/GetBuildMode called by user {User}; Build mode: {Mode}", User.Identity?.Name, mode);

        return Ok(new
        {
            Mode = mode,
            IsDebug = IsDebugBuild
        });
    }
}
