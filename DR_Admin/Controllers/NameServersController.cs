using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages name servers for domains
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NameServersController : ControllerBase
{
    private readonly INameServerService _nameServerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<NameServersController>();

    public NameServersController(INameServerService nameServerService)
    {
        _nameServerService = nameServerService;
    }

    /// <summary>
    /// Retrieves all name servers in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all name servers or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of name servers or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "NameServer.Read")]
    [ProducesResponseType(typeof(IEnumerable<NameServerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<NameServerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllNameServers([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllNameServers (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _nameServerService.GetAllNameServersPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            _log.Information("API: GetAllNameServers called by user {User}", User.Identity?.Name);
            
            var nameServers = await _nameServerService.GetAllNameServersAsync();
            return Ok(nameServers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllNameServers");
            return StatusCode(500, "An error occurred while retrieving name servers");
        }
    }

    /// <summary>
    /// Retrieves a specific name server by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the name server</param>
    /// <returns>The name server information</returns>
    /// <response code="200">Returns the name server data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If name server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "NameServer.ReadOwn")]
    [ProducesResponseType(typeof(NameServerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NameServerDto>> GetNameServerById(int id)
    {
        try
        {
            _log.Information("API: GetNameServerById called for ID {NameServerId} by user {User}", id, User.Identity?.Name);
            
            var nameServer = await _nameServerService.GetNameServerByIdAsync(id);

            if (nameServer == null)
            {
                _log.Information("API: Name server with ID {NameServerId} not found", id);
                return NotFound($"Name server with ID {id} not found");
            }

            return Ok(nameServer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetNameServerById for ID {NameServerId}", id);
            return StatusCode(500, "An error occurred while retrieving the name server");
        }
    }

    /// <summary>
    /// Retrieves all name servers for a specific domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>List of name servers for the domain</returns>
    /// <response code="200">Returns the list of name servers</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("domain/{domainId}")]
    [Authorize(Policy = "NameServer.ReadOwn")]
    [ProducesResponseType(typeof(IEnumerable<NameServerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<NameServerDto>>> GetNameServersByDomainId(int domainId)
    {
        try
        {
            _log.Information("API: GetNameServersByDomainId called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);
            
            var nameServers = await _nameServerService.GetNameServersByDomainIdAsync(domainId);
            return Ok(nameServers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetNameServersByDomainId for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while retrieving name servers for the domain");
        }
    }

    /// <summary>
    /// Creates a new name server for a domain
    /// </summary>
    /// <param name="createDto">Name server information for creation</param>
    /// <returns>The newly created name server</returns>
    /// <response code="201">Returns the newly created name server</response>
    /// <response code="400">If the name server data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "NameServer.WriteOwn")]
    [ProducesResponseType(typeof(NameServerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NameServerDto>> CreateNameServer([FromBody] CreateNameServerDto createDto)
    {
        try
        {
            _log.Information("API: CreateNameServer called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateNameServer");
                return BadRequest(ModelState);
            }

            var nameServer = await _nameServerService.CreateNameServerAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetNameServerById),
                new { id = nameServer.Id },
                nameServer);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateNameServer");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateNameServer");
            return StatusCode(500, "An error occurred while creating the name server");
        }
    }

    /// <summary>
    /// Updates an existing name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to update</param>
    /// <param name="updateDto">Name server information for update</param>
    /// <returns>The updated name server</returns>
    /// <response code="200">Returns the updated name server</response>
    /// <response code="400">If the name server data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If name server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "NameServer.WriteOwn")]
    [ProducesResponseType(typeof(NameServerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NameServerDto>> UpdateNameServer(int id, [FromBody] UpdateNameServerDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateNameServer called for ID {NameServerId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateNameServer");
                return BadRequest(ModelState);
            }

            var nameServer = await _nameServerService.UpdateNameServerAsync(id, updateDto);

            if (nameServer == null)
            {
                _log.Information("API: Name server with ID {NameServerId} not found for update", id);
                return NotFound($"Name server with ID {id} not found");
            }

            return Ok(nameServer);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateNameServer for ID {NameServerId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateNameServer for ID {NameServerId}", id);
            return StatusCode(500, "An error occurred while updating the name server");
        }
    }

    /// <summary>
    /// Deletes a name server
    /// </summary>
    /// <param name="id">The unique identifier of the name server to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the name server was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If name server is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteNameServer(int id)
    {
        try
        {
            _log.Information("API: DeleteNameServer called for ID {NameServerId} by user {User}", id, User.Identity?.Name);

            var result = await _nameServerService.DeleteNameServerAsync(id);

            if (!result)
            {
                _log.Information("API: Name server with ID {NameServerId} not found for deletion", id);
                return NotFound($"Name server with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteNameServer for ID {NameServerId}", id);
            return StatusCode(500, "An error occurred while deleting the name server");
        }
    }
}
