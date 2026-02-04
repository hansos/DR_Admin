using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer address information including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/customers/{customerId}/addresses")]
[Authorize]
public class CustomerAddressesController : ControllerBase
{
    private readonly ICustomerAddressService _customerAddressService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerAddressesController>();

    public CustomerAddressesController(ICustomerAddressService customerAddressService)
    {
        _customerAddressService = customerAddressService;
    }

    /// <summary>
    /// Retrieves all addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>List of all addresses for the customer</returns>
    /// <response code="200">Returns the list of customer addresses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(IEnumerable<CustomerAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCustomerAddresses(int customerId)
    {
        try
        {
            _log.Information("API: GetCustomerAddresses called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);
            
            var addresses = await _customerAddressService.GetCustomerAddressesAsync(customerId);
            return Ok(addresses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerAddresses for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving customer addresses");
        }
    }

    /// <summary>
    /// Retrieves the primary address for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>The primary customer address</returns>
    /// <response code="200">Returns the primary customer address</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If primary address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("primary")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAddressDto>> GetPrimaryAddress(int customerId)
    {
        try
        {
            _log.Information("API: GetPrimaryAddress called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);
            
            var address = await _customerAddressService.GetPrimaryAddressAsync(customerId);

            if (address == null)
            {
                _log.Information("API: Primary address for customer ID {CustomerId} not found", customerId);
                return NotFound($"Primary address for customer ID {customerId} not found");
            }

            return Ok(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPrimaryAddress for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving the primary address");
        }
    }

    /// <summary>
    /// Retrieves a specific customer address by its unique identifier
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the customer address</param>
    /// <returns>The customer address information</returns>
    /// <response code="200">Returns the customer address data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If customer address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAddressDto>> GetCustomerAddressById(int customerId, int id)
    {
        try
        {
            _log.Information("API: GetCustomerAddressById called for ID {CustomerAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);
            
            var address = await _customerAddressService.GetCustomerAddressByIdAsync(id);

            if (address == null || address.CustomerId != customerId)
            {
                _log.Information("API: Customer address with ID {CustomerAddressId} not found for customer ID {CustomerId}", id, customerId);
                return NotFound($"Customer address with ID {id} not found");
            }

            return Ok(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerAddressById for ID {CustomerAddressId}", id);
            return StatusCode(500, "An error occurred while retrieving the customer address");
        }
    }

    /// <summary>
    /// Creates a new customer address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="createDto">Customer address information for creation</param>
    /// <returns>The newly created customer address</returns>
    /// <response code="201">Returns the newly created customer address</response>
    /// <response code="400">If the customer address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAddressDto>> CreateCustomerAddress(int customerId, [FromBody] CreateCustomerAddressDto createDto)
    {
        try
        {
            _log.Information("API: CreateCustomerAddress called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateCustomerAddress");
                return BadRequest(ModelState);
            }

            var address = await _customerAddressService.CreateCustomerAddressAsync(customerId, createDto);
            
            return CreatedAtAction(
                nameof(GetCustomerAddressById),
                new { customerId = customerId, id = address.Id },
                address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCustomerAddress for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while creating the customer address");
        }
    }

    /// <summary>
    /// Updates an existing customer address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the customer address to update</param>
    /// <param name="updateDto">Updated customer address information</param>
    /// <returns>The updated customer address</returns>
    /// <response code="200">Returns the updated customer address</response>
    /// <response code="400">If the customer address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If customer address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAddressDto>> UpdateCustomerAddress(int customerId, int id, [FromBody] UpdateCustomerAddressDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCustomerAddress called for ID {CustomerAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateCustomerAddress");
                return BadRequest(ModelState);
            }

            var address = await _customerAddressService.UpdateCustomerAddressAsync(id, updateDto);

            if (address == null || address.CustomerId != customerId)
            {
                _log.Information("API: Customer address with ID {CustomerAddressId} not found for update", id);
                return NotFound($"Customer address with ID {id} not found");
            }

            return Ok(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCustomerAddress for ID {CustomerAddressId}", id);
            return StatusCode(500, "An error occurred while updating the customer address");
        }
    }

    /// <summary>
    /// Deletes a customer address from the system
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the customer address to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the customer address was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If customer address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Customer.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCustomerAddress(int customerId, int id)
    {
        try
        {
            _log.Information("API: DeleteCustomerAddress called for ID {CustomerAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var address = await _customerAddressService.GetCustomerAddressByIdAsync(id);
            
            if (address == null || address.CustomerId != customerId)
            {
                _log.Information("API: Customer address with ID {CustomerAddressId} not found for deletion", id);
                return NotFound($"Customer address with ID {id} not found");
            }

            var result = await _customerAddressService.DeleteCustomerAddressAsync(id);

            if (!result)
            {
                return NotFound($"Customer address with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCustomerAddress for ID {CustomerAddressId}", id);
            return StatusCode(500, "An error occurred while deleting the customer address");
        }
    }

    /// <summary>
    /// Sets a customer address as the primary address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the customer address to set as primary</param>
    /// <returns>The updated customer address</returns>
    /// <response code="200">Returns the updated customer address</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If customer address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}/set-primary")]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(CustomerAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAddressDto>> SetPrimaryAddress(int customerId, int id)
    {
        try
        {
            _log.Information("API: SetPrimaryAddress called for ID {CustomerAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var address = await _customerAddressService.SetPrimaryAddressAsync(id);

            if (address == null || address.CustomerId != customerId)
            {
                _log.Information("API: Customer address with ID {CustomerAddressId} not found", id);
                return NotFound($"Customer address with ID {id} not found");
            }

            return Ok(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetPrimaryAddress for ID {CustomerAddressId}", id);
            return StatusCode(500, "An error occurred while setting the primary address");
        }
    }
}
