using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages email accounts for hosting accounts
/// </summary>
[ApiController]
[Route("api/v1/hosting-accounts/{hostingAccountId}/emails")]
[Authorize]
public class HostingEmailController : ControllerBase
{
    private readonly IHostingEmailService _emailService;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingEmailController>();

    public HostingEmailController(IHostingEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(IEnumerable<HostingEmailAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HostingEmailAccountDto>>> GetEmailAccounts(int hostingAccountId)
    {
        try
        {
            _log.Information("API: GetEmailAccounts called for hosting account {AccountId}", hostingAccountId);
            var emails = await _emailService.GetEmailAccountsByHostingAccountAsync(hostingAccountId);
            return Ok(emails);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetEmailAccounts");
            return StatusCode(500, "An error occurred while retrieving email accounts");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Hosting.Read")]
    [ProducesResponseType(typeof(HostingEmailAccountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HostingEmailAccountDto>> GetEmailAccount(int hostingAccountId, int id)
    {
        try
        {
            var email = await _emailService.GetEmailAccountAsync(id);
            if (email == null || email.HostingAccountId != hostingAccountId)
            {
                return NotFound($"Email account with ID {id} not found");
            }
            return Ok(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetEmailAccount");
            return StatusCode(500, "An error occurred while retrieving the email account");
        }
    }

    [HttpPost]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingEmailAccountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HostingEmailAccountDto>> CreateEmailAccount(
        int hostingAccountId,
        [FromBody] HostingEmailAccountCreateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            _log.Information("API: CreateEmailAccount called for {EmailAddress}", dto.EmailAddress);
            dto.HostingAccountId = hostingAccountId;
            var email = await _emailService.CreateEmailAccountAsync(dto, syncToServer);
            return CreatedAtAction(nameof(GetEmailAccount), new { hostingAccountId, id = email.Id }, email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateEmailAccount");
            return StatusCode(500, "An error occurred while creating the email account");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(HostingEmailAccountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HostingEmailAccountDto>> UpdateEmailAccount(
        int hostingAccountId,
        int id,
        [FromBody] HostingEmailAccountUpdateDto dto,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            var email = await _emailService.UpdateEmailAccountAsync(id, dto, syncToServer);
            return Ok(email);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateEmailAccount");
            return StatusCode(500, "An error occurred while updating the email account");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEmailAccount(int hostingAccountId, int id, [FromQuery] bool deleteFromServer = false)
    {
        try
        {
            var result = await _emailService.DeleteEmailAccountAsync(id, deleteFromServer);
            if (!result) return NotFound($"Email account with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteEmailAccount");
            return StatusCode(500, "An error occurred while deleting the email account");
        }
    }

    [HttpPost("{id}/change-password")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangeEmailPassword(
        int hostingAccountId,
        int id,
        [FromBody] string newPassword,
        [FromQuery] bool syncToServer = false)
    {
        try
        {
            var result = await _emailService.ChangeEmailPasswordAsync(id, newPassword, syncToServer);
            if (!result) return NotFound($"Email account with ID {id} not found");
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ChangeEmailPassword");
            return StatusCode(500, "An error occurred while changing the password");
        }
    }

    [HttpPost("sync")]
    [Authorize(Policy = "Hosting.Write")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncResultDto>> SyncEmailAccountsFromServer(int hostingAccountId)
    {
        try
        {
            _log.Information("API: SyncEmailAccountsFromServer called for account {AccountId}", hostingAccountId);
            var result = await _emailService.SyncEmailAccountsFromServerAsync(hostingAccountId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SyncEmailAccountsFromServer");
            return StatusCode(500, "An error occurred while syncing email accounts");
        }
    }
}
