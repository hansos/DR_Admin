using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public RoleService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        try
        {
            _logger.Information("Fetching all roles");
            
            var roles = await _context.Roles
                .AsNoTracking()
                .ToListAsync();

            var roleDtos = roles.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} roles", roles.Count);
            return roleDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all roles");
            throw;
        }
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching role with ID: {RoleId}", id);
            
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                _logger.Warning("Role with ID {RoleId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched role with ID: {RoleId}", id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching role with ID: {RoleId}", id);
            throw;
        }
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createDto)
    {
        try
        {
            _logger.Information("Creating new role with name: {RoleName}", createDto.Name);

            var role = new Role
            {
                Name = createDto.Name,
                Description = createDto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created role with ID: {RoleId}", role.Id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating role with name: {RoleName}", createDto.Name);
            throw;
        }
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateDto)
    {
        try
        {
            _logger.Information("Updating role with ID: {RoleId}", id);

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                _logger.Warning("Role with ID {RoleId} not found for update", id);
                return null;
            }

            role.Name = updateDto.Name;
            role.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated role with ID: {RoleId}", id);
            return MapToDto(role);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating role with ID: {RoleId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        try
        {
            _logger.Information("Deleting role with ID: {RoleId}", id);

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                _logger.Warning("Role with ID {RoleId} not found for deletion", id);
                return false;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted role with ID: {RoleId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting role with ID: {RoleId}", id);
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
