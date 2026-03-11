using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages tax jurisdictions used by VAT and TAX calculation.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaxJurisdictionsController : ControllerBase
{
    private readonly ITaxJurisdictionService _taxJurisdictionService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TaxJurisdictionsController>();

    public TaxJurisdictionsController(ITaxJurisdictionService taxJurisdictionService)
    {
        _taxJurisdictionService = taxJurisdictionService;
    }

    /// <summary>
    /// Retrieves all tax jurisdictions.
    /// </summary>
    /// <returns>List of tax jurisdictions.</returns>
    [HttpGet]
    [Authorize(Policy = "TaxJurisdiction.Read")]
    [ProducesResponseType(typeof(IEnumerable<TaxJurisdictionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<TaxJurisdictionDto>>> GetAllTaxJurisdictions()
    {
        _log.Information("API: GetAllTaxJurisdictions called by user {User}", User.Identity?.Name);
        var result = await _taxJurisdictionService.GetAllTaxJurisdictionsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a tax jurisdiction by identifier.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>Tax jurisdiction details.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "TaxJurisdiction.Read")]
    [ProducesResponseType(typeof(TaxJurisdictionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxJurisdictionDto>> GetTaxJurisdictionById(int id)
    {
        var result = await _taxJurisdictionService.GetTaxJurisdictionByIdAsync(id);
        if (result == null)
        {
            return NotFound($"Tax jurisdiction with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a tax jurisdiction.
    /// </summary>
    /// <param name="dto">Tax jurisdiction create payload.</param>
    /// <returns>Created tax jurisdiction.</returns>
    [HttpPost]
    [Authorize(Policy = "TaxJurisdiction.Write")]
    [ProducesResponseType(typeof(TaxJurisdictionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxJurisdictionDto>> CreateTaxJurisdiction([FromBody] CreateTaxJurisdictionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _taxJurisdictionService.CreateTaxJurisdictionAsync(dto);
        return CreatedAtAction(nameof(GetTaxJurisdictionById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates a tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <param name="dto">Tax jurisdiction update payload.</param>
    /// <returns>Updated tax jurisdiction.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "TaxJurisdiction.Write")]
    [ProducesResponseType(typeof(TaxJurisdictionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxJurisdictionDto>> UpdateTaxJurisdiction(int id, [FromBody] UpdateTaxJurisdictionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _taxJurisdictionService.UpdateTaxJurisdictionAsync(id, dto);
        if (result == null)
        {
            return NotFound($"Tax jurisdiction with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "TaxJurisdiction.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTaxJurisdiction(int id)
    {
        var deleted = await _taxJurisdictionService.DeleteTaxJurisdictionAsync(id);
        if (!deleted)
        {
            return NotFound($"Tax jurisdiction with ID {id} not found");
        }

        return NoContent();
    }
}
