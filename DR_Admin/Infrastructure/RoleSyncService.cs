using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Infrastructure;

/// <summary>
/// Service for synchronizing role changes and invalidating caches.
/// Allows runtime updates to user roles without requiring application restart.
/// </summary>
public interface IRoleSyncService
{
    Task AssignRoleToUserAsync(int userId, int roleId);
    Task RemoveRoleFromUserAsync(int userId, int roleId);
    Task SyncUserRolesAsync(int userId, IEnumerable<int> roleIds);
    Task<IEnumerable<Role>> GetAllRolesAsync();
}

public class RoleSyncService : IRoleSyncService
{
    private readonly ApplicationDbContext _context;
    private readonly IRolePermissionService _rolePermissionService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RoleSyncService>();

    public RoleSyncService(
        ApplicationDbContext context,
        IRolePermissionService rolePermissionService)
    {
        _context = context;
        _rolePermissionService = rolePermissionService;
    }

    public async Task AssignRoleToUserAsync(int userId, int roleId)
    {
        try
        {
            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Check if role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found");
            }

            // Check if assignment already exists
            var existingAssignment = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (existingAssignment != null)
            {
                _log.Information("User {UserId} already has role {RoleId}", userId, roleId);
                return;
            }

            // Create new role assignment
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            _rolePermissionService.InvalidateUserRoleCache(userId);

            _log.Information("Assigned role {RoleId} to user {UserId}", roleId, userId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task RemoveRoleFromUserAsync(int userId, int roleId)
    {
        try
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                _log.Warning("User {UserId} does not have role {RoleId}", userId, roleId);
                return;
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            _rolePermissionService.InvalidateUserRoleCache(userId);

            _log.Information("Removed role {RoleId} from user {UserId}", roleId, userId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task SyncUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        try
        {
            _log.Information("Synchronizing roles for user {UserId}", userId);

            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            var roleIdList = roleIds.ToList();

            // Validate all roles exist
            var existingRoleIds = await _context.Roles
                .Where(r => roleIdList.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            var invalidRoleIds = roleIdList.Except(existingRoleIds).ToList();
            if (invalidRoleIds.Any())
            {
                throw new InvalidOperationException(
                    $"Invalid role IDs: {string.Join(", ", invalidRoleIds)}");
            }

            // Get current role assignments
            var currentAssignments = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            var currentRoleIds = currentAssignments.Select(ur => ur.RoleId).ToList();

            // Determine roles to add and remove
            var rolesToAdd = roleIdList.Except(currentRoleIds).ToList();
            var rolesToRemove = currentRoleIds.Except(roleIdList).ToList();

            // Remove old assignments
            var assignmentsToRemove = currentAssignments
                .Where(ur => rolesToRemove.Contains(ur.RoleId))
                .ToList();

            if (assignmentsToRemove.Any())
            {
                _context.UserRoles.RemoveRange(assignmentsToRemove);
            }

            // Add new assignments
            if (rolesToAdd.Any())
            {
                var newAssignments = rolesToAdd.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                }).ToList();

                _context.UserRoles.AddRange(newAssignments);
            }

            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            _rolePermissionService.InvalidateUserRoleCache(userId);

            _log.Information(
                "Synchronized roles for user {UserId}: Added {AddedCount}, Removed {RemovedCount}",
                userId, rolesToAdd.Count, rolesToRemove.Count);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error synchronizing roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        try
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching all roles");
            throw;
        }
    }
}
