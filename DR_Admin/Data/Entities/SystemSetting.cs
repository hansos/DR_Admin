namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a key-value system setting stored in the database.
/// Used for application-wide configuration that can be modified at runtime.
/// </summary>
public class SystemSetting : EntityBase
{
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
}
