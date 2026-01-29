using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages subscription billing history records for audit and tracking purposes
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SubscriptionBillingHistoriesController : ControllerBase
{
    private readonly ISubscriptionBillingHistoryService _billingHistoryService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SubscriptionBillingHistoriesController>();

    public SubscriptionBillingHistoriesController(ISubscriptionBillingHistoryService billingHistoryService)
    {
        _billingHistoryService = billingHistoryService;
    }

    /// <summary>
    /// Retrieves all subscription billing history records in the system
    /// </summary>
    /// <returns>List of all billing history records</returns>
    /// <response code="200">Returns the list of billing histories</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "SubscriptionBillingHistory.Read")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionBillingHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionBillingHistoryDto>>> GetAllBillingHistories()
    {
        try
        {
            _log.Information("API: GetAllBillingHistories called by user {User}", User.Identity?.Name);
            var histories = await _billingHistoryService.GetAllBillingHistoriesAsync();
            return Ok(histories);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllBillingHistories");
            return StatusCode(500, "An error occurred while retrieving billing histories");
        }
    }

    /// <summary>
    /// Retrieves all billing history records for a specific subscription
    /// </summary>
    /// <param name="subscriptionId">The unique identifier of the subscription</param>
    /// <returns>List of billing history records for the specified subscription</returns>
    /// <response code="200">Returns the list of billing histories for the subscription</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("subscription/{subscriptionId}")]
    [Authorize(Policy = "SubscriptionBillingHistory.Read")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionBillingHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionBillingHistoryDto>>> GetBillingHistoriesBySubscriptionId(int subscriptionId)
    {
        try
        {
            _log.Information("API: GetBillingHistoriesBySubscriptionId called for subscription {SubscriptionId} by user {User}",
                subscriptionId, User.Identity?.Name);
            var histories = await _billingHistoryService.GetBillingHistoriesBySubscriptionIdAsync(subscriptionId);
            return Ok(histories);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetBillingHistoriesBySubscriptionId for subscription {SubscriptionId}", subscriptionId);
            return StatusCode(500, "An error occurred while retrieving billing histories");
        }
    }

    /// <summary>
    /// Retrieves a specific billing history record by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the billing history record</param>
    /// <returns>The billing history information</returns>
    /// <response code="200">Returns the billing history data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If billing history record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "SubscriptionBillingHistory.Read")]
    [ProducesResponseType(typeof(SubscriptionBillingHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionBillingHistoryDto>> GetBillingHistoryById(int id)
    {
        try
        {
            _log.Information("API: GetBillingHistoryById called for ID {BillingHistoryId} by user {User}",
                id, User.Identity?.Name);
            var history = await _billingHistoryService.GetBillingHistoryByIdAsync(id);

            if (history == null)
            {
                _log.Warning("API: Billing history with ID {BillingHistoryId} not found", id);
                return NotFound($"Billing history with ID {id} not found");
            }

            return Ok(history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetBillingHistoryById for ID {BillingHistoryId}", id);
            return StatusCode(500, "An error occurred while retrieving the billing history");
        }
    }

    /// <summary>
    /// Creates a new billing history record (typically used for manual entries)
    /// </summary>
    /// <param name="createDto">The billing history data to create</param>
    /// <returns>The created billing history record</returns>
    /// <response code="201">Returns the newly created billing history</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SubscriptionBillingHistoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionBillingHistoryDto>> CreateBillingHistory([FromBody] CreateSubscriptionBillingHistoryDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state in CreateBillingHistory");
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateBillingHistory called by user {User} for subscription {SubscriptionId}",
                User.Identity?.Name, createDto.SubscriptionId);

            var history = await _billingHistoryService.CreateBillingHistoryAsync(createDto);

            return CreatedAtAction(nameof(GetBillingHistoryById),
                new { id = history.Id }, history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateBillingHistory");
            return StatusCode(500, "An error occurred while creating the billing history");
        }
    }

    /// <summary>
    /// Deletes a billing history record
    /// </summary>
    /// <param name="id">The unique identifier of the billing history record to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the billing history was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If billing history record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBillingHistory(int id)
    {
        try
        {
            _log.Information("API: DeleteBillingHistory called for ID {BillingHistoryId} by user {User}",
                id, User.Identity?.Name);

            var result = await _billingHistoryService.DeleteBillingHistoryAsync(id);

            if (!result)
            {
                _log.Warning("API: Billing history with ID {BillingHistoryId} not found for deletion", id);
                return NotFound($"Billing history with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteBillingHistory for ID {BillingHistoryId}", id);
            return StatusCode(500, "An error occurred while deleting the billing history");
        }
    }
}
