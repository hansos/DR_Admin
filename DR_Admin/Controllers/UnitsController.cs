using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;
    private static readonly Serilog.ILogger _log = Log.ForContext<UnitsController>();

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    /// <summary>
    /// Get all units
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetAllUnits()
    {
        try
        {
            _log.Information("API: GetAllUnits called by user {User}", User.Identity?.Name);
            
            var units = await _unitService.GetAllUnitsAsync();
            return Ok(units);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllUnits");
            return StatusCode(500, "An error occurred while retrieving units");
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<UnitDto>> GetUnitById(int id)
    {
        try
        {
            _log.Information("API: GetUnitById called for ID {UnitId} by user {User}", id, User.Identity?.Name);
            
            var unit = await _unitService.GetUnitByIdAsync(id);

            if (unit == null)
            {
                _log.Information("API: Unit with ID {UnitId} not found", id);
                return NotFound($"Unit with ID {id} not found");
            }

            return Ok(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetUnitById for ID {UnitId}", id);
            return StatusCode(500, "An error occurred while retrieving the unit");
        }
    }

    /// <summary>
    /// Get unit by code
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<UnitDto>> GetUnitByCode(string code)
    {
        try
        {
            _log.Information("API: GetUnitByCode called for code {UnitCode} by user {User}", code, User.Identity?.Name);
            
            var unit = await _unitService.GetUnitByCodeAsync(code);

            if (unit == null)
            {
                _log.Information("API: Unit with code {UnitCode} not found", code);
                return NotFound($"Unit with code {code} not found");
            }

            return Ok(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetUnitByCode for code {UnitCode}", code);
            return StatusCode(500, "An error occurred while retrieving the unit");
        }
    }

    /// <summary>
    /// Create a new unit
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnitDto>> CreateUnit([FromBody] CreateUnitDto createDto)
    {
        try
        {
            _log.Information("API: CreateUnit called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateUnit");
                return BadRequest(ModelState);
            }

            var unit = await _unitService.CreateUnitAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetUnitById),
                new { id = unit.Id },
                unit);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateUnit");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateUnit");
            return StatusCode(500, "An error occurred while creating the unit");
        }
    }

    /// <summary>
    /// Update an existing unit
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnitDto>> UpdateUnit(int id, [FromBody] UpdateUnitDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateUnit called for ID {UnitId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateUnit");
                return BadRequest(ModelState);
            }

            var unit = await _unitService.UpdateUnitAsync(id, updateDto);

            if (unit == null)
            {
                _log.Information("API: Unit with ID {UnitId} not found for update", id);
                return NotFound($"Unit with ID {id} not found");
            }

            return Ok(unit);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateUnit");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateUnit for ID {UnitId}", id);
            return StatusCode(500, "An error occurred while updating the unit");
        }
    }

    /// <summary>
    /// Delete a unit (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUnit(int id)
    {
        try
        {
            _log.Information("API: DeleteUnit called for ID {UnitId} by user {User}", id, User.Identity?.Name);

            var result = await _unitService.DeleteUnitAsync(id);

            if (!result)
            {
                _log.Information("API: Unit with ID {UnitId} not found for deletion", id);
                return NotFound($"Unit with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteUnit for ID {UnitId}", id);
            return StatusCode(500, "An error occurred while deleting the unit");
        }
    }
}
