using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistrarsController : ControllerBase
{
    private readonly IRegistrarService _registrarService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarsController>();

    public RegistrarsController(IRegistrarService registrarService)
    {
        _registrarService = registrarService;
    }

    /// <summary>
    /// Assign a TLD to a registrar by IDs
    /// </summary>
    [HttpPost("{registrarId}/tld/{tldId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarTldDto>> AssignTldToRegistrarByIds(int registrarId, int tldId)
    {
        try
        {
            _log.Information("API: AssignTldToRegistrarByIds called for registrar {RegistrarId} and TLD {TldId} by user {User}", registrarId, tldId, User.Identity?.Name);

            var result = await _registrarService.AssignTldToRegistrarAsync(registrarId, tldId);

            return CreatedAtAction(null, result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in AssignTldToRegistrarByIds");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignTldToRegistrarByIds for registrar {RegistrarId} and TLD {TldId}", registrarId, tldId);
            return StatusCode(500, "An error occurred while assigning the TLD to the registrar");
        }
    }

    /// <summary>
    /// Assign a TLD to a registrar by DTO
    /// </summary>
    [HttpPost("{registrarId}/tld")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarTldDto>> AssignTldToRegistrarByDto(int registrarId, [FromBody] TldDto tldDto)
    {
        try
        {
            _log.Information("API: AssignTldToRegistrarByDto called for registrar {RegistrarId} by user {User}", registrarId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for AssignTldToRegistrarByDto");
                return BadRequest(ModelState);
            }

            var result = await _registrarService.AssignTldToRegistrarAsync(registrarId, tldDto);

            return CreatedAtAction(null, result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in AssignTldToRegistrarByDto");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignTldToRegistrarByDto");
            return StatusCode(500, "An error occurred while assigning the TLD to the registrar");
        }
    }

    /// <summary>
    /// Get all registrars
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<IEnumerable<RegistrarDto>>> GetAllRegistrars()
    {
        try
        {
            _log.Information("API: GetAllRegistrars called by user {User}", User.Identity?.Name);
            
            var registrars = await _registrarService.GetAllRegistrarsAsync();
            return Ok(registrars);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRegistrars");
            return StatusCode(500, "An error occurred while retrieving registrars");
        }
    }

    /// <summary>
    /// Get active registrars only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<RegistrarDto>>> GetActiveRegistrars()
    {
        try
        {
            _log.Information("API: GetActiveRegistrars called by user {User}", User.Identity?.Name);
            
            var registrars = await _registrarService.GetActiveRegistrarsAsync();
            return Ok(registrars);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveRegistrars");
            return StatusCode(500, "An error occurred while retrieving active registrars");
        }
    }

    /// <summary>
    /// Get registrar by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<RegistrarDto>> GetRegistrarById(int id)
    {
        try
        {
            _log.Information("API: GetRegistrarById called for ID {RegistrarId} by user {User}", id, User.Identity?.Name);
            
            var registrar = await _registrarService.GetRegistrarByIdAsync(id);

            if (registrar == null)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return Ok(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarById for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while retrieving the registrar");
        }
    }

    /// <summary>
    /// Get registrar by code
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<RegistrarDto>> GetRegistrarByCode(string code)
    {
        try
        {
            _log.Information("API: GetRegistrarByCode called for code {RegistrarCode} by user {User}", 
                code, User.Identity?.Name);
            
            var registrar = await _registrarService.GetRegistrarByCodeAsync(code);

            if (registrar == null)
            {
                _log.Information("API: Registrar with code {RegistrarCode} not found", code);
                return NotFound($"Registrar with code {code} not found");
            }

            return Ok(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarByCode for code {RegistrarCode}", code);
            return StatusCode(500, "An error occurred while retrieving the registrar");
        }
    }

    /// <summary>
    /// Create a new registrar
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarDto>> CreateRegistrar([FromBody] CreateRegistrarDto createDto)
    {
        try
        {
            _log.Information("API: CreateRegistrar called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRegistrar");
                return BadRequest(ModelState);
            }

            var registrar = await _registrarService.CreateRegistrarAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetRegistrarById),
                new { id = registrar.Id },
                registrar);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRegistrar");
            return StatusCode(500, "An error occurred while creating the registrar");
        }
    }

    /// <summary>
    /// Update an existing registrar
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarDto>> UpdateRegistrar(int id, [FromBody] UpdateRegistrarDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRegistrar called for ID {RegistrarId} by user {User}", 
                id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRegistrar");
                return BadRequest(ModelState);
            }

            var registrar = await _registrarService.UpdateRegistrarAsync(id, updateDto);

            if (registrar == null)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found for update", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return Ok(registrar);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRegistrar for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while updating the registrar");
        }
    }

    /// <summary>
    /// Delete a registrar
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRegistrar(int id)
    {
        try
        {
            _log.Information("API: DeleteRegistrar called for ID {RegistrarId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _registrarService.DeleteRegistrarAsync(id);

            if (!result)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found for deletion", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRegistrar for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while deleting the registrar");
        }
    }
}
