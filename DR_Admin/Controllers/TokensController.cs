using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages authentication and refresh tokens
/// </summary>
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
    /// Retrieves all tokens in the system (Admin only)
    /// </summary>
    /// <returns>List of all tokens</returns>
    /// <response code="200">Returns the list of tokens</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<TokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves a specific token by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the token</param>
    /// <returns>The token information</returns>
    /// <response code="200">Returns the token data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If token is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Creates a new token (Admin only)
    /// </summary>
    /// <param name="createDto">Token information for creation</param>
    /// <returns>The newly created token</returns>
    /// <response code="201">Returns the newly created token</response>
    /// <response code="400">If the token data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Updates an existing token's information
    /// </summary>
    /// <param name="id">The unique identifier of the token to update</param>
    /// <param name="updateDto">Updated token information</param>
    /// <returns>The updated token</returns>
    /// <response code="200">Returns the updated token</response>
    /// <response code="400">If the token data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If token is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
