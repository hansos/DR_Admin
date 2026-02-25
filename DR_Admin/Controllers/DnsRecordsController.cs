using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages DNS records for domains.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsRecordsController : ControllerBase
{
    private readonly IDnsRecordService _dnsRecordService;
    private readonly IDomainManagerService _domainManagerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordsController>();

    public DnsRecordsController(IDnsRecordService dnsRecordService, IDomainManagerService domainManagerService)
    {
        _dnsRecordService = dnsRecordService;
        _domainManagerService = domainManagerService;
    }

    /// <summary>
    /// Retrieves all non-deleted DNS records in the system.
    /// </summary>
    /// <param name="pageNumber">Optional page number for pagination (default: returns all).</param>
    /// <param name="pageSize">Optional number of items per page (default: 10, max: 100).</param>
    /// <returns>List of all DNS records, or a paginated result when pagination parameters are provided.</returns>
    /// <response code="200">Returns the list of DNS records or a paginated result.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllDnsRecords([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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

                _log.Information("API: GetAllDnsRecords (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}",
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _dnsRecordService.GetAllDnsRecordsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

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
    /// Retrieves a specific DNS record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record.</param>
    /// <returns>The DNS record information.</returns>
    /// <response code="200">Returns the DNS record data.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
    /// Retrieves all non-deleted DNS records for a specific domain.
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain.</param>
    /// <returns>List of non-deleted DNS records for the domain.</returns>
    /// <response code="200">Returns the list of DNS records.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
    /// Retrieves all soft-deleted DNS records for a specific domain.
    /// These records are pending hard-deletion once removal is confirmed on the DNS server.
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain.</param>
    /// <returns>List of soft-deleted DNS records for the domain.</returns>
    /// <response code="200">Returns the list of soft-deleted DNS records.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("domain/{domainId}/deleted")]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsRecordDto>>> GetDeletedDnsRecords(int domainId)
    {
        try
        {
            _log.Information("API: GetDeletedDnsRecords called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);

            var dnsRecords = await _dnsRecordService.GetDeletedDnsRecordsAsync(domainId);
            return Ok(dnsRecords);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDeletedDnsRecords for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while retrieving soft-deleted DNS records");
        }
    }

    /// <summary>
    /// Retrieves all DNS records for a specific domain that are flagged as pending synchronisation to the DNS server.
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain.</param>
    /// <returns>List of DNS records pending synchronisation.</returns>
    /// <response code="200">Returns the list of pending-sync DNS records.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("domain/{domainId}/pending-sync")]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsRecordDto>>> GetPendingSyncDnsRecords(int domainId)
    {
        try
        {
            _log.Information("API: GetPendingSyncDnsRecords called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);

            var dnsRecords = await _dnsRecordService.GetPendingSyncRecordsAsync(domainId);
            return Ok(dnsRecords);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPendingSyncDnsRecords for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while retrieving pending-sync DNS records");
        }
    }

    /// <summary>
    /// Retrieves the count of DNS records that are pending synchronisation to the DNS server.
    /// </summary>
    /// <returns>Count of pending-sync DNS records.</returns>
    /// <response code="200">Returns the pending-sync record count.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("pending/count")]
    [Authorize(Policy = "DnsRecord.Read")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPendingSyncCount()
    {
        try
        {
            _log.Information("API: GetPendingSyncCount called by user {User}", User.Identity?.Name);

            var count = await _dnsRecordService.GetPendingSyncCountAsync();
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPendingSyncCount");
            return StatusCode(500, "An error occurred while retrieving pending-sync DNS record count");
        }
    }

    /// <summary>
    /// Retrieves all DNS records of a specific type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.).
    /// </summary>
    /// <param name="type">The DNS record type string (e.g., "A", "CNAME", "MX").</param>
    /// <returns>List of non-deleted DNS records of the specified type.</returns>
    /// <response code="200">Returns the list of DNS records.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
    /// Creates a new DNS record for a domain. The record is flagged as pending synchronisation by default.
    /// </summary>
    /// <param name="createDto">DNS record information for creation.</param>
    /// <returns>The newly created DNS record.</returns>
    /// <response code="201">Returns the newly created DNS record.</response>
    /// <response code="400">If the DNS record data is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
    /// Updates an existing DNS record. The record is automatically marked as pending synchronisation.
    /// System-managed records (IsEditableByUser = false) can only be edited by Admin or Support.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record to update.</param>
    /// <param name="updateDto">Updated DNS record data.</param>
    /// <returns>The updated DNS record.</returns>
    /// <response code="200">Returns the updated DNS record.</response>
    /// <response code="400">If the updated data is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role or the record is system-managed.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "DnsRecord.WriteOwn")]
    [ProducesResponseType(typeof(DnsRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            var existingRecord = await _dnsRecordService.GetDnsRecordByIdAsync(id);
            if (existingRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for update", id);
                return NotFound($"DNS record with ID {id} not found");
            }

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
    /// Soft-deletes a DNS record by flagging it as deleted and pending synchronisation.
    /// The record is retained until hard-deleted after the removal is confirmed on the DNS server.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record to soft-delete.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The DNS record was successfully soft-deleted.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "DnsRecord.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDnsRecord(int id)
    {
        try
        {
            _log.Information("API: DeleteDnsRecord (soft) called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsRecordService.SoftDeleteDnsRecordAsync(id);

            if (!result)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for soft-delete", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while soft-deleting the DNS record");
        }
    }

    /// <summary>
    /// Permanently removes a DNS record from the database.
    /// Use this endpoint only after the deletion has been confirmed on the DNS server.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record to permanently delete.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The DNS record was successfully hard-deleted.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id}/hard")]
    [Authorize(Policy = "DnsRecord.HardDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> HardDeleteDnsRecord(int id)
    {
        try
        {
            _log.Information("API: HardDeleteDnsRecord called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsRecordService.HardDeleteDnsRecordAsync(id);

            if (!result)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for hard-delete", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in HardDeleteDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while hard-deleting the DNS record");
        }
    }

    /// <summary>
    /// Restores a soft-deleted DNS record and marks it as pending synchronisation.
    /// </summary>
    /// <param name="id">The unique identifier of the soft-deleted DNS record to restore.</param>
    /// <returns>The restored DNS record.</returns>
    /// <response code="200">Returns the restored DNS record.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found or is not soft-deleted.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("{id}/restore")]
    [Authorize(Policy = "DnsRecord.Write")]
    [ProducesResponseType(typeof(DnsRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsRecordDto>> RestoreDnsRecord(int id)
    {
        try
        {
            _log.Information("API: RestoreDnsRecord called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var dnsRecord = await _dnsRecordService.RestoreDnsRecordAsync(id);

            if (dnsRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found or is not soft-deleted", id);
                return NotFound($"DNS record with ID {id} not found or is not soft-deleted");
            }

            return Ok(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RestoreDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while restoring the DNS record");
        }
    }

    /// <summary>
    /// Clears the pending-sync flag on a DNS record after it has been successfully pushed to the DNS server.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record to mark as synced.</param>
    /// <returns>The updated DNS record with IsPendingSync = false.</returns>
    /// <response code="200">Returns the updated DNS record.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("{id}/mark-synced")]
    [Authorize(Policy = "DnsRecord.Write")]
    [ProducesResponseType(typeof(DnsRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsRecordDto>> MarkDnsRecordAsSynced(int id)
    {
        try
        {
            _log.Information("API: MarkDnsRecordAsSynced called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var dnsRecord = await _dnsRecordService.MarkAsSyncedAsync(id);

            if (dnsRecord == null)
            {
                _log.Information("API: DNS record with ID {DnsRecordId} not found for mark-as-synced", id);
                return NotFound($"DNS record with ID {id} not found");
            }

            return Ok(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in MarkDnsRecordAsSynced for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while marking the DNS record as synced");
        }
    }

    /// <summary>
    /// Pushes a single DNS record to the registrar's DNS server.
    /// Non-deleted records are upserted (created or updated); soft-deleted records are removed
    /// from the server and permanently deleted locally.
    /// IsPendingSync is cleared on success.
    /// </summary>
    /// <param name="id">The unique identifier of the DNS record to push.</param>
    /// <returns>Result of the push operation.</returns>
    /// <response code="200">Returns the push result.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the DNS record is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("{id}/push")]
    [Authorize(Policy = "DnsRecord.Write")]
    [ProducesResponseType(typeof(DnsPushRecordResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsPushRecordResult>> PushDnsRecord(int id)
    {
        try
        {
            _log.Information("API: PushDnsRecord called for ID {DnsRecordId} by user {User}", id, User.Identity?.Name);

            var result = await _domainManagerService.PushDnsRecordAsync(id);

            if (!result.Success && result.Message.Contains("not found"))
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in PushDnsRecord for ID {DnsRecordId}", id);
            return StatusCode(500, "An error occurred while pushing the DNS record to the registrar");
        }
    }

    /// <summary>
    /// Pushes all pending-sync DNS records for a domain to the registrar's DNS server.
    /// Non-deleted records are upserted; soft-deleted records are removed from the server
    /// and permanently deleted locally.
    /// </summary>
    /// <param name="domainId">The unique identifier of the domain.</param>
    /// <returns>Aggregated push result with per-record details.</returns>
    /// <response code="200">Returns the aggregated push result.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have the required role.</response>
    /// <response code="404">If the domain is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("domain/{domainId}/push-pending")]
    [Authorize(Policy = "DnsRecord.Write")]
    [ProducesResponseType(typeof(DnsPushPendingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsPushPendingResult>> PushPendingSyncDnsRecords(int domainId)
    {
        try
        {
            _log.Information("API: PushPendingSyncDnsRecords called for domain ID {DomainId} by user {User}", domainId, User.Identity?.Name);

            var result = await _domainManagerService.PushPendingSyncRecordsAsync(domainId);

            if (!result.Success && result.Message.Contains("not found"))
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in PushPendingSyncDnsRecords for domain ID {DomainId}", domainId);
            return StatusCode(500, "An error occurred while pushing pending-sync DNS records to the registrar");
        }
    }
}
