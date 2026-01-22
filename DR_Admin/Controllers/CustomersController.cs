using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomersController>();

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        try
        {
            _log.Information("API: GetAllCustomers called by user {User}", User.Identity?.Name);
            
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllCustomers");
            return StatusCode(500, "An error occurred while retrieving customers");
        }
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
    {
        try
        {
            _log.Information("API: GetCustomerById called for ID {CustomerId} by user {User}", id, User.Identity?.Name);
            
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                _log.Information("API: Customer with ID {CustomerId} not found", id);
                return NotFound($"Customer with ID {id} not found");
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerById for ID {CustomerId}", id);
            return StatusCode(500, "An error occurred while retrieving the customer");
        }
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createDto)
    {
        try
        {
            _log.Information("API: CreateCustomer called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateCustomer");
                return BadRequest(ModelState);
            }

            var customer = await _customerService.CreateCustomerAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCustomer");
            return StatusCode(500, "An error occurred while creating the customer");
        }
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCustomer called for ID {CustomerId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateCustomer");
                return BadRequest(ModelState);
            }

            var customer = await _customerService.UpdateCustomerAsync(id, updateDto);

            if (customer == null)
            {
                _log.Information("API: Customer with ID {CustomerId} not found for update", id);
                return NotFound($"Customer with ID {id} not found");
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCustomer for ID {CustomerId}", id);
            return StatusCode(500, "An error occurred while updating the customer");
        }
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCustomer(int id)
    {
        try
        {
            _log.Information("API: DeleteCustomer called for ID {CustomerId} by user {User}", id, User.Identity?.Name);

            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result)
            {
                _log.Information("API: Customer with ID {CustomerId} not found for deletion", id);
                return NotFound($"Customer with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCustomer for ID {CustomerId}", id);
            return StatusCode(500, "An error occurred while deleting the customer");
        }
    }
}
