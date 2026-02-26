using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages contact persons for customers including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContactPersonsController : ControllerBase
{
    private readonly IContactPersonService _contactPersonService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ContactPersonsController>();

    public ContactPersonsController(IContactPersonService contactPersonService)
    {
        _contactPersonService = contactPersonService;
    }

    /// <summary>
    /// Retrieves all contact persons in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <param name="customerId">Optional: Filter by customer ID</param>
    /// <param name="search">Optional: Search in name, email or phone</param>
    /// <returns>List of all contact persons or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of contact persons or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(IEnumerable<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllContactPersons([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null, [FromQuery] int? customerId = null, [FromQuery] string? search = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue || customerId.HasValue || !string.IsNullOrWhiteSpace(search))
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllContactPersons (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize}, CustomerId: {CustomerId}, Search: {Search} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, customerId, search, User.Identity?.Name);

                var pagedResult = await _contactPersonService.GetAllContactPersonsPagedAsync(paginationParams, customerId, search);
                return Ok(pagedResult);
            }

            _log.Information("API: GetAllContactPersons called by user {User}", User.Identity?.Name);
            
            var contactPersons = await _contactPersonService.GetAllContactPersonsAsync();
            return Ok(contactPersons);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllContactPersons");
            return StatusCode(500, "An error occurred while retrieving contact persons");
        }
    }

    /// <summary>
    /// Retrieves all contact persons that are available globally for domains
    /// </summary>
    /// <returns>List of contact persons with IsDomainGlobal set to true</returns>
    /// <response code="200">Returns the list of domain global contact persons</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("domain-global")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(IEnumerable<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ContactPersonDto>>> GetDomainGlobalContactPersons()
    {
        try
        {
            _log.Information("API: GetDomainGlobalContactPersons called by user {User}", User.Identity?.Name);

            var contactPersons = await _contactPersonService.GetDomainGlobalContactPersonsAsync();
            return Ok(contactPersons);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainGlobalContactPersons");
            return StatusCode(500, "An error occurred while retrieving domain global contact persons");
        }
    }

    /// <summary>
    /// Retrieves all contact persons for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>List of contact persons for the specified customer</returns>
    /// <response code="200">Returns the list of contact persons for the customer</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(IEnumerable<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ContactPersonDto>>> GetContactPersonsByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetContactPersonsByCustomerId called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);
            
            var contactPersons = await _contactPersonService.GetContactPersonsByCustomerIdAsync(customerId);
            return Ok(contactPersons);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetContactPersonsByCustomerId for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving contact persons");
        }
    }

    /// <summary>
    /// Retrieves a specific contact person by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the contact person</param>
    /// <returns>The contact person information</returns>
    /// <response code="200">Returns the contact person data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="404">If contact person is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(ContactPersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactPersonDto>> GetContactPersonById(int id)
    {
        try
        {
            _log.Information("API: GetContactPersonById called for ID {ContactPersonId} by user {User}", id, User.Identity?.Name);
            
            var contactPerson = await _contactPersonService.GetContactPersonByIdAsync(id);

            if (contactPerson == null)
            {
                _log.Information("API: Contact person with ID {ContactPersonId} not found", id);
                return NotFound($"Contact person with ID {id} not found");
            }

            return Ok(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetContactPersonById for ID {ContactPersonId}", id);
            return StatusCode(500, "An error occurred while retrieving the contact person");
        }
    }

    /// <summary>
    /// Creates a new contact person in the system
    /// </summary>
    /// <param name="createDto">Contact person information for creation</param>
    /// <returns>The newly created contact person</returns>
    /// <response code="201">Returns the newly created contact person</response>
    /// <response code="400">If the contact person data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(ContactPersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactPersonDto>> CreateContactPerson([FromBody] CreateContactPersonDto createDto)
    {
        try
        {
            _log.Information("API: CreateContactPerson called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateContactPerson");
                return BadRequest(ModelState);
            }

            var contactPerson = await _contactPersonService.CreateContactPersonAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetContactPersonById),
                new { id = contactPerson.Id },
                contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateContactPerson");
            return StatusCode(500, "An error occurred while creating the contact person");
        }
    }

    /// <summary>
    /// Updates an existing contact person's information
    /// </summary>
    /// <param name="id">The unique identifier of the contact person to update</param>
    /// <param name="updateDto">Updated contact person information</param>
    /// <returns>The updated contact person</returns>
    /// <response code="200">Returns the updated contact person</response>
    /// <response code="400">If the contact person data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin or Sales)</response>
    /// <response code="404">If contact person is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Customer.Write")]
    [ProducesResponseType(typeof(ContactPersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactPersonDto>> UpdateContactPerson(int id, [FromBody] UpdateContactPersonDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateContactPerson called for ID {ContactPersonId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateContactPerson");
                return BadRequest(ModelState);
            }

            var contactPerson = await _contactPersonService.UpdateContactPersonAsync(id, updateDto);

            if (contactPerson == null)
            {
                _log.Information("API: Contact person with ID {ContactPersonId} not found for update", id);
                return NotFound($"Contact person with ID {id} not found");
            }

            return Ok(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateContactPerson for ID {ContactPersonId}", id);
            return StatusCode(500, "An error occurred while updating the contact person");
        }
    }

    /// <summary>
    /// Updates the IsDomainGlobal flag for an existing contact person
    /// </summary>
    /// <param name="id">The unique identifier of the contact person to update</param>
    /// <param name="updateDto">The new IsDomainGlobal value</param>
    /// <returns>The updated contact person</returns>
    /// <response code="200">Returns the updated contact person</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If contact person is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPatch("{id}/domain-global")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ContactPersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactPersonDto>> PatchContactPersonIsDomainGlobal(int id, [FromBody] UpdateContactPersonIsDomainGlobalDto updateDto)
    {
        try
        {
            _log.Information("API: PatchContactPersonIsDomainGlobal called for ID {ContactPersonId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for PatchContactPersonIsDomainGlobal");
                return BadRequest(ModelState);
            }

            var contactPerson = await _contactPersonService.UpdateContactPersonIsDomainGlobalAsync(id, updateDto.IsDomainGlobal);

            if (contactPerson == null)
            {
                _log.Information("API: Contact person with ID {ContactPersonId} not found for IsDomainGlobal patch", id);
                return NotFound($"Contact person with ID {id} not found");
            }

            return Ok(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in PatchContactPersonIsDomainGlobal for ID {ContactPersonId}", id);
            return StatusCode(500, "An error occurred while updating contact person IsDomainGlobal");
        }
    }

    /// <summary>
    /// Deletes a contact person from the system
    /// </summary>
    /// <param name="id">The unique identifier of the contact person to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If contact person was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If contact person is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Customer.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteContactPerson(int id)
    {
        try
        {
            _log.Information("API: DeleteContactPerson called for ID {ContactPersonId} by user {User}", id, User.Identity?.Name);

            var result = await _contactPersonService.DeleteContactPersonAsync(id);

            if (!result)
            {
                _log.Information("API: Contact person with ID {ContactPersonId} not found for deletion", id);
                return NotFound($"Contact person with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteContactPerson for ID {ContactPersonId}", id);
            return StatusCode(500, "An error occurred while deleting the contact person");
        }
    }

    /// <summary>
    /// Retrieves contact persons for a customer categorized by role preference and usage.
    /// Returns a three-tiered list:
    /// 1. Preferred - Contact persons marked as default for the specified role
    /// 2. Frequently Used - Contact persons used 3+ times for the specified role
    /// 3. Available - All other contact persons
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="roleType">The domain contact role type (Registrant=1, Administrative=2, Technical=3, Billing=4)</param>
    /// <returns>Categorized list of contact persons sorted by preference and usage</returns>
    /// <response code="200">Returns the categorized list of contact persons</response>
    /// <response code="400">If the role type is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Support, or Sales)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}/for-role/{roleType}")]
    [Authorize(Policy = "Customer.Read")]
    [ProducesResponseType(typeof(CategorizedContactPersonListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategorizedContactPersonListResponse>> GetContactPersonsForRole(
        int customerId, 
        ContactRoleType roleType)
    {
        try
        {
            _log.Information("API: GetContactPersonsForRole called for customer ID {CustomerId}, role {RoleType} by user {User}", 
                customerId, roleType, User.Identity?.Name);

            if (!Enum.IsDefined(typeof(ContactRoleType), roleType))
            {
                _log.Warning("API: Invalid role type {RoleType} provided", roleType);
                return BadRequest($"Invalid role type: {roleType}. Valid values are: Registrant (1), Administrative (2), Technical (3), Billing (4)");
            }

            var result = await _contactPersonService.GetContactPersonsForRoleAsync(customerId, roleType);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetContactPersonsForRole for customer ID {CustomerId}, role {RoleType}", 
                customerId, roleType);
            return StatusCode(500, "An error occurred while retrieving contact persons");
        }
    }
}
