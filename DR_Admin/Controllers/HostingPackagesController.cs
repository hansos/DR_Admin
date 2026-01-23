using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages hosting packages including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class HostingPackagesController : ControllerBase
{
    private readonly IHostingPackageService _hostingPackageService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingPackagesController>();

    public HostingPackagesController(IHostingPackageService hostingPackageService)
    {
        _hostingPackageService = hostingPackageService;
    }

    /// <summary>
    /// Retrieves all hosting packages in the system
    /// </summary>
    /// <returns>List of all hosting packages</returns>
    /// <response code="200">Returns the list of hosting packages</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<HostingPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingPackageDto>>> GetAllHostingPackages()
    {
        try
        {
            _log.Information("API: GetAllHostingPackages called by user {User}", User.Identity?.Name);
            
            var packages = await _hostingPackageService.GetAllHostingPackagesAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllHostingPackages");
            return StatusCode(500, "An error occurred while retrieving hosting packages");
        }
    }

    /// <summary>
    /// Retrieves only active hosting packages
    /// </summary>
    /// <returns>List of active hosting packages</returns>
    /// <response code="200">Returns the list of active hosting packages</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<HostingPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingPackageDto>>> GetActiveHostingPackages()
    {
        try
        {
            _log.Information("API: GetActiveHostingPackages called by user {User}", User.Identity?.Name);
            
            var packages = await _hostingPackageService.GetActiveHostingPackagesAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveHostingPackages");
            return StatusCode(500, "An error occurred while retrieving active hosting packages");
        }
    }

    /// <summary>
    /// Retrieves a specific hosting package by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the hosting package</param>
    /// <returns>The hosting package information</returns>
    /// <response code="200">Returns the hosting package data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If hosting package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(HostingPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingPackageDto>> GetHostingPackageById(int id)
    {
        try
        {
            _log.Information("API: GetHostingPackageById called for ID {PackageId} by user {User}", id, User.Identity?.Name);
            
            var package = await _hostingPackageService.GetHostingPackageByIdAsync(id);

            if (package == null)
            {
                _log.Information("API: Hosting package with ID {PackageId} not found", id);
                return NotFound($"Hosting package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostingPackageById for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while retrieving the hosting package");
        }
    }

    /// <summary>
    /// Creates a new hosting package in the system
    /// </summary>
    /// <param name="createDto">Hosting package information for creation</param>
    /// <returns>The newly created hosting package</returns>
    /// <response code="201">Returns the newly created hosting package</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(HostingPackageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingPackageDto>> CreateHostingPackage([FromBody] CreateHostingPackageDto createDto)
    {
        try
        {
            _log.Information("API: CreateHostingPackage called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateHostingPackage");
                return BadRequest(ModelState);
            }

            var package = await _hostingPackageService.CreateHostingPackageAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetHostingPackageById),
                new { id = package.Id },
                package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateHostingPackage");
            return StatusCode(500, "An error occurred while creating the hosting package");
        }
    }

    /// <summary>
    /// Updates an existing hosting package's information
    /// </summary>
    /// <param name="id">The unique identifier of the hosting package to update</param>
    /// <param name="updateDto">Updated hosting package information</param>
    /// <returns>The updated hosting package</returns>
    /// <response code="200">Returns the updated hosting package</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If hosting package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(HostingPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingPackageDto>> UpdateHostingPackage(int id, [FromBody] UpdateHostingPackageDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateHostingPackage called for ID {PackageId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateHostingPackage");
                return BadRequest(ModelState);
            }

            var package = await _hostingPackageService.UpdateHostingPackageAsync(id, updateDto);

            if (package == null)
            {
                _log.Information("API: Hosting package with ID {PackageId} not found for update", id);
                return NotFound($"Hosting package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateHostingPackage for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while updating the hosting package");
        }
    }

    /// <summary>
    /// Deletes a hosting package from the system
    /// </summary>
    /// <param name="id">The unique identifier of the hosting package to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If hosting package was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If hosting package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteHostingPackage(int id)
    {
        try
        {
            _log.Information("API: DeleteHostingPackage called for ID {PackageId} by user {User}", id, User.Identity?.Name);

            var result = await _hostingPackageService.DeleteHostingPackageAsync(id);

            if (!result)
            {
                _log.Information("API: Hosting package with ID {PackageId} not found for deletion", id);
                return NotFound($"Hosting package with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteHostingPackage for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while deleting the hosting package");
        }
    }
}
