using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages server types including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServerTypesController : ControllerBase
{
    private readonly IServerTypeService _serverTypeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerTypesController>();

    public ServerTypesController(IServerTypeService serverTypeService)
    {
        _serverTypeService = serverTypeService;
    }

    /// <summary>
    /// Retrieves all server types in the system
    /// </summary>
    /// <returns>List of all server types</returns>
    /// <response code="200">Returns the list of server types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "ServerType.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServerTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerTypeDto>>> GetAllServerTypes()
    {
        try
        {
            _log.Information("API: GetAllServerTypes called by user {User}", User.Identity?.Name);

            var types = await _serverTypeService.GetAllServerTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServerTypes");
            return StatusCode(500, "An error occurred while retrieving server types");
        }
    }

    /// <summary>
    /// Retrieves only active server types
    /// </summary>
    /// <returns>List of active server types</returns>
    /// <response code="200">Returns the list of active server types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "ServerType.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServerTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerTypeDto>>> GetActiveServerTypes()
    {
        try
        {
            _log.Information("API: GetActiveServerTypes called by user {User}", User.Identity?.Name);

            var types = await _serverTypeService.GetActiveServerTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveServerTypes");
            return StatusCode(500, "An error occurred while retrieving active server types");
        }
    }

    /// <summary>
    /// Retrieves a specific server type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the server type</param>
    /// <returns>The server type information</returns>
    /// <response code="200">Returns the server type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If server type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "ServerType.Read")]
    [ProducesResponseType(typeof(ServerTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerTypeDto>> GetServerTypeById(int id)
    {
        try
        {
            _log.Information("API: GetServerTypeById called for ID {TypeId} by user {User}", id, User.Identity?.Name);

            var type = await _serverTypeService.GetServerTypeByIdAsync(id);

            if (type == null)
            {
                _log.Information("API: Server type with ID {TypeId} not found", id);
                return NotFound($"Server type with ID {id} not found");
            }

            return Ok(type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerTypeById for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the server type");
        }
    }

    /// <summary>
    /// Creates a new server type in the system
    /// </summary>
    /// <param name="createDto">Server type information for creation</param>
    /// <returns>The newly created server type</returns>
    /// <response code="201">Returns the newly created server type</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "ServerType.Write")]
    [ProducesResponseType(typeof(ServerTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerTypeDto>> CreateServerType([FromBody] CreateServerTypeDto createDto)
    {
        try
        {
            _log.Information("API: CreateServerType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateServerType");
                return BadRequest(ModelState);
            }

            var type = await _serverTypeService.CreateServerTypeAsync(createDto);

            return CreatedAtAction(
                nameof(GetServerTypeById),
                new { id = type.Id },
                type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateServerType");
            return StatusCode(500, "An error occurred while creating the server type");
        }
    }

    /// <summary>
    /// Updates an existing server type's information
    /// </summary>
    /// <param name="id">The unique identifier of the server type to update</param>
    /// <param name="updateDto">Updated server type information</param>
    /// <returns>The updated server type</returns>
    /// <response code="200">Returns the updated server type</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If server type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "ServerType.Write")]
    [ProducesResponseType(typeof(ServerTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerTypeDto>> UpdateServerType(int id, [FromBody] UpdateServerTypeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateServerType called for ID {TypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateServerType");
                return BadRequest(ModelState);
            }

            var type = await _serverTypeService.UpdateServerTypeAsync(id, updateDto);

            if (type == null)
            {
                _log.Information("API: Server type with ID {TypeId} not found for update", id);
                return NotFound($"Server type with ID {id} not found");
            }

            return Ok(type);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateServerType for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while updating the server type");
        }
    }

    /// <summary>
    /// Deletes a server type from the system
    /// </summary>
    /// <param name="id">The unique identifier of the server type to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If server type was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If server type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "ServerType.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteServerType(int id)
    {
        try
        {
            _log.Information("API: DeleteServerType called for ID {TypeId} by user {User}", id, User.Identity?.Name);

            var result = await _serverTypeService.DeleteServerTypeAsync(id);

            if (!result)
            {
                _log.Information("API: Server type with ID {TypeId} not found for deletion", id);
                return NotFound($"Server type with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteServerType for ID {TypeId}", id);
            return StatusCode(500, "An error occurred while deleting the server type");
        }
    }
}
