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
