using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ISPAdmin.Services;

public class RoleInitializationService
{
    /// <summary>
    /// Scans all controllers in the application and extracts unique role names from Authorize attributes
    /// </summary>
    /// <returns>A list of unique role names found in controller Authorize attributes</returns>
    public static List<string> GetAllRolesFromControllers()
    {
        var roles = new HashSet<string>();
        
        // Get all types in the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        
        // Find all controller types
        var controllerTypes = assembly.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            // Check class-level Authorize attributes
            var classAuthorizeAttributes = controllerType.GetCustomAttributes<AuthorizeAttribute>();
            ExtractRolesFromAttributes(classAuthorizeAttributes, roles);

            // Check method-level Authorize attributes
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodAuthorizeAttributes = method.GetCustomAttributes<AuthorizeAttribute>();
                ExtractRolesFromAttributes(methodAuthorizeAttributes, roles);
            }
        }

        return roles.OrderBy(r => r).ToList();
    }

    /// <summary>
    /// Extracts role names from Authorize attributes and adds them to the roles set
    /// </summary>
    private static void ExtractRolesFromAttributes(IEnumerable<AuthorizeAttribute> attributes, HashSet<string> roles)
    {
        foreach (var attribute in attributes)
        {
            if (!string.IsNullOrWhiteSpace(attribute.Roles))
            {
                // Roles can be comma-separated, so split them
                var roleNames = attribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .Where(r => !string.IsNullOrWhiteSpace(r));

                foreach (var roleName in roleNames)
                {
                    roles.Add(roleName);
                }
            }
        }
    }
}
