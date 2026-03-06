using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IAuthService
{
    /// <summary>
    /// Authenticates a user by username/email and password.
    /// </summary>
    Task<LoginResponseDto?> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Verifies two-factor code and issues authentication tokens.
    /// </summary>
    Task<LoginResponseDto?> VerifyTwoFactorAsync(string challengeToken, string code);

    /// <summary>
    /// Resends mail two-factor code for an active challenge.
    /// </summary>
    Task<bool> ResendMailTwoFactorCodeAsync(string challengeToken);

    /// <summary>
    /// Refreshes access token using refresh token.
    /// </summary>
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes an active refresh token.
    /// </summary>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}
