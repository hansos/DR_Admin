namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a domain contact person
/// </summary>
public class DomainContactDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the domain contact
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of contact (Registrant, Admin, Technical, Billing)
    /// </summary>
    public string ContactType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization/company name
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    /// Gets or sets the email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the fax number of the contact person
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// Gets or sets the first line of the address
    /// </summary>
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the second line of the address
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state/province/region
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the postal/ZIP code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this contact is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the contact
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the domain ID associated with this contact
    /// </summary>
    public int DomainId { get; set; }

    /// <summary>
    /// Gets or sets the source contact person ID (if linked to master data)
    /// </summary>
    public int? SourceContactPersonId { get; set; }

    /// <summary>
    /// Gets or sets when this contact was last synced with the registrar
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Gets or sets whether this contact needs to be synced to the registrar
    /// </summary>
    public bool NeedsSync { get; set; }

    /// <summary>
    /// Gets or sets the registrar contact ID (external reference)
    /// </summary>
    public string? RegistrarContactId { get; set; }

    /// <summary>
    /// Gets or sets the registrar type
    /// </summary>
    public string? RegistrarType { get; set; }

    /// <summary>
    /// Gets or sets whether privacy protection is enabled
    /// </summary>
    public bool IsPrivacyProtected { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current version
    /// </summary>
    public bool IsCurrentVersion { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the contact was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the contact was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new domain contact
/// </summary>
public class CreateDomainContactDto
{
    /// <summary>
    /// Type of contact (Registrant, Admin, Technical, Billing)
    /// </summary>
    public string ContactType { get; set; } = string.Empty;

    /// <summary>
    /// First name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Organization/company name
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    /// Email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Fax number of the contact person
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// First line of the address
    /// </summary>
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// Second line of the address
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State/province/region
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether this contact is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional notes about the contact
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Domain ID associated with this contact
    /// </summary>
    public int DomainId { get; set; }

    /// <summary>
    /// Source contact person ID (if linked to master data)
    /// </summary>
    public int? SourceContactPersonId { get; set; }

    /// <summary>
    /// Registrar contact ID (external reference)
    /// </summary>
    public string? RegistrarContactId { get; set; }

    /// <summary>
    /// Registrar type
    /// </summary>
    public string? RegistrarType { get; set; }

    /// <summary>
    /// Whether privacy protection is enabled
    /// </summary>
    public bool IsPrivacyProtected { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing domain contact
/// </summary>
public class UpdateDomainContactDto
{
    /// <summary>
    /// Type of contact (Registrant, Admin, Technical, Billing)
    /// </summary>
    public string ContactType { get; set; } = string.Empty;

    /// <summary>
    /// First name of the contact person
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the contact person
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Organization/company name
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    /// Email address of the contact person
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the contact person
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Fax number of the contact person
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// First line of the address
    /// </summary>
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// Second line of the address
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State/province/region
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether this contact is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Additional notes about the contact
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Domain ID associated with this contact
    /// </summary>
    public int DomainId { get; set; }
}
