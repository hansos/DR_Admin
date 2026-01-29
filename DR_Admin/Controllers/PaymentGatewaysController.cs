using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages payment gateway configurations including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentGatewaysController : ControllerBase
{
    private readonly IPaymentGatewayService _paymentGatewayService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentGatewaysController>();

    public PaymentGatewaysController(IPaymentGatewayService paymentGatewayService)
    {
        _paymentGatewayService = paymentGatewayService;
    }

    /// <summary>
    /// Retrieves all payment gateways in the system
    /// </summary>
    /// <returns>List of all payment gateways</returns>
    /// <response code="200">Returns the list of payment gateways</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "PaymentGateway.Read")]
    [ProducesResponseType(typeof(IEnumerable<PaymentGatewayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PaymentGatewayDto>>> GetAllPaymentGateways()
    {
        try
        {
            _log.Information("API: GetAllPaymentGateways called by user {User}", User.Identity?.Name);
            var gateways = await _paymentGatewayService.GetAllPaymentGatewaysAsync();
            return Ok(gateways);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllPaymentGateways");
            return StatusCode(500, "An error occurred while retrieving payment gateways");
        }
    }

    /// <summary>
    /// Retrieves all active payment gateways
    /// </summary>
    /// <returns>List of active payment gateways</returns>
    /// <response code="200">Returns the list of active payment gateways</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<PaymentGatewayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PaymentGatewayDto>>> GetActivePaymentGateways()
    {
        try
        {
            _log.Information("API: GetActivePaymentGateways called by user {User}", User.Identity?.Name);
            var gateways = await _paymentGatewayService.GetActivePaymentGatewaysAsync();
            return Ok(gateways);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActivePaymentGateways");
            return StatusCode(500, "An error occurred while retrieving active payment gateways");
        }
    }

    /// <summary>
    /// Retrieves the default payment gateway
    /// </summary>
    /// <returns>Default payment gateway information</returns>
    /// <response code="200">Returns the default payment gateway</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If no default payment gateway is found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default")]
    [ProducesResponseType(typeof(PaymentGatewayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentGatewayDto>> GetDefaultPaymentGateway()
    {
        try
        {
            _log.Information("API: GetDefaultPaymentGateway called by user {User}", User.Identity?.Name);
            var gateway = await _paymentGatewayService.GetDefaultPaymentGatewayAsync();

            if (gateway == null)
            {
                _log.Information("API: No default payment gateway found");
                return NotFound("No default payment gateway found");
            }

            return Ok(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultPaymentGateway");
            return StatusCode(500, "An error occurred while retrieving the default payment gateway");
        }
    }

    /// <summary>
    /// Retrieves a specific payment gateway by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the payment gateway</param>
    /// <returns>The payment gateway information</returns>
    /// <response code="200">Returns the payment gateway data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "PaymentGateway.Read")]
    [ProducesResponseType(typeof(PaymentGatewayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentGatewayDto>> GetPaymentGatewayById(int id)
    {
        try
        {
            _log.Information("API: GetPaymentGatewayById called for ID {GatewayId} by user {User}", id, User.Identity?.Name);
            var gateway = await _paymentGatewayService.GetPaymentGatewayByIdAsync(id);

            if (gateway == null)
            {
                _log.Information("API: Payment gateway with ID {GatewayId} not found", id);
                return NotFound($"Payment gateway with ID {id} not found");
            }

            return Ok(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentGatewayById for ID {GatewayId}", id);
            return StatusCode(500, "An error occurred while retrieving the payment gateway");
        }
    }

    /// <summary>
    /// Retrieves a payment gateway by provider code
    /// </summary>
    /// <param name="providerCode">The provider code (stripe, paypal, square)</param>
    /// <returns>The payment gateway information</returns>
    /// <response code="200">Returns the payment gateway data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("provider/{providerCode}")]
    [ProducesResponseType(typeof(PaymentGatewayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentGatewayDto>> GetPaymentGatewayByProvider(string providerCode)
    {
        try
        {
            _log.Information("API: GetPaymentGatewayByProvider called for provider {ProviderCode} by user {User}", providerCode, User.Identity?.Name);
            var gateway = await _paymentGatewayService.GetPaymentGatewayByProviderAsync(providerCode);

            if (gateway == null)
            {
                _log.Information("API: Payment gateway with provider {ProviderCode} not found", providerCode);
                return NotFound($"Payment gateway with provider '{providerCode}' not found");
            }

            return Ok(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentGatewayByProvider for provider {ProviderCode}", providerCode);
            return StatusCode(500, "An error occurred while retrieving the payment gateway");
        }
    }

    /// <summary>
    /// Creates a new payment gateway
    /// </summary>
    /// <param name="dto">Payment gateway creation data</param>
    /// <returns>The created payment gateway</returns>
    /// <response code="201">Returns the newly created payment gateway</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentGatewayDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentGatewayDto>> CreatePaymentGateway([FromBody] CreatePaymentGatewayDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: CreatePaymentGateway called with invalid model state by user {User}", User.Identity?.Name);
                return BadRequest(ModelState);
            }

            _log.Information("API: CreatePaymentGateway called for {GatewayName} by user {User}", dto.Name, User.Identity?.Name);
            var gateway = await _paymentGatewayService.CreatePaymentGatewayAsync(dto);

            return CreatedAtAction(
                nameof(GetPaymentGatewayById),
                new { id = gateway.Id },
                gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreatePaymentGateway for {GatewayName}", dto.Name);
            return StatusCode(500, "An error occurred while creating the payment gateway");
        }
    }

    /// <summary>
    /// Updates an existing payment gateway
    /// </summary>
    /// <param name="id">The unique identifier of the payment gateway to update</param>
    /// <param name="dto">Payment gateway update data</param>
    /// <returns>The updated payment gateway</returns>
    /// <response code="200">Returns the updated payment gateway</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentGatewayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentGatewayDto>> UpdatePaymentGateway(int id, [FromBody] UpdatePaymentGatewayDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("API: UpdatePaymentGateway called with invalid model state for ID {GatewayId} by user {User}", id, User.Identity?.Name);
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdatePaymentGateway called for ID {GatewayId} by user {User}", id, User.Identity?.Name);
            var gateway = await _paymentGatewayService.UpdatePaymentGatewayAsync(id, dto);

            if (gateway == null)
            {
                _log.Information("API: Payment gateway with ID {GatewayId} not found for update", id);
                return NotFound($"Payment gateway with ID {id} not found");
            }

            return Ok(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdatePaymentGateway for ID {GatewayId}", id);
            return StatusCode(500, "An error occurred while updating the payment gateway");
        }
    }

    /// <summary>
    /// Sets a payment gateway as the default
    /// </summary>
    /// <param name="id">The unique identifier of the payment gateway</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment gateway was set as default successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/set-default")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetDefaultPaymentGateway(int id)
    {
        try
        {
            _log.Information("API: SetDefaultPaymentGateway called for ID {GatewayId} by user {User}", id, User.Identity?.Name);
            var success = await _paymentGatewayService.SetDefaultPaymentGatewayAsync(id);

            if (!success)
            {
                _log.Information("API: Payment gateway with ID {GatewayId} not found for setting default", id);
                return NotFound($"Payment gateway with ID {id} not found");
            }

            return Ok(new { message = "Payment gateway set as default successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetDefaultPaymentGateway for ID {GatewayId}", id);
            return StatusCode(500, "An error occurred while setting the default payment gateway");
        }
    }

    /// <summary>
    /// Activates or deactivates a payment gateway
    /// </summary>
    /// <param name="id">The unique identifier of the payment gateway</param>
    /// <param name="isActive">Active status to set</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment gateway status was updated successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/set-active")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetPaymentGatewayActiveStatus(int id, [FromBody] bool isActive)
    {
        try
        {
            _log.Information("API: SetPaymentGatewayActiveStatus called for ID {GatewayId} with status {IsActive} by user {User}", id, isActive, User.Identity?.Name);
            var success = await _paymentGatewayService.SetPaymentGatewayActiveStatusAsync(id, isActive);

            if (!success)
            {
                _log.Information("API: Payment gateway with ID {GatewayId} not found for setting active status", id);
                return NotFound($"Payment gateway with ID {id} not found");
            }

            return Ok(new { message = $"Payment gateway {(isActive ? "activated" : "deactivated")} successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetPaymentGatewayActiveStatus for ID {GatewayId}", id);
            return StatusCode(500, "An error occurred while updating the payment gateway status");
        }
    }

    /// <summary>
    /// Deletes a payment gateway (soft delete)
    /// </summary>
    /// <param name="id">The unique identifier of the payment gateway to delete</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment gateway was deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If payment gateway is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletePaymentGateway(int id)
    {
        try
        {
            _log.Information("API: DeletePaymentGateway called for ID {GatewayId} by user {User}", id, User.Identity?.Name);
            var success = await _paymentGatewayService.DeletePaymentGatewayAsync(id);

            if (!success)
            {
                _log.Information("API: Payment gateway with ID {GatewayId} not found for deletion", id);
                return NotFound($"Payment gateway with ID {id} not found");
            }

            return Ok(new { message = "Payment gateway deleted successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeletePaymentGateway for ID {GatewayId}", id);
            return StatusCode(500, "An error occurred while deleting the payment gateway");
        }
    }
}
