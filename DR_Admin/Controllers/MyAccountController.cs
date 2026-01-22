using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MyAccountController : ControllerBase
{
    private readonly IMyAccountService _myAccountService;
    private readonly Serilog.ILogger _logger;

    public MyAccountController(IMyAccountService myAccountService, Serilog.ILogger logger)
    {
        _myAccountService = myAccountService;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password, returns access and refresh tokens
    /// </summary>
    /// <param name="request">Email and password</param>
    /// <returns>Access token, refresh token, and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<MyAccountLoginResponseDto>> Login([FromBody] MyAccountLoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.Warning("Login attempt with empty email or password");
                return BadRequest(new { message = "Email and password are required" });
            }

            var result = await _myAccountService.LoginAsync(request.Email, request.Password);

            if (result == null)
            {
                _logger.Warning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            _logger.Information("Successful login for email: {Email}", request.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Register a new account with user and customer information
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>User ID, email, and confirmation token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterAccountResponseDto>> Register([FromBody] RegisterAccountRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for registration");
                return BadRequest(ModelState);
            }

            var result = await _myAccountService.RegisterAsync(request);

            _logger.Information("Account registered successfully: {Email}", request.Email);
            return CreatedAtAction(nameof(GetMyAccount), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Confirm email address using confirmation token
    /// </summary>
    /// <param name="request">Email and confirmation token</param>
    /// <returns>Success status</returns>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.ConfirmationToken))
            {
                _logger.Warning("Email confirmation attempt with empty email or token");
                return BadRequest(new { message = "Email and confirmation token are required" });
            }

            var result = await _myAccountService.ConfirmEmailAsync(request.Email, request.ConfirmationToken);

            if (!result)
            {
                _logger.Warning("Email confirmation failed for: {Email}", request.Email);
                return BadRequest(new { message = "Invalid or expired confirmation token" });
            }

            _logger.Information("Email confirmed successfully: {Email}", request.Email);
            return Ok(new { message = "Email confirmed successfully" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during email confirmation for: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during email confirmation" });
        }
    }

    /// <summary>
    /// Set password for new account using token (e.g., after password reset)
    /// </summary>
    /// <param name="request">Email, token, and new password</param>
    /// <returns>Success status</returns>
    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<ActionResult> SetPassword([FromBody] SetPasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || 
                string.IsNullOrEmpty(request.NewPassword))
            {
                _logger.Warning("Set password attempt with missing required fields");
                return BadRequest(new { message = "Email, token, and new password are required" });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.Warning("Set password attempt with mismatched passwords");
                return BadRequest(new { message = "Passwords do not match" });
            }

            var result = await _myAccountService.SetPasswordAsync(request.Email, request.Token, request.NewPassword);

            if (!result)
            {
                _logger.Warning("Set password failed for: {Email}", request.Email);
                return BadRequest(new { message = "Invalid or expired token" });
            }

            _logger.Information("Password set successfully for: {Email}", request.Email);
            return Ok(new { message = "Password set successfully" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during set password for: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred while setting password" });
        }
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="request">Current password, new password, and confirmation</param>
    /// <returns>Success status</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                _logger.Warning("Change password attempt with missing required fields");
                return BadRequest(new { message = "Current password and new password are required" });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.Warning("Change password attempt with mismatched passwords");
                return BadRequest(new { message = "Passwords do not match" });
            }

            var result = await _myAccountService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!result)
            {
                _logger.Warning("Change password failed for user: {UserId}", userId);
                return BadRequest(new { message = "Current password is incorrect" });
            }

            _logger.Information("Password changed successfully for user: {UserId}", userId);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during password change");
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    /// <summary>
    /// Update email address for authenticated user
    /// </summary>
    /// <param name="request">New email and password for verification</param>
    /// <returns>Success status</returns>
    [HttpPatch("email")]
    [Authorize]
    public async Task<ActionResult> PatchEmail([FromBody] PatchEmailRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(request.NewEmail) || string.IsNullOrEmpty(request.Password))
            {
                _logger.Warning("Patch email attempt with missing required fields");
                return BadRequest(new { message = "New email and password are required" });
            }

            var result = await _myAccountService.PatchEmailAsync(userId, request.NewEmail, request.Password);

            if (!result)
            {
                _logger.Warning("Patch email failed for user: {UserId}", userId);
                return BadRequest(new { message = "Invalid password or email already in use" });
            }

            _logger.Information("Email updated successfully for user: {UserId}", userId);
            return Ok(new { message = "Email updated successfully. Please confirm your new email address." });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during email update");
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
                _logger.Warning("Invalid model state for customer info update");
                return BadRequest(ModelState);
            }

            var result = await _myAccountService.PatchCustomerInfoAsync(userId, request);

            if (result == null)
            {
                _logger.Warning("Patch customer info failed for user: {UserId}", userId);
                return BadRequest(new { message = "Unable to update customer information" });
            }

            _logger.Information("Customer info updated successfully for user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during customer info update");
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
                _logger.Warning("Get account failed for user: {UserId}", userId);
                return NotFound(new { message = "Account not found" });
            }

            _logger.Information("Account information retrieved for user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving account information");
            return StatusCode(500, new { message = "An error occurred while retrieving account information" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                _logger.Warning("Refresh token attempt with empty token");
                return BadRequest(new { message = "Refresh token is required" });
            }

            var result = await _myAccountService.RefreshTokenAsync(request.RefreshToken);

            if (result == null)
            {
                _logger.Warning("Refresh token failed: Invalid or expired token");
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            _logger.Information("Token refreshed successfully");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Logout by revoking refresh token
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                _logger.Warning("Logout attempt with empty token");
                return BadRequest(new { message = "Refresh token is required" });
            }

            var result = await _myAccountService.RevokeRefreshTokenAsync(request.RefreshToken);

            if (!result)
            {
                _logger.Warning("Logout failed: Token not found");
                return BadRequest(new { message = "Token not found or already revoked" });
            }

            _logger.Information("User logged out successfully");
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
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
