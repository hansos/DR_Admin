namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a snapshot/cache of contact information as it was sent to or received from a domain registrar.
/// This serves as an audit trail showing exactly what the registrar has on file.
/// The master/normalized contact data lives in ContactPerson.
/// </summary>
public class DomainContact : EntityBase
{
    /// <summary>
    /// Gets or sets the role type for this contact on the domain
    /// </summary>
    public ContactRoleType RoleType { get; set; }

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
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets additional notes about the contact
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the normalized version of the first name for case-insensitive searches
    /// </summary>
    public string NormalizedFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized version of the last name for case-insensitive searches
    /// </summary>
    public string NormalizedLastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized version of the email for case-insensitive searches
    /// </summary>
    public string NormalizedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the domain
    /// </summary>
    public int DomainId { get; set; }

    /// <summary>
    /// Gets or sets the domain associated with this contact
    /// </summary>
    public RegisteredDomain Domain { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional foreign key linking back to the master ContactPerson record.
    /// If null, this is a standalone registrar contact not managed in our system.
    /// </summary>
    public int? SourceContactPersonId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the source ContactPerson (if linked)
    /// </summary>
    public ContactPerson? SourceContactPerson { get; set; }

    /// <summary>
    /// Gets or sets when this contact was last synchronized with the registrar
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }

    /// <summary>
    /// Gets or sets whether this contact needs to be synchronized to the registrar
    /// </summary>
    public bool NeedsSync { get; set; }

    /// <summary>
    /// Gets or sets the external contact ID from the registrar (if applicable)
    /// </summary>
    public string? RegistrarContactId { get; set; }

    /// <summary>
    /// Gets or sets the type/name of the registrar this contact is associated with
    /// </summary>
    public string? RegistrarType { get; set; }

    /// <summary>
    /// Gets or sets whether this domain uses privacy protection (WHOIS privacy)
    /// </summary>
    public bool IsPrivacyProtected { get; set; }

    /// <summary>
    /// Gets or sets a JSON snapshot of the exact data received from/sent to the registrar.
    /// This serves as a complete audit trail of registrar interactions.
    /// </summary>
    public string? RegistrarSnapshot { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current/active version of the contact for this domain and role.
    /// Used if implementing historical versioning (append-only pattern).
    /// </summary>
    public bool IsCurrentVersion { get; set; } = true;
}
