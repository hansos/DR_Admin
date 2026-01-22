using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

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
    /// Get all roles
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
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
    /// Get role by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
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
    /// Create a new role
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    /// Update an existing role
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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
    /// Delete a role
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
