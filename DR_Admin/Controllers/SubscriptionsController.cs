using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages recurring billing subscriptions including creation, updates, cancellation, and billing operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SubscriptionsController>();

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Retrieves all subscriptions in the system
    /// </summary>
    /// <returns>List of all subscriptions</returns>
    /// <response code="200">Returns the list of subscriptions</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Subscription.Read")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetAllSubscriptions()
    {
        try
        {
            _log.Information("API: GetAllSubscriptions called by user {User}", User.Identity?.Name);
            var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllSubscriptions");
            return StatusCode(500, "An error occurred while retrieving subscriptions");
        }
    }

    /// <summary>
    /// Retrieves all subscriptions for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>List of subscriptions for the specified customer</returns>
    /// <response code="200">Returns the list of customer subscriptions</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "Subscription.Read")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptionsByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetSubscriptionsByCustomerId called for customer {CustomerId} by user {User}",
                customerId, User.Identity?.Name);
            var subscriptions = await _subscriptionService.GetSubscriptionsByCustomerIdAsync(customerId);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSubscriptionsByCustomerId for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving subscriptions");
        }
    }

    /// <summary>
    /// Retrieves all subscriptions with a specific status
    /// </summary>
    /// <param name="status">The subscription status to filter by</param>
    /// <returns>List of subscriptions with the specified status</returns>
    /// <response code="200">Returns the list of subscriptions matching the status</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("status/{status}")]
    [Authorize(Policy = "Subscription.Read")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptionsByStatus(SubscriptionStatus status)
    {
        try
        {
            _log.Information("API: GetSubscriptionsByStatus called for status {Status} by user {User}",
                status, User.Identity?.Name);
            var subscriptions = await _subscriptionService.GetSubscriptionsByStatusAsync(status);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSubscriptionsByStatus for status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving subscriptions");
        }
    }

    /// <summary>
    /// Retrieves subscriptions that are due for billing
    /// </summary>
    /// <param name="dueDate">Optional date to check for due subscriptions (defaults to current date/time)</param>
    /// <returns>List of subscriptions due for billing</returns>
    /// <response code="200">Returns the list of due subscriptions</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("due")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetDueSubscriptions([FromQuery] DateTime? dueDate = null)
    {
        try
        {
            _log.Information("API: GetDueSubscriptions called by user {User}", User.Identity?.Name);
            var subscriptions = await _subscriptionService.GetDueSubscriptionsAsync(dueDate);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDueSubscriptions");
            return StatusCode(500, "An error occurred while retrieving due subscriptions");
        }
    }

    /// <summary>
    /// Retrieves a specific subscription by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the subscription</param>
    /// <returns>The subscription information</returns>
    /// <response code="200">Returns the subscription data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(int id)
    {
        try
        {
            _log.Information("API: GetSubscriptionById called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);

            if (subscription == null)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSubscriptionById for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while retrieving the subscription");
        }
    }

    /// <summary>
    /// Creates a new subscription
    /// </summary>
    /// <param name="createDto">The subscription data to create</param>
    /// <returns>The created subscription</returns>
    /// <response code="201">Returns the newly created subscription</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription([FromBody] CreateSubscriptionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state in CreateSubscription");
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateSubscription called by user {User} for customer {CustomerId}",
                User.Identity?.Name, createDto.CustomerId);

            var subscription = await _subscriptionService.CreateSubscriptionAsync(createDto);

            return CreatedAtAction(nameof(GetSubscriptionById),
                new { id = subscription.Id }, subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateSubscription");
            return StatusCode(500, "An error occurred while creating the subscription");
        }
    }

    /// <summary>
    /// Updates an existing subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to update</param>
    /// <param name="updateDto">The updated subscription data</param>
    /// <returns>The updated subscription</returns>
    /// <response code="200">Returns the updated subscription</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(int id, [FromBody] UpdateSubscriptionDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state in UpdateSubscription for ID {SubscriptionId}", id);
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateSubscription called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var subscription = await _subscriptionService.UpdateSubscriptionAsync(id, updateDto);

            if (subscription == null)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found for update", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateSubscription for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while updating the subscription");
        }
    }

    /// <summary>
    /// Cancels a subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to cancel</param>
    /// <param name="cancelDto">The cancellation details</param>
    /// <returns>The cancelled subscription</returns>
    /// <response code="200">Returns the cancelled subscription</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> CancelSubscription(int id, [FromBody] CancelSubscriptionDto cancelDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state in CancelSubscription for ID {SubscriptionId}", id);
                return BadRequest(ModelState);
            }

            _log.Information("API: CancelSubscription called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var subscription = await _subscriptionService.CancelSubscriptionAsync(id, cancelDto);

            if (subscription == null)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found for cancellation", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CancelSubscription for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while cancelling the subscription");
        }
    }

    /// <summary>
    /// Pauses a subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to pause</param>
    /// <param name="pauseDto">The pause details</param>
    /// <returns>The paused subscription</returns>
    /// <response code="200">Returns the paused subscription</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/pause")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> PauseSubscription(int id, [FromBody] PauseSubscriptionDto pauseDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state in PauseSubscription for ID {SubscriptionId}", id);
                return BadRequest(ModelState);
            }

            _log.Information("API: PauseSubscription called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var subscription = await _subscriptionService.PauseSubscriptionAsync(id, pauseDto);

            if (subscription == null)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found for pause", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in PauseSubscription for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while pausing the subscription");
        }
    }

    /// <summary>
    /// Resumes a paused subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to resume</param>
    /// <returns>The resumed subscription</returns>
    /// <response code="200">Returns the resumed subscription</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/resume")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubscriptionDto>> ResumeSubscription(int id)
    {
        try
        {
            _log.Information("API: ResumeSubscription called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var subscription = await _subscriptionService.ResumeSubscriptionAsync(id);

            if (subscription == null)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found for resume", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return Ok(subscription);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in ResumeSubscription for ID {SubscriptionId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ResumeSubscription for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while resuming the subscription");
        }
    }

    /// <summary>
    /// Deletes a subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the subscription was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSubscription(int id)
    {
        try
        {
            _log.Information("API: DeleteSubscription called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var result = await _subscriptionService.DeleteSubscriptionAsync(id);

            if (!result)
            {
                _log.Warning("API: Subscription with ID {SubscriptionId} not found for deletion", id);
                return NotFound($"Subscription with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteSubscription for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while deleting the subscription");
        }
    }

    /// <summary>
    /// Manually processes billing for a specific subscription
    /// </summary>
    /// <param name="id">The unique identifier of the subscription to bill</param>
    /// <returns>Success or failure result of the billing process</returns>
    /// <response code="200">If billing was processed successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If subscription is not found</response>
    /// <response code="500">If an internal server error occurs or billing fails</response>
    [HttpPost("{id}/process-billing")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessSubscriptionBilling(int id)
    {
        try
        {
            _log.Information("API: ProcessSubscriptionBilling called for ID {SubscriptionId} by user {User}",
                id, User.Identity?.Name);

            var result = await _subscriptionService.ProcessSubscriptionBillingAsync(id);

            if (!result)
            {
                _log.Warning("API: Failed to process billing for subscription {SubscriptionId}", id);
                return StatusCode(500, "Failed to process subscription billing");
            }

            return Ok(new { message = "Billing processed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ProcessSubscriptionBilling for ID {SubscriptionId}", id);
            return StatusCode(500, "An error occurred while processing subscription billing");
        }
    }
}
