using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages user roles including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RolesController>();

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Retrieves all roles in the system
    /// </summary>
    /// <returns>List of all roles</returns>
    /// <response code="200">Returns the list of roles</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        try
        {
            _log.Information("API: GetAllRoles called by user {User}", User.Identity?.Name);
            
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRoles");
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }

    /// <summary>
    /// Retrieves a specific role by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the role</param>
    /// <returns>The role information</returns>
    /// <response code="200">Returns the role data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If role is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> GetRoleById(int id)
    {
        try
        {
            _log.Information("API: GetRoleById called for ID {RoleId} by user {User}", id, User.Identity?.Name);
            
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
            {
                _log.Information("API: Role with ID {RoleId} not found", id);
                return NotFound($"Role with ID {id} not found");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRoleById for ID {RoleId}", id);
            return StatusCode(500, "An error occurred while retrieving the role");
        }
    }

    /// <summary>
    /// Creates a new role in the system
    /// </summary>
    /// <param name="createDto">Role information for creation</param>
    /// <returns>The newly created role</returns>
    /// <response code="201">Returns the newly created role</response>
    /// <response code="400">If the role data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createDto)
    {
        try
        {
            _log.Information("API: CreateRole called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRole");
                return BadRequest(ModelState);
            }

            var role = await _roleService.CreateRoleAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = role.Id },
                role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRole");
            return StatusCode(500, "An error occurred while creating the role");
        }
    }

    /// <summary>
    /// Updates an existing role's information
    /// </summary>
    /// <param name="id">The unique identifier of the role to update</param>
    /// <param name="updateDto">Updated role information</param>
    /// <returns>The updated role</returns>
    /// <response code="200">Returns the updated role</response>
    /// <response code="400">If the role data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If role is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRole called for ID {RoleId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRole");
                return BadRequest(ModelState);
            }

            var role = await _roleService.UpdateRoleAsync(id, updateDto);

            if (role == null)
            {
                _log.Information("API: Role with ID {RoleId} not found for update", id);
                return NotFound($"Role with ID {id} not found");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRole for ID {RoleId}", id);
            return StatusCode(500, "An error occurred while updating the role");
        }
    }

    /// <summary>
    /// Deletes a role from the system
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If role was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If role is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteRole(int id)
    {
        try
        {
            _log.Information("API: DeleteRole called for ID {RoleId} by user {User}", id, User.Identity?.Name);

            var result = await _roleService.DeleteRoleAsync(id);

            if (!result)
            {
                _log.Information("API: Role with ID {RoleId} not found for deletion", id);
                return NotFound($"Role with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRole for ID {RoleId}", id);
            return StatusCode(500, "An error occurred while deleting the role");
        }
    }
}
