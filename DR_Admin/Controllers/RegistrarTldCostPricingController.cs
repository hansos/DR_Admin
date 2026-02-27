using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages registrar TLD cost pricing (temporal pricing for registrar costs)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistrarTldCostPricingController : ControllerBase
{
    private readonly ITldPricingService _pricingService;
    private readonly IRegistrarTldPriceSyncService _priceSyncService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldCostPricingController>();

    public RegistrarTldCostPricingController(
        ITldPricingService pricingService,
        IRegistrarTldPriceSyncService priceSyncService)
    {
        _pricingService = pricingService;
        _priceSyncService = priceSyncService;
    }

    /// <summary>
    /// Retrieves cost pricing history for a specific registrar-TLD relationship
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <param name="includeArchived">Whether to include archived pricing</param>
    /// <returns>List of cost pricing records</returns>
    /// <response code="200">Returns the cost pricing history</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar-tld/{registrarTldId}")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<RegistrarTldCostPricingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarTldCostPricingDto>>> GetCostPricingHistory(
        int registrarTldId,
        [FromQuery] bool includeArchived = false)
    {
        try
        {
            _log.Information("API: GetCostPricingHistory called for RegistrarTld {RegistrarTldId} by user {User}",
                registrarTldId, User.Identity?.Name);

            var history = await _pricingService.GetCostPricingHistoryAsync(registrarTldId, includeArchived);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCostPricingHistory for RegistrarTld {RegistrarTldId}", registrarTldId);
            return StatusCode(500, "An error occurred while retrieving cost pricing history");
        }
    }

    /// <summary>
    /// Retrieves current registrar cost pricing rows for a TLD, and triggers registrar API download if no prices exist.
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="effectiveDate">Optional date to check (default: now)</param>
    /// <returns>List of current registrar cost pricing rows</returns>
    /// <response code="200">Returns current registrar cost pricing rows, including rows populated after sync</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("tld/{tldId}/current/ensure")]
    [Authorize(Policy = "Pricing.Write")]
    [ProducesResponseType(typeof(List<RegistrarCurrentCostByTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarCurrentCostByTldDto>>> EnsureCurrentRegistrarCostsByTld(
        int tldId,
        [FromQuery] DateTime? effectiveDate = null)
    {
        try
        {
            var rows = await _pricingService.GetCurrentRegistrarCostsByTldAsync(tldId, effectiveDate);
            var hasAnyPrice = rows.Any(r => r.RegistrationCost.HasValue || r.RenewalCost.HasValue || r.TransferCost.HasValue);

            if (!hasAnyPrice)
            {
                _log.Information("API: No registrar cost prices found for TLD {TldId}. Triggering sync by user {User}",
                    tldId, User.Identity?.Name);

                await _priceSyncService.SyncRegistrarsForTldAsync(
                    tldId,
                    $"ManualEnsureFromTldList:{User.Identity?.Name ?? "unknown"}");

                rows = await _pricingService.GetCurrentRegistrarCostsByTldAsync(tldId, effectiveDate);
            }

            return Ok(rows);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in EnsureCurrentRegistrarCostsByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while ensuring current registrar cost pricing");
        }
    }

    /// <summary>
    /// Downloads registrar price preview for a TLD extension without persisting to database.
    /// </summary>
    /// <param name="extension">The TLD extension</param>
    /// <returns>List of registrar cost pricing preview rows</returns>
    /// <response code="200">Returns registrar price preview rows</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("preview/extension/{extension}")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<RegistrarCurrentCostByTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarCurrentCostByTldDto>>> PreviewRegistrarCostsByExtension(string extension)
    {
        try
        {
            var rows = await _priceSyncService.PreviewRegistrarCostsByExtensionAsync(extension);
            return Ok(rows);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in PreviewRegistrarCostsByExtension for extension {Extension}", extension);
            return StatusCode(500, "An error occurred while previewing registrar cost pricing");
        }
    }

    /// <summary>
    /// Retrieves current registrar cost pricing rows for all active registrar-TLD relationships linked to a TLD.
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="effectiveDate">Optional date to check (default: now)</param>
    /// <returns>List of current registrar cost pricing rows</returns>
    /// <response code="200">Returns the current registrar cost pricing rows</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("tld/{tldId}/current")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<RegistrarCurrentCostByTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarCurrentCostByTldDto>>> GetCurrentRegistrarCostsByTld(
        int tldId,
        [FromQuery] DateTime? effectiveDate = null)
    {
        try
        {
            _log.Information("API: GetCurrentRegistrarCostsByTld called for TLD {TldId} by user {User}", tldId, User.Identity?.Name);

            var rows = await _pricingService.GetCurrentRegistrarCostsByTldAsync(tldId, effectiveDate);
            return Ok(rows);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCurrentRegistrarCostsByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving current registrar cost pricing");
        }
    }

    /// <summary>
    /// Retrieves all registrar TLD cost pricing records for a specific TLD.
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <returns>List of all registrar TLD cost pricing records for the TLD</returns>
    /// <response code="200">Returns all registrar TLD cost pricing records for the TLD</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("tld/{tldId}")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<RegistrarTldCostPricingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarTldCostPricingDto>>> GetCostPricingByTld(int tldId)
    {
        try
        {
            _log.Information("API: GetCostPricingByTld called for TLD {TldId} by user {User}", tldId, User.Identity?.Name);

            var rows = await _pricingService.GetCostPricingByTldAsync(tldId);
            return Ok(rows);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCostPricingByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving registrar TLD cost pricing");
        }
    }

    /// <summary>
    /// Retrieves the current effective cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <param name="effectiveDate">Optional date to check (default: now)</param>
    /// <returns>Current cost pricing or null if none found</returns>
    /// <response code="200">Returns the current cost pricing</response>
    /// <response code="404">If no current pricing is found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar-tld/{registrarTldId}/current")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(RegistrarTldCostPricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldCostPricingDto>> GetCurrentCostPricing(
        int registrarTldId,
        [FromQuery] DateTime? effectiveDate = null)
    {
        try
        {
            _log.Information("API: GetCurrentCostPricing called for RegistrarTld {RegistrarTldId} by user {User}",
                registrarTldId, User.Identity?.Name);

            var pricing = await _pricingService.GetCurrentCostPricingAsync(registrarTldId, effectiveDate);

            if (pricing == null)
            {
                return NotFound($"No current cost pricing found for registrar-TLD {registrarTldId}");
            }

            return Ok(pricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCurrentCostPricing for RegistrarTld {RegistrarTldId}", registrarTldId);
            return StatusCode(500, "An error occurred while retrieving current cost pricing");
        }
    }

    /// <summary>
    /// Retrieves future scheduled cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <returns>List of future cost pricing records</returns>
    /// <response code="200">Returns the future cost pricing</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar-tld/{registrarTldId}/future")]
    [Authorize(Policy = "Pricing.Read")]
    [ProducesResponseType(typeof(List<RegistrarTldCostPricingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RegistrarTldCostPricingDto>>> GetFutureCostPricing(int registrarTldId)
    {
        try
        {
            _log.Information("API: GetFutureCostPricing called for RegistrarTld {RegistrarTldId} by user {User}",
                registrarTldId, User.Identity?.Name);

            var futurePricing = await _pricingService.GetFutureCostPricingAsync(registrarTldId);
            return Ok(futurePricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetFutureCostPricing for RegistrarTld {RegistrarTldId}", registrarTldId);
            return StatusCode(500, "An error occurred while retrieving future cost pricing");
        }
    }

    /// <summary>
    /// Creates new cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="createDto">The cost pricing data</param>
    /// <returns>The created cost pricing</returns>
    /// <response code="201">Returns the created cost pricing</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Pricing.Write")]
    [ProducesResponseType(typeof(RegistrarTldCostPricingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldCostPricingDto>> CreateCostPricing(
        [FromBody] CreateRegistrarTldCostPricingDto createDto)
    {
        try
        {
            _log.Information("API: CreateCostPricing called for RegistrarTld {RegistrarTldId} by user {User}",
                createDto.RegistrarTldId, User.Identity?.Name);

            var created = await _pricingService.CreateCostPricingAsync(createDto, User.Identity?.Name);

            return CreatedAtAction(
                nameof(GetCurrentCostPricing),
                new { registrarTldId = createDto.RegistrarTldId },
                created);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateCostPricing");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCostPricing");
            return StatusCode(500, "An error occurred while creating cost pricing");
        }
    }

    /// <summary>
    /// Updates existing cost pricing (only future pricing if configured)
    /// </summary>
    /// <param name="id">The cost pricing ID</param>
    /// <param name="updateDto">The update data</param>
    /// <returns>The updated cost pricing</returns>
    /// <response code="200">Returns the updated cost pricing</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="404">If the cost pricing is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Pricing.Write")]
    [ProducesResponseType(typeof(RegistrarTldCostPricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldCostPricingDto>> UpdateCostPricing(
        int id,
        [FromBody] UpdateRegistrarTldCostPricingDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCostPricing called for {CostPricingId} by user {User}",
                id, User.Identity?.Name);

            var updated = await _pricingService.UpdateCostPricingAsync(id, updateDto, User.Identity?.Name);

            if (updated == null)
            {
                return NotFound($"Cost pricing {id} not found");
            }

            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateCostPricing");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCostPricing");
            return StatusCode(500, "An error occurred while updating cost pricing");
        }
    }

    /// <summary>
    /// Deletes future cost pricing (only if not yet effective and configured to allow)
    /// </summary>
    /// <param name="id">The cost pricing ID</param>
    /// <returns>Success indicator</returns>
    /// <response code="204">If the cost pricing was deleted</response>
    /// <response code="400">If the cost pricing cannot be deleted</response>
    /// <response code="404">If the cost pricing is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Pricing.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteFutureCostPricing(int id)
    {
        try
        {
            _log.Information("API: DeleteFutureCostPricing called for {CostPricingId} by user {User}",
                id, User.Identity?.Name);

            var deleted = await _pricingService.DeleteFutureCostPricingAsync(id);

            if (!deleted)
            {
                return NotFound($"Cost pricing {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteFutureCostPricing");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteFutureCostPricing");
            return StatusCode(500, "An error occurred while deleting cost pricing");
        }
    }
}
