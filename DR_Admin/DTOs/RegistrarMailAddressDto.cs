namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a registrar mail address
/// </summary>
public class RegistrarMailAddressDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the registrar mail address
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the mail address
    /// </summary>
    public string MailAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether this is the default mail address for the customer
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Gets or sets whether this mail address is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the mail address was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the mail address was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new registrar mail address
/// </summary>
public class CreateRegistrarMailAddressDto
{
    /// <summary>
    /// Mail address
    /// </summary>
    public string MailAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this is the default mail address for the customer
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Whether this mail address is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing registrar mail address
/// </summary>
public class UpdateRegistrarMailAddressDto
{
    /// <summary>
    /// Mail address
    /// </summary>
    public string MailAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this is the default mail address for the customer
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Whether this mail address is active
    /// </summary>
    public bool IsActive { get; set; }
}
