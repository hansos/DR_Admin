using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages domains including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegisteredDomainsController : ControllerBase
{
    private readonly IRegisteredDomainService _domainService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegisteredDomainsController>();

    public RegisteredDomainsController(IRegisteredDomainService domainService)
    {
        _domainService = domainService;
    }

    /// <summary>
    /// Retrieves all domains in the system
    /// </summary>
    /// <returns>List of all domains</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Admin.Only")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllDomains([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
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

                _log.Information("API: GetAllDomains (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}",
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _domainService.GetAllDomainsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            _log.Information("API: GetAllDomains called by user {User}", User.Identity?.Name);

            var domains = await _domainService.GetAllDomainsAsync();
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllDomains");
            return StatusCode(500, "An error occurred while retrieving domains");
        }
    }

    /// <summary>
    /// Retrieves domains for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of domains for the customer</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainDto>>> GetDomainsByCustomerId(int customerId)
    {
        try
        {
            _log.Information("API: GetDomainsByCustomerId called for customer {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            
            var domains = await _domainService.GetDomainsByCustomerIdAsync(customerId);
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainsByCustomerId for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving customer domains");
        }
    }

    /// <summary>
    /// Retrieves domains for a specific registrar
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <returns>List of domains for the registrar</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarId}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainDto>>> GetDomainsByRegistrarId(int registrarId)
    {
        try
        {
            _log.Information("API: GetDomainsByRegistrarId called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);
            
            var domains = await _domainService.GetDomainsByRegistrarIdAsync(registrarId);
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainsByRegistrarId for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while retrieving registrar domains");
        }
    }

    /// <summary>
    /// Retrieves domains by status
    /// </summary>
    /// <param name="status">The domain status</param>
    /// <returns>List of domains with the specified status</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("status/{status}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainDto>>> GetDomainsByStatus(string status)
    {
        try
        {
            _log.Information("API: GetDomainsByStatus called for status {Status} by user {User}", 
                status, User.Identity?.Name);
            
            var domains = await _domainService.GetDomainsByStatusAsync(status);
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainsByStatus for status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving domains by status");
        }
    }

    /// <summary>
    /// Retrieves domains expiring within specified days
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>List of domains expiring within the specified days</returns>
    /// <response code="200">Returns the list of domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("expiring/{days}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegisteredDomainDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegisteredDomainDto>>> GetDomainsExpiringInDays(int days)
    {
        try
        {
            _log.Information("API: GetDomainsExpiringInDays called for {Days} days by user {User}", 
                days, User.Identity?.Name);
            
            var domains = await _domainService.GetDomainsExpiringInDaysAsync(days);
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainsExpiringInDays for {Days} days", days);
            return StatusCode(500, "An error occurred while retrieving expiring domains");
        }
    }

    /// <summary>
    /// Retrieves a specific domain by ID
    /// </summary>
    /// <param name="id">The domain ID</param>
    /// <returns>The domain details</returns>
    /// <response code="200">Returns the domain</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(RegisteredDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainDto>> GetDomainById(int id)
    {
        try
        {
            _log.Information("API: GetDomainById called with ID {DomainId} by user {User}", 
                id, User.Identity?.Name);
            
            var domain = await _domainService.GetDomainByIdAsync(id);
            
            if (domain == null)
            {
                return NotFound($"Domain with ID {id} not found");
            }
            
            return Ok(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainById for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while retrieving the domain");
        }
    }

    /// <summary>
    /// Retrieves a specific domain by name
    /// </summary>
    /// <param name="name">The domain name</param>
    /// <returns>The domain details</returns>
    /// <response code="200">Returns the domain</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("name/{name}")]
    [Authorize(Policy = "Domain.Read")]
    [ProducesResponseType(typeof(RegisteredDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainDto>> GetDomainByName(string name)
    {
        try
        {
            _log.Information("API: GetDomainByName called with name {DomainName} by user {User}", 
                name, User.Identity?.Name);
            
            var domain = await _domainService.GetDomainByNameAsync(name);
            
            if (domain == null)
            {
                return NotFound($"Domain with name {name} not found");
            }
            
            return Ok(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainByName for name {DomainName}", name);
            return StatusCode(500, "An error occurred while retrieving the domain");
        }
    }

    /// <summary>
    /// Creates a new domain
    /// </summary>
    /// <param name="createDto">The domain creation data</param>
    /// <returns>The created domain</returns>
    /// <response code="201">Returns the newly created domain</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(RegisteredDomainDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainDto>> CreateDomain([FromBody] CreateRegisteredDomainDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateDomain called by user {User}", User.Identity?.Name);
            
            var domain = await _domainService.CreateDomainAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetDomainById),
                new { id = domain.Id },
                domain);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateDomain");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateDomain");
            return StatusCode(500, "An error occurred while creating the domain");
        }
    }

    /// <summary>
    /// Updates an existing domain
    /// </summary>
    /// <param name="id">The domain ID to update</param>
    /// <param name="updateDto">The updated domain data</param>
    /// <returns>The updated domain</returns>
    /// <response code="200">Returns the updated domain</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Domain.Write")]
    [ProducesResponseType(typeof(RegisteredDomainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisteredDomainDto>> UpdateDomain(int id, [FromBody] UpdateRegisteredDomainDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateDomain called for ID {DomainId} by user {User}", 
                id, User.Identity?.Name);
            
            var domain = await _domainService.UpdateDomainAsync(id, updateDto);
            
            if (domain == null)
            {
                return NotFound($"Domain with ID {id} not found");
            }
            
            return Ok(domain);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateDomain for ID {DomainId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateDomain for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while updating the domain");
        }
    }

    /// <summary>
    /// Deletes a domain
    /// </summary>
    /// <param name="id">The domain ID to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the domain was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If domain is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Domain.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteDomain(int id)
    {
        try
        {
            _log.Information("API: DeleteDomain called for ID {DomainId} by user {User}", 
                id, User.Identity?.Name);
            
            var deleted = await _domainService.DeleteDomainAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Domain with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteDomain for ID {DomainId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteDomain for ID {DomainId}", id);
            return StatusCode(500, "An error occurred while deleting the domain");
        }
    }

    /// <summary>
    /// Registers a new domain for the authenticated customer
    /// </summary>
    /// <param name="dto">Domain registration details</param>
    /// <returns>Registration result with order and invoice information</returns>
    /// <response code="200">Returns the registration result</response>
    /// <response code="400">If the request data is invalid or registration failed</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role or customer registration is disabled</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("register")]
    [Authorize(Policy = "Domain.Register")]
    [ProducesResponseType(typeof(DomainRegistrationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainRegistrationResponseDto>> RegisterDomain([FromBody] RegisterDomainDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get customer ID from claims
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out var customerId))
            {
                _log.Warning("API: RegisterDomain called without valid CustomerId claim by user {User}", 
                    User.Identity?.Name);
                return BadRequest("Customer ID not found in authentication token");
            }

            _log.Information("API: RegisterDomain called for domain {DomainName} by customer {CustomerId}", 
                dto.DomainName, customerId);

            var result = await _domainService.RegisterDomainAsync(dto, customerId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in RegisterDomain for domain {DomainName}", dto.DomainName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RegisterDomain for domain {DomainName}", dto.DomainName);
            return StatusCode(500, "An error occurred while registering the domain");
        }
    }

    /// <summary>
    /// Registers a new domain for a specific customer (Admin/Sales only)
    /// </summary>
    /// <param name="dto">Domain registration details including customer ID</param>
    /// <returns>Registration result with order and invoice information</returns>
    /// <response code="200">Returns the registration result</response>
    /// <response code="400">If the request data is invalid or registration failed</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("register-for-customer")]
    [Authorize(Policy = "Domain.RegisterForCustomer")]
    [ProducesResponseType(typeof(DomainRegistrationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainRegistrationResponseDto>> RegisterDomainForCustomer([FromBody] RegisterDomainForCustomerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: RegisterDomainForCustomer called for domain {DomainName}, customer {CustomerId} by user {User}", 
                dto.DomainName, dto.CustomerId, User.Identity?.Name);

            var result = await _domainService.RegisterDomainForCustomerAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in RegisterDomainForCustomer for domain {DomainName}", 
                dto.DomainName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RegisterDomainForCustomer for domain {DomainName}", dto.DomainName);
            return StatusCode(500, "An error occurred while registering the domain");
        }
    }

    /// <summary>
    /// Checks if a domain is available for registration
    /// </summary>
    /// <param name="dto">Domain availability check request</param>
    /// <returns>Availability information</returns>
    /// <response code="200">Returns the availability information</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("check-availability")]
    [Authorize(Policy = "Domain.CheckAvailability")]
    [ProducesResponseType(typeof(DomainAvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainAvailabilityResponseDto>> CheckDomainAvailability([FromBody] CheckDomainAvailabilityDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CheckDomainAvailability called for domain {DomainName} by user {User}", 
                dto.DomainName, User.Identity?.Name);

            var result = await _domainService.CheckDomainAvailabilityAsync(dto.DomainName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CheckDomainAvailability for domain {DomainName}", dto.DomainName);
            return StatusCode(500, "An error occurred while checking domain availability");
        }
    }

    /// <summary>
    /// Gets pricing information for a specific TLD
    /// </summary>
    /// <param name="tld">The top-level domain (e.g., "com", "net")</param>
    /// <param name="registrarId">Optional: specific registrar ID</param>
    /// <returns>Pricing information</returns>
    /// <response code="200">Returns the pricing information</response>
    /// <response code="404">If pricing not found for the TLD</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("pricing/{tld}")]
    [Authorize]
    [ProducesResponseType(typeof(DomainPricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DomainPricingDto>> GetDomainPricing(string tld, [FromQuery] int? registrarId = null)
    {
        try
        {
            _log.Information("API: GetDomainPricing called for TLD {Tld}, registrar {RegistrarId} by user {User}", 
                tld, registrarId, User.Identity?.Name);

            var pricing = await _domainService.GetDomainPricingAsync(tld, registrarId);

            if (pricing == null)
            {
                return NotFound($"Pricing not found for TLD: {tld}");
            }

            return Ok(pricing);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDomainPricing for TLD {Tld}", tld);
            return StatusCode(500, "An error occurred while retrieving domain pricing");
        }
    }

    /// <summary>
    /// Gets all available TLDs with pricing
    /// </summary>
    /// <returns>List of available TLDs</returns>
    /// <response code="200">Returns the list of available TLDs</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("available-tlds")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AvailableTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AvailableTldDto>>> GetAvailableTlds()
    {
        try
        {
            _log.Information("API: GetAvailableTlds called by user {User}", User.Identity?.Name);

            var tlds = await _domainService.GetAvailableTldsAsync();
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAvailableTlds");
            return StatusCode(500, "An error occurred while retrieving available TLDs");
        }
    }
}
