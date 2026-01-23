using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

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
    /// Get all DNS records
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
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
    /// Get DNS record by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Customer")]
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
    /// Get DNS records by domain ID
    /// </summary>
    [HttpGet("domain/{domainId}")]
    [Authorize(Roles = "Admin,Support,Customer")]
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
    /// Get DNS records by type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)
    /// </summary>
    [HttpGet("type/{type}")]
    [Authorize(Roles = "Admin,Support")]
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
    /// Create a new DNS record
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Support,Customer")]
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
                return Forbid("This DNS record is system-managed and cannot be edited by regular users");
            }

            // Regular users cannot change the IsEditableByUser flag
            if (!isAdminOrSupport && updateDto.IsEditableByUser != existingRecord.IsEditableByUser)
            {
                _log.Warning("API: User {User} attempted to change IsEditableByUser flag for DNS record {DnsRecordId}", User.Identity?.Name, id);
                updateDto.IsEditableByUser = existingRecord.IsEditableByUser; // Restore original value
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
