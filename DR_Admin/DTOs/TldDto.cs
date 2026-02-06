namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a Top-Level Domain (TLD)
/// </summary>
public class TldDto
{
    public int Id { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? DefaultRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public bool RequiresPrivacy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new TLD
/// </summary>
public class CreateTldDto
{
    public string Extension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSecondLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public int? DefaultRegistrationYears { get; set; } = 1;
    public int? MaxRegistrationYears { get; set; } = 10;
    public bool RequiresPrivacy { get; set; }
    public string? Notes { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing TLD
/// </summary>
public class UpdateTldDto
{
    public string Extension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? DefaultRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public bool RequiresPrivacy { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for importing TLDs for a registrar from form data
/// </summary>
public class ImportRegistrarTldsDto
{
    /// <summary>
    /// Gets or sets the content containing TLD data in CSV format (Tld, Description)
    /// Example format:
    /// ac,
    /// academy,
    /// airforce, US Airforce only
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default registration cost for imported TLDs
    /// </summary>
    public decimal? DefaultRegistrationCost { get; set; }

    /// <summary>
    /// Gets or sets the default registration price for imported TLDs
    /// </summary>
    public decimal? DefaultRegistrationPrice { get; set; }

    /// <summary>
    /// Gets or sets the default renewal cost for imported TLDs
    /// </summary>
    public decimal? DefaultRenewalCost { get; set; }

    /// <summary>
    /// Gets or sets the default renewal price for imported TLDs
    /// </summary>
    public decimal? DefaultRenewalPrice { get; set; }

    /// <summary>
    /// Gets or sets the default transfer cost for imported TLDs
    /// </summary>
    public decimal? DefaultTransferCost { get; set; }

    /// <summary>
    /// Gets or sets the default transfer price for imported TLDs
    /// </summary>
    public decimal? DefaultTransferPrice { get; set; }

    /// <summary>
    /// Gets or sets whether imported TLDs should be marked as available
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to activate TLDs that don't exist in the Tlds table
    /// </summary>
    public bool ActivateNewTlds { get; set; } = false;

    /// <summary>
    /// Gets or sets the currency for pricing
    /// </summary>
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// Response data transfer object for TLD import operation
/// </summary>
public class ImportRegistrarTldsResponseDto
{
    /// <summary>
    /// Gets or sets whether the import operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message describing the result
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of TLDs added to the Tlds table
    /// </summary>
    public int TldsAdded { get; set; }

    /// <summary>
    /// Gets or sets the number of TLDs that already existed in the Tlds table
    /// </summary>
    public int TldsExisting { get; set; }

    /// <summary>
    /// Gets or sets the number of RegistrarTld records created
    /// </summary>
    public int RegistrarTldsCreated { get; set; }

    /// <summary>
    /// Gets or sets the number of RegistrarTld records that already existed
    /// </summary>
    public int RegistrarTldsExisting { get; set; }

    /// <summary>
    /// Gets or sets the number of lines skipped (empty or invalid)
    /// </summary>
    public int LinesSkipped { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the import was performed
    /// </summary>
    public DateTime ImportTimestamp { get; set; }
}
