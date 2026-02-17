namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a domain contact assignment
/// </summary>
public class DomainContactAssignmentDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the assignment
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the registered domain ID
    /// </summary>
    public int RegisteredDomainId { get; set; }
    
    /// <summary>
    /// Gets or sets the contact person ID
    /// </summary>
    public int ContactPersonId { get; set; }
    
    /// <summary>
    /// Gets or sets the role type (Registrant, Administrative, Technical, Billing)
    /// </summary>
    public string RoleType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets when the assignment was created
    /// </summary>
    public DateTime AssignedAt { get; set; }
    
    /// <summary>
    /// Gets or sets whether the assignment is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets optional notes about the assignment
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the assignment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the assignment was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the contact person details (optional, for expanded results)
    /// </summary>
    public ContactPersonDto? ContactPerson { get; set; }
}

/// <summary>
/// Data transfer object for creating a new domain contact assignment
/// </summary>
public class CreateDomainContactAssignmentDto
{
    /// <summary>
    /// The registered domain ID
    /// </summary>
    public int RegisteredDomainId { get; set; }
    
    /// <summary>
    /// The contact person ID
    /// </summary>
    public int ContactPersonId { get; set; }
    
    /// <summary>
    /// The role type (Registrant, Administrative, Technical, Billing)
    /// </summary>
    public string RoleType { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the assignment is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Optional notes about the assignment
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing domain contact assignment
/// </summary>
public class UpdateDomainContactAssignmentDto
{
    /// <summary>
    /// The contact person ID
    /// </summary>
    public int ContactPersonId { get; set; }
    
    /// <summary>
    /// The role type (Registrant, Administrative, Technical, Billing)
    /// </summary>
    public string RoleType { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the assignment is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Optional notes about the assignment
    /// </summary>
    public string? Notes { get; set; }
}
