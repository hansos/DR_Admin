namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing TLD registry policy rules.
/// </summary>
public class TldRegistryRuleDto
{
    /// <summary>
    /// Gets or sets the rule identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the related TLD identifier.
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Gets or sets the TLD extension.
    /// </summary>
    public string TldExtension { get; set; } = string.Empty;

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
    /// Gets or sets transfer lock days.
    /// </summary>
    public int? TransferLockDays { get; set; }

    /// <summary>
    /// Gets or sets renewal grace period days.
    /// </summary>
    public int? RenewalGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets redemption grace period days.
    /// </summary>
    public int? RedemptionGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets optional notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets updated timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating TLD registry policy rules.
/// </summary>
public class CreateTldRegistryRuleDto
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
    /// Gets or sets transfer lock days.
    /// </summary>
    public int? TransferLockDays { get; set; }

    /// <summary>
    /// Gets or sets renewal grace period days.
    /// </summary>
    public int? RenewalGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets redemption grace period days.
    /// </summary>
    public int? RedemptionGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets optional notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating TLD registry policy rules.
/// </summary>
public class UpdateTldRegistryRuleDto
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
    /// Gets or sets transfer lock days.
    /// </summary>
    public int? TransferLockDays { get; set; }

    /// <summary>
    /// Gets or sets renewal grace period days.
    /// </summary>
    public int? RenewalGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets redemption grace period days.
    /// </summary>
    public int? RedemptionGracePeriodDays { get; set; }

    /// <summary>
    /// Gets or sets optional notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is active.
    /// </summary>
    public bool IsActive { get; set; }
}
