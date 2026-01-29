using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages server control panels including creation, retrieval, updates, deletion, and connection testing
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServerControlPanelsController : ControllerBase
{
    private readonly IServerControlPanelService _serverControlPanelService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerControlPanelsController>();

    public ServerControlPanelsController(IServerControlPanelService serverControlPanelService)
    {
        _serverControlPanelService = serverControlPanelService;
    }

    /// <summary>
    /// Retrieves all server control panels in the system
    /// </summary>
    /// <returns>List of all server control panels</returns>
    /// <response code="200">Returns the list of server control panels</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "ServerControlPanel.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServerControlPanelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerControlPanelDto>>> GetAllServerControlPanels()
    {
        try
        {
            _log.Information("API: GetAllServerControlPanels called by user {User}", User.Identity?.Name);
            
            var panels = await _serverControlPanelService.GetAllServerControlPanelsAsync();
            return Ok(panels);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServerControlPanels");
            return StatusCode(500, "An error occurred while retrieving server control panels");
        }
    }

    /// <summary>
    /// Retrieves control panels for a specific server
    /// </summary>
    /// <param name="serverId">The unique identifier of the server</param>
    /// <returns>List of control panels for the server</returns>
    /// <response code="200">Returns the list of server control panels</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("server/{serverId}")]
    [Authorize(Policy = "ServerControlPanel.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServerControlPanelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerControlPanelDto>>> GetServerControlPanelsByServerId(int serverId)
    {
        try
        {
            _log.Information("API: GetServerControlPanelsByServerId called for server ID {ServerId} by user {User}", serverId, User.Identity?.Name);
            
            var panels = await _serverControlPanelService.GetServerControlPanelsByServerIdAsync(serverId);
            return Ok(panels);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerControlPanelsByServerId for server ID {ServerId}", serverId);
            return StatusCode(500, "An error occurred while retrieving server control panels");
        }
    }

    /// <summary>
    /// Retrieves a specific server control panel by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the control panel</param>
    /// <returns>The server control panel information</returns>
    /// <response code="200">Returns the server control panel data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If control panel is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "ServerControlPanel.Read")]
    [ProducesResponseType(typeof(ServerControlPanelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerControlPanelDto>> GetServerControlPanelById(int id)
    {
        try
        {
            _log.Information("API: GetServerControlPanelById called for ID {PanelId} by user {User}", id, User.Identity?.Name);
            
            var panel = await _serverControlPanelService.GetServerControlPanelByIdAsync(id);

            if (panel == null)
            {
                _log.Information("API: Server control panel with ID {PanelId} not found", id);
                return NotFound($"Server control panel with ID {id} not found");
            }

            return Ok(panel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerControlPanelById for ID {PanelId}", id);
            return StatusCode(500, "An error occurred while retrieving the server control panel");
        }
    }

    /// <summary>
    /// Creates a new server control panel in the system
    /// </summary>
    /// <param name="createDto">Server control panel information for creation</param>
    /// <returns>The newly created server control panel</returns>
    /// <response code="201">Returns the newly created server control panel</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServerControlPanelDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerControlPanelDto>> CreateServerControlPanel([FromBody] CreateServerControlPanelDto createDto)
    {
        try
        {
            _log.Information("API: CreateServerControlPanel called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateServerControlPanel");
                return BadRequest(ModelState);
            }

            var panel = await _serverControlPanelService.CreateServerControlPanelAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetServerControlPanelById),
                new { id = panel.Id },
                panel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateServerControlPanel");
            return StatusCode(500, "An error occurred while creating the server control panel");
        }
    }

    /// <summary>
    /// Updates an existing server control panel's information
    /// </summary>
    /// <param name="id">The unique identifier of the control panel to update</param>
    /// <param name="updateDto">Updated control panel information</param>
    /// <returns>The updated server control panel</returns>
    /// <response code="200">Returns the updated server control panel</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If control panel is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServerControlPanelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerControlPanelDto>> UpdateServerControlPanel(int id, [FromBody] UpdateServerControlPanelDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateServerControlPanel called for ID {PanelId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateServerControlPanel");
                return BadRequest(ModelState);
            }

            var panel = await _serverControlPanelService.UpdateServerControlPanelAsync(id, updateDto);

            if (panel == null)
            {
                _log.Information("API: Server control panel with ID {PanelId} not found for update", id);
                return NotFound($"Server control panel with ID {id} not found");
            }

            return Ok(panel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateServerControlPanel for ID {PanelId}", id);
            return StatusCode(500, "An error occurred while updating the server control panel");
        }
    }

    /// <summary>
    /// Deletes a server control panel from the system
    /// </summary>
    /// <param name="id">The unique identifier of the control panel to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If control panel was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If control panel is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteServerControlPanel(int id)
    {
        try
        {
            _log.Information("API: DeleteServerControlPanel called for ID {PanelId} by user {User}", id, User.Identity?.Name);

            var result = await _serverControlPanelService.DeleteServerControlPanelAsync(id);

            if (!result)
            {
                _log.Information("API: Server control panel with ID {PanelId} not found for deletion", id);
                return NotFound($"Server control panel with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteServerControlPanel for ID {PanelId}", id);
            return StatusCode(500, "An error occurred while deleting the server control panel");
        }
    }

    /// <summary>
    /// Tests the connection to a server control panel
    /// </summary>
    /// <param name="id">The unique identifier of the control panel</param>
    /// <returns>Connection test result</returns>
    /// <response code="200">Returns the connection test result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If control panel is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/test-connection")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> TestConnection(int id)
    {
        try
        {
            _log.Information("API: TestConnection called for control panel ID {PanelId} by user {User}", id, User.Identity?.Name);

            var result = await _serverControlPanelService.TestConnectionAsync(id);

            if (!result)
            {
                _log.Warning("API: Connection test failed for control panel ID {PanelId}", id);
            }

            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TestConnection for control panel ID {PanelId}", id);
            return StatusCode(500, "An error occurred while testing the connection");
        }
    }
}
