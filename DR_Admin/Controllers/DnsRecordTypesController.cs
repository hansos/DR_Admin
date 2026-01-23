using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages DNS record types (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)
/// </summary>
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
    /// Retrieves all DNS record types in the system
    /// </summary>
    /// <returns>List of all DNS record types</returns>
    /// <response code="200">Returns the list of DNS record types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support,Customer")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves only active DNS record types available for customer use
    /// </summary>
    /// <returns>List of active DNS record types</returns>
    /// <response code="200">Returns the list of active DNS record types</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Support,Customer")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves a specific DNS record type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record type</param>
    /// <returns>The DNS record type information</returns>
    /// <response code="200">Returns the DNS record type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS record type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(DnsRecordTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves a specific DNS record type by its type name (e.g., A, AAAA, CNAME, MX, TXT)
    /// </summary>
    /// <param name="type">The DNS record type name (e.g., "A", "CNAME", "MX")</param>
    /// <returns>The DNS record type information</returns>
    /// <response code="200">Returns the DNS record type data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS record type is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("type/{type}")]
    [Authorize(Roles = "Admin,Support,Customer")]
    [ProducesResponseType(typeof(DnsRecordTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
