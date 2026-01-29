using ISPAdmin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace ISPAdmin.Infrastructure;

/// <summary>
/// Service for managing role-based permissions with caching support.
/// Provides dynamic policy evaluation based on database roles.
/// </summary>
public interface IRolePermissionService
{
    Task<bool> UserHasRoleAsync(int userId, string roleName);
    Task<bool> UserHasAnyRoleAsync(int userId, params string[] roleNames);
    Task<bool> UserHasAllRolesAsync(int userId, params string[] roleNames);
    Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    void InvalidateUserRoleCache(int userId);
    void InvalidateAllRoleCache();
}

public class RolePermissionService : IRolePermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly Serilog.ILogger _log = Log.ForContext<RolePermissionService>();
    private const string CacheKeyPrefix = "UserRoles_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public RolePermissionService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out List<string>? cachedRoles) && cachedRoles != null)
        {
            _log.Debug("Retrieved roles from cache for user {UserId}", userId);
            return cachedRoles;
        }

        // Load from database
        try
        {
            var roles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            // Cache the result
            _cache.Set(cacheKey, roles, CacheDuration);
            _log.Debug("Loaded and cached {RoleCount} roles for user {UserId}", roles.Count, userId);

            return roles;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error loading roles for user {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> UserHasRoleAsync(int userId, string roleName)
    {
        var roles = await GetUserRolesAsync(userId);
        return roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UserHasAnyRoleAsync(int userId, params string[] roleNames)
    {
        if (roleNames == null || roleNames.Length == 0)
        {
            return false;
        }

        var roles = await GetUserRolesAsync(userId);
        return roleNames.Any(reqRole => 
            roles.Any(userRole => userRole.Equals(reqRole, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<bool> UserHasAllRolesAsync(int userId, params string[] roleNames)
    {
        if (roleNames == null || roleNames.Length == 0)
        {
            return false;
        }

        var roles = await GetUserRolesAsync(userId);
        var userRolesSet = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        return roleNames.All(reqRole => userRolesSet.Contains(reqRole));
    }

    public void InvalidateUserRoleCache(int userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        _cache.Remove(cacheKey);
        _log.Information("Invalidated role cache for user {UserId}", userId);
    }

    public void InvalidateAllRoleCache()
    {
        // Note: This is a simple implementation. For production, consider using a more sophisticated
        // cache invalidation strategy or a distributed cache with tag-based invalidation.
        _log.Information("Cache invalidation requested for all user roles");
        
        // Since MemoryCache doesn't support pattern-based removal, we rely on the cache expiration.
        // For immediate effect, you could maintain a list of active user IDs or use a different cache strategy.
    }
}
