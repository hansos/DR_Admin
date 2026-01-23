using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages server IP addresses including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServerIpAddressesController : ControllerBase
{
    private readonly IServerIpAddressService _serverIpAddressService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerIpAddressesController>();

    public ServerIpAddressesController(IServerIpAddressService serverIpAddressService)
    {
        _serverIpAddressService = serverIpAddressService;
    }

    /// <summary>
    /// Retrieves all server IP addresses in the system
    /// </summary>
    /// <returns>List of all server IP addresses</returns>
    /// <response code="200">Returns the list of server IP addresses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<ServerIpAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerIpAddressDto>>> GetAllServerIpAddresses()
    {
        try
        {
            _log.Information("API: GetAllServerIpAddresses called by user {User}", User.Identity?.Name);
            
            var ipAddresses = await _serverIpAddressService.GetAllServerIpAddressesAsync();
            return Ok(ipAddresses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServerIpAddresses");
            return StatusCode(500, "An error occurred while retrieving server IP addresses");
        }
    }

    /// <summary>
    /// Retrieves IP addresses for a specific server
    /// </summary>
    /// <param name="serverId">The unique identifier of the server</param>
    /// <returns>List of IP addresses for the server</returns>
    /// <response code="200">Returns the list of server IP addresses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("server/{serverId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<ServerIpAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServerIpAddressDto>>> GetServerIpAddressesByServerId(int serverId)
    {
        try
        {
            _log.Information("API: GetServerIpAddressesByServerId called for server ID {ServerId} by user {User}", serverId, User.Identity?.Name);
            
            var ipAddresses = await _serverIpAddressService.GetServerIpAddressesByServerIdAsync(serverId);
            return Ok(ipAddresses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerIpAddressesByServerId for server ID {ServerId}", serverId);
            return StatusCode(500, "An error occurred while retrieving server IP addresses");
        }
    }

    /// <summary>
    /// Retrieves a specific server IP address by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the IP address</param>
    /// <returns>The server IP address information</returns>
    /// <response code="200">Returns the server IP address data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If IP address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(ServerIpAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerIpAddressDto>> GetServerIpAddressById(int id)
    {
        try
        {
            _log.Information("API: GetServerIpAddressById called for ID {IpAddressId} by user {User}", id, User.Identity?.Name);
            
            var ipAddress = await _serverIpAddressService.GetServerIpAddressByIdAsync(id);

            if (ipAddress == null)
            {
                _log.Information("API: Server IP address with ID {IpAddressId} not found", id);
                return NotFound($"Server IP address with ID {id} not found");
            }

            return Ok(ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServerIpAddressById for ID {IpAddressId}", id);
            return StatusCode(500, "An error occurred while retrieving the server IP address");
        }
    }

    /// <summary>
    /// Creates a new server IP address in the system
    /// </summary>
    /// <param name="createDto">Server IP address information for creation</param>
    /// <returns>The newly created server IP address</returns>
    /// <response code="201">Returns the newly created server IP address</response>
    /// <response code="400">If the IP address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServerIpAddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerIpAddressDto>> CreateServerIpAddress([FromBody] CreateServerIpAddressDto createDto)
    {
        try
        {
            _log.Information("API: CreateServerIpAddress called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateServerIpAddress");
                return BadRequest(ModelState);
            }

            var ipAddress = await _serverIpAddressService.CreateServerIpAddressAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetServerIpAddressById),
                new { id = ipAddress.Id },
                ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateServerIpAddress");
            return StatusCode(500, "An error occurred while creating the server IP address");
        }
    }

    /// <summary>
    /// Updates an existing server IP address's information
    /// </summary>
    /// <param name="id">The unique identifier of the IP address to update</param>
    /// <param name="updateDto">Updated IP address information</param>
    /// <returns>The updated server IP address</returns>
    /// <response code="200">Returns the updated server IP address</response>
    /// <response code="400">If the IP address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If IP address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServerIpAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServerIpAddressDto>> UpdateServerIpAddress(int id, [FromBody] UpdateServerIpAddressDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateServerIpAddress called for ID {IpAddressId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateServerIpAddress");
                return BadRequest(ModelState);
            }

            var ipAddress = await _serverIpAddressService.UpdateServerIpAddressAsync(id, updateDto);

            if (ipAddress == null)
            {
                _log.Information("API: Server IP address with ID {IpAddressId} not found for update", id);
                return NotFound($"Server IP address with ID {id} not found");
            }

            return Ok(ipAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateServerIpAddress for ID {IpAddressId}", id);
            return StatusCode(500, "An error occurred while updating the server IP address");
        }
    }

    /// <summary>
    /// Deletes a server IP address from the system
    /// </summary>
    /// <param name="id">The unique identifier of the IP address to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If IP address was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If IP address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteServerIpAddress(int id)
    {
        try
        {
            _log.Information("API: DeleteServerIpAddress called for ID {IpAddressId} by user {User}", id, User.Identity?.Name);

            var result = await _serverIpAddressService.DeleteServerIpAddressAsync(id);

            if (!result)
            {
                _log.Information("API: Server IP address with ID {IpAddressId} not found for deletion", id);
                return NotFound($"Server IP address with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteServerIpAddress for ID {IpAddressId}", id);
            return StatusCode(500, "An error occurred while deleting the server IP address");
        }
    }
}
