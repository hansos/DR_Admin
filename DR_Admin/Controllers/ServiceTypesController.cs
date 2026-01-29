using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages service types including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServiceTypesController : ControllerBase
{
    private readonly IServiceTypeService _serviceTypeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServiceTypesController>();

    public ServiceTypesController(IServiceTypeService serviceTypeService)
    {
        _serviceTypeService = serviceTypeService;
    }

    /// <summary>
    /// Retrieves all service types in the system
    /// </summary>
    /// <returns>List of all service types</returns>
    /// <response code="200">Returns the list of service types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "ServiceType.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServiceTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServiceTypeDto>>> GetAllServiceTypes()
    {
        try
        {
            _log.Information("API: GetAllServiceTypes called by user {User}", User.Identity?.Name);
            
            var serviceTypes = await _serviceTypeService.GetAllServiceTypesAsync();
            return Ok(serviceTypes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServiceTypes");
            return StatusCode(500, "An error occurred while retrieving service types");
        }
    }

    /// <summary>
    /// Retrieves a specific service type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the service type</param>
    /// <returns>The service type information</returns>
    /// <response code="200">Returns the service type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If service type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "ServiceType.Read")]
    [ProducesResponseType(typeof(ServiceTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceTypeDto>> GetServiceTypeById(int id)
    {
        try
        {
            _log.Information("API: GetServiceTypeById called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);
            
            var serviceType = await _serviceTypeService.GetServiceTypeByIdAsync(id);

            if (serviceType == null)
            {
                _log.Information("API: Service type with ID {ServiceTypeId} not found", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return Ok(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServiceTypeById for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the service type");
        }
    }

    /// <summary>
    /// Creates a new service type in the system
    /// </summary>
    /// <param name="createDto">Service type information for creation</param>
    /// <returns>The newly created service type</returns>
    /// <response code="201">Returns the newly created service type</response>
    /// <response code="400">If the service type data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "ServiceType.Write")]
    [ProducesResponseType(typeof(ServiceTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceTypeDto>> CreateServiceType([FromBody] CreateServiceTypeDto createDto)
    {
        try
        {
            _log.Information("API: CreateServiceType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateServiceType");
                return BadRequest(ModelState);
            }

            var serviceType = await _serviceTypeService.CreateServiceTypeAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetServiceTypeById),
                new { id = serviceType.Id },
                serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateServiceType");
            return StatusCode(500, "An error occurred while creating the service type");
        }
    }

    /// <summary>
    /// Updates an existing service type's information
    /// </summary>
    /// <param name="id">The unique identifier of the service type to update</param>
    /// <param name="updateDto">Updated service type information</param>
    /// <returns>The updated service type</returns>
    /// <response code="200">Returns the updated service type</response>
    /// <response code="400">If the service type data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If service type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceTypeDto>> UpdateServiceType(int id, [FromBody] UpdateServiceTypeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateServiceType called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateServiceType");
                return BadRequest(ModelState);
            }

            var serviceType = await _serviceTypeService.UpdateServiceTypeAsync(id, updateDto);

            if (serviceType == null)
            {
                _log.Information("API: Service type with ID {ServiceTypeId} not found for update", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return Ok(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateServiceType for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while updating the service type");
        }
    }

    /// <summary>
    /// Deletes a service type from the system
    /// </summary>
    /// <param name="id">The unique identifier of the service type to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If service type was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If service type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteServiceType(int id)
    {
        try
        {
            _log.Information("API: DeleteServiceType called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);

            var result = await _serviceTypeService.DeleteServiceTypeAsync(id);

            if (!result)
            {
                _log.Information("API: Service type with ID {ServiceTypeId} not found for deletion", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteServiceType for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while deleting the service type");
        }
    }
}
