namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a server type
/// </summary>
public class ServerTypeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the server type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the internal name of the server type (physical, cloud, virtual)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the server type
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the server type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this server type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the server type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the server type was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new server type
/// </summary>
public class CreateServerTypeDto
{
    /// <summary>
    /// Gets or sets the internal name of the server type (physical, cloud, virtual)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the server type
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the server type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this server type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing server type
/// </summary>
public class UpdateServerTypeDto
{
    /// <summary>
    /// Gets or sets the internal name of the server type (physical, cloud, virtual)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the server type
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the server type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this server type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
