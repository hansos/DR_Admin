using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages TLD sales pricing (temporal pricing for customer-facing prices)
/// </summary>
[ApiController]
[Route("api/v1/tld-pricing")]
[Authorize]
public class TldPricingController : ControllerBase
{
    private readonly ITldPricingService _pricingService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldPricingController>();

    public TldPricingController(ITldPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    // ==================== TLD Sales Pricing Endpoints ====================

    /// <summary>
    /// Retrieves sales pricing history for a specific TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="includeArchived">Whether to include archived pricing</param>
    /// <returns>List of sales pricing records</returns>
    [HttpGet("sales/tld/{tldId}")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<TldSalesPricingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TldSalesPricingDto>>> GetSalesPricingHistory(
        int tldId,
        [FromQuery] bool includeArchived = false)
    {
        try
        {
            var history = await _pricingService.GetSalesPricingHistoryAsync(tldId, includeArchived);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving sales pricing history");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Retrieves the current effective sales pricing for a TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="effectiveDate">Optional date to check (default: now)</param>
    /// <returns>Current sales pricing or null</returns>
    [HttpGet("sales/tld/{tldId}/current")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(TldSalesPricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TldSalesPricingDto>> GetCurrentSalesPricing(
        int tldId,
        [FromQuery] DateTime? effectiveDate = null)
    {
        try
        {
            var pricing = await _pricingService.GetCurrentSalesPricingAsync(tldId, effectiveDate);
            if (pricing == null)
            {
                return NotFound($"No current sales pricing found for TLD {tldId}");
            }
            return Ok(pricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving current sales pricing");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Creates new sales pricing for a TLD
    /// </summary>
    /// <param name="createDto">The sales pricing data</param>
    /// <returns>The created sales pricing</returns>
    [HttpPost("sales")]
    [Authorize(Policy = "Pricing.Write")]
    [ProducesResponseType(typeof(TldSalesPricingDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TldSalesPricingDto>> CreateSalesPricing(
        [FromBody] CreateTldSalesPricingDto createDto)
    {
        try
        {
            var created = await _pricingService.CreateSalesPricingAsync(createDto, User.Identity?.Name);
            return CreatedAtAction(nameof(GetCurrentSalesPricing), new { tldId = createDto.TldId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating sales pricing");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Updates existing sales pricing (only future pricing if configured)
    /// </summary>
    /// <param name="id">The sales pricing ID</param>
    /// <param name="updateDto">The update data</param>
    /// <returns>The updated sales pricing</returns>
    [HttpPut("sales/{id}")]
    [Authorize(Policy = "Pricing.Write")]
    [ProducesResponseType(typeof(TldSalesPricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TldSalesPricingDto>> UpdateSalesPricing(
        int id,
        [FromBody] UpdateTldSalesPricingDto updateDto)
    {
        try
        {
            var updated = await _pricingService.UpdateSalesPricingAsync(id, updateDto, User.Identity?.Name);
            if (updated == null)
            {
                return NotFound($"Sales pricing {id} not found");
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating sales pricing");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Deletes future sales pricing
    /// </summary>
    /// <param name="id">The sales pricing ID</param>
    /// <returns>Success indicator</returns>
    [HttpDelete("sales/{id}")]
    [Authorize(Policy = "Pricing.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFutureSalesPricing(int id)
    {
        try
        {
            var deleted = await _pricingService.DeleteFutureSalesPricingAsync(id);
            if (!deleted)
            {
                return NotFound($"Sales pricing {id} not found");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting sales pricing");
            return StatusCode(500, "An error occurred");
        }
    }

    // ==================== Pricing Calculation Endpoints ====================

    /// <summary>
    /// Calculates final pricing for a TLD including discounts and promotions
    /// </summary>
    /// <param name="request">The calculation request</param>
    /// <returns>Calculated pricing response</returns>
    [HttpPost("calculate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CalculatePricingResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CalculatePricingResponse>> CalculatePricing(
        [FromBody] CalculatePricingRequest request)
    {
        try
        {
            var response = await _pricingService.CalculatePricingAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error calculating pricing");
            return StatusCode(500, "An error occurred");
        }
    }

    // ==================== Margin Analysis Endpoints ====================

    /// <summary>
    /// Calculates margin for a specific TLD and operation type
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="operationType">The operation type (Registration, Renewal, Transfer)</param>
    /// <param name="registrarId">Optional specific registrar ID</param>
    /// <returns>Margin analysis result</returns>
    [HttpGet("margin/tld/{tldId}")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(MarginAnalysisResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<MarginAnalysisResult>> CalculateMargin(
        int tldId,
        [FromQuery] string operationType = "Registration",
        [FromQuery] int? registrarId = null)
    {
        try
        {
            var margin = await _pricingService.CalculateMarginAsync(tldId, operationType, registrarId);
            return Ok(margin);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error calculating margin");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Gets all TLDs with negative margins (cost > price)
    /// </summary>
    /// <returns>List of margin analysis results with negative margins</returns>
    [HttpGet("margin/negative")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<MarginAnalysisResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MarginAnalysisResult>>> GetNegativeMarginReport()
    {
        try
        {
            var report = await _pricingService.GetNegativeMarginReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error generating negative margin report");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Gets all TLDs with low margins (below threshold)
    /// </summary>
    /// <returns>List of margin analysis results with low margins</returns>
    [HttpGet("margin/low")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<MarginAnalysisResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MarginAnalysisResult>>> GetLowMarginReport()
    {
        try
        {
            var report = await _pricingService.GetLowMarginReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error generating low margin report");
            return StatusCode(500, "An error occurred");
        }
    }

    // ==================== Archive Management Endpoints ====================

    /// <summary>
    /// Archives old pricing data based on retention policy
    /// </summary>
    /// <returns>Number of records archived</returns>
    [HttpPost("archive")]
    [Authorize(Policy = "Pricing.Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> ArchiveOldPricing()
    {
        try
        {
            var costArchived = await _pricingService.ArchiveOldCostPricingAsync();
            var salesArchived = await _pricingService.ArchiveOldSalesPricingAsync();
            var discountsArchived = await _pricingService.ArchiveOldDiscountsAsync();

            return Ok(new
            {
                CostPricingArchived = costArchived,
                SalesPricingArchived = salesArchived,
                DiscountsArchived = discountsArchived,
                TotalArchived = costArchived + salesArchived + discountsArchived
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error archiving old pricing data");
            return StatusCode(500, "An error occurred");
        }
    }
}
