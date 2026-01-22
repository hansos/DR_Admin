using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

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
    /// Get all registrar TLD offerings
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
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
    /// Get available registrar TLD offerings
    /// </summary>
    [HttpGet("available")]
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
    /// Get registrar TLD offerings by registrar
    /// </summary>
    [HttpGet("registrar/{registrarId}")]
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
    /// Get registrar TLD offerings by TLD
    /// </summary>
    [HttpGet("tld/{tldId}")]
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
    /// Get registrar TLD offering by ID
    /// </summary>
    [HttpGet("{id}")]
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
