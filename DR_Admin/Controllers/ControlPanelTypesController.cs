using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages control panel types including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ControlPanelTypesController : ControllerBase
{
    private readonly IControlPanelTypeService _controlPanelTypeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ControlPanelTypesController>();

    public ControlPanelTypesController(IControlPanelTypeService controlPanelTypeService)
    {
        _controlPanelTypeService = controlPanelTypeService;
    }

    /// <summary>
    /// Retrieves all control panel types in the system
    /// </summary>
    /// <returns>List of all control panel types</returns>
    /// <response code="200">Returns the list of control panel types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<ControlPanelTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ControlPanelTypeDto>>> GetAllControlPanelTypes()
    {
        try
        {
            _log.Information("API: GetAllControlPanelTypes called by user {User}", User.Identity?.Name);
            
            var types = await _controlPanelTypeService.GetAllControlPanelTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllControlPanelTypes");
            return StatusCode(500, "An error occurred while retrieving control panel types");
        }
    }

    /// <summary>
    /// Retrieves only active control panel types
    /// </summary>
    /// <returns>List of active control panel types</returns>
    /// <response code="200">Returns the list of active control panel types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<ControlPanelTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ControlPanelTypeDto>>> GetActiveControlPanelTypes()
    {
        try
        {
            _log.Information("API: GetActiveControlPanelTypes called by user {User}", User.Identity?.Name);
            
            var types = await _controlPanelTypeService.GetActiveControlPanelTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveControlPanelTypes");
            return StatusCode(500, "An error occurred while retrieving active control panel types");
        }
    }

    /// <summary>
    /// Retrieves a specific control panel type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the control panel type</param>
    /// <returns>The control panel type information</returns>
    /// <response code="200">Returns the control panel type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If control panel type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(ControlPanelTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ControlPanelTypeDto>> GetControlPanelTypeById(int id)
    {
        try
        {
            _log.Information("API: GetControlPanelTypeById called for ID {TypeId} by user {User}", id, User.Identity?.Name);
            
            var type = await _controlPanelTypeService.GetControlPanelTypeByIdAsync(id);

            if (type == null)
            {
                _log.Information("API: Control panel type with ID {TypeId} not found", id);
                return NotFound($"Control panel type with ID {id} not found");
            }

            return Ok(type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetControlPanelTypeById for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the control panel type");
        }
    }

    /// <summary>
    /// Creates a new control panel type in the system
    /// </summary>
    /// <param name="createDto">Control panel type information for creation</param>
    /// <returns>The newly created control panel type</returns>
    /// <response code="201">Returns the newly created control panel type</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ControlPanelTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ControlPanelTypeDto>> CreateControlPanelType([FromBody] CreateControlPanelTypeDto createDto)
    {
        try
        {
            _log.Information("API: CreateControlPanelType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateControlPanelType");
                return BadRequest(ModelState);
            }

            var type = await _controlPanelTypeService.CreateControlPanelTypeAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetControlPanelTypeById),
                new { id = type.Id },
                type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateControlPanelType");
            return StatusCode(500, "An error occurred while creating the control panel type");
        }
    }

    /// <summary>
    /// Updates an existing control panel type's information
    /// </summary>
    /// <param name="id">The unique identifier of the control panel type to update</param>
    /// <param name="updateDto">Updated control panel type information</param>
    /// <returns>The updated control panel type</returns>
    /// <response code="200">Returns the updated control panel type</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If control panel type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ControlPanelTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ControlPanelTypeDto>> UpdateControlPanelType(int id, [FromBody] UpdateControlPanelTypeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateControlPanelType called for ID {TypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateControlPanelType");
                return BadRequest(ModelState);
            }

            var type = await _controlPanelTypeService.UpdateControlPanelTypeAsync(id, updateDto);

            if (type == null)
            {
                _log.Information("API: Control panel type with ID {TypeId} not found for update", id);
                return NotFound($"Control panel type with ID {id} not found");
            }

            return Ok(type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateControlPanelType for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while updating the control panel type");
        }
    }

    /// <summary>
    /// Deletes a control panel type from the system
    /// </summary>
    /// <param name="id">The unique identifier of the control panel type to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If control panel type was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If control panel type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteControlPanelType(int id)
    {
        try
        {
            _log.Information("API: DeleteControlPanelType called for ID {TypeId} by user {User}", id, User.Identity?.Name);

            var result = await _controlPanelTypeService.DeleteControlPanelTypeAsync(id);

            if (!result)
            {
                _log.Information("API: Control panel type with ID {TypeId} not found for deletion", id);
                return NotFound($"Control panel type with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteControlPanelType for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while deleting the control panel type");
        }
    }
}
