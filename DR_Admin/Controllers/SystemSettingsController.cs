using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages system settings including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingService _systemSettingService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemSettingsController>();

    public SystemSettingsController(ISystemSettingService systemSettingService)
    {
        _systemSettingService = systemSettingService;
    }

    /// <summary>
    /// Retrieves all system settings
    /// </summary>
    /// <returns>List of all system settings</returns>
    /// <response code="200">Returns the list of system settings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "SystemSetting.Read")]
    [ProducesResponseType(typeof(IEnumerable<SystemSettingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SystemSettingDto>>> GetAllSystemSettings()
    {
        try
        {
            _log.Information("API: GetAllSystemSettings called by user {User}", User.Identity?.Name);

            var settings = await _systemSettingService.GetAllSystemSettingsAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllSystemSettings");
            return StatusCode(500, "An error occurred while retrieving system settings");
        }
    }

    /// <summary>
    /// Retrieves a specific system setting by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the system setting</param>
    /// <returns>The system setting information</returns>
    /// <response code="200">Returns the system setting data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If system setting is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "SystemSetting.Read")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemSettingDto>> GetSystemSettingById(int id)
    {
        try
        {
            _log.Information("API: GetSystemSettingById called for ID {SystemSettingId} by user {User}", id, User.Identity?.Name);

            var setting = await _systemSettingService.GetSystemSettingByIdAsync(id);

            if (setting == null)
            {
                _log.Information("API: System setting with ID {SystemSettingId} not found", id);
                return NotFound($"System setting with ID {id} not found");
            }

            return Ok(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSystemSettingById for ID {SystemSettingId}", id);
            return StatusCode(500, "An error occurred while retrieving the system setting");
        }
    }

    /// <summary>
    /// Retrieves a specific system setting by its key
    /// </summary>
    /// <param name="key">The unique key of the system setting (e.g., "CustomerNumber.NextValue")</param>
    /// <returns>The system setting information</returns>
    /// <response code="200">Returns the system setting data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If system setting is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("key/{key}")]
    [Authorize(Policy = "SystemSetting.Read")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemSettingDto>> GetSystemSettingByKey(string key)
    {
        try
        {
            _log.Information("API: GetSystemSettingByKey called for key '{Key}' by user {User}", key, User.Identity?.Name);

            var setting = await _systemSettingService.GetSystemSettingByKeyAsync(key);

            if (setting == null)
            {
                _log.Information("API: System setting with key '{Key}' not found", key);
                return NotFound($"System setting with key '{key}' not found");
            }

            return Ok(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSystemSettingByKey for key '{Key}'", key);
            return StatusCode(500, "An error occurred while retrieving the system setting");
        }
    }

    /// <summary>
    /// Creates a new system setting
    /// </summary>
    /// <param name="createDto">System setting information for creation</param>
    /// <returns>The newly created system setting</returns>
    /// <response code="201">Returns the newly created system setting</response>
    /// <response code="400">If the system setting data is invalid or the key already exists</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemSettingDto>> CreateSystemSetting([FromBody] CreateSystemSettingDto createDto)
    {
        try
        {
            _log.Information("API: CreateSystemSetting called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var setting = await _systemSettingService.CreateSystemSettingAsync(createDto);

            _log.Information("API: Successfully created system setting with ID: {SystemSettingId}", setting.Id);
            return CreatedAtAction(nameof(GetSystemSettingById), new { id = setting.Id }, setting);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation while creating system setting");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateSystemSetting");
            return StatusCode(500, "An error occurred while creating the system setting");
        }
    }

    /// <summary>
    /// Updates an existing system setting
    /// </summary>
    /// <param name="id">The unique identifier of the system setting to update</param>
    /// <param name="updateDto">The updated system setting information</param>
    /// <returns>The updated system setting</returns>
    /// <response code="200">Returns the updated system setting</response>
    /// <response code="400">If the system setting data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="404">If system setting is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemSettingDto>> UpdateSystemSetting(int id, [FromBody] UpdateSystemSettingDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateSystemSetting called for ID {SystemSettingId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var setting = await _systemSettingService.UpdateSystemSettingAsync(id, updateDto);

            if (setting == null)
            {
                _log.Information("API: System setting with ID {SystemSettingId} not found for update", id);
                return NotFound($"System setting with ID {id} not found");
            }

            _log.Information("API: Successfully updated system setting with ID: {SystemSettingId}", id);
            return Ok(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateSystemSetting for ID {SystemSettingId}", id);
            return StatusCode(500, "An error occurred while updating the system setting");
        }
    }

    /// <summary>
    /// Deletes a system setting
    /// </summary>
    /// <param name="id">The unique identifier of the system setting to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the system setting was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="404">If system setting is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSystemSetting(int id)
    {
        try
        {
            _log.Information("API: DeleteSystemSetting called for ID {SystemSettingId} by user {User}", id, User.Identity?.Name);

            var result = await _systemSettingService.DeleteSystemSettingAsync(id);

            if (!result)
            {
                _log.Information("API: System setting with ID {SystemSettingId} not found for deletion", id);
                return NotFound($"System setting with ID {id} not found");
            }

            _log.Information("API: Successfully deleted system setting with ID: {SystemSettingId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteSystemSetting for ID {SystemSettingId}", id);
            return StatusCode(500, "An error occurred while deleting the system setting");
        }
    }
}
