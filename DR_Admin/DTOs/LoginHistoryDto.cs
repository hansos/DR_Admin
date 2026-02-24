namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a login attempt.
/// </summary>
public class LoginHistoryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the login history entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user identifier when the login attempt is associated with a known user.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the associated username when the login attempt is linked to a user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the username/email identifier used during the login attempt.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the login attempt was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the login attempt occurred.
    /// </summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the client making the login attempt.
    /// </summary>
    public string IPAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user agent string of the client making the login attempt.
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the failure reason for unsuccessful login attempts.
    /// </summary>
    public string? FailureReason { get; set; }
}

/// <summary>
/// Data transfer object for creating a login history entry.
/// </summary>
public class CreateLoginHistoryDto
{
    /// <summary>
    /// Gets or sets the user identifier when the login attempt is associated with a known user.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the username/email identifier used during the login attempt.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the login attempt was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the login attempt occurred.
    /// </summary>
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the IP address of the client making the login attempt.
    /// </summary>
    public string IPAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user agent string of the client making the login attempt.
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the failure reason for unsuccessful login attempts.
    /// </summary>
    public string? FailureReason { get; set; }
}
