using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages the reseller's own company profile used in invoices, mail templates, and letterheads.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MyCompanyController : ControllerBase
{
    private readonly IMyCompanyService _myCompanyService;
    private static readonly Serilog.ILogger _log = Log.ForContext<MyCompanyController>();

    public MyCompanyController(IMyCompanyService myCompanyService)
    {
        _myCompanyService = myCompanyService;
    }

    /// <summary>
    /// Retrieves the current company profile.
    /// </summary>
    /// <returns>The current company profile.</returns>
    /// <response code="200">Returns the current company profile.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required role.</response>
    /// <response code="404">If no company profile exists yet.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "MyCompany.Read")]
    [ProducesResponseType(typeof(MyCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MyCompanyDto>> GetMyCompany()
    {
        try
        {
            _log.Information("API: GetMyCompany called by user {User}", User.Identity?.Name);

            var company = await _myCompanyService.GetMyCompanyAsync();
            if (company == null)
            {
                return NotFound("My company profile not found");
            }

            return Ok(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetMyCompany");
            return StatusCode(500, "An error occurred while retrieving company profile");
        }
    }

    /// <summary>
    /// Creates or updates the company profile.
    /// </summary>
    /// <param name="dto">The company profile data.</param>
    /// <returns>The updated company profile.</returns>
    /// <response code="200">Returns the updated company profile.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut]
    [Authorize(Policy = "MyCompany.Write")]
    [ProducesResponseType(typeof(MyCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MyCompanyDto>> UpsertMyCompany([FromBody] UpsertMyCompanyDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _myCompanyService.UpsertMyCompanyAsync(dto);
            return Ok(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpsertMyCompany");
            return StatusCode(500, "An error occurred while saving company profile");
        }
    }
}