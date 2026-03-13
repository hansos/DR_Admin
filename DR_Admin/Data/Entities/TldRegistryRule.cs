namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents registry policy rules for a specific top-level domain.
/// </summary>
public class TldRegistryRule : EntityBase
{
    /// <summary>
    /// Gets or sets the related TLD identifier.
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Gets or sets whether registrant contact is required.
    /// </summary>
    public bool RequireRegistrantContact { get; set; }

    /// <summary>
    /// Gets or sets whether administrative contact is required.
    /// </summary>
    public bool RequireAdministrativeContact { get; set; }

    /// <summary>
    /// Gets or sets whether technical contact is required.
    /// </summary>
    public bool RequireTechnicalContact { get; set; }

    /// <summary>
    /// Gets or sets whether billing contact is required.
    /// </summary>
    public bool RequireBillingContact { get; set; }

    /// <summary>
    /// Gets or sets whether auth code is required for transfer.
    /// </summary>
    public bool RequiresAuthCodeForTransfer { get; set; }

    /// <summary>
    /// Gets or sets transfer lock period in days.
    /// </summary>
    public int? TransferLockDays { get; set; }

    /// <summary>
    /// Gets or sets renewal grace period in days.
    /// </summary>
    public int? RenewalGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets redemption grace period in days.
    /// </summary>
    public int? RedemptionGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets optional rule notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this registry rule is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets related TLD.
    /// </summary>
    public Tld Tld { get; set; } = null!;
}
