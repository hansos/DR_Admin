namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a login attempt (successful or failed) for auditing and security monitoring.
/// </summary>
public class LoginHistory : EntityBase
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

    /// <summary>
    /// Navigation property for the associated user.
    /// </summary>
    public User? User { get; set; }
}
