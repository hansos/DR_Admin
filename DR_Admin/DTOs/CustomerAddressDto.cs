namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer address
/// </summary>
public class CustomerAddressDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the customer address
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the address type ID
    /// </summary>
    public int AddressTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the address type code
    /// </summary>
    public string AddressTypeCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the address type name
    /// </summary>
    public string AddressTypeName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the postal code ID
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
    /// Gets or sets the city (from PostalCode)
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the state/province (from PostalCode)
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code (from PostalCode)
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the country code (from PostalCode)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;
    
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
    /// Gets or sets the date and time when the address was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the address was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new customer address
/// </summary>
public class CreateCustomerAddressDto
{
    /// <summary>
    /// Address type ID
    /// </summary>
    public int AddressTypeId { get; set; }
    
    /// <summary>
    /// Postal code ID (references PostalCode table)
    /// </summary>
    public int PostalCodeId { get; set; }
    
    /// <summary>
    /// First address line
    /// </summary>
    public string AddressLine1 { get; set; } = string.Empty;
    
    /// <summary>
    /// Second address line
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Third address line
    /// </summary>
    public string? AddressLine3 { get; set; }
    
    /// <summary>
    /// Fourth address line
    /// </summary>
    public string? AddressLine4 { get; set; }
    
    /// <summary>
    /// Whether this is the primary address for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether this address is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Additional notes about the address
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing customer address
/// </summary>
public class UpdateCustomerAddressDto
{
    /// <summary>
    /// Address type ID
    /// </summary>
    public int AddressTypeId { get; set; }
    
    /// <summary>
    /// Postal code ID (references PostalCode table)
    /// </summary>
    public int PostalCodeId { get; set; }
    
    /// <summary>
    /// First address line
    /// </summary>
    public string AddressLine1 { get; set; } = string.Empty;
    
    /// <summary>
    /// Second address line
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Third address line
    /// </summary>
    public string? AddressLine3 { get; set; }
    
    /// <summary>
    /// Fourth address line
    /// </summary>
    public string? AddressLine4 { get; set; }
    
    /// <summary>
    /// Whether this is the primary address for the customer
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether this address is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Additional notes about the address
    /// </summary>
    public string? Notes { get; set; }
}
