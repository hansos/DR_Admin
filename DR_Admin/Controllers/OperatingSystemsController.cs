using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages operating systems including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OperatingSystemsController : ControllerBase
{
    private readonly IOperatingSystemService _operatingSystemService;
    private static readonly Serilog.ILogger _log = Log.ForContext<OperatingSystemsController>();

    public OperatingSystemsController(IOperatingSystemService operatingSystemService)
    {
        _operatingSystemService = operatingSystemService;
    }

    /// <summary>
    /// Retrieves all operating systems in the system
    /// </summary>
    /// <returns>List of all operating systems</returns>
    /// <response code="200">Returns the list of operating systems</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "OperatingSystem.Read")]
    [ProducesResponseType(typeof(IEnumerable<OperatingSystemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OperatingSystemDto>>> GetAllOperatingSystems()
    {
        try
        {
            _log.Information("API: GetAllOperatingSystems called by user {User}", User.Identity?.Name);

            var operatingSystems = await _operatingSystemService.GetAllOperatingSystemsAsync();
            return Ok(operatingSystems);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllOperatingSystems");
            return StatusCode(500, "An error occurred while retrieving operating systems");
        }
    }

    /// <summary>
    /// Retrieves only active operating systems
    /// </summary>
    /// <returns>List of active operating systems</returns>
    /// <response code="200">Returns the list of active operating systems</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "OperatingSystem.Read")]
    [ProducesResponseType(typeof(IEnumerable<OperatingSystemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OperatingSystemDto>>> GetActiveOperatingSystems()
    {
        try
        {
            _log.Information("API: GetActiveOperatingSystems called by user {User}", User.Identity?.Name);

            var operatingSystems = await _operatingSystemService.GetActiveOperatingSystemsAsync();
            return Ok(operatingSystems);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveOperatingSystems");
            return StatusCode(500, "An error occurred while retrieving active operating systems");
        }
    }

    /// <summary>
    /// Retrieves a specific operating system by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the operating system</param>
    /// <returns>The operating system information</returns>
    /// <response code="200">Returns the operating system data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If operating system is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "OperatingSystem.Read")]
    [ProducesResponseType(typeof(OperatingSystemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OperatingSystemDto>> GetOperatingSystemById(int id)
    {
        try
        {
            _log.Information("API: GetOperatingSystemById called for ID {OsId} by user {User}", id, User.Identity?.Name);

            var operatingSystem = await _operatingSystemService.GetOperatingSystemByIdAsync(id);

            if (operatingSystem == null)
            {
                _log.Information("API: Operating system with ID {OsId} not found", id);
                return NotFound($"Operating system with ID {id} not found");
            }

            return Ok(operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetOperatingSystemById for ID {OsId}", id);
            return StatusCode(500, "An error occurred while retrieving the operating system");
        }
    }

    /// <summary>
    /// Creates a new operating system in the system
    /// </summary>
    /// <param name="createDto">Operating system information for creation</param>
    /// <returns>The newly created operating system</returns>
    /// <response code="201">Returns the newly created operating system</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "OperatingSystem.Write")]
    [ProducesResponseType(typeof(OperatingSystemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OperatingSystemDto>> CreateOperatingSystem([FromBody] CreateOperatingSystemDto createDto)
    {
        try
        {
            _log.Information("API: CreateOperatingSystem called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateOperatingSystem");
                return BadRequest(ModelState);
            }

            var operatingSystem = await _operatingSystemService.CreateOperatingSystemAsync(createDto);

            return CreatedAtAction(
                nameof(GetOperatingSystemById),
                new { id = operatingSystem.Id },
                operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateOperatingSystem");
            return StatusCode(500, "An error occurred while creating the operating system");
        }
    }

    /// <summary>
    /// Updates an existing operating system's information
    /// </summary>
    /// <param name="id">The unique identifier of the operating system to update</param>
    /// <param name="updateDto">Updated operating system information</param>
    /// <returns>The updated operating system</returns>
    /// <response code="200">Returns the updated operating system</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If operating system is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "OperatingSystem.Write")]
    [ProducesResponseType(typeof(OperatingSystemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OperatingSystemDto>> UpdateOperatingSystem(int id, [FromBody] UpdateOperatingSystemDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateOperatingSystem called for ID {OsId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateOperatingSystem");
                return BadRequest(ModelState);
            }

            var operatingSystem = await _operatingSystemService.UpdateOperatingSystemAsync(id, updateDto);

            if (operatingSystem == null)
            {
                _log.Information("API: Operating system with ID {OsId} not found for update", id);
                return NotFound($"Operating system with ID {id} not found");
            }

            return Ok(operatingSystem);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateOperatingSystem for ID {OsId}", id);
            return StatusCode(500, "An error occurred while updating the operating system");
        }
    }

    /// <summary>
    /// Deletes an operating system from the system
    /// </summary>
    /// <param name="id">The unique identifier of the operating system to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If operating system was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If operating system is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "OperatingSystem.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteOperatingSystem(int id)
    {
        try
        {
            _log.Information("API: DeleteOperatingSystem called for ID {OsId} by user {User}", id, User.Identity?.Name);

            var result = await _operatingSystemService.DeleteOperatingSystemAsync(id);

            if (!result)
            {
                _log.Information("API: Operating system with ID {OsId} not found for deletion", id);
                return NotFound($"Operating system with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteOperatingSystem for ID {OsId}", id);
            return StatusCode(500, "An error occurred while deleting the operating system");
        }
    }
}
