namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for system initialization response
/// </summary>
public class InitializationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for user panel initialization response.
/// </summary>
public class UserPanelInitializationResponseDto
{
    /// <summary>
    /// Gets or sets a value indicating whether initialization completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the result message for the initialization operation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created user identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the created username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created customer/company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
}
