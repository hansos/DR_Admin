using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServiceTypesController : ControllerBase
{
    private readonly IServiceTypeService _serviceTypeService;
    private readonly Serilog.ILogger _logger;

    public ServiceTypesController(IServiceTypeService serviceTypeService, Serilog.ILogger logger)
    {
        _serviceTypeService = serviceTypeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all service types
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<ServiceTypeDto>>> GetAllServiceTypes()
    {
        try
        {
            _logger.Information("API: GetAllServiceTypes called by user {User}", User.Identity?.Name);
            
            var serviceTypes = await _serviceTypeService.GetAllServiceTypesAsync();
            return Ok(serviceTypes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetAllServiceTypes");
            return StatusCode(500, "An error occurred while retrieving service types");
        }
    }

    /// <summary>
    /// Get service type by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<ServiceTypeDto>> GetServiceTypeById(int id)
    {
        try
        {
            _logger.Information("API: GetServiceTypeById called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);
            
            var serviceType = await _serviceTypeService.GetServiceTypeByIdAsync(id);

            if (serviceType == null)
            {
                _logger.Information("API: Service type with ID {ServiceTypeId} not found", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return Ok(serviceType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetServiceTypeById for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the service type");
        }
    }

    /// <summary>
    /// Create a new service type
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceTypeDto>> CreateServiceType([FromBody] CreateServiceTypeDto createDto)
    {
        try
        {
            _logger.Information("API: CreateServiceType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for CreateServiceType");
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
            _logger.Error(ex, "API: Error in CreateServiceType");
            return StatusCode(500, "An error occurred while creating the service type");
        }
    }

    /// <summary>
    /// Update an existing service type
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceTypeDto>> UpdateServiceType(int id, [FromBody] UpdateServiceTypeDto updateDto)
    {
        try
        {
            _logger.Information("API: UpdateServiceType called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for UpdateServiceType");
                return BadRequest(ModelState);
            }

            var serviceType = await _serviceTypeService.UpdateServiceTypeAsync(id, updateDto);

            if (serviceType == null)
            {
                _logger.Information("API: Service type with ID {ServiceTypeId} not found for update", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return Ok(serviceType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in UpdateServiceType for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while updating the service type");
        }
    }

    /// <summary>
    /// Delete a service type
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteServiceType(int id)
    {
        try
        {
            _logger.Information("API: DeleteServiceType called for ID {ServiceTypeId} by user {User}", id, User.Identity?.Name);

            var result = await _serviceTypeService.DeleteServiceTypeAsync(id);

            if (!result)
            {
                _logger.Information("API: Service type with ID {ServiceTypeId} not found for deletion", id);
                return NotFound($"Service type with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in DeleteServiceType for ID {ServiceTypeId}", id);
            return StatusCode(500, "An error occurred while deleting the service type");
        }
    }
}
