namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the authentication status for payment
/// </summary>
public enum AuthenticationStatus
{
    /// <summary>
    /// No authentication required
    /// </summary>
    NotRequired = 0,
    
    /// <summary>
    /// Authentication is pending
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Customer is authenticating
    /// </summary>
    InProgress = 2,
    
    /// <summary>
    /// Authentication succeeded
    /// </summary>
    Succeeded = 3,
    
    /// <summary>
    /// Authentication failed
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// Authentication expired
    /// </summary>
    Expired = 5
}
