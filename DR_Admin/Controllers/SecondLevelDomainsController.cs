using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages Second-Level Domains (e.g., co.uk, net.uk)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SecondLevelDomainsController : ControllerBase
{
    private readonly ISecondLevelDomainService _secondLevelDomainService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SecondLevelDomainsController>();

    public SecondLevelDomainsController(ISecondLevelDomainService secondLevelDomainService)
    {
        _secondLevelDomainService = secondLevelDomainService;
    }

    /// <summary>
    /// Retrieves all second-level domains in the system
    /// </summary>
    /// <returns>List of all second-level domains</returns>
    /// <response code="200">Returns the list of second-level domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SecondLevelDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SecondLevelDomainDto>>> GetAllSecondLevelDomains()
    {
        try
        {
            _log.Information("API: GetAllSecondLevelDomains called by user {User}", User.Identity?.Name);
            
            var secondLevelDomains = await _secondLevelDomainService.GetAllSecondLevelDomainsAsync();
            return Ok(secondLevelDomains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllSecondLevelDomains");
            return StatusCode(500, "An error occurred while retrieving second-level domains");
        }
    }

    /// <summary>
    /// Retrieves only active second-level domains
    /// </summary>
    /// <returns>List of active second-level domains</returns>
    /// <response code="200">Returns the list of active second-level domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<SecondLevelDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SecondLevelDomainDto>>> GetActiveSecondLevelDomains()
    {
        try
        {
            _log.Information("API: GetActiveSecondLevelDomains called by user {User}", User.Identity?.Name);
            
            var secondLevelDomains = await _secondLevelDomainService.GetActiveSecondLevelDomainsAsync();
            return Ok(secondLevelDomains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveSecondLevelDomains");
            return StatusCode(500, "An error occurred while retrieving active second-level domains");
        }
    }

    /// <summary>
    /// Retrieves a specific second-level domain by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the second-level domain</param>
    /// <returns>The second-level domain information</returns>
    /// <response code="200">Returns the second-level domain data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If second-level domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SecondLevelDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SecondLevelDomainDto>> GetSecondLevelDomainById(int id)
    {
        try
        {
            _log.Information("API: GetSecondLevelDomainById called for ID {Id} by user {User}", id, User.Identity?.Name);
            
            var secondLevelDomain = await _secondLevelDomainService.GetSecondLevelDomainByIdAsync(id);

            if (secondLevelDomain == null)
            {
                _log.Information("API: Second-level domain with ID {Id} not found", id);
                return NotFound($"Second-level domain with ID {id} not found");
            }

            return Ok(secondLevelDomain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSecondLevelDomainById for ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the second-level domain");
        }
    }

    /// <summary>
    /// Retrieves all second-level domains for a specific TLD
    /// </summary>
    /// <param name="tldId">The unique identifier of the parent TLD</param>
    /// <returns>List of second-level domains for the specified TLD</returns>
    /// <response code="200">Returns the list of second-level domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-tld/{tldId}")]
    [ProducesResponseType(typeof(IEnumerable<SecondLevelDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SecondLevelDomainDto>>> GetSecondLevelDomainsByTld(int tldId)
    {
        try
        {
            _log.Information("API: GetSecondLevelDomainsByTld called for TLD ID {TldId} by user {User}", tldId, User.Identity?.Name);
            
            var secondLevelDomains = await _secondLevelDomainService.GetSecondLevelDomainsByTldIdAsync(tldId);
            return Ok(secondLevelDomains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSecondLevelDomainsByTld for TLD ID {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving second-level domains for the TLD");
        }
    }

    /// <summary>
    /// Creates a new second-level domain
    /// </summary>
    /// <param name="createDto">The data transfer object containing the second-level domain details</param>
    /// <returns>The created second-level domain</returns>
    /// <response code="201">Returns the created second-level domain</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user does not have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "SecondLevelDomain.Write")]
    [ProducesResponseType(typeof(SecondLevelDomainDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SecondLevelDomainDto>> CreateSecondLevelDomain([FromBody] CreateSecondLevelDomainDto createDto)
    {
        try
        {
            _log.Information("API: CreateSecondLevelDomain called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateSecondLevelDomain");
                return BadRequest(ModelState);
            }

            var secondLevelDomain = await _secondLevelDomainService.CreateSecondLevelDomainAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetSecondLevelDomainById),
                new { id = secondLevelDomain.Id },
                secondLevelDomain);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateSecondLevelDomain");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateSecondLevelDomain");
            return StatusCode(500, "An error occurred while creating the second-level domain");
        }
    }

    /// <summary>
    /// Updates an existing second-level domain
    /// </summary>
    /// <param name="id">The unique identifier of the second-level domain to update</param>
    /// <param name="updateDto">The data transfer object containing the updated details</param>
    /// <returns>The updated second-level domain</returns>
    /// <response code="200">Returns the updated second-level domain</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user does not have required permissions</response>
    /// <response code="404">If second-level domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SecondLevelDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SecondLevelDomainDto>> UpdateSecondLevelDomain(int id, [FromBody] UpdateSecondLevelDomainDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateSecondLevelDomain called for ID {Id} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateSecondLevelDomain");
                return BadRequest(ModelState);
            }

            var secondLevelDomain = await _secondLevelDomainService.UpdateSecondLevelDomainAsync(id, updateDto);

            if (secondLevelDomain == null)
            {
                _log.Information("API: Second-level domain with ID {Id} not found for update", id);
                return NotFound($"Second-level domain with ID {id} not found");
            }

            return Ok(secondLevelDomain);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateSecondLevelDomain");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateSecondLevelDomain for ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the second-level domain");
        }
    }

    /// <summary>
    /// Deletes a second-level domain
    /// </summary>
    /// <param name="id">The unique identifier of the second-level domain to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the deletion was successful</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user does not have required permissions</response>
    /// <response code="404">If second-level domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSecondLevelDomain(int id)
    {
        try
        {
            _log.Information("API: DeleteSecondLevelDomain called for ID {Id} by user {User}", id, User.Identity?.Name);

            var result = await _secondLevelDomainService.DeleteSecondLevelDomainAsync(id);

            if (!result)
            {
                _log.Information("API: Second-level domain with ID {Id} not found for deletion", id);
                return NotFound($"Second-level domain with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteSecondLevelDomain for ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the second-level domain");
        }
    }
}
