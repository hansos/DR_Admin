using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages DNS zone packages including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DnsZonePackagesController : ControllerBase
{
    private readonly IDnsZonePackageService _dnsZonePackageService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsZonePackagesController>();

    public DnsZonePackagesController(IDnsZonePackageService dnsZonePackageService)
    {
        _dnsZonePackageService = dnsZonePackageService;
    }

    /// <summary>
    /// Retrieves all DNS zone packages in the system
    /// </summary>
    /// <returns>List of all DNS zone packages</returns>
    /// <response code="200">Returns the list of DNS zone packages</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageDto>>> GetAllDnsZonePackages()
    {
        try
        {
            _log.Information("API: GetAllDnsZonePackages called by user {User}", User.Identity?.Name);
            
            var packages = await _dnsZonePackageService.GetAllDnsZonePackagesAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDnsZonePackages");
            return StatusCode(500, "An error occurred while retrieving DNS zone packages");
        }
    }

    /// <summary>
    /// Retrieves all DNS zone packages with their records
    /// </summary>
    /// <returns>List of all DNS zone packages including their records</returns>
    /// <response code="200">Returns the list of DNS zone packages with records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("with-records")]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageDto>>> GetAllDnsZonePackagesWithRecords()
    {
        try
        {
            _log.Information("API: GetAllDnsZonePackagesWithRecords called by user {User}", User.Identity?.Name);
            
            var packages = await _dnsZonePackageService.GetAllDnsZonePackagesWithRecordsAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDnsZonePackagesWithRecords");
            return StatusCode(500, "An error occurred while retrieving DNS zone packages with records");
        }
    }

    /// <summary>
    /// Retrieves only active DNS zone packages
    /// </summary>
    /// <returns>List of active DNS zone packages</returns>
    /// <response code="200">Returns the list of active DNS zone packages</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageDto>>> GetActiveDnsZonePackages()
    {
        try
        {
            _log.Information("API: GetActiveDnsZonePackages called by user {User}", User.Identity?.Name);
            
            var packages = await _dnsZonePackageService.GetActiveDnsZonePackagesAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveDnsZonePackages");
            return StatusCode(500, "An error occurred while retrieving active DNS zone packages");
        }
    }

    /// <summary>
    /// Retrieves the default DNS zone package
    /// </summary>
    /// <returns>The default DNS zone package with its records</returns>
    /// <response code="200">Returns the default DNS zone package</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If no default package is configured</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> GetDefaultDnsZonePackage()
    {
        try
        {
            _log.Information("API: GetDefaultDnsZonePackage called by user {User}", User.Identity?.Name);
            
            var package = await _dnsZonePackageService.GetDefaultDnsZonePackageAsync();

            if (package == null)
            {
                _log.Information("API: No default DNS zone package found");
                return NotFound("No default DNS zone package is configured");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultDnsZonePackage");
            return StatusCode(500, "An error occurred while retrieving the default DNS zone package");
        }
    }

    /// <summary>
    /// Retrieves a specific DNS zone package by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package</param>
    /// <returns>The DNS zone package information</returns>
    /// <response code="200">Returns the DNS zone package data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS zone package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> GetDnsZonePackageById(int id)
    {
        try
        {
            _log.Information("API: GetDnsZonePackageById called for ID {PackageId} by user {User}", id, User.Identity?.Name);
            
            var package = await _dnsZonePackageService.GetDnsZonePackageByIdAsync(id);

            if (package == null)
            {
                _log.Information("API: DNS zone package with ID {PackageId} not found", id);
                return NotFound($"DNS zone package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsZonePackageById for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS zone package");
        }
    }

    /// <summary>
    /// Retrieves a specific DNS zone package with its records by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package</param>
    /// <returns>The DNS zone package information with records</returns>
    /// <response code="200">Returns the DNS zone package data with records</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS zone package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/with-records")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> GetDnsZonePackageWithRecordsById(int id)
    {
        try
        {
            _log.Information("API: GetDnsZonePackageWithRecordsById called for ID {PackageId} by user {User}", id, User.Identity?.Name);
            
            var package = await _dnsZonePackageService.GetDnsZonePackageWithRecordsByIdAsync(id);

            if (package == null)
            {
                _log.Information("API: DNS zone package with ID {PackageId} not found", id);
                return NotFound($"DNS zone package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsZonePackageWithRecordsById for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS zone package with records");
        }
    }

    /// <summary>
    /// Creates a new DNS zone package in the system
    /// </summary>
    /// <param name="createDto">DNS zone package information for creation</param>
    /// <returns>The newly created DNS zone package</returns>
    /// <response code="201">Returns the newly created DNS zone package</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> CreateDnsZonePackage([FromBody] CreateDnsZonePackageDto createDto)
    {
        try
        {
            _log.Information("API: CreateDnsZonePackage called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateDnsZonePackage");
                return BadRequest(ModelState);
            }

            var package = await _dnsZonePackageService.CreateDnsZonePackageAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetDnsZonePackageById),
                new { id = package.Id },
                package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDnsZonePackage");
            return StatusCode(500, "An error occurred while creating the DNS zone package");
        }
    }

    /// <summary>
    /// Updates an existing DNS zone package's information
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package to update</param>
    /// <param name="updateDto">Updated DNS zone package information</param>
    /// <returns>The updated DNS zone package</returns>
    /// <response code="200">Returns the updated DNS zone package</response>
    /// <response code="400">If the data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If DNS zone package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> UpdateDnsZonePackage(int id, [FromBody] UpdateDnsZonePackageDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateDnsZonePackage called for ID {PackageId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateDnsZonePackage");
                return BadRequest(ModelState);
            }

            var package = await _dnsZonePackageService.UpdateDnsZonePackageAsync(id, updateDto);

            if (package == null)
            {
                _log.Information("API: DNS zone package with ID {PackageId} not found for update", id);
                return NotFound($"DNS zone package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDnsZonePackage for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while updating the DNS zone package");
        }
    }

    /// <summary>
    /// Deletes a DNS zone package from the system
    /// </summary>
    /// <param name="id">The unique identifier of the DNS zone package to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If DNS zone package was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If DNS zone package is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDnsZonePackage(int id)
    {
        try
        {
            _log.Information("API: DeleteDnsZonePackage called for ID {PackageId} by user {User}", id, User.Identity?.Name);

            var result = await _dnsZonePackageService.DeleteDnsZonePackageAsync(id);

            if (!result)
            {
                _log.Information("API: DNS zone package with ID {PackageId} not found for deletion", id);
                return NotFound($"DNS zone package with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDnsZonePackage for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while deleting the DNS zone package");
        }
    }

    /// <summary>
    /// Applies a DNS zone package to a domain by creating DNS records
    /// </summary>
    /// <param name="packageId">The unique identifier of the DNS zone package</param>
    /// <param name="domainId">The unique identifier of the domain</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the package was successfully applied</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If DNS zone package or domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{packageId}/apply-to-domain/{domainId}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ApplyPackageToDomain(int packageId, int domainId)
    {
        try
        {
            _log.Information("API: ApplyPackageToDomain called for package {PackageId} to domain {DomainId} by user {User}", 
                packageId, domainId, User.Identity?.Name);

            var result = await _dnsZonePackageService.ApplyPackageToDomainAsync(packageId, domainId);

            if (!result)
            {
                _log.Warning("API: Failed to apply package {PackageId} to domain {DomainId}", packageId, domainId);
                return NotFound("DNS zone package or domain not found");
            }

            return Ok(new { success = true, message = "DNS zone package successfully applied to domain" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ApplyPackageToDomain for package {PackageId} to domain {DomainId}", packageId, domainId);
            return StatusCode(500, "An error occurred while applying the DNS zone package");
        }
    }

    [HttpGet("{id}/assignments")]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(DnsZonePackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsZonePackageDto>> GetDnsZonePackageWithAssignments(int id)
    {
        try
        {
            _log.Information("API: GetDnsZonePackageWithAssignments called for ID {PackageId} by {User}", id, User.Identity?.Name);
            var package = await _dnsZonePackageService.GetDnsZonePackageWithAssignmentsAsync(id);
            if (package == null) return NotFound($"DNS zone package with ID {id} not found");
            return Ok(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDnsZonePackageWithAssignments for ID {PackageId}", id);
            return StatusCode(500, "An error occurred while retrieving the DNS zone package assignments");
        }
    }

    [HttpGet("by-control-panel/{controlPanelId}")]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageDto>>> GetPackagesByControlPanel(int controlPanelId)
    {
        try
        {
            _log.Information("API: GetPackagesByControlPanel called for panel {ControlPanelId} by {User}", controlPanelId, User.Identity?.Name);
            var packages = await _dnsZonePackageService.GetPackagesByControlPanelAsync(controlPanelId);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPackagesByControlPanel for panel {ControlPanelId}", controlPanelId);
            return StatusCode(500, "An error occurred while retrieving DNS zone packages for the control panel");
        }
    }

    [HttpPost("{packageId}/control-panels/{controlPanelId}")]
    [Authorize(Policy = "DnsZonePackage.Write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AssignControlPanel(int packageId, int controlPanelId)
    {
        try
        {
            _log.Information("API: AssignControlPanel {ControlPanelId} to package {PackageId} by {User}", controlPanelId, packageId, User.Identity?.Name);
            var result = await _dnsZonePackageService.AssignControlPanelAsync(packageId, controlPanelId);
            if (!result) return NotFound("DNS zone package or control panel not found");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignControlPanel {ControlPanelId} to package {PackageId}", controlPanelId, packageId);
            return StatusCode(500, "An error occurred while assigning the control panel");
        }
    }

    [HttpDelete("{packageId}/control-panels/{controlPanelId}")]
    [Authorize(Policy = "DnsZonePackage.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveControlPanel(int packageId, int controlPanelId)
    {
        try
        {
            _log.Information("API: RemoveControlPanel {ControlPanelId} from package {PackageId} by {User}", controlPanelId, packageId, User.Identity?.Name);
            var result = await _dnsZonePackageService.RemoveControlPanelAsync(packageId, controlPanelId);
            if (!result) return NotFound("Assignment not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RemoveControlPanel {ControlPanelId} from package {PackageId}", controlPanelId, packageId);
            return StatusCode(500, "An error occurred while removing the control panel assignment");
        }
    }

    [HttpGet("by-server/{serverId}")]
    [Authorize(Policy = "DnsZonePackage.Read")]
    [ProducesResponseType(typeof(IEnumerable<DnsZonePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DnsZonePackageDto>>> GetPackagesByServer(int serverId)
    {
        try
        {
            _log.Information("API: GetPackagesByServer called for server {ServerId} by {User}", serverId, User.Identity?.Name);
            var packages = await _dnsZonePackageService.GetPackagesByServerAsync(serverId);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPackagesByServer for server {ServerId}", serverId);
            return StatusCode(500, "An error occurred while retrieving DNS zone packages for the server");
        }
    }

    [HttpPost("{packageId}/servers/{serverId}")]
    [Authorize(Policy = "DnsZonePackage.Write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AssignServer(int packageId, int serverId)
    {
        try
        {
            _log.Information("API: AssignServer {ServerId} to package {PackageId} by {User}", serverId, packageId, User.Identity?.Name);
            var result = await _dnsZonePackageService.AssignServerAsync(packageId, serverId);
            if (!result) return NotFound("DNS zone package or server not found");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignServer {ServerId} to package {PackageId}", serverId, packageId);
            return StatusCode(500, "An error occurred while assigning the server");
        }
    }

    [HttpDelete("{packageId}/servers/{serverId}")]
    [Authorize(Policy = "DnsZonePackage.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveServer(int packageId, int serverId)
    {
        try
        {
            _log.Information("API: RemoveServer {ServerId} from package {PackageId} by {User}", serverId, packageId, User.Identity?.Name);
            var result = await _dnsZonePackageService.RemoveServerAsync(packageId, serverId);
            if (!result) return NotFound("Assignment not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RemoveServer {ServerId} from package {PackageId}", serverId, packageId);
            return StatusCode(500, "An error occurred while removing the server assignment");
        }
    }
}
