using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides read access to registered domain history entries.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegisteredDomainHistoriesController : ControllerBase
{
    private readonly IRegisteredDomainHistoryService _registeredDomainHistoryService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegisteredDomainHistoriesController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisteredDomainHistoriesController"/> class.
    /// </summary>
    /// <param name="registeredDomainHistoryService">The registered domain history service.</param>
    public RegisteredDomainHistoriesController(IRegisteredDomainHistoryService registeredDomainHistoryService)
    {
        _registeredDomainHistoryService = registeredDomainHistoryService;
    }

    /// <summary>
    /// Retrieves DNS change history entries across all domains with optional filters.
    /// </summary>
    /// <param name="domainName">Optional domain name search filter.</param>
    /// <param name="occurredFrom">Optional lower bound for occurrence timestamp (UTC).</param>
    /// <param name="occurredTo">Optional upper bound for occurrence timestamp (UTC).</param>
    /// <returns>DNS change history entries.</returns>
    /// <response code="200">Returns DNS change history entries.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("dns-changes")]
    [Authorize(Policy = "DomainHistory.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainHistoryDto>>> GetDnsChanges(
        [FromQuery] string? domainName = null,
        [FromQuery] DateTime? occurredFrom = null,
        [FromQuery] DateTime? occurredTo = null)
    {
        try
        {
            _log.Information("API: Get DNS change history called by user {User} with DomainName={DomainName}, OccurredFrom={OccurredFrom}, OccurredTo={OccurredTo}",
                User.Identity?.Name,
                domainName,
                occurredFrom,
                occurredTo);

            var items = await _registeredDomainHistoryService.GetDnsChangesAsync(domainName, occurredFrom, occurredTo);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error getting DNS change history");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving DNS change history entries");
        }
    }

    /// <summary>
    /// Retrieves all history entries for a specific registered domain.
    /// </summary>
    /// <param name="registeredDomainId">The registered domain identifier.</param>
    /// <returns>History entries for the specified registered domain.</returns>
    /// <response code="200">Returns the history entries.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("domain/{registeredDomainId:int}")]
    [Authorize(Policy = "DomainHistory.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainHistoryDto>>> GetByRegisteredDomainId(int registeredDomainId)
    {
        try
        {
            _log.Information("API: Get registered domain history for domain {RegisteredDomainId} called by user {User}",
                registeredDomainId,
                User.Identity?.Name);

            var items = await _registeredDomainHistoryService.GetByRegisteredDomainIdAsync(registeredDomainId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error getting registered domain history for domain {RegisteredDomainId}", registeredDomainId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving domain history entries");
        }
    }

    /// <summary>
    /// Retrieves a specific registered domain history entry by identifier.
    /// </summary>
    /// <param name="id">The history entry identifier.</param>
    /// <returns>The history entry if found.</returns>
    /// <response code="200">Returns the history entry.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="404">If the history entry is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "DomainHistory.Read")]
    [ProducesResponseType(typeof(RegisteredDomainHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainHistoryDto>> GetById(int id)
    {
        try
        {
            _log.Information("API: Get registered domain history by ID {HistoryId} called by user {User}", id, User.Identity?.Name);

            var item = await _registeredDomainHistoryService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Registered domain history with ID {id} not found");
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error getting registered domain history by ID {HistoryId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the domain history entry");
        }
    }
}
