namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for user account information
/// </summary>
public class UserAccountDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? EmailConfirmed { get; set; }
    public CustomerAccountDto? Customer { get; set; }
}

/// <summary>
/// DTO for customer information in account context
/// </summary>
public class CustomerAccountDto
{
    public int Id { get; set; }
    public long ReferenceNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
        // Address moved to CustomerAddress; not included here
        public string Address { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for new account registration
/// </summary>
public class RegisterAccountRequestDto
{
    /// <summary>
    /// Gets or sets the username for the new user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user login email for the new account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for the new account.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation value.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional frontend site code used for generated links.
    /// </summary>
    public string? SiteCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer was created through self-service registration.
    /// </summary>
    public bool IsSelfRegisteredCustomer { get; set; } = true;
    
    // Customer information

    /// <summary>
    /// Gets or sets the customer display name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer contact email.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer contact phone.
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer address text.
    /// </summary>
    public string CustomerAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the primary contact person created for the customer.
    /// </summary>
    public string ContactFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the primary contact person created for the customer.
    /// </summary>
    public string ContactLastName { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for successful registration
/// </summary>
public class RegisterAccountResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EmailConfirmationToken { get; set; }
}

/// <summary>
/// Request DTO for email confirmation
/// </summary>
public class ConfirmEmailRequestDto
{
    public string ConfirmationToken { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for setting password (first time or after reset)
/// </summary>
public class SetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for requesting a password reset
/// </summary>
public class RequestPasswordResetDto
{
    public string Email { get; set; } = string.Empty;
    public string? SiteCode { get; set; }
}

/// <summary>
/// Request DTO for requesting an email verification message
/// </summary>
public class RequestEmailConfirmationDto
{
    public string? SiteCode { get; set; }
}

/// <summary>
/// Request DTO for updating two-factor authentication settings.
/// </summary>
public class UpdateTwoFactorSettingsRequestDto
{
    /// <summary>
    /// Gets or sets a value indicating whether mail two-factor authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the requested two-factor method. Supported values: Email, Authenticator.
    /// </summary>
    public string? Method { get; set; }
}

/// <summary>
/// DTO for two-factor authentication status.
/// </summary>
public class TwoFactorStatusDto
{
    /// <summary>
    /// Gets or sets a value indicating whether two-factor authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the configured second-factor method.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Gets or sets the remaining count of recovery codes.
    /// </summary>
    public int? RecoveryCodesRemaining { get; set; }
}

/// <summary>
/// Response DTO for authenticator app setup details.
/// </summary>
public class AuthenticatorSetupDto
{
    /// <summary>
    /// Gets or sets the base32 shared secret key for manual entry.
    /// </summary>
    public string SharedKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the otpauth URI used for QR code generation.
    /// </summary>
    public string QrCodeUri { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for confirming authenticator app setup.
/// </summary>
public class ConfirmAuthenticatorSetupRequestDto
{
    /// <summary>
    /// Gets or sets the one-time code from authenticator app.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for resetting password with token
/// </summary>
public class ResetPasswordWithTokenRequestDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for changing password (authenticated user)
/// </summary>
public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating email address
/// </summary>
public class PatchEmailRequestDto
{
    public string NewEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? SiteCode { get; set; }
}

/// <summary>
/// Request DTO for updating customer information
/// </summary>
public class PatchCustomerInfoRequestDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// Request DTO for refreshing access token
/// </summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for token refresh
/// </summary>
public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
