using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer payment methods
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomerPaymentMethodsController : ControllerBase
{
    private readonly ICustomerPaymentMethodService _paymentMethodService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerPaymentMethodsController>();

    public CustomerPaymentMethodsController(ICustomerPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    /// <summary>
    /// Retrieves all payment methods for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of customer payment methods</returns>
    /// <response code="200">Returns the list of payment methods</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<CustomerPaymentMethodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerPaymentMethodDto>>> GetPaymentMethodsByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetPaymentMethodsByCustomerId called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var paymentMethods = await _paymentMethodService.GetPaymentMethodsByCustomerIdAsync(customerId);
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentMethodsByCustomerId for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving payment methods");
        }
    }

    /// <summary>
    /// Retrieves a specific payment method by ID
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <returns>The payment method details</returns>
    /// <response code="200">Returns the payment method</response>
    /// <response code="404">If the payment method is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(CustomerPaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerPaymentMethodDto>> GetPaymentMethodById(int id)
    {
        try
        {
            _log.Information("API: GetPaymentMethodById called for ID: {PaymentMethodId} by user {User}", 
                id, User.Identity?.Name);
            var paymentMethod = await _paymentMethodService.GetPaymentMethodByIdAsync(id);

            if (paymentMethod == null)
            {
                _log.Warning("API: Payment method with ID {PaymentMethodId} not found", id);
                return NotFound($"Payment method with ID {id} not found");
            }

            return Ok(paymentMethod);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPaymentMethodById for ID: {PaymentMethodId}", id);
            return StatusCode(500, "An error occurred while retrieving the payment method");
        }
    }

    /// <summary>
    /// Retrieves the default payment method for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The default payment method</returns>
    /// <response code="200">Returns the default payment method</response>
    /// <response code="404">If no default payment method is found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}/default")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(CustomerPaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerPaymentMethodDto>> GetDefaultPaymentMethod(int customerId)
    {
        try
        {
            _log.Information("API: GetDefaultPaymentMethod called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var paymentMethod = await _paymentMethodService.GetDefaultPaymentMethodAsync(customerId);

            if (paymentMethod == null)
            {
                _log.Warning("API: No default payment method found for customer {CustomerId}", customerId);
                return NotFound($"No default payment method found for customer {customerId}");
            }

            return Ok(paymentMethod);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultPaymentMethod for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving the default payment method");
        }
    }

    /// <summary>
    /// Creates a new payment method for a customer
    /// </summary>
    /// <param name="createDto">The payment method creation data</param>
    /// <returns>The created payment method</returns>
    /// <response code="201">Returns the newly created payment method</response>
    /// <response code="400">If the payment method data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CustomerPaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerPaymentMethodDto>> CreatePaymentMethod([FromBody] CreateCustomerPaymentMethodDto createDto)
    {
        try
        {
            _log.Information("API: CreatePaymentMethod called for customer: {CustomerId} by user {User}", 
                createDto.CustomerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentMethod = await _paymentMethodService.CreatePaymentMethodAsync(createDto);
            
            _log.Information("API: Payment method created with ID: {PaymentMethodId}", paymentMethod.Id);
            return CreatedAtAction(nameof(GetPaymentMethodById), new { id = paymentMethod.Id }, paymentMethod);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreatePaymentMethod");
            return StatusCode(500, "An error occurred while creating the payment method");
        }
    }

    /// <summary>
    /// Sets a payment method as the default for a customer
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <param name="customerId">The customer ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the payment method was set as default successfully</response>
    /// <response code="404">If the payment method is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/set-default")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetAsDefault(int id, [FromQuery] int customerId)
    {
        try
        {
            _log.Information("API: SetAsDefault called for payment method: {PaymentMethodId}, customer: {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var result = await _paymentMethodService.SetAsDefaultAsync(id, customerId);

            if (!result)
            {
                _log.Warning("API: Payment method {PaymentMethodId} not found or doesn't belong to customer {CustomerId}", 
                    id, customerId);
                return NotFound($"Payment method with ID {id} not found");
            }

            _log.Information("API: Payment method {PaymentMethodId} set as default", id);
            return Ok(new { message = "Payment method set as default successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetAsDefault for payment method: {PaymentMethodId}", id);
            return StatusCode(500, "An error occurred while setting the default payment method");
        }
    }

    /// <summary>
    /// Deletes a payment method
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <param name="customerId">The customer ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the payment method was successfully deleted</response>
    /// <response code="404">If the payment method is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletePaymentMethod(int id, [FromQuery] int customerId)
    {
        try
        {
            _log.Information("API: DeletePaymentMethod called for ID: {PaymentMethodId}, customer: {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var result = await _paymentMethodService.DeletePaymentMethodAsync(id, customerId);

            if (!result)
            {
                _log.Warning("API: Payment method {PaymentMethodId} not found for deletion", id);
                return NotFound($"Payment method with ID {id} not found");
            }

            _log.Information("API: Payment method deleted with ID: {PaymentMethodId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeletePaymentMethod for ID: {PaymentMethodId}", id);
            return StatusCode(500, "An error occurred while deleting the payment method");
        }
    }
}
