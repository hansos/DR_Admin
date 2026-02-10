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
    /// Get current user account information
    /// </summary>
    Task<UserAccountDto?> GetMyAccountAsync(int userId);

    /// <summary>
    /// Request password reset and send email with reset token
    /// </summary>
    Task<bool> RequestPasswordResetAsync(string email);
}
