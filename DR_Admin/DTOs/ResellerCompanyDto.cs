namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a reseller company
/// </summary>
public class ResellerCompanyDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the reseller company
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the contact person name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Gets or sets the company email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the company phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the company street address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the company registration number
    /// </summary>
    public string? CompanyRegistrationNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Gets or sets the VAT number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Gets or sets whether the reseller company is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether this is the default reseller company
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Gets or sets additional notes about the reseller company
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the default currency for this reseller (ISO 4217 code)
    /// </summary>
    public string DefaultCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Gets or sets the comma-separated list of supported currencies
    /// </summary>
    public string? SupportedCurrencies { get; set; }
    
    /// <summary>
    /// Gets or sets whether to automatically apply currency markup
    /// </summary>
    public bool ApplyCurrencyMarkup { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the default currency markup percentage
    /// </summary>
    public decimal DefaultCurrencyMarkup { get; set; } = 0m;
    
    /// <summary>
    /// Gets or sets the date and time when the reseller company was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the reseller company was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new reseller company
/// </summary>
public class CreateResellerCompanyDto
{
    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the contact person name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Gets or sets the company email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the company phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the company street address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the company registration number
    /// </summary>
    public string? CompanyRegistrationNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Gets or sets the VAT number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Gets or sets whether the reseller company is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether this is the default reseller company
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Gets or sets additional notes about the reseller company
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the default currency for this reseller (ISO 4217 code)
    /// </summary>
    public string DefaultCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Gets or sets the comma-separated list of supported currencies
    /// </summary>
    public string? SupportedCurrencies { get; set; }
    
    /// <summary>
    /// Gets or sets whether to automatically apply currency markup
    /// </summary>
    public bool ApplyCurrencyMarkup { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the default currency markup percentage
    /// </summary>
    public decimal DefaultCurrencyMarkup { get; set; } = 0m;
}

/// <summary>
/// Data transfer object for updating an existing reseller company
/// </summary>
public class UpdateResellerCompanyDto
{
    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the contact person name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Gets or sets the company email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the company phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the company street address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the company registration number
    /// </summary>
    public string? CompanyRegistrationNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Gets or sets the VAT number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Gets or sets whether the reseller company is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the default reseller company
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Gets or sets additional notes about the reseller company
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the default currency for this reseller (ISO 4217 code)
    /// </summary>
    public string DefaultCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Gets or sets the comma-separated list of supported currencies
    /// </summary>
    public string? SupportedCurrencies { get; set; }
    
    /// <summary>
    /// Gets or sets whether to automatically apply currency markup
    /// </summary>
    public bool ApplyCurrencyMarkup { get; set; }
    
    /// <summary>
    /// Gets or sets the default currency markup percentage
    /// </summary>
    public decimal DefaultCurrencyMarkup { get; set; }
}

