namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a registrar's TLD offering with pricing information
/// </summary>
public class RegistrarTldDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the registrar-TLD relationship
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Gets or sets the name of the registrar
    /// </summary>
    public string? RegistrarName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Gets or sets the TLD extension (e.g., "com", "org")
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// Gets or sets the cost for registering this TLD with the registrar
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for registering this TLD
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for renewing this TLD with the registrar
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for renewing this TLD
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for transferring this TLD with the registrar
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for transferring this TLD
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for privacy protection with the registrar
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for privacy protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Gets or sets the currency code for all pricing (e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets whether this TLD is active and available for purchase
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether domains should be set to auto-renew by default
    /// </summary>
    public bool AutoRenew { get; set; }

    /// <summary>
    /// Gets or sets the minimum registration period in years
    /// </summary>
    public int? MinRegistrationYears { get; set; }

    /// <summary>
    /// Gets or sets the maximum registration period in years
    /// </summary>
    public int? MaxRegistrationYears { get; set; }

    /// <summary>
    /// Gets or sets additional notes or information about this offering
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}



/// <summary>
/// Data transfer object for creating a new registrar-TLD offering
/// </summary>
public class CreateRegistrarTldDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Gets or sets the cost for registering this TLD with the registrar
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for registering this TLD
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for renewing this TLD with the registrar
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for renewing this TLD
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for transferring this TLD with the registrar
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for transferring this TLD
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for privacy protection with the registrar
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for privacy protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Gets or sets the currency code for all pricing (e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets whether this TLD should be active and available for purchase
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether domains should be set to auto-renew by default
    /// </summary>
    public bool AutoRenew { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum registration period in years
    /// </summary>
    public int? MinRegistrationYears { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum registration period in years
    /// </summary>
    public int? MaxRegistrationYears { get; set; } = 10;

    /// <summary>
    /// Gets or sets additional notes or information about this offering
    /// </summary>
    public string? Notes { get; set; }
}



/// <summary>
/// Data transfer object for updating an existing registrar-TLD offering
/// </summary>
public class UpdateRegistrarTldDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Gets or sets the cost for registering this TLD with the registrar
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for registering this TLD
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for renewing this TLD with the registrar
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for renewing this TLD
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for transferring this TLD with the registrar
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for transferring this TLD
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost for privacy protection with the registrar
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Gets or sets the price charged to customers for privacy protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Gets or sets the currency code for all pricing (e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets whether this TLD should be active and available for purchase
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether domains should be set to auto-renew by default
    /// </summary>
    public bool AutoRenew { get; set; }

    /// <summary>
    /// Gets or sets the minimum registration period in years
    /// </summary>
    public int? MinRegistrationYears { get; set; }

    /// <summary>
    /// Gets or sets the maximum registration period in years
    /// </summary>
    public int? MaxRegistrationYears { get; set; }

    /// <summary>
    /// Gets or sets additional notes or information about this offering
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for bulk updating the active status of all registrar-TLD offerings
/// </summary>
public class BulkUpdateRegistrarTldStatusDto
{
    /// <summary>
    /// Gets or sets the optional registrar ID to filter by (null updates all registrars)
    /// </summary>
    public int? RegistrarId { get; set; }

    /// <summary>
    /// Gets or sets whether the registrar-TLD offerings should be set to active or inactive
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Data transfer object for bulk updating the active status of specific registrar-TLD offerings by TLD extensions
/// </summary>
public class BulkUpdateRegistrarTldStatusByTldDto
{
    /// <summary>
    /// Gets or sets the optional registrar ID to filter by (null updates all registrars)
    /// </summary>
    public int? RegistrarId { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated list of TLD extensions (e.g., "com,net,org")
    /// </summary>
    public string TldExtensions { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the registrar-TLD offerings should be set to active or inactive
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Data transfer object representing the result of a bulk update operation
/// </summary>
public class BulkUpdateResultDto
{
    /// <summary>
    /// Gets or sets the number of records updated
    /// </summary>
    public int UpdatedCount { get; set; }

    /// <summary>
    /// Gets or sets the message describing the update result
    /// </summary>
    public string Message { get; set; } = string.Empty;
}


