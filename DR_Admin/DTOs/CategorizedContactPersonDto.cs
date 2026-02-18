using ISPAdmin.Data.Entities;

namespace ISPAdmin.DTOs;

/// <summary>
/// Response containing categorized contact persons
/// </summary>
public class CategorizedContactPersonListResponse
{
    /// <summary>
    /// List of contact persons with their categories
    /// </summary>
    public List<CategorizedContactPersonDto> ContactPersons { get; set; } = new();
}

/// <summary>
/// Contact person with categorization for role-based selection
/// </summary>
public class CategorizedContactPersonDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the contact person
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the first name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the full name of the contact person
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
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
    public bool IsActive { get; set; }
    
    /// <summary>
    /// The category this contact person falls into for the requested role
    /// </summary>
    public ContactPersonCategory Category { get; set; }
    
    /// <summary>
    /// Number of times this contact person has been used for the requested role
    /// </summary>
    public int UsageCount { get; set; }
    
    /// <summary>
    /// The customer ID associated with this contact person
    /// </summary>
    public int? CustomerId { get; set; }
}

/// <summary>
/// Categorization of contact persons based on their usage and preferences
/// </summary>
public enum ContactPersonCategory
{
    /// <summary>
    /// Contact person is marked as default/preferred for this role
    /// </summary>
    Preferred = 1,
    
    /// <summary>
    /// Contact person has been frequently used for this role
    /// </summary>
    FrequentlyUsed = 2,
    
    /// <summary>
    /// Contact person is available but not frequently used or preferred
    /// </summary>
    Available = 3
}
