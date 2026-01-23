using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages billing cycles including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BillingCyclesController : ControllerBase
{
    private readonly IBillingCycleService _billingCycleService;
    private static readonly Serilog.ILogger _log = Log.ForContext<BillingCyclesController>();

    public BillingCyclesController(IBillingCycleService billingCycleService)
    {
        _billingCycleService = billingCycleService;
    }

    /// <summary>
    /// Retrieves all billing cycles in the system
    /// </summary>
    /// <returns>List of all billing cycles</returns>
    /// <response code="200">Returns the list of billing cycles</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<BillingCycleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BillingCycleDto>>> GetAllBillingCycles()
    {
        try
        {
            _log.Information("API: GetAllBillingCycles called by user {User}", User.Identity?.Name);
            var billingCycles = await _billingCycleService.GetAllBillingCyclesAsync();
            return Ok(billingCycles);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllBillingCycles");
            return StatusCode(500, "An error occurred while retrieving billing cycles");
        }
    }

    /// <summary>
    /// Retrieves a specific billing cycle by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the billing cycle</param>
    /// <returns>The billing cycle information</returns>
    /// <response code="200">Returns the billing cycle data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If billing cycle is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(BillingCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BillingCycleDto>> GetBillingCycleById(int id)
    {
        try
        {
            _log.Information("API: GetBillingCycleById called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);
            var billingCycle = await _billingCycleService.GetBillingCycleByIdAsync(id);

            if (billingCycle == null)
            {
                _log.Information("API: Billing cycle with ID {BillingCycleId} not found", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return Ok(billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetBillingCycleById for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while retrieving the billing cycle");
        }
    }

    /// <summary>
    /// Creates a new billing cycle in the system
    /// </summary>
    /// <param name="createDto">Billing cycle information for creation</param>
    /// <returns>The newly created billing cycle</returns>
    /// <response code="201">Returns the newly created billing cycle</response>
    /// <response code="400">If the billing cycle data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BillingCycleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BillingCycleDto>> CreateBillingCycle([FromBody] CreateBillingCycleDto createDto)
    {
        try
        {
            _log.Information("API: CreateBillingCycle called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateBillingCycle");
                return BadRequest(ModelState);
            }

            var billingCycle = await _billingCycleService.CreateBillingCycleAsync(createDto);
            return CreatedAtAction(nameof(GetBillingCycleById), new { id = billingCycle.Id }, billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateBillingCycle");
            return StatusCode(500, "An error occurred while creating the billing cycle");
        }
    }

    /// <summary>
    /// Updates an existing billing cycle's information
    /// </summary>
    /// <param name="id">The unique identifier of the billing cycle to update</param>
    /// <param name="updateDto">Updated billing cycle information</param>
    /// <returns>The updated billing cycle</returns>
    /// <response code="200">Returns the updated billing cycle</response>
    /// <response code="400">If the billing cycle data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If billing cycle is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BillingCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BillingCycleDto>> UpdateBillingCycle(int id, [FromBody] UpdateBillingCycleDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateBillingCycle called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateBillingCycle");
                return BadRequest(ModelState);
            }

            var billingCycle = await _billingCycleService.UpdateBillingCycleAsync(id, updateDto);

            if (billingCycle == null)
            {
                _log.Information("API: Billing cycle with ID {BillingCycleId} not found for update", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return Ok(billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateBillingCycle for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while updating the billing cycle");
        }
    }

    /// <summary>
    /// Deletes a billing cycle from the system
    /// </summary>
    /// <param name="id">The unique identifier of the billing cycle to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If billing cycle was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If billing cycle is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteBillingCycle(int id)
    {
        try
        {
            _log.Information("API: DeleteBillingCycle called for ID {BillingCycleId} by user {User}", id, User.Identity?.Name);
            var result = await _billingCycleService.DeleteBillingCycleAsync(id);

            if (!result)
            {
                _log.Information("API: Billing cycle with ID {BillingCycleId} not found for deletion", id);
                return NotFound($"Billing cycle with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteBillingCycle for ID {BillingCycleId}", id);
            return StatusCode(500, "An error occurred while deleting the billing cycle");
        }
    }
}
