namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a type of address that can be assigned to customer addresses
/// </summary>
public class AddressType : EntityBase
{
    /// <summary>
    /// Gets or sets the unique code for the address type
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the address type
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the address type
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether this address type is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the default address type for new addresses
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for displaying address types
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the normalized version of Code for case-insensitive searches
    /// </summary>
    public string NormalizedCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the collection of customer addresses with this address type
    /// </summary>
    public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
}
