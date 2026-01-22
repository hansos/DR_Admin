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
    private static readonly Serilog.ILogger _log = Log.ForContext<AuthController>();

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login endpoint to get JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            _log.Warning("Login attempt with empty username or password");
            return BadRequest(new { message = "Username and password are required" });
        }

        var result = await _authService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

        if (result == null)
        {
            _log.Warning("Failed login attempt for username: {Username}", loginRequest.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _log.Information("Successful login for username: {Username}", loginRequest.Username);
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
    /// Logout endpoint to revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
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
