using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly Serilog.ILogger _logger;

    public ServicesController(IServiceService serviceService, Serilog.ILogger logger)
    {
        _serviceService = serviceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all services
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllServices()
    {
        try
        {
            _logger.Information("API: GetAllServices called by user {User}", User.Identity?.Name);
            
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetAllServices");
            return StatusCode(500, "An error occurred while retrieving services");
        }
    }

    /// <summary>
    /// Get service by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<ServiceDto>> GetServiceById(int id)
    {
        try
        {
            _logger.Information("API: GetServiceById called for ID {ServiceId} by user {User}", id, User.Identity?.Name);
            
            var service = await _serviceService.GetServiceByIdAsync(id);

            if (service == null)
            {
                _logger.Information("API: Service with ID {ServiceId} not found", id);
                return NotFound($"Service with ID {id} not found");
            }

            return Ok(service);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetServiceById for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while retrieving the service");
        }
    }

    /// <summary>
    /// Create a new service
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto createDto)
    {
        try
        {
            _logger.Information("API: CreateService called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for CreateService");
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
            _logger.Error(ex, "API: Error in CreateService");
            return StatusCode(500, "An error occurred while creating the service");
        }
    }

    /// <summary>
    /// Update an existing service
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> UpdateService(int id, [FromBody] UpdateServiceDto updateDto)
    {
        try
        {
            _logger.Information("API: UpdateService called for ID {ServiceId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for UpdateService");
                return BadRequest(ModelState);
            }

            var service = await _serviceService.UpdateServiceAsync(id, updateDto);

            if (service == null)
            {
                _logger.Information("API: Service with ID {ServiceId} not found for update", id);
                return NotFound($"Service with ID {id} not found");
            }

            return Ok(service);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in UpdateService for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while updating the service");
        }
    }

    /// <summary>
    /// Delete a service
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteService(int id)
    {
        try
        {
            _logger.Information("API: DeleteService called for ID {ServiceId} by user {User}", id, User.Identity?.Name);

            var result = await _serviceService.DeleteServiceAsync(id);

            if (!result)
            {
                _logger.Information("API: Service with ID {ServiceId} not found for deletion", id);
                return NotFound($"Service with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in DeleteService for ID {ServiceId}", id);
            return StatusCode(500, "An error occurred while deleting the service");
        }
    }
}
