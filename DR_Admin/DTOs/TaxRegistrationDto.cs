namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a seller tax registration.
/// </summary>
public class TaxRegistrationDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tax jurisdiction identifier.
    /// </summary>
    public int TaxJurisdictionId { get; set; }

    /// <summary>
    /// Gets or sets the legal entity name.
    /// </summary>
    public string LegalEntityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registration number.
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets effective start date.
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// Gets or sets optional effective end date.
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    /// <summary>
    /// Gets or sets whether the registration is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets internal notes.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets last update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
