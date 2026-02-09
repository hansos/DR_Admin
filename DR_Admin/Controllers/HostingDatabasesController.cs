using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages databases and database users for hosting accounts
/// </summary>
[ApiController]
[Route("api/v1/hosting-accounts/{hostingAccountId}/databases")]
[Authorize]
public class HostingDatabasesController : ControllerBase
{
    private readonly IHostingDatabaseService _databaseService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingDatabasesController>();

    public HostingDatabasesController(IHostingDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingDatabaseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HostingDatabaseDto>>> GetDatabases(int hostingAccountId)
    {
        try
        {
            _log.Information("API: GetDatabases called for hosting account {AccountId}", hostingAccountId);
            var databases = await _databaseService.GetDatabasesByHostingAccountAsync(hostingAccountId);
            return Ok(databases);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDatabases");
            return StatusCode(500, "An error occurred while retrieving databases");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingDatabaseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HostingDatabaseDto>> GetDatabase(int hostingAccountId, int id)
    {
        try
        {
            var database = await _databaseService.GetDatabaseAsync(id);
            if (database == null || database.HostingAccountId != hostingAccountId)
            {
                return NotFound($"Database with ID {id} not found");
            }
            return Ok(database);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDatabase");
            return StatusCode(500, "An error occurred while retrieving the database");
        }
    }

    [HttpPost]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingDatabaseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HostingDatabaseDto>> CreateDatabase(
        int hostingAccountId,
        [FromBody] HostingDatabaseCreateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: CreateDatabase called for {DatabaseName}", dto.DatabaseName);
            dto.HostingAccountId = hostingAccountId;
            var database = await _databaseService.CreateDatabaseAsync(dto, syncToServer);
            return CreatedAtAction(nameof(GetDatabase), new { hostingAccountId, id = database.Id }, database);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDatabase");
            return StatusCode(500, "An error occurred while creating the database");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteDatabase(int hostingAccountId, int id, [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            var result = await _databaseService.DeleteDatabaseAsync(id, deleteFromServer);
            if (!result) return NotFound($"Database with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDatabase");
            return StatusCode(500, "An error occurred while deleting the database");
        }
    }

    // Database Users
    [HttpGet("{databaseId}/users")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingDatabaseUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HostingDatabaseUserDto>>> GetDatabaseUsers(int hostingAccountId, int databaseId)
    {
        try
        {
            var users = await _databaseService.GetDatabaseUsersByDatabaseAsync(databaseId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDatabaseUsers");
            return StatusCode(500, "An error occurred while retrieving database users");
        }
    }

    [HttpPost("{databaseId}/users")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingDatabaseUserDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HostingDatabaseUserDto>> CreateDatabaseUser(
        int hostingAccountId,
        int databaseId,
        [FromBody] HostingDatabaseUserCreateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            dto.HostingDatabaseId = databaseId;
            var user = await _databaseService.CreateDatabaseUserAsync(dto, syncToServer);
            return CreatedAtAction(nameof(GetDatabaseUsers), new { hostingAccountId, databaseId }, user);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDatabaseUser");
            return StatusCode(500, "An error occurred while creating the database user");
        }
    }

    [HttpDelete("{databaseId}/users/{userId}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteDatabaseUser(
        int hostingAccountId,
        int databaseId,
        int userId,
        [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            var result = await _databaseService.DeleteDatabaseUserAsync(userId, deleteFromServer);
            if (!result) return NotFound($"Database user with ID {userId} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDatabaseUser");
            return StatusCode(500, "An error occurred while deleting the database user");
        }
    }

    [HttpPost("sync")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncResultDto>> SyncDatabasesFromServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: SyncDatabasesFromServer called for account {AccountId}", hostingAccountId);
            var result = await _databaseService.SyncDatabasesFromServerAsync(hostingAccountId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncDatabasesFromServer");
            return StatusCode(500, "An error occurred while syncing databases");
        }
    }
}
