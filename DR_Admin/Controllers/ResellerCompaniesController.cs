using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages reseller companies including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ResellerCompaniesController : ControllerBase
{
    private readonly IResellerCompanyService _resellerCompanyService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ResellerCompaniesController>();

    public ResellerCompaniesController(IResellerCompanyService resellerCompanyService)
    {
        _resellerCompanyService = resellerCompanyService;
    }

    /// <summary>
    /// Retrieves all reseller companies in the system
    /// </summary>
    /// <returns>List of all reseller companies</returns>
    /// <response code="200">Returns the list of reseller companies</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ResellerCompanyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResellerCompanyDto>>> GetAllResellerCompanies()
    {
        try
        {
            _log.Information("API: GetAllResellerCompanies called by user {User}", User.Identity?.Name);
            
            var companies = await _resellerCompanyService.GetAllResellerCompaniesAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllResellerCompanies");
            return StatusCode(500, "An error occurred while retrieving reseller companies");
        }
    }

    /// <summary>
    /// Retrieves only active reseller companies
    /// </summary>
    /// <returns>List of active reseller companies</returns>
    /// <response code="200">Returns the list of active reseller companies</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(IEnumerable<ResellerCompanyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResellerCompanyDto>>> GetActiveResellerCompanies()
    {
        try
        {
            _log.Information("API: GetActiveResellerCompanies called by user {User}", User.Identity?.Name);
            
            var companies = await _resellerCompanyService.GetActiveResellerCompaniesAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveResellerCompanies");
            return StatusCode(500, "An error occurred while retrieving active reseller companies");
        }
    }

    /// <summary>
    /// Retrieves a specific reseller company by ID
    /// </summary>
    /// <param name="id">The reseller company ID</param>
    /// <returns>The reseller company details</returns>
    /// <response code="200">Returns the reseller company</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If reseller company is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResellerCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResellerCompanyDto>> GetResellerCompanyById(int id)
    {
        try
        {
            _log.Information("API: GetResellerCompanyById called with ID {ResellerCompanyId} by user {User}", 
                id, User.Identity?.Name);
            
            var company = await _resellerCompanyService.GetResellerCompanyByIdAsync(id);
            
            if (company == null)
            {
                return NotFound($"Reseller company with ID {id} not found");
            }
            
            return Ok(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetResellerCompanyById for ID {ResellerCompanyId}", id);
            return StatusCode(500, "An error occurred while retrieving the reseller company");
        }
    }

    /// <summary>
    /// Creates a new reseller company
    /// </summary>
    /// <param name="createDto">The reseller company creation data</param>
    /// <returns>The created reseller company</returns>
    /// <response code="201">Returns the newly created reseller company</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResellerCompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResellerCompanyDto>> CreateResellerCompany([FromBody] CreateResellerCompanyDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateResellerCompany called by user {User}", User.Identity?.Name);
            
            var company = await _resellerCompanyService.CreateResellerCompanyAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetResellerCompanyById),
                new { id = company.Id },
                company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateResellerCompany");
            return StatusCode(500, "An error occurred while creating the reseller company");
        }
    }

    /// <summary>
    /// Updates an existing reseller company
    /// </summary>
    /// <param name="id">The reseller company ID to update</param>
    /// <param name="updateDto">The updated reseller company data</param>
    /// <returns>The updated reseller company</returns>
    /// <response code="200">Returns the updated reseller company</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If reseller company is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResellerCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResellerCompanyDto>> UpdateResellerCompany(int id, [FromBody] UpdateResellerCompanyDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateResellerCompany called for ID {ResellerCompanyId} by user {User}", 
                id, User.Identity?.Name);
            
            var company = await _resellerCompanyService.UpdateResellerCompanyAsync(id, updateDto);
            
            if (company == null)
            {
                return NotFound($"Reseller company with ID {id} not found");
            }
            
            return Ok(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateResellerCompany for ID {ResellerCompanyId}", id);
            return StatusCode(500, "An error occurred while updating the reseller company");
        }
    }

    /// <summary>
    /// Deletes a reseller company
    /// </summary>
    /// <param name="id">The reseller company ID to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the reseller company was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If reseller company is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteResellerCompany(int id)
    {
        try
        {
            _log.Information("API: DeleteResellerCompany called for ID {ResellerCompanyId} by user {User}", 
                id, User.Identity?.Name);
            
            var deleted = await _resellerCompanyService.DeleteResellerCompanyAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Reseller company with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteResellerCompany for ID {ResellerCompanyId}", id);
            return StatusCode(500, "An error occurred while deleting the reseller company");
        }
    }
}
