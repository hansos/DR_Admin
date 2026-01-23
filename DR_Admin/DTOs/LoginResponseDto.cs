namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for login response containing authentication tokens and user information
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Gets or sets the JWT access token for API authentication
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the authenticated user's username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the expiration date and time of the access token
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Gets or sets the roles assigned to the authenticated user
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
