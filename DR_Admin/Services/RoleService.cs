using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RoleService>();

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        try
        {
            _log.Information("Fetching all roles");
            
            var roles = await _context.Roles
                .AsNoTracking()
                .ToListAsync();

            var roleDtos = roles.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} roles", roles.Count);
            return roleDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all roles");
            throw;
        }
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching role with ID: {RoleId}", id);
            
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                _log.Warning("Role with ID {RoleId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched role with ID: {RoleId}", id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching role with ID: {RoleId}", id);
            throw;
        }
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createDto)
    {
        try
        {
            _log.Information("Creating new role with name: {RoleName}", createDto.Name);

            var role = new Role
            {
                Name = createDto.Name,
                Description = createDto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created role with ID: {RoleId}", role.Id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating role with name: {RoleName}", createDto.Name);
            throw;
        }
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateDto)
    {
        try
        {
            _log.Information("Updating role with ID: {RoleId}", id);

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                _log.Warning("Role with ID {RoleId} not found for update", id);
                return null;
            }

            role.Name = updateDto.Name;
            role.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated role with ID: {RoleId}", id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating role with ID: {RoleId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        try
        {
            _log.Information("Deleting role with ID: {RoleId}", id);

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                _log.Warning("Role with ID {RoleId} not found for deletion", id);
                return false;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted role with ID: {RoleId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting role with ID: {RoleId}", id);
            throw;
        }
    }

    public async Task EnsureRoleExistsAsync(string roleName, string description = "")
    {
        try
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (existingRole == null)
            {
                _log.Information("Creating role: {RoleName}", roleName);
                
                var role = new Role
                {
                    Name = roleName,
                    Description = string.IsNullOrWhiteSpace(description) 
                        ? $"{roleName} role" 
                        : description
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                
                _log.Information("Successfully created role: {RoleName} with ID: {RoleId}", roleName, role.Id);
            }
            else
            {
                _log.Debug("Role {RoleName} already exists with ID: {RoleId}", roleName, existingRole.Id);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while ensuring role exists: {RoleName}", roleName);
            throw;
        }
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }
}
