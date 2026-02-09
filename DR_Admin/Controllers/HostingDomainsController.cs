using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages domains for hosting accounts (main, addon, parked, subdomains)
/// </summary>
[ApiController]
[Route("api/v1/hosting-accounts/{hostingAccountId}/domains")]
[Authorize]
public class HostingDomainsController : ControllerBase
{
    private readonly IHostingDomainService _domainService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingDomainsController>();

    public HostingDomainsController(IHostingDomainService domainService)
    {
        _domainService = domainService;
    }

    /// <summary>
    /// Retrieves all domains for a hosting account
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <returns>List of domains</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingDomainDto>>> GetDomains(int hostingAccountId)
    {
        try
        {
            _log.Information("API: GetDomains called for hosting account {AccountId} by user {User}", hostingAccountId, User.Identity?.Name);
            var domains = await _domainService.GetDomainsByHostingAccountAsync(hostingAccountId);
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomains for hosting account {AccountId}", hostingAccountId);
            return StatusCode(500, "An error occurred while retrieving domains");
        }
    }

    /// <summary>
    /// Retrieves a specific domain by ID
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <param name="id">The domain ID</param>
    /// <returns>The domain information</returns>
    /// <response code="200">Returns the domain data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingDomainDto>> GetDomain(int hostingAccountId, int id)
    {
        try
        {
            _log.Information("API: GetDomain called for domain {DomainId} by user {User}", id, User.Identity?.Name);
            var domain = await _domainService.GetDomainAsync(id);

            if (domain == null)
            {
                _log.Warning("API: Domain {DomainId} not found", id);
                return NotFound($"Domain with ID {id} not found");
            }

            if (domain.HostingAccountId != hostingAccountId)
            {
                _log.Warning("API: Domain {DomainId} does not belong to account {AccountId}", id, hostingAccountId);
                return NotFound($"Domain with ID {id} not found for this hosting account");
            }

            return Ok(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomain for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while retrieving the domain");
        }
    }

    /// <summary>
    /// Creates a new domain for a hosting account
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <param name="dto">The domain creation data</param>
    /// <param name="syncToServer">Whether to sync to the hosting server</param>
    /// <returns>The created domain</returns>
    /// <response code="201">Returns the created domain</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingDomainDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingDomainDto>> CreateDomain(
        int hostingAccountId,
        [FromBody] HostingDomainCreateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: CreateDomain called for account {AccountId}, domain {DomainName} by user {User}", 
                hostingAccountId, dto.DomainName, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dto.HostingAccountId = hostingAccountId;
            var domain = await _domainService.CreateDomainAsync(dto, syncToServer);
            
            _log.Information("API: Domain created with ID {DomainId}", domain.Id);
            return CreatedAtAction(nameof(GetDomain), new { hostingAccountId, id = domain.Id }, domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDomain");
            return StatusCode(500, "An error occurred while creating the domain");
        }
    }

    /// <summary>
    /// Updates a domain
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <param name="id">The domain ID</param>
    /// <param name="dto">The domain update data</param>
    /// <param name="syncToServer">Whether to sync to the hosting server</param>
    /// <returns>The updated domain</returns>
    /// <response code="200">Returns the updated domain</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingDomainDto>> UpdateDomain(
        int hostingAccountId,
        int id,
        [FromBody] HostingDomainUpdateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: UpdateDomain called for domain {DomainId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var domain = await _domainService.UpdateDomainAsync(id, dto, syncToServer);
            
            _log.Information("API: Domain {DomainId} updated successfully", id);
            return Ok(domain);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Domain {DomainId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDomain for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while updating the domain");
        }
    }

    /// <summary>
    /// Deletes a domain
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <param name="id">The domain ID</param>
    /// <param name="deleteFromServer">Whether to also delete from the hosting server</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the domain was deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDomain(int hostingAccountId, int id, [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            _log.Information("API: DeleteDomain called for domain {DomainId} by user {User}", id, User.Identity?.Name);

            var result = await _domainService.DeleteDomainAsync(id, deleteFromServer);

            if (!result)
            {
                _log.Warning("API: Domain {DomainId} not found", id);
                return NotFound($"Domain with ID {id} not found");
            }

            _log.Information("API: Domain {DomainId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDomain for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while deleting the domain");
        }
    }

    /// <summary>
    /// Synchronizes domains from the hosting server to the database
    /// </summary>
    /// <param name="hostingAccountId">The hosting account ID</param>
    /// <returns>Sync operation result</returns>
    /// <response code="200">Returns the sync result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("sync")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncResultDto>> SyncDomainsFromServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: SyncDomainsFromServer called for account {AccountId} by user {User}", 
                hostingAccountId, User.Identity?.Name);

            var result = await _domainService.SyncDomainsFromServerAsync(hostingAccountId);
            
            _log.Information("API: Domain sync completed for account {AccountId}. Synced: {Count}", 
                hostingAccountId, result.RecordsSynced);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncDomainsFromServer for account {AccountId}", hostingAccountId);
            return StatusCode(500, "An error occurred while syncing domains");
        }
    }
}
