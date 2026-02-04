namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a registrar mail address
/// </summary>
public class RegistrarMailAddress : EntityBase
{
    /// <summary>
    /// Gets or sets the customer ID (foreign key to Customer table)
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
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;
}
