using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILoginHistoryService _loginHistoryService;
    private static readonly Serilog.ILogger _log = Log.ForContext<AuthController>();

    public AuthController(IAuthService authService, ILoginHistoryService loginHistoryService)
    {
        _authService = authService;
        _loginHistoryService = loginHistoryService;
    }

    /// <summary>
    /// Login endpoint to get JWT token. Accepts both username and email address as identification.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            _log.Warning("Login attempt with empty username or password");
            return BadRequest(new { message = "Username/email and password are required" });
        }

        var result = await _authService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

        if (result == null)
        {
            _log.Warning("Failed login attempt for username/email: {UsernameOrEmail}", loginRequest.Username);

            try
            {
                await _loginHistoryService.CreateLoginHistoryAsync(new CreateLoginHistoryDto
                {
                    UserId = null,
                    Identifier = loginRequest.Username,
                    IsSuccessful = false,
                    AttemptedAt = DateTime.UtcNow,
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    FailureReason = "Invalid username/email or password"
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to write login history entry for unsuccessful login attempt");
            }

            return Unauthorized(new { message = "Invalid username/email or password" });
        }

        _log.Information("Successful login for username: {Username}", loginRequest.Username);

        try
        {
            await _loginHistoryService.CreateLoginHistoryAsync(new CreateLoginHistoryDto
            {
                UserId = result.UserId,
                Identifier = loginRequest.Username,
                IsSuccessful = true,
                AttemptedAt = DateTime.UtcNow,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                UserAgent = Request.Headers.UserAgent.ToString()
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to write login history entry for successful login");
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            _log.Warning("Refresh token attempt with empty token");
            return BadRequest(new { message = "Refresh token is required" });
        }

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (result == null)
        {
            _log.Warning("Failed refresh token attempt");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        _log.Information("Successfully refreshed token for user: {Username}", result.Username);
        return Ok(result);
    }

    /// <summary>
    /// Logs out the current user by revoking their refresh token
    /// </summary>
    /// <param name="request">Request containing the refresh token to revoke</param>
    /// <returns>Success message if logout is successful</returns>
    /// <response code="200">If logout is successful</response>
    /// <response code="400">If refresh token is empty or revocation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            _log.Warning("Logout attempt with empty refresh token");
            return BadRequest(new { message = "Refresh token is required" });
        }

        var success = await _authService.RevokeRefreshTokenAsync(request.RefreshToken);

        if (!success)
        {
            _log.Warning("Failed to revoke refresh token");
            return BadRequest(new { message = "Failed to revoke token" });
        }

        _log.Information("User logged out successfully: {Username}", User.Identity?.Name);
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Test endpoint to verify authentication is working
    /// </summary>
    [HttpGet("verify")]
    [Authorize]
    public ActionResult<object> VerifyToken()
    {
        var username = User.Identity?.Name;
        var roles = User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        _log.Information("Token verified for user: {Username}", username);

        return Ok(new
        {
            username = username,
            roles = roles,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }
}
