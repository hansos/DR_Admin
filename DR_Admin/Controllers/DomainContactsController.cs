using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages domain contact persons for domain registrations including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DomainContactsController : ControllerBase
{
    private readonly IDomainContactService _domainContactService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainContactsController>();

    public DomainContactsController(IDomainContactService domainContactService)
    {
        _domainContactService = domainContactService;
    }

    /// <summary>
    /// Retrieves all domain contacts in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all domain contacts or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of domain contacts or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<DomainContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<DomainContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllDomainContacts([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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

                _log.Information("API: GetAllDomainContacts (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}",
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _domainContactService.GetAllDomainContactsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            _log.Information("API: GetAllDomainContacts called by user {User}", User.Identity?.Name);

            var domainContacts = await _domainContactService.GetAllDomainContactsAsync();
            return Ok(domainContacts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDomainContacts");
            return StatusCode(500, "An error occurred while retrieving domain contacts");
        }
    }

    /// <summary>
    /// Retrieves all domain contacts for a specific domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>List of domain contacts for the specified domain</returns>
    /// <response code="200">Returns the list of domain contacts for the domain</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("domain/{domainId}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<DomainContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DomainContactDto>>> GetDomainContactsByDomainId(int domainId)
    {
        try
        {
            _log.Information("API: GetDomainContactsByDomainId called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);

            var domainContacts = await _domainContactService.GetDomainContactsByDomainIdAsync(domainId);
            return Ok(domainContacts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainContactsByDomainId for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while retrieving domain contacts");
        }
    }

    /// <summary>
    /// Retrieves all domain contacts of a specific type for a domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <param name="contactType">The contact type (Registrant, Admin, Technical, Billing)</param>
    /// <returns>List of domain contacts matching the criteria</returns>
    /// <response code="200">Returns the list of domain contacts matching the criteria</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("domain/{domainId}/type/{contactType}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<DomainContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DomainContactDto>>> GetDomainContactsByType(int domainId, string contactType)
    {
        try
        {
            _log.Information("API: GetDomainContactsByType called for domain ID {DomainId} with type {ContactType} by user {User}",
                domainId, contactType, User.Identity?.Name);

            var domainContacts = await _domainContactService.GetDomainContactsByTypeAsync(domainId, contactType);
            return Ok(domainContacts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainContactsByType for domain ID {DomainId} with type {ContactType}", domainId, contactType);
            return StatusCode(500, "An error occurred while retrieving domain contacts");
        }
    }

    /// <summary>
    /// Retrieves a specific domain contact by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the domain contact</param>
    /// <returns>The domain contact information</returns>
    /// <response code="200">Returns the domain contact data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain contact is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(DomainContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainContactDto>> GetDomainContactById(int id)
    {
        try
        {
            _log.Information("API: GetDomainContactById called for ID {DomainContactId} by user {User}", id, User.Identity?.Name);

            var domainContact = await _domainContactService.GetDomainContactByIdAsync(id);

            if (domainContact == null)
            {
                _log.Information("API: Domain contact with ID {DomainContactId} not found", id);
                return NotFound($"Domain contact with ID {id} not found");
            }

            return Ok(domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainContactById for ID {DomainContactId}", id);
            return StatusCode(500, "An error occurred while retrieving the domain contact");
        }
    }

    /// <summary>
    /// Creates a new domain contact in the system
    /// </summary>
    /// <param name="createDto">Domain contact information for creation</param>
    /// <returns>The newly created domain contact</returns>
    /// <response code="201">Returns the newly created domain contact</response>
    /// <response code="400">If the domain contact data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(DomainContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainContactDto>> CreateDomainContact([FromBody] CreateDomainContactDto createDto)
    {
        try
        {
            _log.Information("API: CreateDomainContact called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateDomainContact");
                return BadRequest(ModelState);
            }

            var domainContact = await _domainContactService.CreateDomainContactAsync(createDto);

            return CreatedAtAction(
                nameof(GetDomainContactById),
                new { id = domainContact.Id },
                domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDomainContact");
            return StatusCode(500, "An error occurred while creating the domain contact");
        }
    }

    /// <summary>
    /// Updates an existing domain contact's information
    /// </summary>
    /// <param name="id">The unique identifier of the domain contact to update</param>
    /// <param name="updateDto">Updated domain contact information</param>
    /// <returns>The updated domain contact</returns>
    /// <response code="200">Returns the updated domain contact</response>
    /// <response code="400">If the domain contact data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain contact is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(DomainContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainContactDto>> UpdateDomainContact(int id, [FromBody] UpdateDomainContactDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateDomainContact called for ID {DomainContactId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateDomainContact");
                return BadRequest(ModelState);
            }

            var domainContact = await _domainContactService.UpdateDomainContactAsync(id, updateDto);

            if (domainContact == null)
            {
                _log.Information("API: Domain contact with ID {DomainContactId} not found for update", id);
                return NotFound($"Domain contact with ID {id} not found");
            }

            return Ok(domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDomainContact for ID {DomainContactId}", id);
            return StatusCode(500, "An error occurred while updating the domain contact");
        }
    }

    /// <summary>
    /// Deletes a domain contact from the system
    /// </summary>
    /// <param name="id">The unique identifier of the domain contact to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If domain contact was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If domain contact is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Domain.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDomainContact(int id)
    {
        try
        {
            _log.Information("API: DeleteDomainContact called for ID {DomainContactId} by user {User}", id, User.Identity?.Name);

            var deleted = await _domainContactService.DeleteDomainContactAsync(id);

            if (!deleted)
            {
                _log.Information("API: Domain contact with ID {DomainContactId} not found for deletion", id);
                return NotFound($"Domain contact with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDomainContact for ID {DomainContactId}", id);
            return StatusCode(500, "An error occurred while deleting the domain contact");
        }
    }
}
