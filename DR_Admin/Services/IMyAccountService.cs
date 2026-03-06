using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IMyAccountService
{
    /// <summary>
    /// Register a new account with user and customer information
    /// </summary>
    Task<RegisterAccountResponseDto> RegisterAsync(RegisterAccountRequestDto request);

    /// <summary>
    /// Confirm email address using confirmation token
    /// </summary>
    Task<bool> ConfirmEmailAsync(string confirmationToken);

    /// <summary>
    /// Request a new email confirmation for an authenticated user
    /// </summary>
    Task<bool> RequestEmailConfirmationAsync(int userId, string? siteCode = null);

    /// <summary>
    /// Gets two-factor authentication status for authenticated user.
    /// </summary>
    Task<TwoFactorStatusDto?> GetTwoFactorStatusAsync(int userId);

    /// <summary>
    /// Updates two-factor authentication setting for authenticated user.
    /// </summary>
    Task<bool> UpdateTwoFactorSettingAsync(int userId, bool enabled, string? method);

    /// <summary>
    /// Starts authenticator app setup by generating shared key and provisioning URI.
    /// </summary>
    Task<AuthenticatorSetupDto?> BeginAuthenticatorSetupAsync(int userId);

    /// <summary>
    /// Confirms authenticator app setup using a valid one-time code.
    /// </summary>
    Task<bool> ConfirmAuthenticatorSetupAsync(int userId, string code);

    /// <summary>
    /// Deletes all two-factor authentication configuration for authenticated user.
    /// </summary>
    Task<bool> DeleteTwoFactorAsync(int userId);

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
    Task<bool> PatchEmailAsync(int userId, string newEmail, string password, string? siteCode = null);

    /// <summary>
    /// Update customer information for authenticated user
    /// </summary>
    Task<CustomerAccountDto?> PatchCustomerInfoAsync(int userId, PatchCustomerInfoRequestDto request);

    /// <summary>
    /// Get current user account information
    /// </summary>
    Task<UserAccountDto?> GetMyAccountAsync(int userId);

    /// <summary>
    /// Request password reset and send email with reset token
    /// </summary>
    Task<bool> RequestPasswordResetAsync(string email, string? siteCode = null);

    /// <summary>
    /// Reset password using password reset token (no email required)
    /// </summary>
    Task<bool> ResetPasswordWithTokenAsync(string token, string newPassword);
}
