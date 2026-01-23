using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Handles system initialization with the first admin user
/// </summary>
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
    /// Initializes the system with the first admin user (only works if no users exist)
    /// </summary>
    /// <param name="request">Initial admin user credentials and information</param>
    /// <returns>Initialization result with user information</returns>
    /// <response code="200">Returns the initialization result if successful</response>
    /// <response code="400">If required fields are missing, users already exist, or input is invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    /// <remarks>
    /// This endpoint can only be used once to create the first admin user. Subsequent calls will fail.
    /// </remarks>
    [HttpPost("initialize")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(InitializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
