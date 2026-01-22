using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TldsController : ControllerBase
{
    private readonly ITldService _tldService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldsController>();

    public TldsController(ITldService tldService)
    {
        _tldService = tldService;
    }

    /// <summary>
    /// Get all TLDs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TldDto>>> GetAllTlds()
    {
        try
        {
            _log.Information("API: GetAllTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetAllTldsAsync();
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTlds");
            return StatusCode(500, "An error occurred while retrieving TLDs");
        }
    }

    /// <summary>
    /// Get active TLDs only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<TldDto>>> GetActiveTlds()
    {
        try
        {
            _log.Information("API: GetActiveTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetActiveTldsAsync();
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveTlds");
            return StatusCode(500, "An error occurred while retrieving active TLDs");
        }
    }

    /// <summary>
    /// Get TLD by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TldDto>> GetTldById(int id)
    {
        try
        {
            _log.Information("API: GetTldById called for ID {TldId} by user {User}", id, User.Identity?.Name);
            
            var tld = await _tldService.GetTldByIdAsync(id);

            if (tld == null)
            {
                _log.Information("API: TLD with ID {TldId} not found", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return Ok(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTldById for ID {TldId}", id);
            return StatusCode(500, "An error occurred while retrieving the TLD");
        }
    }

    /// <summary>
    /// Get TLD by extension
    /// </summary>
    [HttpGet("extension/{extension}")]
    public async Task<ActionResult<TldDto>> GetTldByExtension(string extension)
    {
        try
        {
            _log.Information("API: GetTldByExtension called for extension {Extension} by user {User}", extension, User.Identity?.Name);
            
            var tld = await _tldService.GetTldByExtensionAsync(extension);

            if (tld == null)
            {
                _log.Information("API: TLD with extension {Extension} not found", extension);
                return NotFound($"TLD with extension {extension} not found");
            }

            return Ok(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTldByExtension for extension {Extension}", extension);
            return StatusCode(500, "An error occurred while retrieving the TLD");
        }
    }

    /// <summary>
    /// Create a new TLD
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TldDto>> CreateTld([FromBody] CreateTldDto createDto)
    {
        try
        {
            _log.Information("API: CreateTld called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateTld");
                return BadRequest(ModelState);
            }

            var tld = await _tldService.CreateTldAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetTldById),
                new { id = tld.Id },
                tld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateTld");
            return StatusCode(500, "An error occurred while creating the TLD");
        }
    }

    /// <summary>
    /// Update an existing TLD
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TldDto>> UpdateTld(int id, [FromBody] UpdateTldDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateTld called for ID {TldId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateTld");
                return BadRequest(ModelState);
            }

            var tld = await _tldService.UpdateTldAsync(id, updateDto);

            if (tld == null)
            {
                _log.Information("API: TLD with ID {TldId} not found for update", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return Ok(tld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateTld for ID {TldId}", id);
            return StatusCode(500, "An error occurred while updating the TLD");
        }
    }

    /// <summary>
    /// Delete a TLD
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTld(int id)
    {
        try
        {
            _log.Information("API: DeleteTld called for ID {TldId} by user {User}", id, User.Identity?.Name);

            var result = await _tldService.DeleteTldAsync(id);

            if (!result)
            {
                _log.Information("API: TLD with ID {TldId} not found for deletion", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteTld for ID {TldId}", id);
            return StatusCode(500, "An error occurred while deleting the TLD");
        }
    }
}
