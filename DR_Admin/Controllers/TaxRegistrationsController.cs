using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages seller tax registrations used by VAT and TAX reporting.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaxRegistrationsController : ControllerBase
{
    private readonly ITaxRegistrationService _taxRegistrationService;

    public TaxRegistrationsController(ITaxRegistrationService taxRegistrationService)
    {
        _taxRegistrationService = taxRegistrationService;
    }

    /// <summary>
    /// Retrieves all tax registrations.
    /// </summary>
    /// <returns>List of tax registrations.</returns>
    [HttpGet]
    [Authorize(Policy = "TaxRegistration.Read")]
    [ProducesResponseType(typeof(IEnumerable<TaxRegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<TaxRegistrationDto>>> GetAllTaxRegistrations()
    {
        var result = await _taxRegistrationService.GetAllTaxRegistrationsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a tax registration by identifier.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>Tax registration details.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "TaxRegistration.Read")]
    [ProducesResponseType(typeof(TaxRegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxRegistrationDto>> GetTaxRegistrationById(int id)
    {
        var result = await _taxRegistrationService.GetTaxRegistrationByIdAsync(id);
        if (result == null)
        {
            return NotFound($"Tax registration with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a tax registration.
    /// </summary>
    /// <param name="dto">Tax registration create payload.</param>
    /// <returns>Created tax registration.</returns>
    [HttpPost]
    [Authorize(Policy = "TaxRegistration.Write")]
    [ProducesResponseType(typeof(TaxRegistrationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxRegistrationDto>> CreateTaxRegistration([FromBody] CreateTaxRegistrationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _taxRegistrationService.CreateTaxRegistrationAsync(dto);
        return CreatedAtAction(nameof(GetTaxRegistrationById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates a tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <param name="dto">Tax registration update payload.</param>
    /// <returns>Updated tax registration.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "TaxRegistration.Write")]
    [ProducesResponseType(typeof(TaxRegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxRegistrationDto>> UpdateTaxRegistration(int id, [FromBody] UpdateTaxRegistrationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _taxRegistrationService.UpdateTaxRegistrationAsync(id, dto);
        if (result == null)
        {
            return NotFound($"Tax registration with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "TaxRegistration.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTaxRegistration(int id)
    {
        var deleted = await _taxRegistrationService.DeleteTaxRegistrationAsync(id);
        if (!deleted)
        {
            return NotFound($"Tax registration with ID {id} not found");
        }

        return NoContent();
    }
}
