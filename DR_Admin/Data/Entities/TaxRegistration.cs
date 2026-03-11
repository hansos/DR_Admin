namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a seller tax registration in a specific jurisdiction.
/// </summary>
public class TaxRegistration : EntityBase
{
    /// <summary>
    /// Foreign key to the tax jurisdiction.
    /// </summary>
    public int TaxJurisdictionId { get; set; }

    /// <summary>
    /// Legal entity name that owns the tax registration.
    /// </summary>
    public string LegalEntityName { get; set; } = string.Empty;

    /// <summary>
    /// Official tax registration number in the jurisdiction.
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Start date of validity for this registration.
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// End date of validity for this registration. Null means active indefinitely.
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    /// <summary>
    /// Indicates if this registration is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional internal notes.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the jurisdiction.
    /// </summary>
    public TaxJurisdiction TaxJurisdiction { get; set; } = null!;
}
