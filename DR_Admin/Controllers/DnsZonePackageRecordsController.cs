using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages DNS zone package records including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsZonePackageRecordsController : ControllerBase
{
    private readonly IDnsZonePackageRecordService _dnsZonePackageRecordService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsZonePackageRecordsController>();

    public DnsZonePackageRecordsController(IDnsZonePackageRecordService dnsZonePackageRecordService)
    {
        _dnsZonePackageRecordService = dnsZonePackageRecordService;
    }

    /// <summary>
    /// Retrieves all DNS zone package records in the system
    /// </summary>
    /// <returns>List of all DNS zone package records</returns>
    /// <response code="200">Returns the list of DNS zone package records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageRecordDto>>> GetAllDnsZonePackageRecords()
    {
        try
        {
            _log.Information("API: GetAllDnsZonePackageRecords called by user {User}", User.Identity?.Name);
            
            var records = await _dnsZonePackageRecordService.GetAllDnsZonePackageRecordsAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDnsZonePackageRecords");
            return StatusCode(500, "An error occurred while retrieving DNS zone package records");
        }
    }

    /// <summary>
    /// Retrieves DNS zone package records for a specific package
    /// </summary>
    /// <param name="packageId">The unique identifier of the DNS zone package</param>
    /// <returns>List of DNS zone package records</returns>
    /// <response code="200">Returns the list of DNS zone package records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("package/{packageId}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageRecordDto>>> GetRecordsByPackageId(int packageId)
    {
        try
        {
            _log.Information("API: GetRecordsByPackageId called for package ID {PackageId} by user {User}", 
                packageId, User.Identity?.Name);
            
            var records = await _dnsZonePackageRecordService.GetRecordsByPackageIdAsync(packageId);
            return Ok(records);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRecordsByPackageId for package ID {PackageId}", packageId);
            return StatusCode(500, "An error occurred while retrieving DNS zone package records");
        }
    }

    /// <summary>
    /// Retrieves a specific DNS zone package record by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package record</param>
    /// <returns>The DNS zone package record information</returns>
    /// <response code="200">Returns the DNS zone package record data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS zone package record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(DnsZonePackageRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageRecordDto>> GetDnsZonePackageRecordById(int id)
    {
        try
        {
            _log.Information("API: GetDnsZonePackageRecordById called for ID {RecordId} by user {User}", id, User.Identity?.Name);
            
            var record = await _dnsZonePackageRecordService.GetDnsZonePackageRecordByIdAsync(id);

            if (record == null)
            {
                _log.Information("API: DNS zone package record with ID {RecordId} not found", id);
                return NotFound($"DNS zone package record with ID {id} not found");
            }

            return Ok(record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsZonePackageRecordById for ID {RecordId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS zone package record");
        }
    }

    /// <summary>
    /// Creates a new DNS zone package record in the system
    /// </summary>
    /// <param name="createDto">DNS zone package record information for creation</param>
    /// <returns>The newly created DNS zone package record</returns>
    /// <response code="201">Returns the newly created DNS zone package record</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DnsZonePackageRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageRecordDto>> CreateDnsZonePackageRecord([FromBody] CreateDnsZonePackageRecordDto createDto)
    {
        try
        {
            _log.Information("API: CreateDnsZonePackageRecord called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateDnsZonePackageRecord");
                return BadRequest(ModelState);
            }

            var record = await _dnsZonePackageRecordService.CreateDnsZonePackageRecordAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetDnsZonePackageRecordById),
                new { id = record.Id },
                record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDnsZonePackageRecord");
            return StatusCode(500, "An error occurred while creating the DNS zone package record");
        }
    }

    /// <summary>
    /// Updates an existing DNS zone package record's information
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package record to update</param>
    /// <param name="updateDto">Updated DNS zone package record information</param>
    /// <returns>The updated DNS zone package record</returns>
    /// <response code="200">Returns the updated DNS zone package record</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If DNS zone package record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DnsZonePackageRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageRecordDto>> UpdateDnsZonePackageRecord(int id, [FromBody] UpdateDnsZonePackageRecordDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateDnsZonePackageRecord called for ID {RecordId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateDnsZonePackageRecord");
                return BadRequest(ModelState);
            }

            var record = await _dnsZonePackageRecordService.UpdateDnsZonePackageRecordAsync(id, updateDto);

            if (record == null)
            {
                _log.Information("API: DNS zone package record with ID {RecordId} not found for update", id);
                return NotFound($"DNS zone package record with ID {id} not found");
            }

            return Ok(record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDnsZonePackageRecord for ID {RecordId}", id);
            return StatusCode(500, "An error occurred while updating the DNS zone package record");
        }
    }

    /// <summary>
    /// Deletes a DNS zone package record from the system
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package record to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If DNS zone package record was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If DNS zone package record is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDnsZonePackageRecord(int id)
    {
        try
        {
            _log.Information("API: DeleteDnsZonePackageRecord called for ID {RecordId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsZonePackageRecordService.DeleteDnsZonePackageRecordAsync(id);

            if (!result)
            {
                _log.Information("API: DNS zone package record with ID {RecordId} not found for deletion", id);
                return NotFound($"DNS zone package record with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDnsZonePackageRecord for ID {RecordId}", id);
            return StatusCode(500, "An error occurred while deleting the DNS zone package record");
        }
    }
}
