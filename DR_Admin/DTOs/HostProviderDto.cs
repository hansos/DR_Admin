namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a hosting provider
/// </summary>
public class HostProviderDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the host provider
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the internal name of the host provider (aws, azure, digitalocean)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the host provider
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the host provider
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the website URL of the host provider
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Gets or sets whether this host provider is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the host provider was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the host provider was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new host provider
/// </summary>
public class CreateHostProviderDto
{
    /// <summary>
    /// Gets or sets the internal name of the host provider (aws, azure, digitalocean)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the host provider
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the host provider
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the website URL of the host provider
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Gets or sets whether this host provider is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing host provider
/// </summary>
public class UpdateHostProviderDto
{
    /// <summary>
    /// Gets or sets the internal name of the host provider (aws, azure, digitalocean)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the host provider
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the host provider
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the website URL of the host provider
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Gets or sets whether this host provider is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
