namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a customer address with a specific address type
/// </summary>
public class CustomerAddress : EntityBase
{
    /// <summary>
    /// Gets or sets the customer ID (foreign key to Customer table)
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the address type ID (foreign key to AddressType table)
    /// </summary>
    public int AddressTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code ID (foreign key to PostalCode table)
    /// </summary>
    public int PostalCodeId { get; set; }
    
    /// <summary>
    /// Gets or sets the first address line
    /// </summary>
    public string AddressLine1 { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the second address line
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Gets or sets the third address line
    /// </summary>
    public string? AddressLine3 { get; set; }
    
    /// <summary>
    /// Gets or sets the fourth address line
    /// </summary>
    public string? AddressLine4 { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary address for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets whether this address is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the address
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the address type
    /// </summary>
    public AddressType AddressType { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the postal code (contains city, state, postal code, and country)
    /// </summary>
    public PostalCode PostalCode { get; set; } = null!;
}
