using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages profit margin settings for product classes.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProfitMarginSettingsController : ControllerBase
{
    private readonly IProfitMarginSettingService _service;
    private static readonly Serilog.ILogger _log = Log.ForContext<ProfitMarginSettingsController>();

    public ProfitMarginSettingsController(IProfitMarginSettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all profit margin settings.
    /// </summary>
    /// <returns>List of profit margin settings.</returns>
    [HttpGet]
    [Authorize(Policy = "ProfitMargin.Read")]
    [ProducesResponseType(typeof(IEnumerable<ProfitMarginSettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProfitMarginSettingDto>>> GetAll()
    {
        var rows = await _service.GetAllAsync();
        return Ok(rows);
    }

    /// <summary>
    /// Retrieves a profit margin setting by id.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>Profit margin setting if found.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "ProfitMargin.Read")]
    [ProducesResponseType(typeof(ProfitMarginSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfitMarginSettingDto>> GetById(int id)
    {
        var row = await _service.GetByIdAsync(id);
        if (row == null)
        {
            return NotFound($"Profit margin setting {id} not found.");
        }

        return Ok(row);
    }

    /// <summary>
    /// Retrieves active profit margin setting for a product class.
    /// </summary>
    /// <param name="productClass">Product class.</param>
    /// <returns>Profit margin setting if found.</returns>
    [HttpGet("by-class/{productClass}")]
    [Authorize(Policy = "ProfitMargin.Read")]
    [ProducesResponseType(typeof(ProfitMarginSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfitMarginSettingDto>> GetByProductClass(ProfitProductClass productClass)
    {
        var row = await _service.GetByProductClassAsync(productClass);
        if (row == null)
        {
            return NotFound($"Profit margin setting for '{productClass}' not found.");
        }

        return Ok(row);
    }

    /// <summary>
    /// Creates a new profit margin setting.
    /// </summary>
    /// <param name="dto">Create payload.</param>
    /// <returns>Created profit margin setting.</returns>
    [HttpPost]
    [Authorize(Policy = "ProfitMargin.Write")]
    [ProducesResponseType(typeof(ProfitMarginSettingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProfitMarginSettingDto>> Create([FromBody] CreateProfitMarginSettingDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "Create profit margin setting failed");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <param name="dto">Update payload.</param>
    /// <returns>Updated profit margin setting.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "ProfitMargin.Write")]
    [ProducesResponseType(typeof(ProfitMarginSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfitMarginSettingDto>> Update(int id, [FromBody] UpdateProfitMarginSettingDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
        {
            return NotFound($"Profit margin setting {id} not found.");
        }

        return Ok(updated);
    }

    /// <summary>
    /// Deletes a profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>No content when deleted.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ProfitMargin.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound($"Profit margin setting {id} not found.");
        }

        return NoContent();
    }
}
