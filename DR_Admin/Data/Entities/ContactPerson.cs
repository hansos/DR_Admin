namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a contact person associated with a customer
/// </summary>
public class ContactPerson : EntityBase
{
    /// <summary>
    /// Gets or sets the first name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the position/title of the contact person
    /// </summary>
    public string? Position { get; set; }
    
    /// <summary>
    /// Gets or sets the department where the contact person works
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary contact person for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets whether the contact person is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets additional notes about the contact person
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this contact person is the default owner/registrant for domain registrations
    /// </summary>
    public bool IsDefaultOwner { get; set; }

    /// <summary>
    /// Gets or sets whether this contact person is the default billing contact for domain registrations
    /// </summary>
    public bool IsDefaultBilling { get; set; }

    /// <summary>
    /// Gets or sets whether this contact person is the default technical contact for domain registrations
    /// </summary>
    public bool IsDefaultTech { get; set; }

    /// <summary>
    /// Gets or sets whether this contact person is the default administrative contact for domain registrations
    /// </summary>
    public bool IsDefaultAdministrator { get; set; }

    /// <summary>
    /// Gets or sets whether this contact person can be used globally across domains
    /// </summary>
    public bool IsDomainGlobal { get; set; }
    
    /// <summary>
    /// Gets or sets the normalized version of the first name for case-insensitive searches
    /// </summary>
    public string NormalizedFirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the normalized version of the last name for case-insensitive searches
    /// </summary>
    public string NormalizedLastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the foreign key to the customer
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with this contact person
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Gets or sets the collection of domain contact assignments for this contact person
    /// </summary>
    public ICollection<DomainContactAssignment> DomainContactAssignments { get; set; } = new List<DomainContactAssignment>();

    /// <summary>
    /// Gets or sets the collection of domain contacts that were sourced from this contact person
    /// </summary>
    public ICollection<DomainContact> SourcedDomainContacts { get; set; } = new List<DomainContact>();
}
