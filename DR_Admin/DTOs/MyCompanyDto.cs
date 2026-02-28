namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing the reseller's own company profile.
/// </summary>
public class MyCompanyDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the legal registered company name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the company email used for communication.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the company phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the first address line.
    /// </summary>
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second address line.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or region.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2).
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the organization or registration number.
    /// </summary>
    public string? OrganizationNumber { get; set; }

    /// <summary>
    /// Gets or sets the tax id for local taxation.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets the VAT number.
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Gets or sets the billing email used for invoices.
    /// </summary>
    public string? InvoiceEmail { get; set; }

    /// <summary>
    /// Gets or sets the website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the logo URL used in visual templates.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets optional letterhead footer text.
    /// </summary>
    public string? LetterheadFooter { get; set; }

    /// <summary>
    /// Gets or sets when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating or updating the reseller's own company profile.
/// </summary>
public class UpsertMyCompanyDto
{
    /// <summary>
    /// Gets or sets the display name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the legal registered company name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the company email used for communication.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the company phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the first address line.
    /// </summary>
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second address line.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or region.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2).
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the organization or registration number.
    /// </summary>
    public string? OrganizationNumber { get; set; }

    /// <summary>
    /// Gets or sets the tax id for local taxation.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets the VAT number.
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Gets or sets the billing email used for invoices.
    /// </summary>
    public string? InvoiceEmail { get; set; }

    /// <summary>
    /// Gets or sets the website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the logo URL used in visual templates.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets optional letterhead footer text.
    /// </summary>
    public string? LetterheadFooter { get; set; }
}