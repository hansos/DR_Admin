using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer information including creation, retrieval, updates, and deletion
/// </summary>
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
    /// Retrieves all customers in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all customers or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of customers or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllCustomers([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllCustomers (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _customerService.GetAllCustomersPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

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
    /// Retrieves a specific customer by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the customer</param>
    /// <returns>The customer information</returns>
    /// <response code="200">Returns the customer data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="404">If customer is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Creates a new customer in the system
    /// </summary>
    /// <param name="createDto">Customer information for creation</param>
    /// <returns>The newly created customer</returns>
    /// <response code="201">Returns the newly created customer</response>
    /// <response code="400">If the customer data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Updates an existing customer's information
    /// </summary>
    /// <param name="id">The unique identifier of the customer to update</param>
    /// <param name="updateDto">Updated customer information</param>
    /// <returns>The updated customer</returns>
    /// <response code="200">Returns the updated customer</response>
    /// <response code="400">If the customer data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin or Sales)</response>
    /// <response code="404">If customer is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Deletes a customer from the system
    /// </summary>
    /// <param name="id">The unique identifier of the customer to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If customer was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If customer is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Customer.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Checks if an email address exists in the customer database
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>Information about whether the email exists</returns>
    /// <response code="200">Returns the email existence information</response>
    /// <response code="400">If the email parameter is empty or invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("check-email")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(EmailExistsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailExistsDto>> CheckEmailExists([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _log.Warning("API: CheckEmailExists called with empty email");
                return BadRequest("Email parameter is required");
            }

            _log.Information("API: CheckEmailExists called for email {Email} by user {User}", email, User.Identity?.Name);

            var exists = await _customerService.CheckEmailExistsAsync(email);

            var result = new EmailExistsDto
            {
                Email = email,
                Exists = exists
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckEmailExists for email {Email}", email);
            return StatusCode(500, "An error occurred while checking email existence");
        }
    }
}
