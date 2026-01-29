using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages payment intents for processing payments
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentIntentsController : ControllerBase
{
    private readonly IPaymentIntentService _paymentIntentService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentIntentsController>();

    public PaymentIntentsController(IPaymentIntentService paymentIntentService)
    {
        _paymentIntentService = paymentIntentService;
    }

    /// <summary>
    /// Retrieves all payment intents in the system
    /// </summary>
    /// <returns>List of all payment intents</returns>
    /// <response code="200">Returns the list of payment intents</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "PaymentIntent.Read")]
    [ProducesResponseType(typeof(IEnumerable<PaymentIntentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PaymentIntentDto>>> GetAllPaymentIntents()
    {
        try
        {
            _log.Information("API: GetAllPaymentIntents called by user {User}", User.Identity?.Name);
            var paymentIntents = await _paymentIntentService.GetAllPaymentIntentsAsync();
            return Ok(paymentIntents);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllPaymentIntents");
            return StatusCode(500, "An error occurred while retrieving payment intents");
        }
    }

    /// <summary>
    /// Retrieves a specific payment intent by ID
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <returns>The payment intent details</returns>
    /// <response code="200">Returns the payment intent</response>
    /// <response code="404">If the payment intent is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "PaymentIntent.Read")]
    [ProducesResponseType(typeof(PaymentIntentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentIntentDto>> GetPaymentIntentById(int id)
    {
        try
        {
            _log.Information("API: GetPaymentIntentById called for ID: {PaymentIntentId} by user {User}", 
                id, User.Identity?.Name);
            var paymentIntent = await _paymentIntentService.GetPaymentIntentByIdAsync(id);

            if (paymentIntent == null)
            {
                _log.Warning("API: Payment intent with ID {PaymentIntentId} not found", id);
                return NotFound($"Payment intent with ID {id} not found");
            }

            return Ok(paymentIntent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentIntentById for ID: {PaymentIntentId}", id);
            return StatusCode(500, "An error occurred while retrieving the payment intent");
        }
    }

    /// <summary>
    /// Retrieves all payment intents for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of payment intents for the customer</returns>
    /// <response code="200">Returns the list of payment intents</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "PaymentIntent.Read")]
    [ProducesResponseType(typeof(IEnumerable<PaymentIntentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PaymentIntentDto>>> GetPaymentIntentsByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetPaymentIntentsByCustomerId called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var paymentIntents = await _paymentIntentService.GetPaymentIntentsByCustomerIdAsync(customerId);
            return Ok(paymentIntents);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentIntentsByCustomerId for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving payment intents");
        }
    }

    /// <summary>
    /// Creates a new payment intent
    /// </summary>
    /// <param name="createDto">The payment intent creation data</param>
    /// <returns>The created payment intent</returns>
    /// <response code="201">Returns the newly created payment intent</response>
    /// <response code="400">If the payment intent data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "PaymentIntent.Write")]
    [ProducesResponseType(typeof(PaymentIntentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentIntentDto>> CreatePaymentIntent([FromBody] CreatePaymentIntentDto createDto)
    {
        try
        {
            _log.Information("API: CreatePaymentIntent called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get customer ID from claims or use the one from DTO
            var customerIdClaim = User.FindFirst("customerId") ?? User.FindFirst("id");
            int customerId = 0;
            
            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int claimCustomerId))
            {
                customerId = claimCustomerId;
            }

            var paymentIntent = await _paymentIntentService.CreatePaymentIntentAsync(createDto, customerId);
            
            _log.Information("API: Payment intent created with ID: {PaymentIntentId}", paymentIntent.Id);
            return CreatedAtAction(nameof(GetPaymentIntentById), new { id = paymentIntent.Id }, paymentIntent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreatePaymentIntent");
            return StatusCode(500, "An error occurred while creating the payment intent");
        }
    }

    /// <summary>
    /// Confirms a payment intent with a payment method
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <param name="paymentMethodToken">The payment method token</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment intent was confirmed successfully</response>
    /// <response code="404">If the payment intent is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/confirm")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ConfirmPaymentIntent(int id, [FromBody] string paymentMethodToken)
    {
        try
        {
            _log.Information("API: ConfirmPaymentIntent called for ID: {PaymentIntentId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _paymentIntentService.ConfirmPaymentIntentAsync(id, paymentMethodToken);

            if (!result)
            {
                _log.Warning("API: Failed to confirm payment intent with ID {PaymentIntentId}", id);
                return NotFound($"Payment intent with ID {id} not found or could not be confirmed");
            }

            _log.Information("API: Payment intent confirmed with ID: {PaymentIntentId}", id);
            return Ok(new { message = "Payment intent confirmed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ConfirmPaymentIntent for ID: {PaymentIntentId}", id);
            return StatusCode(500, "An error occurred while confirming the payment intent");
        }
    }

    /// <summary>
    /// Cancels a payment intent
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment intent was cancelled successfully</response>
    /// <response code="404">If the payment intent is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelPaymentIntent(int id)
    {
        try
        {
            _log.Information("API: CancelPaymentIntent called for ID: {PaymentIntentId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _paymentIntentService.CancelPaymentIntentAsync(id);

            if (!result)
            {
                _log.Warning("API: Failed to cancel payment intent with ID {PaymentIntentId}", id);
                return NotFound($"Payment intent with ID {id} not found or could not be cancelled");
            }

            _log.Information("API: Payment intent cancelled with ID: {PaymentIntentId}", id);
            return Ok(new { message = "Payment intent cancelled successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CancelPaymentIntent for ID: {PaymentIntentId}", id);
            return StatusCode(500, "An error occurred while cancelling the payment intent");
        }
    }

    /// <summary>
    /// Processes a webhook from a payment gateway
    /// </summary>
    /// <param name="gatewayId">The payment gateway ID</param>
    /// <param name="payload">The webhook payload</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the webhook was processed successfully</response>
    /// <response code="400">If the payload is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("webhook/{gatewayId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ProcessWebhook(int gatewayId, [FromBody] string payload)
    {
        try
        {
            _log.Information("API: ProcessWebhook called for gateway: {GatewayId}", gatewayId);

            if (string.IsNullOrWhiteSpace(payload))
            {
                return BadRequest("Webhook payload is required");
            }

            var result = await _paymentIntentService.ProcessWebhookAsync(gatewayId, payload);

            if (!result)
            {
                _log.Warning("API: Failed to process webhook for gateway {GatewayId}", gatewayId);
                return BadRequest("Failed to process webhook");
            }

            _log.Information("API: Webhook processed successfully for gateway {GatewayId}", gatewayId);
            return Ok(new { message = "Webhook processed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ProcessWebhook for gateway: {GatewayId}", gatewayId);
            return StatusCode(500, "An error occurred while processing the webhook");
        }
    }
}
