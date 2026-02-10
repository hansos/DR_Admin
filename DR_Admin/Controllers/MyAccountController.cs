using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages user account operations including registration, email confirmation, and password management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MyAccountController : ControllerBase
{
    private readonly IMyAccountService _myAccountService;
    private static readonly Serilog.ILogger _log = Log.ForContext<MyAccountController>();

    public MyAccountController(IMyAccountService myAccountService)
    {
        _myAccountService = myAccountService;
    }

    /// <summary>
    /// Registers a new account with user and customer information
    /// </summary>
    /// <param name="request">Registration details including username, email, password, and customer information</param>
    /// <returns>User ID, email, and email confirmation token</returns>
    /// <response code="201">Returns the registration result with confirmation token</response>
    /// <response code="400">If the registration data is invalid or user already exists</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// Creates both a user account and associated customer record. Email confirmation is required before the account is fully activated.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterAccountResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisterAccountResponseDto>> Register([FromBody] RegisterAccountRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _log.Warning("Invalid model state for registration");
                return BadRequest(ModelState);
            }

            var result = await _myAccountService.RegisterAsync(request);

            _log.Information("Account registered successfully: {Email}", request.Email);
            return CreatedAtAction(nameof(GetMyAccount), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Confirms user email address using the confirmation token sent during registration
    /// </summary>
    /// <param name="request">Email address and confirmation token</param>
    /// <returns>Success status message</returns>
    /// <response code="200">If email was confirmed successfully</response>
    /// <response code="400">If email or token is missing, invalid, or expired</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.ConfirmationToken))
            {
                _log.Warning("Email confirmation attempt with empty email or token");
                return BadRequest(new { message = "Email and confirmation token are required" });
            }

            var result = await _myAccountService.ConfirmEmailAsync(request.Email, request.ConfirmationToken);

            if (!result)
            {
                _log.Warning("Email confirmation failed for: {Email}", request.Email);
                return BadRequest(new { message = "Invalid or expired confirmation token" });
            }

            _log.Information("Email confirmed successfully: {Email}", request.Email);
            return Ok(new { message = "Email confirmed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during email confirmation for: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during email confirmation" });
        }
    }

    /// <summary>
    /// Requests a password reset by sending an email with a reset token
    /// </summary>
    /// <param name="request">Email address for the account</param>
    /// <returns>Success status message</returns>
    /// <response code="200">If the request was processed (email sent if account exists)</response>
    /// <response code="400">If required fields are missing</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("request-password-reset")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                _log.Warning("Password reset request with missing email");
                return BadRequest(new { message = "Email address is required" });
            }

            var result = await _myAccountService.RequestPasswordResetAsync(request.Email);

            if (!result)
            {
                _log.Warning("Password reset request failed for: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }

            _log.Information("Password reset requested for: {Email}", request.Email);
            // Always return the same message to prevent email enumeration
            return Ok(new { message = "If the email address exists in our system, a password reset link has been sent." });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during password reset request");
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Sets password for a new account or after password reset using a token
    /// </summary>
    /// <param name="request">Email, token, new password, and password confirmation</param>
    /// <returns>Success status message</returns>
    /// <response code="200">If password was set successfully</response>
    /// <response code="400">If required fields are missing, passwords don't match, or token is invalid/expired</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("set-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetPassword([FromBody] SetPasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || 
                string.IsNullOrEmpty(request.NewPassword))
            {
                _log.Warning("Set password attempt with missing required fields");
                return BadRequest(new { message = "Email, token, and new password are required" });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _log.Warning("Set password attempt with mismatched passwords");
                return BadRequest(new { message = "Passwords do not match" });
            }

            var result = await _myAccountService.SetPasswordAsync(request.Email, request.Token, request.NewPassword);

            if (!result)
            {
                _log.Warning("Set password failed for: {Email}", request.Email);
                return BadRequest(new { message = "Invalid or expired token" });
            }

            _log.Information("Password set successfully for: {Email}", request.Email);
            return Ok(new { message = "Password set successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during set password for: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred while setting password" });
        }
    }

    /// <summary>
    /// Changes password for the currently authenticated user
    /// </summary>
    /// <param name="request">Current password, new password, and password confirmation</param>
    /// <returns>Success status message</returns>
    /// <response code="200">If password was changed successfully</response>
    /// <response code="400">If required fields are missing, passwords don't match, or current password is incorrect</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                _log.Warning("Change password attempt with missing required fields");
                return BadRequest(new { message = "Current password and new password are required" });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _log.Warning("Change password attempt with mismatched passwords");
                return BadRequest(new { message = "Passwords do not match" });
            }

            var result = await _myAccountService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!result)
            {
                _log.Warning("Change password failed for user: {UserId}", userId);
                return BadRequest(new { message = "Current password is incorrect" });
            }

            _log.Information("Password changed successfully for user: {UserId}", userId);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during password change");
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    /// <summary>
    /// Updates the email address for the currently authenticated user
    /// </summary>
    /// <param name="request">New email address and current password for verification</param>
    /// <returns>Success status message</returns>
    /// <response code="200">If email was updated successfully</response>
    /// <response code="400">If required fields are missing, password is incorrect, or email is already in use</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPatch("email")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PatchEmail([FromBody] PatchEmailRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(request.NewEmail) || string.IsNullOrEmpty(request.Password))
            {
                _log.Warning("Patch email attempt with missing required fields");
                return BadRequest(new { message = "New email and password are required" });
            }

            var result = await _myAccountService.PatchEmailAsync(userId, request.NewEmail, request.Password);

            if (!result)
            {
                _log.Warning("Patch email failed for user: {UserId}", userId);
                return BadRequest(new { message = "Invalid password or email already in use" });
            }

            _log.Information("Email updated successfully for user: {UserId}", userId);
            return Ok(new { message = "Email updated successfully. Please confirm your new email address." });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during email update");
            return StatusCode(500, new { message = "An error occurred while updating email" });
        }
    }

    /// <summary>
    /// Update customer information for authenticated user
    /// </summary>
    /// <param name="request">Customer information to update (name, email, phone, address)</param>
    /// <returns>Updated customer information</returns>
    [HttpPatch("customer")]
    [Authorize]
    public async Task<ActionResult<CustomerAccountDto>> PatchCustomerInfo([FromBody] PatchCustomerInfoRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                _log.Warning("Invalid model state for customer info update");
                return BadRequest(ModelState);
            }

            var result = await _myAccountService.PatchCustomerInfoAsync(userId, request);

            if (result == null)
            {
                _log.Warning("Patch customer info failed for user: {UserId}", userId);
                return BadRequest(new { message = "Unable to update customer information" });
            }

            _log.Information("Customer info updated successfully for user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during customer info update");
            return StatusCode(500, new { message = "An error occurred while updating customer information" });
        }
    }

    /// <summary>
    /// Get current authenticated user's account information
    /// </summary>
    /// <returns>User account information including customer details</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserAccountDto>> GetMyAccount()
    {
        try
        {
            var userId = GetCurrentUserId();

            var result = await _myAccountService.GetMyAccountAsync(userId);

            if (result == null)
            {
                _log.Warning("Get account failed for user: {UserId}", userId);
                return NotFound(new { message = "Account not found" });
            }

            _log.Information("Account information retrieved for user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving account information");
            return StatusCode(500, new { message = "An error occurred while retrieving account information" });
        }
    }

    #region Private Helper Methods

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    #endregion
}
