using DomainRegistrationLib.Models;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages domain registration operations through various registrars
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DomainManagerController : ControllerBase
{
    private readonly IDomainManagerService _domainManagerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainManagerController>();

    public DomainManagerController(IDomainManagerService domainManagerService)
    {
        _domainManagerService = domainManagerService;
    }

    /// <summary>
    /// Registers a domain with the specified registrar
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use for registration</param>
    /// <param name="registeredDomainId">The ID of the registered domain to register with the registrar</param>
    /// <returns>Domain registration result</returns>
    /// <response code="200">Returns the domain registration result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If the domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("registrar/{registrarCode}/domain/{registeredDomainId}")]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(DomainRegistrationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainRegistrationResult>> RegisterDomain(
        string registrarCode, 
        int registeredDomainId)
    {
        try
        {
            _log.Information("API: RegisterDomain called for registrar {RegistrarCode} and domain {DomainId} by user {User}", 
                registrarCode, registeredDomainId, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: RegisterDomain called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            if (registeredDomainId <= 0)
            {
                _log.Warning("API: RegisterDomain called with invalid domain ID {DomainId}", registeredDomainId);
                return BadRequest("Valid domain ID is required");
            }

            var result = await _domainManagerService.RegisterDomainAsync(registrarCode, registeredDomainId);

            if (!result.Success)
            {
                _log.Warning("API: Domain registration failed for domain {DomainId}. Message: {Message}", 
                    registeredDomainId, result.Message);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in RegisterDomain for domain {DomainId}", registeredDomainId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RegisterDomain for domain {DomainId}", registeredDomainId);
            return StatusCode(500, "An error occurred while registering the domain");
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration based on registered domain ID
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use for checking availability</param>
    /// <param name="registeredDomainId">The ID of the registered domain to check</param>
    /// <returns>Domain availability result</returns>
    /// <response code="200">Returns the domain availability result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If the domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarCode}/domain/{registeredDomainId}/is-available")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(DomainAvailabilityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainAvailabilityResult>> CheckDomainAvailabilityById(
        string registrarCode, 
        int registeredDomainId)
    {
        try
        {
            _log.Information("API: CheckDomainAvailabilityById called for registrar {RegistrarCode} and domain {DomainId} by user {User}", 
                registrarCode, registeredDomainId, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: CheckDomainAvailabilityById called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            if (registeredDomainId <= 0)
            {
                _log.Warning("API: CheckDomainAvailabilityById called with invalid domain ID {DomainId}", registeredDomainId);
                return BadRequest("Valid domain ID is required");
            }

            var result = await _domainManagerService.CheckDomainAvailabilityByIdAsync(registrarCode, registeredDomainId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CheckDomainAvailabilityById for domain {DomainId}", registeredDomainId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckDomainAvailabilityById for domain {DomainId}", registeredDomainId);
            return StatusCode(500, "An error occurred while checking domain availability");
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration based on domain name
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use for checking availability</param>
    /// <param name="domainName">The domain name to check availability for</param>
    /// <returns>Domain availability result</returns>
    /// <response code="200">Returns the domain availability result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarCode}/domain/name/{domainName}/is-available")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(DomainAvailabilityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainAvailabilityResult>> CheckDomainAvailabilityByName(
        string registrarCode, 
        string domainName)
    {
        try
        {
            _log.Information("API: CheckDomainAvailabilityByName called for registrar {RegistrarCode} and domain {DomainName} by user {User}", 
                registrarCode, domainName, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: CheckDomainAvailabilityByName called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            if (string.IsNullOrWhiteSpace(domainName))
            {
                _log.Warning("API: CheckDomainAvailabilityByName called with empty domain name");
                return BadRequest("Domain name is required");
            }

            var result = await _domainManagerService.CheckDomainAvailabilityByNameAsync(registrarCode, domainName);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CheckDomainAvailabilityByName for domain {DomainName}", domainName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckDomainAvailabilityByName for domain {DomainName}", domainName);
            return StatusCode(500, "An error occurred while checking domain availability");
        }
    }


    /// <summary>
    /// Downloads DNS records from the registrar for a single domain and merges them into the local database
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <param name="domainName">The fully-qualified domain name</param>
    /// <returns>DNS record sync result</returns>
    /// <response code="200">Returns the sync result</response>
    /// <response code="400">If the request is invalid or domain/registrar mismatch</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("registrar/{registrarCode}/domain/name/{domainName}/dns-records/sync")]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(DnsRecordSyncResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsRecordSyncResult>> SyncDnsRecordsForDomain(
        string registrarCode,
        string domainName)
    {
        try
        {
            _log.Information("API: SyncDnsRecordsForDomain called for registrar {RegistrarCode} and domain {DomainName} by user {User}",
                registrarCode, domainName, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: SyncDnsRecordsForDomain called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            if (string.IsNullOrWhiteSpace(domainName))
            {
                _log.Warning("API: SyncDnsRecordsForDomain called with empty domain name");
                return BadRequest("Domain name is required");
            }

            var result = await _domainManagerService.SyncDnsRecordsByDomainNameAsync(registrarCode, domainName);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in SyncDnsRecordsForDomain for domain {DomainName}", domainName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncDnsRecordsForDomain for domain {DomainName}", domainName);
            return StatusCode(500, "An error occurred while syncing DNS records");
        }
    }

    /// <summary>
    /// Downloads DNS records from the registrar for all domains assigned to that registrar
    /// and merges them into the local database
    /// </summary>
    /// <param name="registrarCode">The code of the registrar to use</param>
    /// <returns>Aggregated bulk sync result with per-domain details</returns>
    /// <response code="200">Returns the bulk sync result</response>
    /// <response code="400">If the registrar is invalid or inactive</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("registrar/{registrarCode}/dns-records/sync")]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(DnsBulkSyncResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DnsBulkSyncResult>> SyncDnsRecordsForAllDomains(string registrarCode)
    {
        try
        {
            _log.Information("API: SyncDnsRecordsForAllDomains called for registrar {RegistrarCode} by user {User}",
                registrarCode, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: SyncDnsRecordsForAllDomains called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            var result = await _domainManagerService.SyncDnsRecordsForAllDomainsAsync(registrarCode);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in SyncDnsRecordsForAllDomains for registrar {RegistrarCode}", registrarCode);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncDnsRecordsForAllDomains for registrar {RegistrarCode}", registrarCode);
            return StatusCode(500, "An error occurred while syncing DNS records");
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration based on domain name
    /// </summary>
    /// <param name="domainName">The domain name to check availability for</param>
    /// <returns>Domain availability result</returns>
    /// <response code="200">Returns the domain availability result</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/default/domain/{domainName}/is-available")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(DomainAvailabilityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainAvailabilityResult>> CheckDomainAvailabilityByName(string domainName)
    {
        try
        {
            var registrarCode = "aws";
            _log.Information("API: CheckDomainAvailabilityByName called for registrar {RegistrarCode} and domain {DomainName} by user {User}",
                registrarCode, domainName, User.Identity?.Name);

            if (string.IsNullOrWhiteSpace(registrarCode))
            {
                _log.Warning("API: CheckDomainAvailabilityByName called with empty registrar code");
                return BadRequest("Registrar code is required");
            }

            if (string.IsNullOrWhiteSpace(domainName))
            {
                _log.Warning("API: CheckDomainAvailabilityByName called with empty domain name");
                return BadRequest("Domain name is required");
            }

            var result = await _domainManagerService.CheckDomainAvailabilityByNameAsync(registrarCode, domainName);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CheckDomainAvailabilityByName for domain {DomainName}", domainName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckDomainAvailabilityByName for domain {DomainName}", domainName);
            return StatusCode(500, "An error occurred while checking domain availability");
        }
    }
}
