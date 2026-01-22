using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InitializationController : ControllerBase
{
    private readonly IInitializationService _initializationService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationController>();

    public InitializationController(IInitializationService initializationService)
    {
        _initializationService = initializationService;
    }

    /// <summary>
    /// Initialize the system with the first admin user. Only works if no users exist.
    /// </summary>
    [HttpPost("initialize")]
    [AllowAnonymous]
    public async Task<ActionResult<InitializationResponseDto>> Initialize([FromBody] InitializationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password) || 
            string.IsNullOrWhiteSpace(request.Email))
        {
            _log.Warning("Initialization attempt with missing required fields");
            return BadRequest(new { message = "Username, password, and email are required" });
        }

        var result = await _initializationService.InitializeAsync(request);

        if (result == null)
        {
            _log.Warning("Initialization failed - users may already exist or invalid input");
            return BadRequest(new { message = "Initialization failed. Users may already exist in the system or invalid input provided." });
        }

        _log.Information("System initialized successfully with user: {Username}", result.Username);
        return Ok(result);
    }
}
