namespace DR_Admin.IntegrationTests;

/// <summary>
/// Static storage for sharing authentication tokens across test classes
/// </summary>
public static class TestTokenStorage
{
    public static string? AccessToken { get; set; }
    public static string? RefreshToken { get; set; }
    public static DateTime? AccessTokenExpiresAt { get; set; }
    public static DateTime? RefreshTokenExpiresAt { get; set; }
    public static int? UserId { get; set; }
    public static string? UserEmail { get; set; }

    /// <summary>
    /// Check if the stored access token is still valid
    /// </summary>
    public static bool HasValidAccessToken()
    {
        return !string.IsNullOrEmpty(AccessToken) 
            && AccessTokenExpiresAt.HasValue 
            && AccessTokenExpiresAt.Value > DateTime.UtcNow.AddMinutes(1);
    }

    /// <summary>
    /// Clear all stored tokens
    /// </summary>
    public static void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        AccessTokenExpiresAt = null;
        RefreshTokenExpiresAt = null;
        UserId = null;
        UserEmail = null;
    }
}
