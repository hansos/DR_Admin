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
    private readonly Serilog.ILogger _logger;

    public AuthController(IAuthService authService, Serilog.ILogger logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint to get JWT token
    /// </summary>
    /// <param name="loginRequest">Username and password</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            _logger.Warning("Login attempt with empty username or password");
            return BadRequest(new { message = "Username and password are required" });
        }

        var result = await _authService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

        if (result == null)
        {
            _logger.Warning("Failed login attempt for username: {Username}", loginRequest.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _logger.Information("Successful login for username: {Username}", loginRequest.Username);
        return Ok(result);
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

        _logger.Information("Token verified for user: {Username}", username);

        return Ok(new
        {
            username = username,
            roles = roles,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }
}
