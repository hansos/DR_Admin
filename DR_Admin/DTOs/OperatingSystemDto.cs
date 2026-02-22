namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an operating system
/// </summary>
public class OperatingSystemDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the operating system
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the internal name of the operating system (ubuntu, windows-server-2022)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the operating system
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the operating system
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the version of the operating system
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets whether this operating system is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the operating system was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the operating system was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new operating system
/// </summary>
public class CreateOperatingSystemDto
{
    /// <summary>
    /// Gets or sets the internal name of the operating system (ubuntu, windows-server-2022)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the operating system
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the operating system
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the version of the operating system
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets whether this operating system is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing operating system
/// </summary>
public class UpdateOperatingSystemDto
{
    /// <summary>
    /// Gets or sets the internal name of the operating system (ubuntu, windows-server-2022)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the operating system
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the operating system
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the version of the operating system
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets whether this operating system is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
