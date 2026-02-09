using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Security.Claims;

namespace ISPAdmin.Infrastructure.Authorization;

/// <summary>
/// Custom requirement for resource-based authorization.
/// Allows checking permissions based on the resource being accessed.
/// </summary>
public class ResourcePermissionRequirement : IAuthorizationRequirement
{
    public string ResourceType { get; }
    public string Permission { get; }

    public ResourcePermissionRequirement(string resourceType, string permission)
    {
        ResourceType = resourceType;
        Permission = permission;
    }
}

/// <summary>
/// Handler for resource-based authorization.
/// Evaluates permissions based on user roles loaded from the database.
/// </summary>
public class ResourcePermissionHandler : AuthorizationHandler<ResourcePermissionRequirement>
{
    private readonly IRolePermissionService _rolePermissionService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ResourcePermissionHandler>();

    public ResourcePermissionHandler(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourcePermissionRequirement requirement)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _log.Warning("Authorization failed: User ID not found in claims");
            context.Fail();
            return;
        }

        try
        {
            // Get user's roles from the permission service
            var userRoles = await _rolePermissionService.GetUserRolesAsync(userId);

            // Check if user has required permission based on role and resource type
            var hasPermission = await CheckPermissionAsync(
                userRoles,
                requirement.ResourceType,
                requirement.Permission);

            if (hasPermission)
            {
                _log.Debug("User {UserId} granted {Permission} permission for {ResourceType}",
                    userId, requirement.Permission, requirement.ResourceType);
                context.Succeed(requirement);
            }
            else
            {
                _log.Warning("User {UserId} denied {Permission} permission for {ResourceType}",
                    userId, requirement.Permission, requirement.ResourceType);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error evaluating resource permission for user {UserId}", userId);
            context.Fail();
        }
    }

    private Task<bool> CheckPermissionAsync(
        IEnumerable<string> userRoles,
        string resourceType,
        string permission)
    {
        // Define permission mappings based on roles and resource types
        // This can be extended to load from database or configuration
        var permissionMap = GetPermissionMap();

        var key = $"{resourceType}.{permission}";
        if (permissionMap.TryGetValue(key, out var allowedRoles))
        {
            return Task.FromResult(userRoles.Any(role =>
                allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase)));
        }

        // If no specific mapping found, deny by default
        return Task.FromResult(false);
    }

    private static Dictionary<string, string[]> GetPermissionMap()
    {
        // This could be loaded from database or configuration
        // For now, returning common permission patterns
        return new Dictionary<string, string[]>
        {
            ["Customer.Read"] = new[] { "Admin", "Support", "Sales" },
            ["Customer.Write"] = new[] { "Admin", "Sales" },
            ["Customer.Delete"] = new[] { "Admin" },
            ["Invoice.Read"] = new[] { "Admin", "Support", "Sales" },
            ["Invoice.Write"] = new[] { "Admin", "Sales" },
            ["Invoice.Delete"] = new[] { "Admin" },
            ["User.Read"] = new[] { "Admin", "Support" },
            ["User.Write"] = new[] { "Admin", "Support" },
            ["User.Delete"] = new[] { "Admin" },
            ["Hosting.Read"] = new[] { "Admin", "Hosting" },
            ["Hosting.Write"] = new[] { "Admin", "Hosting" },
            ["Hosting.Delete"] = new[] { "Admin", "Hosting" },
        };
    }
}

/// <summary>
/// Requirement for ownership-based authorization.
/// Checks if the user owns the resource being accessed.
/// </summary>
public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public string ResourceType { get; }

    public ResourceOwnerRequirement(string resourceType)
    {
        ResourceType = resourceType;
    }
}

/// <summary>
/// Handler for ownership-based authorization.
/// Allows users to access their own resources even if they don't have role-based permissions.
/// </summary>
public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, object>
{
    private static readonly Serilog.ILogger _log = Log.ForContext<ResourceOwnerHandler>();

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        object resource)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Check if the resource belongs to the user
        // This is a simplified example - extend based on your domain model
        var isOwner = false;

        if (resource is IHaveOwnerId ownedResource)
        {
            isOwner = ownedResource.OwnerId == userId;
        }
        else if (resource is IHaveCustomerId customerResource)
        {
            // For customer-owned resources, check if user's customer ID matches
            var customerIdClaim = context.User.FindFirst("CustomerId");
            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out var customerId))
            {
                isOwner = customerResource.CustomerId == customerId;
            }
        }

        if (isOwner)
        {
            _log.Debug("User {UserId} granted ownership access to {ResourceType}", userId, requirement.ResourceType);
            context.Succeed(requirement);
        }
        else
        {
            _log.Debug("User {UserId} denied ownership access to {ResourceType}", userId, requirement.ResourceType);
            context.Fail();
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Interface for resources that have an owner (user)
/// </summary>
public interface IHaveOwnerId
{
    int OwnerId { get; }
}

/// <summary>
/// Interface for resources that belong to a customer
/// </summary>
public interface IHaveCustomerId
{
    int CustomerId { get; }
}
