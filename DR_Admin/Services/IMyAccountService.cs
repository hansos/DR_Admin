using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IMyAccountService
{
    /// <summary>
    /// Authenticate user with email and password, returning access and refresh tokens
    /// </summary>
    Task<MyAccountLoginResponseDto?> LoginAsync(string email, string password);

    /// <summary>
    /// Register a new account with user and customer information
    /// </summary>
    Task<RegisterAccountResponseDto> RegisterAsync(RegisterAccountRequestDto request);

    /// <summary>
    /// Confirm email address using confirmation token
    /// </summary>
    Task<bool> ConfirmEmailAsync(string email, string confirmationToken);

    /// <summary>
    /// Set password for new account using token
    /// </summary>
    Task<bool> SetPasswordAsync(string email, string token, string newPassword);

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    /// <summary>
    /// Update email address for authenticated user
    /// </summary>
    Task<bool> PatchEmailAsync(int userId, string newEmail, string password);

    /// <summary>
    /// Update customer information for authenticated user
    /// </summary>
    Task<CustomerAccountDto?> PatchCustomerInfoAsync(int userId, PatchCustomerInfoRequestDto request);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Get current user account information
    /// </summary>
    Task<UserAccountDto?> GetMyAccountAsync(int userId);

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}
