using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages servers including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServersController : ControllerBase
{
    private readonly IServerService _serverService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServersController>();

    public ServersController(IServerService serverService)
    {
        _serverService = serverService;
    }

    /// <summary>
    /// Retrieves all servers in the system
    /// </summary>
    /// <returns>List of all servers</returns>
    /// <response code="200">Returns the list of servers</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Server.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerDto>>> GetAllServers()
    {
        try
        {
            _log.Information("API: GetAllServers called by user {User}", User.Identity?.Name);
            
            var servers = await _serverService.GetAllServersAsync();
            return Ok(servers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServers");
            return StatusCode(500, "An error occurred while retrieving servers");
        }
    }

    /// <summary>
    /// Retrieves a specific server by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the server</param>
    /// <returns>The server information</returns>
    /// <response code="200">Returns the server data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Server.Read")]
    [ProducesResponseType(typeof(ServerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerDto>> GetServerById(int id)
    {
        try
        {
            _log.Information("API: GetServerById called for ID {ServerId} by user {User}", id, User.Identity?.Name);
            
            var server = await _serverService.GetServerByIdAsync(id);

            if (server == null)
            {
                _log.Information("API: Server with ID {ServerId} not found", id);
                return NotFound($"Server with ID {id} not found");
            }

            return Ok(server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerById for ID {ServerId}", id);
            return StatusCode(500, "An error occurred while retrieving the server");
        }
    }

    /// <summary>
    /// Creates a new server in the system
    /// </summary>
    /// <param name="createDto">Server information for creation</param>
    /// <returns>The newly created server</returns>
    /// <response code="201">Returns the newly created server</response>
    /// <response code="400">If the server data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Server.Write")]
    [ProducesResponseType(typeof(ServerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerDto>> CreateServer([FromBody] CreateServerDto createDto)
    {
        try
        {
            _log.Information("API: CreateServer called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateServer");
                return BadRequest(ModelState);
            }

            var server = await _serverService.CreateServerAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetServerById),
                new { id = server.Id },
                server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateServer");
            return StatusCode(500, "An error occurred while creating the server");
        }
    }

    /// <summary>
    /// Updates an existing server's information
    /// </summary>
    /// <param name="id">The unique identifier of the server to update</param>
    /// <param name="updateDto">Updated server information</param>
    /// <returns>The updated server</returns>
    /// <response code="200">Returns the updated server</response>
    /// <response code="400">If the server data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Server.Write")]
    [ProducesResponseType(typeof(ServerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerDto>> UpdateServer(int id, [FromBody] UpdateServerDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateServer called for ID {ServerId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateServer");
                return BadRequest(ModelState);
            }

            var server = await _serverService.UpdateServerAsync(id, updateDto);

            if (server == null)
            {
                _log.Information("API: Server with ID {ServerId} not found for update", id);
                return NotFound($"Server with ID {id} not found");
            }

            return Ok(server);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateServer for ID {ServerId}", id);
            return StatusCode(500, "An error occurred while updating the server");
        }
    }

    /// <summary>
    /// Deletes a server from the system
    /// </summary>
    /// <param name="id">The unique identifier of the server to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If server was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Server.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteServer(int id)
    {
        try
        {
            _log.Information("API: DeleteServer called for ID {ServerId} by user {User}", id, User.Identity?.Name);

            var result = await _serverService.DeleteServerAsync(id);

            if (!result)
            {
                _log.Information("API: Server with ID {ServerId} not found for deletion", id);
                return NotFound($"Server with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteServer for ID {ServerId}", id);
            return StatusCode(500, "An error occurred while deleting the server");
        }
    }
}
