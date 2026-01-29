using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages services offered to customers including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServicesController>();

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    /// <summary>
    /// Retrieves all services in the system
    /// </summary>
    /// <returns>List of all services</returns>
    /// <response code="200">Returns the list of services</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Service.Read")]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllServices()
    {
        try
        {
            _log.Information("API: GetAllServices called by user {User}", User.Identity?.Name);
            
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllServices");
            return StatusCode(500, "An error occurred while retrieving services");
        }
    }

    /// <summary>
    /// Retrieves a specific service by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the service</param>
    /// <returns>The service information</returns>
    /// <response code="200">Returns the service data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If service is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Service.Read")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceDto>> GetServiceById(int id)
    {
        try
        {
            _log.Information("API: GetServiceById called for ID {ServiceId} by user {User}", id, User.Identity?.Name);
            
            var service = await _serviceService.GetServiceByIdAsync(id);

            if (service == null)
            {
                _log.Information("API: Service with ID {ServiceId} not found", id);
                return NotFound($"Service with ID {id} not found");
            }

            return Ok(service);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetServiceById for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while retrieving the service");
        }
    }

    /// <summary>
    /// Creates a new service in the system
    /// </summary>
    /// <param name="createDto">Service information for creation</param>
    /// <returns>The newly created service</returns>
    /// <response code="201">Returns the newly created service</response>
    /// <response code="400">If the service data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Service.Write")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto createDto)
    {
        try
        {
            _log.Information("API: CreateService called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateService");
                return BadRequest(ModelState);
            }

            var service = await _serviceService.CreateServiceAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetServiceById),
                new { id = service.Id },
                service);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateService");
            return StatusCode(500, "An error occurred while creating the service");
        }
    }

    /// <summary>
    /// Updates an existing service's information
    /// </summary>
    /// <param name="id">The unique identifier of the service to update</param>
    /// <param name="updateDto">Updated service information</param>
    /// <returns>The updated service</returns>
    /// <response code="200">Returns the updated service</response>
    /// <response code="400">If the service data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If service is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Service.Write")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceDto>> UpdateService(int id, [FromBody] UpdateServiceDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateService called for ID {ServiceId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateService");
                return BadRequest(ModelState);
            }

            var service = await _serviceService.UpdateServiceAsync(id, updateDto);

            if (service == null)
            {
                _log.Information("API: Service with ID {ServiceId} not found for update", id);
                return NotFound($"Service with ID {id} not found");
            }

            return Ok(service);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateService for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while updating the service");
        }
    }

    /// <summary>
    /// Deletes a service from the system
    /// </summary>
    /// <param name="id">The unique identifier of the service to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If service was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If service is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Service.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteService(int id)
    {
        try
        {
            _log.Information("API: DeleteService called for ID {ServiceId} by user {User}", id, User.Identity?.Name);

            var result = await _serviceService.DeleteServiceAsync(id);

            if (!result)
            {
                _log.Information("API: Service with ID {ServiceId} not found for deletion", id);
                return NotFound($"Service with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteService for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while deleting the service");
        }
    }
}
