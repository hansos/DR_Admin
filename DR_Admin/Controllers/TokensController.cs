using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TokensController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TokensController>();

    public TokensController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Get all tokens
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<TokenDto>>> GetAllTokens()
    {
        try
        {
            _log.Information("API: GetAllTokens called by user {User}", User.Identity?.Name);
            
            var tokens = await _tokenService.GetAllTokensAsync();
            return Ok(tokens);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTokens");
            return StatusCode(500, "An error occurred while retrieving tokens");
        }
    }

    /// <summary>
    /// Get token by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TokenDto>> GetTokenById(int id)
    {
        try
        {
            _log.Information("API: GetTokenById called for ID {TokenId} by user {User}", id, User.Identity?.Name);
            
            var token = await _tokenService.GetTokenByIdAsync(id);

            if (token == null)
            {
                _log.Information("API: Token with ID {TokenId} not found", id);
                return NotFound($"Token with ID {id} not found");
            }

            return Ok(token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTokenById for ID {TokenId}", id);
            return StatusCode(500, "An error occurred while retrieving the token");
        }
    }

    /// <summary>
    /// Create a new token
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TokenDto>> CreateToken([FromBody] CreateTokenDto createDto)
    {
        try
        {
            _log.Information("API: CreateToken called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateToken");
                return BadRequest(ModelState);
            }

            var token = await _tokenService.CreateTokenAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetTokenById),
                new { id = token.Id },
                token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateToken");
            return StatusCode(500, "An error occurred while creating the token");
        }
    }

    /// <summary>
    /// Update an existing token
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TokenDto>> UpdateToken(int id, [FromBody] UpdateTokenDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateToken called for ID {TokenId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateToken");
                return BadRequest(ModelState);
            }

            var token = await _tokenService.UpdateTokenAsync(id, updateDto);

            if (token == null)
            {
                _log.Information("API: Token with ID {TokenId} not found for update", id);
                return NotFound($"Token with ID {id} not found");
            }

            return Ok(token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateToken for ID {TokenId}", id);
            return StatusCode(500, "An error occurred while updating the token");
        }
    }

    /// <summary>
    /// Delete a token
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteToken(int id)
    {
        try
        {
            _log.Information("API: DeleteToken called for ID {TokenId} by user {User}", id, User.Identity?.Name);

            var result = await _tokenService.DeleteTokenAsync(id);

            if (!result)
            {
                _log.Information("API: Token with ID {TokenId} not found for deletion", id);
                return NotFound($"Token with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteToken for ID {TokenId}", id);
            return StatusCode(500, "An error occurred while deleting the token");
        }
    }
}
