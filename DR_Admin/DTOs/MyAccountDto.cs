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
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    
    // Customer information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
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
