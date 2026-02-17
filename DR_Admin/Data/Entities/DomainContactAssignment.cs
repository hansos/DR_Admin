namespace ISPAdmin.Data.Entities;

/// <summary>
/// Bridge entity that links a ContactPerson to a RegisteredDomain with a specific role.
/// This represents the normalized relationship between master contact data and domains.
/// </summary>
public class DomainContactAssignment : EntityBase
{
    /// <summary>
    /// Gets or sets the foreign key to the registered domain
    /// </summary>
    public int RegisteredDomainId { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key to the contact person
    /// </summary>
    public int ContactPersonId { get; set; }
    
    /// <summary>
    /// Gets or sets the role this contact person has for the domain
    /// </summary>
    public ContactRoleType RoleType { get; set; }
    
    /// <summary>
    /// Gets or sets when this assignment was created
    /// </summary>
    public DateTime AssignedAt { get; set; }
    
    /// <summary>
    /// Gets or sets whether this assignment is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets optional notes about this specific assignment
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Navigation property to the registered domain
    /// </summary>
    public RegisteredDomain RegisteredDomain { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the contact person
    /// </summary>
    public ContactPerson ContactPerson { get; set; } = null!;
}
