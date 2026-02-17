namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents the different roles a contact person can have for a domain
/// </summary>
public enum ContactRoleType
{
    /// <summary>
    /// The registrant/owner of the domain
    /// </summary>
    Registrant = 1,
    
    /// <summary>
    /// The administrative contact for the domain
    /// </summary>
    Administrative = 2,
    
    /// <summary>
    /// The technical contact for the domain
    /// </summary>
    Technical = 3,
    
    /// <summary>
    /// The billing contact for the domain
    /// </summary>
    Billing = 4
}
