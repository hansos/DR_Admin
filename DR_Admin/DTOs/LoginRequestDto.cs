namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for user login requests
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Gets or sets the username or email address for authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password for authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
