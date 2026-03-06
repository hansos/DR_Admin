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
