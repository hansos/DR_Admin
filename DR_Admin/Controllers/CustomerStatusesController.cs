using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer statuses including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomerStatusesController : ControllerBase
{
    private readonly ICustomerStatusService _customerStatusService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerStatusesController>();

    public CustomerStatusesController(ICustomerStatusService customerStatusService)
    {
        _customerStatusService = customerStatusService;
    }

    /// <summary>
    /// Retrieves all customer statuses in the system
    /// </summary>
    /// <returns>List of all customer statuses</returns>
    /// <response code="200">Returns the list of customer statuses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<CustomerStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerStatusDto>>> GetAllCustomerStatuses()
    {
        try
        {
            _log.Information("API: GetAllCustomerStatuses called by user {User}", User.Identity?.Name);
            
            var customerStatuses = await _customerStatusService.GetAllCustomerStatusesAsync();
            return Ok(customerStatuses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllCustomerStatuses");
            return StatusCode(500, "An error occurred while retrieving customer statuses");
        }
    }

    /// <summary>
    /// Retrieves all active customer statuses
    /// </summary>
    /// <returns>List of active customer statuses</returns>
    /// <response code="200">Returns the list of active customer statuses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<CustomerStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerStatusDto>>> GetActiveCustomerStatuses()
    {
        try
        {
            _log.Information("API: GetActiveCustomerStatuses called by user {User}", User.Identity?.Name);
            
            var customerStatuses = await _customerStatusService.GetActiveCustomerStatusesAsync();
            return Ok(customerStatuses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveCustomerStatuses");
            return StatusCode(500, "An error occurred while retrieving active customer statuses");
        }
    }

    /// <summary>
    /// Retrieves the default customer status
    /// </summary>
    /// <returns>The default customer status</returns>
    /// <response code="200">Returns the default customer status</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="404">If no default customer status is found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(CustomerStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerStatusDto>> GetDefaultCustomerStatus()
    {
        try
        {
            _log.Information("API: GetDefaultCustomerStatus called by user {User}", User.Identity?.Name);
            
            var customerStatus = await _customerStatusService.GetDefaultCustomerStatusAsync();

            if (customerStatus == null)
            {
                _log.Information("API: No default customer status found");
                return NotFound("No default customer status found");
            }

            return Ok(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultCustomerStatus");
            return StatusCode(500, "An error occurred while retrieving the default customer status");
        }
    }

    /// <summary>
    /// Retrieves a specific customer status by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the customer status</param>
    /// <returns>The customer status information</returns>
    /// <response code="200">Returns the customer status data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="404">If customer status is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(CustomerStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerStatusDto>> GetCustomerStatusById(int id)
    {
        try
        {
            _log.Information("API: GetCustomerStatusById called for ID {CustomerStatusId} by user {User}", id, User.Identity?.Name);
            
            var customerStatus = await _customerStatusService.GetCustomerStatusByIdAsync(id);

            if (customerStatus == null)
            {
                _log.Information("API: Customer status with ID {CustomerStatusId} not found", id);
                return NotFound($"Customer status with ID {id} not found");
            }

            return Ok(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerStatusById for ID {CustomerStatusId}", id);
            return StatusCode(500, "An error occurred while retrieving the customer status");
        }
    }

    /// <summary>
    /// Retrieves a specific customer status by its code
    /// </summary>
    /// <param name="code">The code of the customer status</param>
    /// <returns>The customer status information</returns>
    /// <response code="200">Returns the customer status data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="404">If customer status is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(CustomerStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerStatusDto>> GetCustomerStatusByCode(string code)
    {
        try
        {
            _log.Information("API: GetCustomerStatusByCode called for code {Code} by user {User}", code, User.Identity?.Name);
            
            var customerStatus = await _customerStatusService.GetCustomerStatusByCodeAsync(code);

            if (customerStatus == null)
            {
                _log.Information("API: Customer status with code {Code} not found", code);
                return NotFound($"Customer status with code '{code}' not found");
            }

            return Ok(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerStatusByCode for code {Code}", code);
            return StatusCode(500, "An error occurred while retrieving the customer status");
        }
    }

    /// <summary>
    /// Creates a new customer status in the system
    /// </summary>
    /// <param name="createDto">Customer status information for creation</param>
    /// <returns>The newly created customer status</returns>
    /// <response code="201">Returns the newly created customer status</response>
    /// <response code="400">If the customer status data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CustomerStatusDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerStatusDto>> CreateCustomerStatus([FromBody] CreateCustomerStatusDto createDto)
    {
        try
        {
            _log.Information("API: CreateCustomerStatus called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerStatus = await _customerStatusService.CreateCustomerStatusAsync(createDto);
            
            _log.Information("API: Successfully created customer status with ID: {CustomerStatusId}", customerStatus.Id);
            return CreatedAtAction(nameof(GetCustomerStatusById), new { id = customerStatus.Id }, customerStatus);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation while creating customer status");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCustomerStatus");
            return StatusCode(500, "An error occurred while creating the customer status");
        }
    }

    /// <summary>
    /// Updates an existing customer status
    /// </summary>
    /// <param name="id">The unique identifier of the customer status to update</param>
    /// <param name="updateDto">The updated customer status information</param>
    /// <returns>The updated customer status</returns>
    /// <response code="200">Returns the updated customer status</response>
    /// <response code="400">If the customer status data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="404">If customer status is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CustomerStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerStatusDto>> UpdateCustomerStatus(int id, [FromBody] UpdateCustomerStatusDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCustomerStatus called for ID {CustomerStatusId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerStatus = await _customerStatusService.UpdateCustomerStatusAsync(id, updateDto);

            if (customerStatus == null)
            {
                _log.Information("API: Customer status with ID {CustomerStatusId} not found for update", id);
                return NotFound($"Customer status with ID {id} not found");
            }

            _log.Information("API: Successfully updated customer status with ID: {CustomerStatusId}", id);
            return Ok(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCustomerStatus for ID {CustomerStatusId}", id);
            return StatusCode(500, "An error occurred while updating the customer status");
        }
    }

    /// <summary>
    /// Deletes a customer status
    /// </summary>
    /// <param name="id">The unique identifier of the customer status to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the customer status was successfully deleted</response>
    /// <response code="400">If the customer status is in use and cannot be deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="404">If customer status is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCustomerStatus(int id)
    {
        try
        {
            _log.Information("API: DeleteCustomerStatus called for ID {CustomerStatusId} by user {User}", id, User.Identity?.Name);

            var result = await _customerStatusService.DeleteCustomerStatusAsync(id);

            if (!result)
            {
                _log.Information("API: Customer status with ID {CustomerStatusId} not found for deletion", id);
                return NotFound($"Customer status with ID {id} not found");
            }

            _log.Information("API: Successfully deleted customer status with ID: {CustomerStatusId}", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation while deleting customer status with ID {CustomerStatusId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCustomerStatus for ID {CustomerStatusId}", id);
            return StatusCode(500, "An error occurred while deleting the customer status");
        }
    }
}
