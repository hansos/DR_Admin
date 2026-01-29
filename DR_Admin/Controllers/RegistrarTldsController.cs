using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages registrar-TLD relationships and pricing
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistrarTldsController : ControllerBase
{
    private readonly IRegistrarTldService _registrarTldService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldsController>();

    public RegistrarTldsController(IRegistrarTldService registrarTldService)
    {
        _registrarTldService = registrarTldService;
    }

    /// <summary>
    /// Retrieves all registrar-TLD offerings in the system
    /// </summary>
    /// <returns>List of all registrar-TLD relationships</returns>
    /// <response code="200">Returns the list of registrar-TLD offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "RegistrarTld.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetAllRegistrarTlds()
    {
        try
        {
            _log.Information("API: GetAllRegistrarTlds called by user {User}", User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetAllRegistrarTldsAsync();
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRegistrarTlds");
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves only available registrar-TLD offerings for purchase
    /// </summary>
    /// <returns>List of available registrar-TLD relationships</returns>
    /// <response code="200">Returns the list of available offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetAvailableRegistrarTlds()
    {
        try
        {
            _log.Information("API: GetAvailableRegistrarTlds called by user {User}", User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetAvailableRegistrarTldsAsync();
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAvailableRegistrarTlds");
            return StatusCode(500, "An error occurred while retrieving available registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves all TLD offerings for a specific registrar
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <returns>List of TLD offerings for the registrar</returns>
    /// <response code="200">Returns the list of TLD offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarId}")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetRegistrarTldsByRegistrar(int registrarId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldsByRegistrar called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetRegistrarTldsByRegistrarAsync(registrarId);
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldsByRegistrar for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves all registrars offering a specific TLD
    /// </summary>
    /// <param name="tldId">The unique identifier of the TLD</param>
    /// <returns>List of registrars offering the TLD</returns>
    /// <response code="200">Returns the list of registrar offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("tld/{tldId}")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetRegistrarTldsByTld(int tldId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldsByTld called for TLD {TldId} by user {User}", 
                tldId, User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetRegistrarTldsByTldAsync(tldId);
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldsByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves a specific registrar-TLD offering by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the registrar-TLD relationship</param>
    /// <returns>The registrar-TLD offering information</returns>
    /// <response code="200">Returns the registrar-TLD offering data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If registrar-TLD offering is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> GetRegistrarTldById(int id)
    {
        try
        {
            _log.Information("API: GetRegistrarTldById called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);
            
            var registrarTld = await _registrarTldService.GetRegistrarTldByIdAsync(id);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return Ok(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldById for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while retrieving the registrar TLD");
        }
    }

    /// <summary>
    /// Get registrar TLD offering by registrar and TLD combination
    /// </summary>
    [HttpGet("registrar/{registrarId}/tld/{tldId}")]
    public async Task<ActionResult<RegistrarTldDto>> GetRegistrarTldByRegistrarAndTld(int registrarId, int tldId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldByRegistrarAndTld called for registrar {RegistrarId} and TLD {TldId} by user {User}", 
                registrarId, tldId, User.Identity?.Name);
            
            var registrarTld = await _registrarTldService.GetRegistrarTldByRegistrarAndTldAsync(registrarId, tldId);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD for registrar {RegistrarId} and TLD {TldId} not found", 
                    registrarId, tldId);
                return NotFound($"Registrar TLD for registrar {registrarId} and TLD {tldId} not found");
            }

            return Ok(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldByRegistrarAndTld for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            return StatusCode(500, "An error occurred while retrieving the registrar TLD");
        }
    }

    /// <summary>
    /// Create a new registrar TLD offering
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarTldDto>> CreateRegistrarTld([FromBody] CreateRegistrarTldDto createDto)
    {
        try
        {
            _log.Information("API: CreateRegistrarTld called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRegistrarTld");
                return BadRequest(ModelState);
            }

            var registrarTld = await _registrarTldService.CreateRegistrarTldAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetRegistrarTldById),
                new { id = registrarTld.Id },
                registrarTld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRegistrarTld");
            return StatusCode(500, "An error occurred while creating the registrar TLD");
        }
    }

    /// <summary>
    /// Update an existing registrar TLD offering
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarTldDto>> UpdateRegistrarTld(int id, [FromBody] UpdateRegistrarTldDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRegistrarTld called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRegistrarTld");
                return BadRequest(ModelState);
            }

            var registrarTld = await _registrarTldService.UpdateRegistrarTldAsync(id, updateDto);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found for update", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return Ok(registrarTld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRegistrarTld for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while updating the registrar TLD");
        }
    }

    /// <summary>
    /// Delete a registrar TLD offering
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRegistrarTld(int id)
    {
        try
        {
            _log.Information("API: DeleteRegistrarTld called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _registrarTldService.DeleteRegistrarTldAsync(id);

            if (!result)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found for deletion", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRegistrarTld for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while deleting the registrar TLD");
        }
    }
}
