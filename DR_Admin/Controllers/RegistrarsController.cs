using DomainRegistrationLib.Models;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages domain registrars and their TLD offerings
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistrarsController : ControllerBase
{
    private readonly IRegistrarService _registrarService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarsController>();

    public RegistrarsController(IRegistrarService registrarService)
    {
        _registrarService = registrarService;
    }

    /// <summary>
    /// Retrieves all TLDs supported by a specific registrar
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <returns>List of TLDs supported by the registrar</returns>
    /// <response code="200">Returns the list of TLDs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{registrarId}/tlds")]
    [ProducesResponseType(typeof(IEnumerable<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TldDto>>> GetTldsByRegistrar(int registrarId)
    {
        try
        {
            _log.Information("API: GetTldsByRegistrar called for registrar {RegistrarId} by user {User}", registrarId, User.Identity?.Name);

            var tlds = await _registrarService.GetTldsByRegistrarAsync(registrarId);
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTldsByRegistrar for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while retrieving TLDs for the registrar");
        }
    }

    /// <summary>
    /// Downloads TLDs for the registrar filtered by a single TLD string and updates the database
    /// </summary>
    [HttpPost("{registrarId}/tlds/download/{tld}")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadTldsForRegistrarFiltered(int registrarId, string tld)
    {
        try
        {
            _log.Information("API: DownloadTldsForRegistrar (filtered) called for registrar {RegistrarId} filter {Tld} by user {User}",
                registrarId, tld, User.Identity?.Name);

            var count = await _registrarService.DownloadTldsForRegistrarAsync(registrarId, tld);

            _log.Information("API: Successfully downloaded {Count} TLDs for registrar {RegistrarId} (filter: {Tld})",
                count, registrarId, tld);

            return Ok(new
            {
                message = $"Successfully downloaded and updated {count} TLDs for the registrar (filter: {tld})",
                count = count,
                registrarId = registrarId
            });
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DownloadTldsForRegistrar (filtered) for registrar {RegistrarId}", registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadTldsForRegistrar (filtered) for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while downloading TLDs for the registrar");
        }
    }

    /// <summary>
    /// Downloads TLDs for the registrar filtered by a list of TLD strings (provided in request body) and updates the database
    /// </summary>
    [HttpPost("{registrarId}/tlds/download/list")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadTldsForRegistrarFilteredList(int registrarId, [FromBody] List<string> tlds)
    {
        try
        {
            _log.Information("API: DownloadTldsForRegistrar (filtered list) called for registrar {RegistrarId} filterCount {Count} by user {User}",
                registrarId, tlds?.Count ?? 0, User.Identity?.Name);

            if (tlds == null || tlds.Count == 0)
            {
                _log.Warning("API: No TLDs supplied for DownloadTldsForRegistrar (filtered list)");
                return BadRequest("A non-empty list of TLDs must be provided in the request body");
            }

            var count = await _registrarService.DownloadTldsForRegistrarAsync(registrarId, tlds);

            _log.Information("API: Successfully downloaded {Count} TLDs for registrar {RegistrarId} (filtered list)",
                count, registrarId);

            return Ok(new
            {
                message = $"Successfully downloaded and updated {count} TLDs for the registrar (filtered list)",
                count = count,
                registrarId = registrarId
            });
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DownloadTldsForRegistrar (filtered list) for registrar {RegistrarId}", registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadTldsForRegistrar (filtered list) for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while downloading TLDs for the registrar");
        }
    }

    /// <summary>
    /// Downloads all TLDs supported by the registrar from their API and updates the database
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <returns>Number of TLDs downloaded and updated</returns>
    /// <response code="200">Returns the count of TLDs downloaded</response>
    /// <response code="400">If the registrar is not found or invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{registrarId}/tlds/download")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadTldsForRegistrar(int registrarId)
    {
        try
        {
            _log.Information("API: DownloadTldsForRegistrar called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);

            var count = await _registrarService.DownloadTldsForRegistrarAsync(registrarId);
            
            _log.Information("API: Successfully downloaded {Count} TLDs for registrar {RegistrarId}", 
                count, registrarId);
            
            return Ok(new { 
                message = $"Successfully downloaded and updated {count} TLDs for the registrar",
                count = count,
                registrarId = registrarId
            });
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DownloadTldsForRegistrar for registrar {RegistrarId}", registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadTldsForRegistrar for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while downloading TLDs for the registrar");
        }
    }

    /// <summary>
    /// Assigns a TLD to a registrar using their unique identifiers
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="tldId">The unique identifier of the TLD</param>
    /// <returns>The created registrar-TLD relationship</returns>
    /// <response code="201">Returns the created registrar-TLD relationship</response>
    /// <response code="400">If the assignment is invalid or already exists</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{registrarId}/tld/{tldId}")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> AssignTldToRegistrarByIds(int registrarId, int tldId)
    {
        try
        {
            _log.Information("API: AssignTldToRegistrarByIds called for registrar {RegistrarId} and TLD {TldId} by user {User}", registrarId, tldId, User.Identity?.Name);

            var result = await _registrarService.AssignTldToRegistrarAsync(registrarId, tldId);

            return CreatedAtAction(null, result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in AssignTldToRegistrarByIds");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignTldToRegistrarByIds for registrar {RegistrarId} and TLD {TldId}", registrarId, tldId);
            return StatusCode(500, "An error occurred while assigning the TLD to the registrar");
        }
    }

    /// <summary>
    /// Assigns a TLD to a registrar using TLD details
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="tldDto">The TLD information</param>
    /// <returns>The created registrar-TLD relationship</returns>
    /// <response code="201">Returns the created registrar-TLD relationship</response>
    /// <response code="400">If the TLD data is invalid or assignment already exists</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{registrarId}/tld")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> AssignTldToRegistrarByDto(int registrarId, [FromBody] TldDto tldDto)
    {
        try
        {
            _log.Information("API: AssignTldToRegistrarByDto called for registrar {RegistrarId} by user {User}", registrarId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for AssignTldToRegistrarByDto");
                return BadRequest(ModelState);
            }

            var result = await _registrarService.AssignTldToRegistrarAsync(registrarId, tldDto);

            return CreatedAtAction(null, result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in AssignTldToRegistrarByDto");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AssignTldToRegistrarByDto");
            return StatusCode(500, "An error occurred while assigning the TLD to the registrar");
        }
    }

    /// <summary>
    /// Retrieves all domain registrars in the system
    /// </summary>
    /// <returns>List of all registrars</returns>
    /// <response code="200">Returns the list of registrars</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Registrar.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarDto>>> GetAllRegistrars()
    {
        try
        {
            _log.Information("API: GetAllRegistrars called by user {User}", User.Identity?.Name);
            
            var registrars = await _registrarService.GetAllRegistrarsAsync();
            return Ok(registrars);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRegistrars");
            return StatusCode(500, "An error occurred while retrieving registrars");
        }
    }

    /// <summary>
    /// Get active registrars only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<RegistrarDto>>> GetActiveRegistrars()
    {
        try
        {
            _log.Information("API: GetActiveRegistrars called by user {User}", User.Identity?.Name);
            
            var registrars = await _registrarService.GetActiveRegistrarsAsync();
            return Ok(registrars);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveRegistrars");
            return StatusCode(500, "An error occurred while retrieving active registrars");
        }
    }

    /// <summary>
    /// Get registrar by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "Registrar.Read")]
    public async Task<ActionResult<RegistrarDto>> GetRegistrarById(int id)
    {
        try
        {
            _log.Information("API: GetRegistrarById called for ID {RegistrarId} by user {User}", id, User.Identity?.Name);
            
            var registrar = await _registrarService.GetRegistrarByIdAsync(id);

            if (registrar == null)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return Ok(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarById for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while retrieving the registrar");
        }
    }

    /// <summary>
    /// Get registrar by code
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<RegistrarDto>> GetRegistrarByCode(string code)
    {
        try
        {
            _log.Information("API: GetRegistrarByCode called for code {RegistrarCode} by user {User}", 
                code, User.Identity?.Name);
            
            var registrar = await _registrarService.GetRegistrarByCodeAsync(code);

            if (registrar == null)
            {
                _log.Information("API: Registrar with code {RegistrarCode} not found", code);
                return NotFound($"Registrar with code {code} not found");
            }

            return Ok(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarByCode for code {RegistrarCode}", code);
            return StatusCode(500, "An error occurred while retrieving the registrar");
        }
    }

    /// <summary>
    /// Create a new registrar
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarDto>> CreateRegistrar([FromBody] CreateRegistrarDto createDto)
    {
        try
        {
            _log.Information("API: CreateRegistrar called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRegistrar");
                return BadRequest(ModelState);
            }

            var registrar = await _registrarService.CreateRegistrarAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetRegistrarById),
                new { id = registrar.Id },
                registrar);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRegistrar");
            return StatusCode(500, "An error occurred while creating the registrar");
        }
    }

    /// <summary>
    /// Update an existing registrar
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrarDto>> UpdateRegistrar(int id, [FromBody] UpdateRegistrarDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRegistrar called for ID {RegistrarId} by user {User}", 
                id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRegistrar");
                return BadRequest(ModelState);
            }

            var registrar = await _registrarService.UpdateRegistrarAsync(id, updateDto);

            if (registrar == null)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found for update", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return Ok(registrar);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRegistrar for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while updating the registrar");
        }
    }

    /// <summary>
    /// Delete a registrar
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRegistrar(int id)
    {
        try
        {
            _log.Information("API: DeleteRegistrar called for ID {RegistrarId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _registrarService.DeleteRegistrarAsync(id);

            if (!result)
            {
                _log.Information("API: Registrar with ID {RegistrarId} not found for deletion", id);
                return NotFound($"Registrar with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteRegistrar");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRegistrar for ID {RegistrarId}", id);
            return StatusCode(500, "An error occurred while deleting the registrar");
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration using a specific registrar
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="domainName">The domain name to check (e.g., example.com)</param>
    /// <returns>Domain availability information</returns>
    /// <response code="200">Returns the domain availability result</response>
    /// <response code="400">If the registrar is not found or not active</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{registrarId}/isavailable/{domainName}")]
    [ProducesResponseType(typeof(DomainRegistrationLib.Models.DomainAvailabilityResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainRegistrationLib.Models.DomainAvailabilityResult>> CheckDomainAvailability(
        int registrarId, 
        string domainName)
    {
        try
        {
            _log.Information("API: CheckDomainAvailability called for domain {DomainName} using registrar {RegistrarId} by user {User}", 
                domainName, registrarId, User.Identity?.Name);

            var result = await _registrarService.CheckDomainAvailabilityAsync(registrarId, domainName);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CheckDomainAvailability for domain {DomainName} using registrar {RegistrarId}", 
                domainName, registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckDomainAvailability for domain {DomainName} using registrar {RegistrarId}", 
                domainName, registrarId);
            return StatusCode(500, "An error occurred while checking domain availability");
        }
    }

    /// <summary>
    /// Downloads all domains registered with a specific registrar and syncs them to the database
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="save">If true, saves domains and TLDs to the database (default: true)</param>
    /// <returns>Number of domains downloaded and updated</returns>
    /// <response code="200">Returns the count of domains downloaded</response>
    /// <response code="400">If the registrar is not found or not active</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{registrarId}/domains/download")]
    [Authorize(Policy = "Registrar.Write")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadDomainsForRegistrar(int registrarId, [FromQuery] bool save = true)
    {
        try
        {
            _log.Information("API: DownloadDomainsForRegistrar called for registrar {RegistrarId} by user {User} (save={Save})", 
                registrarId, User.Identity?.Name, save);

            var count = await _registrarService.DownloadDomainsForRegistrarAsync(registrarId, save);
            
            _log.Information("API: Successfully downloaded {Count} domains for registrar {RegistrarId}", 
                count, registrarId);
            
            return Ok(new { 
                message = $"Successfully downloaded and {(save ? "saved" : "retrieved")} {count} domains for the registrar",
                count = count,
                registrarId = registrarId,
                saved = save
            });
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DownloadDomainsForRegistrar for registrar {RegistrarId}", registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadDomainsForRegistrar for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while downloading domains for the registrar");
        }
    }

    /// <summary>
    /// Gets all domains registered with a specific registrar
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="save">If true, saves TLDs and domains to the database (default: false)</param>
    /// <returns>List of registered domains from the registrar</returns>
    /// <response code="200">Returns the list of registered domains</response>
    /// <response code="400">If the registrar is not found or not active</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{registrarId}/domains")]
    [Authorize(Policy = "Registrar.Read")]
    [ProducesResponseType(typeof(RegisteredDomainsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainsResult>> GetRegisteredDomains(int registrarId, [FromQuery] bool save = false)
    {
        try
        {
            _log.Information("API: GetRegisteredDomains called for registrar {RegistrarId} by user {User} (save={Save})", 
                registrarId, User.Identity?.Name, save);

            var result = await _registrarService.GetRegisteredDomainsAsync(registrarId, save);
            
            _log.Information("API: Successfully retrieved {Count} domains for registrar {RegistrarId} (saved={Saved})", 
                result.Domains?.Count ?? 0, registrarId, save);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in GetRegisteredDomains for registrar {RegistrarId}", registrarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegisteredDomains for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while retrieving domains for the registrar");
        }
    }
}
