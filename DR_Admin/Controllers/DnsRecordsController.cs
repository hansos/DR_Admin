using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages DNS records for domains
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsRecordsController : ControllerBase
{
    private readonly IDnsRecordService _dnsRecordService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordsController>();

    public DnsRecordsController(IDnsRecordService dnsRecordService)
    {
        _dnsRecordService = dnsRecordService;
    }

    /// <summary>
    /// Retrieves all DNS records in the system
    /// </summary>
    /// <returns>List of all DNS records</returns>
    /// <response code="200">Returns the list of DNS records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsRecordDto>>> GetAllDnsRecords()
    {
        try
        {
            _log.Information("API: GetAllDnsRecords called by user {User}", User.Identity?.Name);
            
            var dnsRecords = await _dnsRecordService.GetAllDnsRecordsAsync();
            return Ok(dnsRecords);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDnsRecords");
            return StatusCode(500, "An error occurred while retrieving DNS records");
        }
    }

    /// <summary>
    /// Retrieves a specific DNS record by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record</param>
    /// <returns>The DNS record information</returns>
    /// <response code="200">Returns the DNS record data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "DnsRecord.ReadOwn")]
    [ProducesResponseType(typeof(DnsRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsRecordDto>> GetDnsRecordById(int id)
    {
        try
        {
            _log.Information("API: GetDnsRecordById called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);
            
            var dnsRecord = await _dnsRecordService.GetDnsRecordByIdAsync(id);

            if (dnsRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return Ok(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsRecordById for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS record");
        }
    }

    /// <summary>
    /// Retrieves all DNS records for a specific domain
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>List of DNS records for the domain</returns>
    /// <response code="200">Returns the list of DNS records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("domain/{domainId}")]
    [Authorize(Policy = "DnsRecord.ReadOwn")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsRecordDto>>> GetDnsRecordsByDomainId(int domainId)
    {
        try
        {
            _log.Information("API: GetDnsRecordsByDomainId called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);
            
            var dnsRecords = await _dnsRecordService.GetDnsRecordsByDomainIdAsync(domainId);
            return Ok(dnsRecords);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsRecordsByDomainId for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while retrieving DNS records for the domain");
        }
    }

    /// <summary>
    /// Retrieves all DNS records of a specific type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)
    /// </summary>
    /// <param name="type">The DNS record type (e.g., "A", "CNAME", "MX")</param>
    /// <returns>List of DNS records of the specified type</returns>
    /// <response code="200">Returns the list of DNS records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("type/{type}")]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsRecordDto>>> GetDnsRecordsByType(string type)
    {
        try
        {
            _log.Information("API: GetDnsRecordsByType called for type {Type} by user {User}", type, User.Identity?.Name);
            
            var dnsRecords = await _dnsRecordService.GetDnsRecordsByTypeAsync(type);
            return Ok(dnsRecords);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsRecordsByType for type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving DNS records by type");
        }
    }

    /// <summary>
    /// Creates a new DNS record for a domain
    /// </summary>
    /// <param name="createDto">DNS record information for creation</param>
    /// <returns>The newly created DNS record</returns>
    /// <response code="201">Returns the newly created DNS record</response>
    /// <response code="400">If the DNS record data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "DnsRecord.WriteOwn")]
    [ProducesResponseType(typeof(DnsRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsRecordDto>> CreateDnsRecord([FromBody] CreateDnsRecordDto createDto)
    {
        try
        {
            _log.Information("API: CreateDnsRecord called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateDnsRecord");
                return BadRequest(ModelState);
            }

            var dnsRecord = await _dnsRecordService.CreateDnsRecordAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetDnsRecordById),
                new { id = dnsRecord.Id },
                dnsRecord);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateDnsRecord");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDnsRecord");
            return StatusCode(500, "An error occurred while creating the DNS record");
        }
    }

    /// <summary>
    /// Update an existing DNS record
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Support,Customer")]
    public async Task<ActionResult<DnsRecordDto>> UpdateDnsRecord(int id, [FromBody] UpdateDnsRecordDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateDnsRecord called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateDnsRecord");
                return BadRequest(ModelState);
            }

            // Check if record exists and is editable by user
            var existingRecord = await _dnsRecordService.GetDnsRecordByIdAsync(id);
            if (existingRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for update", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            // Only Admin and Support can edit non-editable records
            var userRoles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdminOrSupport = userRoles.Contains("Admin") || userRoles.Contains("Support");

            if (!existingRecord.IsEditableByUser && !isAdminOrSupport)
            {
                _log.Warning("API: User {User} attempted to edit system-managed DNS record {DnsRecordId}", User.Identity?.Name, id);
                return StatusCode(403, "This DNS record type is system-managed and cannot be edited by regular users");
            }

            var dnsRecord = await _dnsRecordService.UpdateDnsRecordAsync(id, updateDto);

            if (dnsRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for update", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return Ok(dnsRecord);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateDnsRecord for ID {DnsRecordId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while updating the DNS record");
        }
    }

    /// <summary>
    /// Delete a DNS record
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult> DeleteDnsRecord(int id)
    {
        try
        {
            _log.Information("API: DeleteDnsRecord called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsRecordService.DeleteDnsRecordAsync(id);

            if (!result)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for deletion", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while deleting the DNS record");
        }
    }
}
