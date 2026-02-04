using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages address type information including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AddressTypesController : ControllerBase
{
    private readonly IAddressTypeService _addressTypeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<AddressTypesController>();

    public AddressTypesController(IAddressTypeService addressTypeService)
    {
        _addressTypeService = addressTypeService;
    }

    /// <summary>
    /// Retrieves all address types in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all address types or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of address types or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "AddressType.Read")]
    [ProducesResponseType(typeof(IEnumerable<AddressTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<AddressTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllAddressTypes([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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

                _log.Information("API: GetAllAddressTypes (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _addressTypeService.GetAllAddressTypesPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            _log.Information("API: GetAllAddressTypes called by user {User}", User.Identity?.Name);
            
            var addressTypes = await _addressTypeService.GetAllAddressTypesAsync();
            return Ok(addressTypes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllAddressTypes");
            return StatusCode(500, "An error occurred while retrieving address types");
        }
    }

    /// <summary>
    /// Retrieves a specific address type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the address type</param>
    /// <returns>The address type information</returns>
    /// <response code="200">Returns the address type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If address type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "AddressType.Read")]
    [ProducesResponseType(typeof(AddressTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddressTypeDto>> GetAddressTypeById(int id)
    {
        try
        {
            _log.Information("API: GetAddressTypeById called for ID {AddressTypeId} by user {User}", id, User.Identity?.Name);
            
            var addressType = await _addressTypeService.GetAddressTypeByIdAsync(id);

            if (addressType == null)
            {
                _log.Information("API: Address type with ID {AddressTypeId} not found", id);
                return NotFound($"Address type with ID {id} not found");
            }

            return Ok(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAddressTypeById for ID {AddressTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the address type");
        }
    }

    /// <summary>
    /// Creates a new address type in the system
    /// </summary>
    /// <param name="createDto">Address type information for creation</param>
    /// <returns>The newly created address type</returns>
    /// <response code="201">Returns the newly created address type</response>
    /// <response code="400">If the address type data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "AddressType.Write")]
    [ProducesResponseType(typeof(AddressTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddressTypeDto>> CreateAddressType([FromBody] CreateAddressTypeDto createDto)
    {
        try
        {
            _log.Information("API: CreateAddressType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateAddressType");
                return BadRequest(ModelState);
            }

            var addressType = await _addressTypeService.CreateAddressTypeAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetAddressTypeById),
                new { id = addressType.Id },
                addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateAddressType");
            return StatusCode(500, "An error occurred while creating the address type");
        }
    }

    /// <summary>
    /// Updates an existing address type
    /// </summary>
    /// <param name="id">The unique identifier of the address type to update</param>
    /// <param name="updateDto">Updated address type information</param>
    /// <returns>The updated address type</returns>
    /// <response code="200">Returns the updated address type</response>
    /// <response code="400">If the address type data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If address type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "AddressType.Write")]
    [ProducesResponseType(typeof(AddressTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddressTypeDto>> UpdateAddressType(int id, [FromBody] UpdateAddressTypeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateAddressType called for ID {AddressTypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateAddressType");
                return BadRequest(ModelState);
            }

            var addressType = await _addressTypeService.UpdateAddressTypeAsync(id, updateDto);

            if (addressType == null)
            {
                _log.Information("API: Address type with ID {AddressTypeId} not found for update", id);
                return NotFound($"Address type with ID {id} not found");
            }

            return Ok(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateAddressType for ID {AddressTypeId}", id);
            return StatusCode(500, "An error occurred while updating the address type");
        }
    }

    /// <summary>
    /// Deletes an address type from the system
    /// </summary>
    /// <param name="id">The unique identifier of the address type to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the address type was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If address type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AddressType.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAddressType(int id)
    {
        try
        {
            _log.Information("API: DeleteAddressType called for ID {AddressTypeId} by user {User}", id, User.Identity?.Name);

            var result = await _addressTypeService.DeleteAddressTypeAsync(id);

            if (!result)
            {
                _log.Information("API: Address type with ID {AddressTypeId} not found for deletion", id);
                return NotFound($"Address type with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteAddressType for ID {AddressTypeId}", id);
            return StatusCode(500, "An error occurred while deleting the address type");
        }
    }
}
