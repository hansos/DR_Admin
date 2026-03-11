using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides quote and finalize tax endpoints for checkout and invoicing.
/// </summary>
[ApiController]
[Route("api/v1/tax")]
[Authorize]
public class TaxCalculationController : ControllerBase
{
    private readonly ITaxCalculationService _taxCalculationService;

    public TaxCalculationController(ITaxCalculationService taxCalculationService)
    {
        _taxCalculationService = taxCalculationService;
    }

    /// <summary>
    /// Calculates a tax quote without persisting an immutable snapshot.
    /// </summary>
    /// <param name="request">Tax quote request payload.</param>
    /// <returns>Tax quote result.</returns>
    [HttpPost("quote")]
    [Authorize(Policy = "TaxCalculation.Quote")]
    [ProducesResponseType(typeof(TaxQuoteResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxQuoteResultDto>> Quote([FromBody] TaxQuoteRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _taxCalculationService.QuoteTaxAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Finalizes tax calculation and persists an immutable order tax snapshot.
    /// </summary>
    /// <param name="request">Tax finalize request payload.</param>
    /// <returns>Finalized tax result.</returns>
    [HttpPost("finalize")]
    [Authorize(Policy = "TaxCalculation.Finalize")]
    [ProducesResponseType(typeof(TaxQuoteResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaxQuoteResultDto>> Finalize([FromBody] TaxQuoteRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _taxCalculationService.FinalizeTaxAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
