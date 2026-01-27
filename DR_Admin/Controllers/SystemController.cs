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
    [Authorize(Roles = "Admin")]
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
}
