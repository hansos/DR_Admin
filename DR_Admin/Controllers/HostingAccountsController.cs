using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages hosting accounts including creation, retrieval, updates, and synchronization
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class HostingAccountsController : ControllerBase
{
    private readonly IHostingManagerService _hostingManagerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingAccountsController>();

    public HostingAccountsController(IHostingManagerService hostingManagerService)
    {
        _hostingManagerService = hostingManagerService;
    }

    /// <summary>
    /// Retrieves all hosting accounts
    /// </summary>
    /// <returns>List of all hosting accounts</returns>
    /// <response code="200">Returns the list of hosting accounts</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingAccountDto>>> GetAllHostingAccounts()
    {
        try
        {
            _log.Information("API: GetAllHostingAccounts called by user {User}", User.Identity?.Name);
            var accounts = await _hostingManagerService.GetAllHostingAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllHostingAccounts");
            return StatusCode(500, "An error occurred while retrieving hosting accounts");
        }
    }

    /// <summary>
    /// Retrieves a specific hosting account by ID
    /// </summary>
    /// <param name="id">The unique identifier of the hosting account</param>
    /// <returns>The hosting account information</returns>
    /// <response code="200">Returns the hosting account data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingAccountDto>> GetHostingAccount(int id)
    {
        try
        {
            _log.Information("API: GetHostingAccount called for ID {AccountId} by user {User}", id, User.Identity?.Name);
            var account = await _hostingManagerService.GetHostingAccountAsync(id);

            if (account == null)
            {
                _log.Warning("API: Hosting account {AccountId} not found", id);
                return NotFound($"Hosting account with ID {id} not found");
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostingAccount for ID {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving the hosting account");
        }
    }

    /// <summary>
    /// Retrieves a hosting account with full details including domains, emails, databases, and FTP accounts
    /// </summary>
    /// <param name="id">The unique identifier of the hosting account</param>
    /// <returns>The hosting account with detailed resource information</returns>
    /// <response code="200">Returns the hosting account with details</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/details")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingAccountDto>> GetHostingAccountWithDetails(int id)
    {
        try
        {
            _log.Information("API: GetHostingAccountWithDetails called for ID {AccountId} by user {User}", id, User.Identity?.Name);
            var account = await _hostingManagerService.GetHostingAccountWithDetailsAsync(id);

            if (account == null)
            {
                _log.Warning("API: Hosting account {AccountId} not found", id);
                return NotFound($"Hosting account with ID {id} not found");
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostingAccountWithDetails for ID {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving the hosting account details");
        }
    }

    /// <summary>
    /// Retrieves all hosting accounts for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of hosting accounts for the customer</returns>
    /// <response code="200">Returns the list of hosting accounts</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingAccountDto>>> GetHostingAccountsByCustomer(int customerId)
    {
        try
        {
            _log.Information("API: GetHostingAccountsByCustomer called for customer {CustomerId} by user {User}", customerId, User.Identity?.Name);
            var accounts = await _hostingManagerService.GetHostingAccountsByCustomerAsync(customerId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostingAccountsByCustomer for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving hosting accounts");
        }
    }

    /// <summary>
    /// Retrieves all hosting accounts for a specific server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <returns>List of hosting accounts on the server</returns>
    /// <response code="200">Returns the list of hosting accounts</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("server/{serverId}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HostingAccountDto>>> GetHostingAccountsByServer(int serverId)
    {
        try
        {
            _log.Information("API: GetHostingAccountsByServer called for server {ServerId} by user {User}", serverId, User.Identity?.Name);
            var accounts = await _hostingManagerService.GetHostingAccountsByServerAsync(serverId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetHostingAccountsByServer for server {ServerId}", serverId);
            return StatusCode(500, "An error occurred while retrieving hosting accounts");
        }
    }

    /// <summary>
    /// Creates a new hosting account (database only)
    /// </summary>
    /// <param name="dto">The hosting account creation data</param>
    /// <returns>The created hosting account</returns>
    /// <response code="201">Returns the created hosting account</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingAccountDto>> CreateHostingAccount([FromBody] HostingAccountCreateDto dto)
    {
        try
        {
            _log.Information("API: CreateHostingAccount called for customer {CustomerId} by user {User}", dto.CustomerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _hostingManagerService.CreateHostingAccountAsync(dto, syncToServer: false);
            
            _log.Information("API: Hosting account created with ID {AccountId}", account.Id);
            return CreatedAtAction(nameof(GetHostingAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateHostingAccount");
            return StatusCode(500, "An error occurred while creating the hosting account");
        }
    }

    /// <summary>
    /// Creates a new hosting account and synchronizes it to the server
    /// </summary>
    /// <param name="dto">The hosting account creation data</param>
    /// <returns>The created hosting account</returns>
    /// <response code="201">Returns the created hosting account</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("create-and-sync")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingAccountDto>> CreateHostingAccountAndSync([FromBody] HostingAccountCreateDto dto)
    {
        try
        {
            _log.Information("API: CreateHostingAccountAndSync called for customer {CustomerId} by user {User}", dto.CustomerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _hostingManagerService.CreateHostingAccountAsync(dto, syncToServer: true);
            
            _log.Information("API: Hosting account created and synced with ID {AccountId}", account.Id);
            return CreatedAtAction(nameof(GetHostingAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateHostingAccountAndSync");
            return StatusCode(500, "An error occurred while creating and syncing the hosting account");
        }
    }

    /// <summary>
    /// Updates an existing hosting account
    /// </summary>
    /// <param name="id">The hosting account ID</param>
    /// <param name="dto">The hosting account update data</param>
    /// <param name="syncToServer">Whether to sync changes to the hosting server</param>
    /// <returns>The updated hosting account</returns>
    /// <response code="200">Returns the updated hosting account</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HostingAccountDto>> UpdateHostingAccount(
        int id, 
        [FromBody] HostingAccountUpdateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: UpdateHostingAccount called for ID {AccountId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _hostingManagerService.UpdateHostingAccountAsync(id, dto, syncToServer);
            
            _log.Information("API: Hosting account {AccountId} updated successfully", id);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Hosting account {AccountId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateHostingAccount for ID {AccountId}", id);
            return StatusCode(500, "An error occurred while updating the hosting account");
        }
    }

    /// <summary>
    /// Deletes a hosting account
    /// </summary>
    /// <param name="id">The hosting account ID</param>
    /// <param name="deleteFromServer">Whether to also delete from the hosting server</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the hosting account was deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteHostingAccount(int id, [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            _log.Information("API: DeleteHostingAccount called for ID {AccountId} by user {User}", id, User.Identity?.Name);

            var result = await _hostingManagerService.DeleteHostingAccountAsync(id, deleteFromServer);

            if (!result)
            {
                _log.Warning("API: Hosting account {AccountId} not found", id);
                return NotFound($"Hosting account with ID {id} not found");
            }

            _log.Information("API: Hosting account {AccountId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteHostingAccount for ID {AccountId}", id);
            return StatusCode(500, "An error occurred while deleting the hosting account");
        }
    }

    /// <summary>
    /// Gets resource usage statistics for a hosting account
    /// </summary>
    /// <param name="id">The hosting account ID</param>
    /// <returns>Resource usage information</returns>
    /// <response code="200">Returns the resource usage data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/resource-usage")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(ResourceUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResourceUsageDto>> GetResourceUsage(int id)
    {
        try
        {
            _log.Information("API: GetResourceUsage called for account {AccountId} by user {User}", id, User.Identity?.Name);
            var usage = await _hostingManagerService.GetResourceUsageAsync(id);
            return Ok(usage);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Hosting account {AccountId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetResourceUsage for account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving resource usage");
        }
    }

    /// <summary>
    /// Gets sync status for a hosting account
    /// </summary>
    /// <param name="id">The hosting account ID</param>
    /// <returns>Sync status information</returns>
    /// <response code="200">Returns the sync status</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/sync-status")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(SyncStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncStatusDto>> GetSyncStatus(int id)
    {
        try
        {
            _log.Information("API: GetSyncStatus called for account {AccountId} by user {User}", id, User.Identity?.Name);
            var status = await _hostingManagerService.GetSyncStatusAsync(id);
            return Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Hosting account {AccountId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSyncStatus for account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving sync status");
        }
    }

    /// <summary>
    /// Provisions a hosting account on CPanel using a domain from the HostingDomains table
    /// </summary>
    /// <param name="id">The hosting account ID</param>
    /// <param name="domainId">Optional: Specific domain ID to use. If not provided, uses the main domain or first available domain</param>
    /// <returns>Sync operation result</returns>
    /// <response code="200">Returns the provisioning result</response>
    /// <response code="400">If the account is already provisioned or domain not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If the hosting account is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("{id}/provision-on-cpanel")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SyncResultDto>> ProvisionAccountOnCPanel(int id, [FromQuery] int? domainId = null)
    {
        try
        {
            _log.Information("API: ProvisionAccountOnCPanel called for account {AccountId}, domainId {DomainId} by user {User}", 
                id, domainId, User.Identity?.Name);

            var result = await _hostingManagerService.ProvisionAccountOnCPanelAsync(id, domainId);

            if (!result.Success)
            {
                _log.Warning("API: Failed to provision account {AccountId}: {Message}", id, result.Message);
                return BadRequest(result);
            }

            _log.Information("API: Successfully provisioned account {AccountId} on CPanel", id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ProvisionAccountOnCPanel for account {AccountId}", id);
            return StatusCode(500, "An error occurred while provisioning the account on CPanel");
        }
    }
}

