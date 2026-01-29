using ISPAdmin.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace ISPAdmin.Infrastructure;

/// <summary>
/// Transforms user claims by loading roles from the database and adding them to the ClaimsPrincipal.
/// This bridges database roles to runtime authorization on each authenticated request.
/// </summary>
public class DbRoleClaimsTransformation : IClaimsTransformation
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DbRoleClaimsTransformation>();

    public DbRoleClaimsTransformation(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Skip transformation if not authenticated
        if (!principal.Identity?.IsAuthenticated ?? true)
        {
            return principal;
        }

        // Clone the principal to avoid modifying the original
        var clone = principal.Clone();
        var claimsIdentity = (ClaimsIdentity?)clone.Identity;
        
        if (claimsIdentity == null)
        {
            return principal;
        }

        try
        {
            // Extract user ID from claims (set during JWT generation)
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                _log.Warning("User ID claim not found or invalid in JWT token");
                return principal;
            }

            // Check if roles have already been loaded in this request
            var hasDbRolesMarker = principal.HasClaim(c => c.Type == "db_roles_loaded");
            if (hasDbRolesMarker)
            {
                return principal; // Already transformed
            }

            // Load user's roles from database
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            // Remove any existing role claims (from JWT) to avoid duplicates
            var existingRoleClaims = claimsIdentity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .ToList();

            foreach (var claim in existingRoleClaims)
            {
                claimsIdentity.RemoveClaim(claim);
            }

            // Add fresh role claims from database
            foreach (var role in userRoles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            // Add marker to prevent re-transformation in the same request
            claimsIdentity.AddClaim(new Claim("db_roles_loaded", "true"));

            _log.Debug("Loaded {RoleCount} roles from database for user {UserId}", userRoles.Count, userId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error loading roles from database during claims transformation");
            // Return original principal on error to avoid breaking authentication
            return principal;
        }

        return clone;
    }
}
