using ISPAdmin.Data.Entities;

namespace ISPAdmin.Utilities;

/// <summary>
/// Helper class for parsing and normalizing contact role type strings from various registrars
/// </summary>
public static class ContactRoleTypeHelper
{
    /// <summary>
    /// Mapping dictionary for contact type string variations to ContactRoleType enum
    /// Supports aliases used by different registrars (AWS Route 53, Namecheap, GoDaddy, etc.)
    /// </summary>
    private static readonly Dictionary<string, ContactRoleType> _mapping = new(StringComparer.OrdinalIgnoreCase)
    {
        // Standard names (match enum values exactly)
        { "Registrant", ContactRoleType.Registrant },
        { "Administrative", ContactRoleType.Administrative },
        { "Technical", ContactRoleType.Technical },
        { "Billing", ContactRoleType.Billing },
        
        // Common shortened versions (Namecheap, etc.)
        { "Admin", ContactRoleType.Administrative },
        { "Tech", ContactRoleType.Technical },
        
        // Full word variations
        { "Administrator", ContactRoleType.Administrative },
        { "Technician", ContactRoleType.Technical },
        
        // Alternative terms
        { "Owner", ContactRoleType.Registrant },
        { "Registrar", ContactRoleType.Registrant },
        
        // Lowercase variations (OpenSrs, Oxxa, etc.)
        { "registrant", ContactRoleType.Registrant },
        { "administrative", ContactRoleType.Administrative },
        { "admin", ContactRoleType.Administrative },
        { "technical", ContactRoleType.Technical },
        { "tech", ContactRoleType.Technical },
        { "billing", ContactRoleType.Billing }
    };
    
    /// <summary>
    /// Attempts to parse a contact type string to ContactRoleType enum
    /// Handles various string formats and aliases from different registrars
    /// </summary>
    /// <param name="contactType">The contact type string to parse</param>
    /// <param name="result">The parsed ContactRoleType enum value</param>
    /// <returns>True if parsing succeeded, false otherwise</returns>
    public static bool TryParse(string? contactType, out ContactRoleType result)
    {
        if (string.IsNullOrWhiteSpace(contactType))
        {
            result = default;
            return false;
        }
        
        // Try mapping dictionary first (handles all known variations)
        if (_mapping.TryGetValue(contactType.Trim(), out result))
        {
            return true;
        }
        
        // Fallback to standard enum parsing (for any new standard values)
        return Enum.TryParse(contactType.Trim(), ignoreCase: true, out result);
    }
    
    /// <summary>
    /// Parses a contact type string to ContactRoleType enum
    /// Throws ArgumentException if parsing fails
    /// </summary>
    /// <param name="contactType">The contact type string to parse</param>
    /// <returns>The parsed ContactRoleType enum value</returns>
    /// <exception cref="ArgumentException">Thrown when contact type is invalid</exception>
    public static ContactRoleType Parse(string? contactType)
    {
        if (TryParse(contactType, out var result))
        {
            return result;
        }
        
        throw new ArgumentException($"Invalid contact type: '{contactType}'. " +
            $"Valid values are: {string.Join(", ", _mapping.Keys.Take(4))}", nameof(contactType));
    }
    
    /// <summary>
    /// Gets the standard/canonical string representation for a ContactRoleType
    /// Useful when sending data to registrars that expect specific formats
    /// </summary>
    /// <param name="roleType">The ContactRoleType enum value</param>
    /// <returns>The standard string representation</returns>
    public static string ToStandardString(ContactRoleType roleType)
    {
        return roleType.ToString();
    }
    
    /// <summary>
    /// Gets the registrar-specific string format for a ContactRoleType
    /// Some registrars expect shortened versions like "Admin" instead of "Administrative"
    /// </summary>
    /// <param name="roleType">The ContactRoleType enum value</param>
    /// <param name="registrarType">The registrar type (e.g., "Namecheap", "AWS_Route53")</param>
    /// <returns>The registrar-specific string representation</returns>
    public static string ToRegistrarString(ContactRoleType roleType, string registrarType)
    {
        // Registrar-specific mappings
        return registrarType?.ToUpperInvariant() switch
        {
            "NAMECHEAP" => roleType switch
            {
                ContactRoleType.Administrative => "Admin",
                ContactRoleType.Technical => "Tech",
                _ => roleType.ToString()
            },
            "OPENSRS" or "OXXA" => roleType switch
            {
                ContactRoleType.Registrant => "registrant",
                ContactRoleType.Administrative => "admin",
                ContactRoleType.Technical => "tech",
                ContactRoleType.Billing => "billing",
                _ => roleType.ToString().ToLowerInvariant()
            },
            // AWS Route 53 and most others use standard names
            _ => roleType.ToString()
        };
    }
}
