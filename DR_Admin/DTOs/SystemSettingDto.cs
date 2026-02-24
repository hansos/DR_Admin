namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a system setting
/// </summary>
public class SystemSettingDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the system setting
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique setting key (e.g., "CustomerNumber.NextValue")
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a human-readable description of the setting
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the setting is a system key
    /// </summary>
    public bool IsSystemKey { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the setting was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the setting was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new system setting
/// </summary>
public class CreateSystemSettingDto
{
    /// <summary>
    /// Unique setting key (e.g., "CustomerNumber.Prefix"). Must be unique across all settings.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// A human-readable description of the setting
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the setting is a system key
    /// </summary>
    public bool IsSystemKey { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing system setting
/// </summary>
public class UpdateSystemSettingDto
{
    /// <summary>
    /// The updated setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The updated description of the setting
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the setting is a system key
    /// </summary>
    public bool IsSystemKey { get; set; }
}
