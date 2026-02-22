using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages host providers including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class HostProvidersController : ControllerBase
{
    private readonly IHostProviderService _hostProviderService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostProvidersController>();

    public HostProvidersController(IHostProviderService hostProviderService)
    {
        _hostProviderService = hostProviderService;
    }

    /// <summary>
    /// Retrieves all host providers in the system
    /// </summary>
    /// <returns>List of all host providers</returns>
    /// <response code="200">Returns the list of host providers</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "HostProvider.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostProviderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostProviderDto>>> GetAllHostProviders()
    {
        try
        {
            _log.Information("API: GetAllHostProviders called by user {User}", User.Identity?.Name);

            var providers = await _hostProviderService.GetAllHostProvidersAsync();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllHostProviders");
            return StatusCode(500, "An error occurred while retrieving host providers");
        }
    }

    /// <summary>
    /// Retrieves only active host providers
    /// </summary>
    /// <returns>List of active host providers</returns>
    /// <response code="200">Returns the list of active host providers</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "HostProvider.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostProviderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostProviderDto>>> GetActiveHostProviders()
    {
        try
        {
            _log.Information("API: GetActiveHostProviders called by user {User}", User.Identity?.Name);

            var providers = await _hostProviderService.GetActiveHostProvidersAsync();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveHostProviders");
            return StatusCode(500, "An error occurred while retrieving active host providers");
        }
    }

    /// <summary>
    /// Retrieves a specific host provider by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the host provider</param>
    /// <returns>The host provider information</returns>
    /// <response code="200">Returns the host provider data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If host provider is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "HostProvider.Read")]
    [ProducesResponseType(typeof(HostProviderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostProviderDto>> GetHostProviderById(int id)
    {
        try
        {
            _log.Information("API: GetHostProviderById called for ID {ProviderId} by user {User}", id, User.Identity?.Name);

            var provider = await _hostProviderService.GetHostProviderByIdAsync(id);

            if (provider == null)
            {
                _log.Information("API: Host provider with ID {ProviderId} not found", id);
                return NotFound($"Host provider with ID {id} not found");
            }

            return Ok(provider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostProviderById for ID {ProviderId}", id);
            return StatusCode(500, "An error occurred while retrieving the host provider");
        }
    }

    /// <summary>
    /// Creates a new host provider in the system
    /// </summary>
    /// <param name="createDto">Host provider information for creation</param>
    /// <returns>The newly created host provider</returns>
    /// <response code="201">Returns the newly created host provider</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "HostProvider.Write")]
    [ProducesResponseType(typeof(HostProviderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostProviderDto>> CreateHostProvider([FromBody] CreateHostProviderDto createDto)
    {
        try
        {
            _log.Information("API: CreateHostProvider called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateHostProvider");
                return BadRequest(ModelState);
            }

            var provider = await _hostProviderService.CreateHostProviderAsync(createDto);

            return CreatedAtAction(
                nameof(GetHostProviderById),
                new { id = provider.Id },
                provider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateHostProvider");
            return StatusCode(500, "An error occurred while creating the host provider");
        }
    }

    /// <summary>
    /// Updates an existing host provider's information
    /// </summary>
    /// <param name="id">The unique identifier of the host provider to update</param>
    /// <param name="updateDto">Updated host provider information</param>
    /// <returns>The updated host provider</returns>
    /// <response code="200">Returns the updated host provider</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If host provider is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "HostProvider.Write")]
    [ProducesResponseType(typeof(HostProviderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostProviderDto>> UpdateHostProvider(int id, [FromBody] UpdateHostProviderDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateHostProvider called for ID {ProviderId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateHostProvider");
                return BadRequest(ModelState);
            }

            var provider = await _hostProviderService.UpdateHostProviderAsync(id, updateDto);

            if (provider == null)
            {
                _log.Information("API: Host provider with ID {ProviderId} not found for update", id);
                return NotFound($"Host provider with ID {id} not found");
            }

            return Ok(provider);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateHostProvider for ID {ProviderId}", id);
            return StatusCode(500, "An error occurred while updating the host provider");
        }
    }

    /// <summary>
    /// Deletes a host provider from the system
    /// </summary>
    /// <param name="id">The unique identifier of the host provider to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If host provider was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If host provider is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "HostProvider.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteHostProvider(int id)
    {
        try
        {
            _log.Information("API: DeleteHostProvider called for ID {ProviderId} by user {User}", id, User.Identity?.Name);

            var result = await _hostProviderService.DeleteHostProviderAsync(id);

            if (!result)
            {
                _log.Information("API: Host provider with ID {ProviderId} not found for deletion", id);
                return NotFound($"Host provider with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteHostProvider for ID {ProviderId}", id);
            return StatusCode(500, "An error occurred while deleting the host provider");
        }
    }
}
