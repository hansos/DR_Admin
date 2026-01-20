using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly Serilog.ILogger _logger;

    public UsersController(IUserService userService, Serilog.ILogger logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            _logger.Information("API: GetAllUsers called by user {User}", User.Identity?.Name);
            
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetAllUsers");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            _logger.Information("API: GetUserById called for ID {UserId} by user {User}", id, User.Identity?.Name);
            
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.Information("API: User with ID {UserId} not found", id);
                return NotFound($"User with ID {id} not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in GetUserById for ID {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
    {
        try
        {
            _logger.Information("API: CreateUser called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for CreateUser");
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in CreateUser");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
    {
        try
        {
            _logger.Information("API: UpdateUser called for ID {UserId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.Warning("API: Invalid model state for UpdateUser");
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, updateDto);

            if (user == null)
            {
                _logger.Information("API: User with ID {UserId} not found for update", id);
                return NotFound($"User with ID {id} not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in UpdateUser for ID {UserId}", id);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            _logger.Information("API: DeleteUser called for ID {UserId} by user {User}", id, User.Identity?.Name);

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                _logger.Information("API: User with ID {UserId} not found for deletion", id);
                return NotFound($"User with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "API: Error in DeleteUser for ID {UserId}", id);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }
}
