namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of order
/// </summary>
public enum OrderType
{
    /// <summary>
    /// New service order
    /// </summary>
    New = 0,
    
    /// <summary>
    /// Service renewal
    /// </summary>
    Renewal = 1,
    
    /// <summary>
    /// Service upgrade
    /// </summary>
    Upgrade = 2,
    
    /// <summary>
    /// Service downgrade
    /// </summary>
    Downgrade = 3,
    
    /// <summary>
    /// Add-on to existing service
    /// </summary>
    Addon = 4
}
