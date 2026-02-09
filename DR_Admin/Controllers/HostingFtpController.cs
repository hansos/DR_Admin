using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages FTP accounts for hosting accounts
/// </summary>
[ApiController]
[Route("api/v1/hosting-accounts/{hostingAccountId}/ftp")]
[Authorize]
public class HostingFtpController : ControllerBase
{
    private readonly IHostingFtpService _ftpService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingFtpController>();

    public HostingFtpController(IHostingFtpService ftpService)
    {
        _ftpService = ftpService;
    }

    [HttpGet]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingFtpAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HostingFtpAccountDto>>> GetFtpAccounts(int hostingAccountId)
    {
        try
        {
            _log.Information("API: GetFtpAccounts called for hosting account {AccountId}", hostingAccountId);
            var ftpAccounts = await _ftpService.GetFtpAccountsByHostingAccountAsync(hostingAccountId);
            return Ok(ftpAccounts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetFtpAccounts");
            return StatusCode(500, "An error occurred while retrieving FTP accounts");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingFtpAccountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HostingFtpAccountDto>> GetFtpAccount(int hostingAccountId, int id)
    {
        try
        {
            var ftpAccount = await _ftpService.GetFtpAccountAsync(id);
            if (ftpAccount == null || ftpAccount.HostingAccountId != hostingAccountId)
            {
                return NotFound($"FTP account with ID {id} not found");
            }
            return Ok(ftpAccount);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetFtpAccount");
            return StatusCode(500, "An error occurred while retrieving the FTP account");
        }
    }

    [HttpPost]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingFtpAccountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HostingFtpAccountDto>> CreateFtpAccount(
        int hostingAccountId,
        [FromBody] HostingFtpAccountCreateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: CreateFtpAccount called for {Username}", dto.Username);
            dto.HostingAccountId = hostingAccountId;
            var ftpAccount = await _ftpService.CreateFtpAccountAsync(dto, syncToServer);
            return CreatedAtAction(nameof(GetFtpAccount), new { hostingAccountId, id = ftpAccount.Id }, ftpAccount);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateFtpAccount");
            return StatusCode(500, "An error occurred while creating the FTP account");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingFtpAccountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HostingFtpAccountDto>> UpdateFtpAccount(
        int hostingAccountId,
        int id,
        [FromBody] HostingFtpAccountUpdateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            var ftpAccount = await _ftpService.UpdateFtpAccountAsync(id, dto, syncToServer);
            return Ok(ftpAccount);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateFtpAccount");
            return StatusCode(500, "An error occurred while updating the FTP account");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteFtpAccount(int hostingAccountId, int id, [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            var result = await _ftpService.DeleteFtpAccountAsync(id, deleteFromServer);
            if (!result) return NotFound($"FTP account with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteFtpAccount");
            return StatusCode(500, "An error occurred while deleting the FTP account");
        }
    }

    [HttpPost("{id}/change-password")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangeFtpPassword(
        int hostingAccountId,
        int id,
        [FromBody] string newPassword,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            var result = await _ftpService.ChangeFtpPasswordAsync(id, newPassword, syncToServer);
            if (!result) return NotFound($"FTP account with ID {id} not found");
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ChangeFtpPassword");
            return StatusCode(500, "An error occurred while changing the password");
        }
    }

    [HttpPost("sync")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncResultDto>> SyncFtpAccountsFromServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: SyncFtpAccountsFromServer called for account {AccountId}", hostingAccountId);
            var result = await _ftpService.SyncFtpAccountsFromServerAsync(hostingAccountId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncFtpAccountsFromServer");
            return StatusCode(500, "An error occurred while syncing FTP accounts");
        }
    }
}
