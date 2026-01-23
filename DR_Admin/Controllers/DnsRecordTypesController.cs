using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsRecordTypesController : ControllerBase
{
    private readonly IDnsRecordTypeService _dnsRecordTypeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordTypesController>();

    public DnsRecordTypesController(IDnsRecordTypeService dnsRecordTypeService)
    {
        _dnsRecordTypeService = dnsRecordTypeService;
    }

    /// <summary>
    /// Get all DNS record types
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Customer")]
    public async Task<ActionResult<IEnumerable<DnsRecordTypeDto>>> GetAllDnsRecordTypes()
    {
        try
        {
            _log.Information("API: GetAllDnsRecordTypes called by user {User}", User.Identity?.Name);
            
            var dnsRecordTypes = await _dnsRecordTypeService.GetAllDnsRecordTypesAsync();
            return Ok(dnsRecordTypes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDnsRecordTypes");
            return StatusCode(500, "An error occurred while retrieving DNS record types");
        }
    }

    /// <summary>
    /// Get active DNS record types (for client use)
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Support,Customer")]
    public async Task<ActionResult<IEnumerable<DnsRecordTypeDto>>> GetActiveDnsRecordTypes()
    {
        try
        {
            _log.Information("API: GetActiveDnsRecordTypes called by user {User}", User.Identity?.Name);
            
            var dnsRecordTypes = await _dnsRecordTypeService.GetActiveDnsRecordTypesAsync();
            return Ok(dnsRecordTypes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveDnsRecordTypes");
            return StatusCode(500, "An error occurred while retrieving active DNS record types");
        }
    }

    /// <summary>
    /// Get DNS record type by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<DnsRecordTypeDto>> GetDnsRecordTypeById(int id)
    {
        try
        {
            _log.Information("API: GetDnsRecordTypeById called for ID {DnsRecordTypeId} by user {User}", id, User.Identity?.Name);
            
            var dnsRecordType = await _dnsRecordTypeService.GetDnsRecordTypeByIdAsync(id);

            if (dnsRecordType == null)
            {
                _log.Information("API: DNS record type with ID {DnsRecordTypeId} not found", id);
                return NotFound($"DNS record type with ID {id} not found");
            }

            return Ok(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsRecordTypeById for ID {DnsRecordTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS record type");
        }
    }

    /// <summary>
    /// Get DNS record type by type name (e.g., A, AAAA, CNAME)
    /// </summary>
    [HttpGet("type/{type}")]
    [Authorize(Roles = "Admin,Support,Customer")]
    public async Task<ActionResult<DnsRecordTypeDto>> GetDnsRecordTypeByType(string type)
    {
        try
        {
            _log.Information("API: GetDnsRecordTypeByType called for type {Type} by user {User}", type, User.Identity?.Name);
            
            var dnsRecordType = await _dnsRecordTypeService.GetDnsRecordTypeByTypeAsync(type);

            if (dnsRecordType == null)
            {
                _log.Information("API: DNS record type {Type} not found", type);
                return NotFound($"DNS record type '{type}' not found");
            }

            return Ok(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsRecordTypeByType for type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving the DNS record type");
        }
    }

    /// <summary>
    /// Create a new DNS record type
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DnsRecordTypeDto>> CreateDnsRecordType([FromBody] CreateDnsRecordTypeDto createDto)
    {
        try
        {
            _log.Information("API: CreateDnsRecordType called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateDnsRecordType");
                return BadRequest(ModelState);
            }

            var dnsRecordType = await _dnsRecordTypeService.CreateDnsRecordTypeAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetDnsRecordTypeById),
                new { id = dnsRecordType.Id },
                dnsRecordType);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateDnsRecordType");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDnsRecordType");
            return StatusCode(500, "An error occurred while creating the DNS record type");
        }
    }

    /// <summary>
    /// Update an existing DNS record type
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DnsRecordTypeDto>> UpdateDnsRecordType(int id, [FromBody] UpdateDnsRecordTypeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateDnsRecordType called for ID {DnsRecordTypeId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateDnsRecordType");
                return BadRequest(ModelState);
            }

            var dnsRecordType = await _dnsRecordTypeService.UpdateDnsRecordTypeAsync(id, updateDto);

            if (dnsRecordType == null)
            {
                _log.Information("API: DNS record type with ID {DnsRecordTypeId} not found for update", id);
                return NotFound($"DNS record type with ID {id} not found");
            }

            return Ok(dnsRecordType);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateDnsRecordType for ID {DnsRecordTypeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDnsRecordType for ID {DnsRecordTypeId}", id);
            return StatusCode(500, "An error occurred while updating the DNS record type");
        }
    }

    /// <summary>
    /// Delete a DNS record type
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteDnsRecordType(int id)
    {
        try
        {
            _log.Information("API: DeleteDnsRecordType called for ID {DnsRecordTypeId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsRecordTypeService.DeleteDnsRecordTypeAsync(id);

            if (!result)
            {
                _log.Information("API: DNS record type with ID {DnsRecordTypeId} not found for deletion", id);
                return NotFound($"DNS record type with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteDnsRecordType for ID {DnsRecordTypeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDnsRecordType for ID {DnsRecordTypeId}", id);
            return StatusCode(500, "An error occurred while deleting the DNS record type");
        }
    }
}
