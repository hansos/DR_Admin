namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a contact person
/// </summary>
public class ContactPersonDto
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
    /// Gets or sets additional notes about the contact person
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID associated with this contact person
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the contact person was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the contact person was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new contact person
/// </summary>
public class CreateContactPersonDto
{
    /// <summary>
    /// First name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Position/title of the contact person
    /// </summary>
    public string? Position { get; set; }
    
    /// <summary>
    /// Department where the contact person works
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Whether this is the primary contact person for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether the contact person is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Additional notes about the contact person
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Customer ID associated with this contact person
    /// </summary>
    public int CustomerId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing contact person
/// </summary>
public class UpdateContactPersonDto
{
    /// <summary>
    /// First name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Position/title of the contact person
    /// </summary>
    public string? Position { get; set; }
    
    /// <summary>
    /// Department where the contact person works
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Whether this is the primary contact person for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether the contact person is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Additional notes about the contact person
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Customer ID associated with this contact person
    /// </summary>
    public int CustomerId { get; set; }
}
