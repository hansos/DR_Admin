namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of vendor
/// </summary>
public enum VendorType
{
    /// <summary>
    /// Domain registrar (e.g., GoDaddy, Namecheap)
    /// </summary>
    Registrar = 0,
    
    /// <summary>
    /// Hosting infrastructure provider
    /// </summary>
    HostingProvider = 1,
    
    /// <summary>
    /// Email service provider
    /// </summary>
    EmailProvider = 2,
    
    /// <summary>
    /// Other vendor type
    /// </summary>
    Other = 3
}
