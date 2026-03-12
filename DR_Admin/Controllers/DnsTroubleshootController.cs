using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides DNS troubleshooting checks for domains.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsTroubleshootController : ControllerBase
{
    private readonly IDnsTroubleshootService _dnsTroubleshootService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsTroubleshootController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsTroubleshootController"/> class.
    /// </summary>
    /// <param name="dnsTroubleshootService">DNS troubleshoot service.</param>
    public DnsTroubleshootController(IDnsTroubleshootService dnsTroubleshootService)
    {
        _dnsTroubleshootService = dnsTroubleshootService;
    }

    /// <summary>
    /// Runs troubleshooting tests for a domain.
    /// </summary>
    /// <param name="domainId">The domain identifier.</param>
    /// <returns>DNS troubleshooting report.</returns>
    /// <response code="200">Returns DNS troubleshooting report.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user does not have required permission.</response>
    /// <response code="404">If domain is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("domain/{domainId:int}")]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(DnsTroubleshootReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsTroubleshootReportDto>> RunForDomain(int domainId)
    {
        try
        {
            _log.Information("API: DnsTroubleshoot run requested for domain {DomainId} by user {User}", domainId, User.Identity?.Name);

            var report = await _dnsTroubleshootService.RunForDomainAsync(domainId);
            if (report == null)
            {
                return NotFound($"Domain with ID {domainId} not found");
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error running DNS troubleshoot for domain {DomainId}", domainId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while running DNS troubleshoot");
        }
    }
}
