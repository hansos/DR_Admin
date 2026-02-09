using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer tax profiles
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomerTaxProfilesController : ControllerBase
{
    private readonly ICustomerTaxProfileService _customerTaxProfileService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerTaxProfilesController>();

    public CustomerTaxProfilesController(ICustomerTaxProfileService customerTaxProfileService)
    {
        _customerTaxProfileService = customerTaxProfileService;
    }

    /// <summary>
    /// Retrieves all customer tax profiles in the system
    /// </summary>
    /// <returns>List of all customer tax profiles</returns>
    /// <response code="200">Returns the list of customer tax profiles</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<CustomerTaxProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerTaxProfileDto>>> GetAllCustomerTaxProfiles()
    {
        try
        {
            _log.Information("API: GetAllCustomerTaxProfiles called by user {User}", User.Identity?.Name);
            var profiles = await _customerTaxProfileService.GetAllCustomerTaxProfilesAsync();
            return Ok(profiles);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllCustomerTaxProfiles");
            return StatusCode(500, "An error occurred while retrieving customer tax profiles");
        }
    }

    /// <summary>
    /// Retrieves a specific customer tax profile by ID
    /// </summary>
    /// <param name="id">The customer tax profile ID</param>
    /// <returns>The customer tax profile details</returns>
    /// <response code="200">Returns the customer tax profile</response>
    /// <response code="404">If the customer tax profile is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(CustomerTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerTaxProfileDto>> GetCustomerTaxProfileById(int id)
    {
        try
        {
            _log.Information("API: GetCustomerTaxProfileById called for ID: {Id} by user {User}", id, User.Identity?.Name);
            var profile = await _customerTaxProfileService.GetCustomerTaxProfileByIdAsync(id);

            if (profile == null)
            {
                _log.Warning("API: Customer tax profile with ID {Id} not found", id);
                return NotFound($"Customer tax profile with ID {id} not found");
            }

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerTaxProfileById for ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the customer tax profile");
        }
    }

    /// <summary>
    /// Retrieves customer tax profile by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The customer tax profile</returns>
    /// <response code="200">Returns the customer tax profile</response>
    /// <response code="404">If the customer tax profile is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(CustomerTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerTaxProfileDto>> GetCustomerTaxProfileByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetCustomerTaxProfileByCustomerId called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var profile = await _customerTaxProfileService.GetCustomerTaxProfileByCustomerIdAsync(customerId);

            if (profile == null)
            {
                _log.Warning("API: Customer tax profile for customer ID {CustomerId} not found", customerId);
                return NotFound($"Customer tax profile for customer ID {customerId} not found");
            }

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerTaxProfileByCustomerId for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving the customer tax profile");
        }
    }

    /// <summary>
    /// Creates a new customer tax profile
    /// </summary>
    /// <param name="createDto">The customer tax profile creation data</param>
    /// <returns>The created customer tax profile</returns>
    /// <response code="201">Returns the newly created customer tax profile</response>
    /// <response code="400">If the customer tax profile data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(CustomerTaxProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerTaxProfileDto>> CreateCustomerTaxProfile([FromBody] CreateCustomerTaxProfileDto createDto)
    {
        try
        {
            _log.Information("API: CreateCustomerTaxProfile called for customer: {CustomerId} by user {User}", 
                createDto.CustomerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var profile = await _customerTaxProfileService.CreateCustomerTaxProfileAsync(createDto);
            
            _log.Information("API: Customer tax profile created with ID: {Id}", profile.Id);
            return CreatedAtAction(nameof(GetCustomerTaxProfileById), new { id = profile.Id }, profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCustomerTaxProfile");
            return StatusCode(500, "An error occurred while creating the customer tax profile");
        }
    }

    /// <summary>
    /// Updates an existing customer tax profile
    /// </summary>
    /// <param name="id">The customer tax profile ID</param>
    /// <param name="updateDto">The updated customer tax profile data</param>
    /// <returns>The updated customer tax profile</returns>
    /// <response code="200">Returns the updated customer tax profile</response>
    /// <response code="400">If the customer tax profile data is invalid</response>
    /// <response code="404">If the customer tax profile is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(CustomerTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerTaxProfileDto>> UpdateCustomerTaxProfile(int id, [FromBody] UpdateCustomerTaxProfileDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCustomerTaxProfile called for ID: {Id} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var profile = await _customerTaxProfileService.UpdateCustomerTaxProfileAsync(id, updateDto);

            if (profile == null)
            {
                _log.Warning("API: Customer tax profile with ID {Id} not found for update", id);
                return NotFound($"Customer tax profile with ID {id} not found");
            }

            _log.Information("API: Customer tax profile with ID {Id} updated successfully", id);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCustomerTaxProfile for ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the customer tax profile");
        }
    }

    /// <summary>
    /// Deletes a customer tax profile
    /// </summary>
    /// <param name="id">The customer tax profile ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the customer tax profile was successfully deleted</response>
    /// <response code="404">If the customer tax profile is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCustomerTaxProfile(int id)
    {
        try
        {
            _log.Information("API: DeleteCustomerTaxProfile called for ID: {Id} by user {User}", id, User.Identity?.Name);

            var result = await _customerTaxProfileService.DeleteCustomerTaxProfileAsync(id);

            if (!result)
            {
                _log.Warning("API: Customer tax profile with ID {Id} not found for deletion", id);
                return NotFound($"Customer tax profile with ID {id} not found");
            }

            _log.Information("API: Customer tax profile with ID {Id} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCustomerTaxProfile for ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the customer tax profile");
        }
    }

    /// <summary>
    /// Validates a customer's tax ID
    /// </summary>
    /// <param name="validateDto">The validation request data</param>
    /// <returns>The validation result</returns>
    /// <response code="200">Returns the validation result</response>
    /// <response code="400">If the validation data is invalid</response>
    /// <response code="404">If the customer tax profile is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("validate")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(TaxIdValidationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaxIdValidationResultDto>> ValidateTaxId([FromBody] ValidateTaxIdDto validateDto)
    {
        try
        {
            _log.Information("API: ValidateTaxId called for profile ID: {Id} by user {User}", 
                validateDto.CustomerTaxProfileId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerTaxProfileService.ValidateTaxIdAsync(validateDto);

            if (result == null)
            {
                _log.Warning("API: Customer tax profile with ID {Id} not found for validation", validateDto.CustomerTaxProfileId);
                return NotFound($"Customer tax profile with ID {validateDto.CustomerTaxProfileId} not found");
            }

            _log.Information("API: Tax ID validation completed for profile ID: {Id}, Valid: {IsValid}", 
                validateDto.CustomerTaxProfileId, result.IsValid);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ValidateTaxId for profile ID: {Id}", validateDto.CustomerTaxProfileId);
            return StatusCode(500, "An error occurred while validating the tax ID");
        }
    }
}
