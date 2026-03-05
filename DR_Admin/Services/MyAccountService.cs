using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;
using MessagingTemplateLib.Templating;
using MessagingTemplateLib.Models;
using MessagingTemplateLib;
using static ISPAdmin.Infrastructure.RoleNames;

namespace ISPAdmin.Services;

public class MyAccountService : IMyAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly AppSettings _appSettings;
    private readonly IEmailQueueService _emailQueueService;
    private readonly MessagingService _messagingService;
    private static readonly Serilog.ILogger _log = Log.ForContext<MyAccountService>();

    public MyAccountService(
        ApplicationDbContext context, 
        AppSettings appSettings, 
        IEmailQueueService emailQueueService,
        MessagingService messagingService)
    {
        _context = context;
        _appSettings = appSettings;
        _emailQueueService = emailQueueService;
        _messagingService = messagingService;
    }

    public async Task<RegisterAccountResponseDto> RegisterAsync(RegisterAccountRequestDto request)
    {
        try
        {
            // Validate passwords match
            if (request.Password != request.ConfirmPassword)
            {
                throw new InvalidOperationException("Passwords do not match");
            }

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Check if username already exists
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUsername == StringNormalizationExtensions.Normalize(request.Username));
            if (existingUsername != null)
            {
                throw new InvalidOperationException("Username already taken");
            }

            var nextReferenceNumber = await GetNextReferenceNumberAsync();

            // Create customer
            var customer = new Customer
            {
                ReferenceNumber = nextReferenceNumber,
                Name = request.CustomerName,
                Email = request.CustomerEmail,
                Phone = request.CustomerPhone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
            // Create user
            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            var user = new User
            {
                CustomerId = customer.Id,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash, // TODO: Hash this properly
                EmailConfirmed = null,
                IsMailTwoFactorEnabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == CUSTOMER);
            if (customerRole == null)
            {
                throw new InvalidOperationException("Customer role is not configured");
            }

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id
            });

            await _context.SaveChangesAsync();

            // Generate email confirmation token
            var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id);

            // Queue email confirmation email
            await QueueEmailConfirmationAsync(user.Email, confirmationToken, user.Id, customer.Id, request.SiteCode);

            _log.Information("User registered successfully: {Email}", request.Email);

            return new RegisterAccountResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Message = "Registration successful. Please confirm your email address.",
                EmailConfirmationToken = confirmationToken
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<bool> ConfirmEmailAsync(string confirmationToken)
    {
        try
        {
            var token = await _context.Tokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenType == "EmailConfirmation" 
                    && t.TokenValue == confirmationToken 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (token == null)
            {
                _log.Warning("Email confirmation failed: Invalid or expired token");
                return false;
            }

            var user = token.User;
            if (user == null)
            {
                _log.Warning("Email confirmation failed: User not found for token");
                return false;
            }

            user.EmailConfirmed = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            token.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Email confirmed successfully: {Email}", user.Email);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during email confirmation");
            return false;
        }
    }

    public async Task<bool> RequestEmailConfirmationAsync(int userId, string? siteCode = null)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _log.Warning("Request email confirmation failed: User not found - {UserId}", userId);
                return false;
            }

            if (user.EmailConfirmed.HasValue)
            {
                _log.Information("Request email confirmation skipped: Email already verified - {UserId}", userId);
                return true;
            }

            var existingTokens = await _context.Tokens
                .Where(t => t.UserId == user.Id && t.TokenType == "EmailConfirmation" && t.RevokedAt == null)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id);
            await QueueEmailConfirmationAsync(user.Email, confirmationToken, user.Id, user.CustomerId, siteCode);

            _log.Information("Email confirmation requested successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error requesting email confirmation for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<TwoFactorStatusDto?> GetTwoFactorStatusAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _log.Warning("Get two-factor status failed: User not found - {UserId}", userId);
                return null;
            }

            return new TwoFactorStatusDto
            {
                Enabled = user.IsMailTwoFactorEnabled,
                Method = user.IsMailTwoFactorEnabled ? "Email" : null,
                RecoveryCodesRemaining = null
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting two-factor status for user: {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> UpdateMailTwoFactorSettingAsync(int userId, bool enabled)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _log.Warning("Update two-factor setting failed: User not found - {UserId}", userId);
                return false;
            }

            if (enabled && !user.EmailConfirmed.HasValue)
            {
                _log.Warning("Update two-factor setting failed: Email not verified - {UserId}", userId);
                return false;
            }

            user.IsMailTwoFactorEnabled = enabled;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _log.Information("Mail two-factor setting updated for user {UserId}: {Enabled}", userId, enabled);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating two-factor setting for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> SetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _log.Warning("Set password failed: User not found - {Email}", email);
                return false;
            }

            var passwordResetToken = user.Tokens
                .FirstOrDefault(t => t.TokenType == "PasswordReset" 
                    && t.TokenValue == token 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (passwordResetToken == null)
            {
                _log.Warning("Set password failed: Invalid or expired token - {Email}", email);
                return false;
            }

            // Hash the new password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.PasswordHash = passwordHash;
            user.UpdatedAt = DateTime.UtcNow;
            passwordResetToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Password set successfully: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during set password for: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                _log.Warning("Change password failed: User not found - {UserId}", userId);
                return false;
            }

            // Verify current password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                _log.Warning("Change password failed: Invalid current password - {UserId}", userId);
                return false;
            }

            // Hash the new password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.PasswordHash = passwordHash;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Password changed successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during password change for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> PatchEmailAsync(int userId, string newEmail, string password, string? siteCode = null)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                _log.Warning("Patch email failed: User not found - {UserId}", userId);
                return false;
            }

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _log.Warning("Patch email failed: Invalid password - {UserId}", userId);
                return false;
            }

            // Check if new email is already in use
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == newEmail && u.Id != userId);

            if (existingUser != null)
            {
                _log.Warning("Patch email failed: Email already in use - {Email}", newEmail);
                return false;
            }

            user.Email = newEmail;
            user.EmailConfirmed = null; // Require re-confirmation
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Generate new confirmation token and queue re-confirmation email
            var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id);
            await QueueEmailConfirmationAsync(newEmail, confirmationToken, user.Id, user.CustomerId, siteCode);

            _log.Information("Email updated successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during email update for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<CustomerAccountDto?> PatchCustomerInfoAsync(int userId, PatchCustomerInfoRequestDto request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Customer == null)
            {
                _log.Warning("Patch customer info failed: User or customer not found - {UserId}", userId);
                return null;
            }

            var customer = user.Customer;
            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                customer.Name = request.Name;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                customer.Email = request.Email;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                customer.Phone = request.Phone;
                hasChanges = true;
            }

            // Address updates should go through CustomerAddress API; ignore inline address

            if (hasChanges)
            {
                customer.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _log.Information("Customer info updated successfully for user: {UserId}", userId);
            }

            return new CustomerAccountDto
            {
                Id = customer.Id,
                ReferenceNumber = customer.ReferenceNumber,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = string.Empty
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during customer info update for user: {UserId}", userId);
            return null;
        }
    }

    public async Task<UserAccountDto?> GetMyAccountAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _log.Warning("Get account failed: User not found - {UserId}", userId);
                return null;
            }

            return new UserAccountDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Customer = user.Customer != null ? new CustomerAccountDto
                {
                    Id = user.Customer.Id,
                    ReferenceNumber = user.Customer.ReferenceNumber,
                    Name = user.Customer.Name,
                    Email = user.Customer.Email,
                    Phone = user.Customer.Phone,
                    // Address moved to CustomerAddress; not available on Customer entity
                    Address = string.Empty
                } : null
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting account for user: {UserId}", userId);
            return null;
        }
    }

    #region Private Helper Methods

    private async Task<string> GenerateEmailConfirmationTokenAsync(int userId)
    {
        var tokenValue = GenerateSecureToken();
        
        var token = new Token
        {
            UserId = userId,
            TokenType = "EmailConfirmation",
            TokenValue = tokenValue,
            Expiry = DateTime.UtcNow.AddDays(3), // Valid for 3 days
            CreatedAt = DateTime.UtcNow
        };

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        return tokenValue;
    }

    private async Task<string> GeneratePasswordResetTokenAsync(int userId)
    {
        var tokenValue = GenerateSecureToken();
        
        var token = new Token
        {
            UserId = userId,
            TokenType = "PasswordReset",
            TokenValue = tokenValue,
            Expiry = DateTime.UtcNow.AddHours(24), // Valid for 24 hours
            CreatedAt = DateTime.UtcNow
        };

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        return tokenValue;
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    private async Task<long> GetNextReferenceNumberAsync()
    {
        const string key = "PNR";
        const long defaultStartValue = 1001;

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new Data.Entities.SystemSetting
            {
                Key = key,
                Value = (defaultStartValue + 1).ToString(),
                Description = "The next customer reference number (PNR) to assign. Auto-incremented on each new customer creation.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            _log.Information("Initialized {Key} system setting with starting value {Value}", key, defaultStartValue);
            return defaultStartValue;
        }

        if (!long.TryParse(setting.Value, out var currentValue))
        {
            _log.Warning("Invalid {Key} value '{Value}', resetting to default {Default}", key, setting.Value, defaultStartValue);
            currentValue = defaultStartValue;
        }

        setting.Value = (currentValue + 1).ToString();
        setting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return currentValue;
    }

    private FrontendSiteSettings ResolveFrontendSite(string? siteCode)
    {
        if (_appSettings.FrontendSites.Count > 0)
        {
            var requestedCode = string.IsNullOrWhiteSpace(siteCode)
                ? _appSettings.DefaultFrontendSiteCode
                : siteCode;

            var matchedSite = _appSettings.FrontendSites.FirstOrDefault(site =>
                string.Equals(site.Code, requestedCode, StringComparison.OrdinalIgnoreCase));

            if (matchedSite != null)
            {
                return matchedSite;
            }

            var defaultSite = _appSettings.FrontendSites.FirstOrDefault(site =>
                string.Equals(site.Code, _appSettings.DefaultFrontendSiteCode, StringComparison.OrdinalIgnoreCase));

            if (defaultSite != null)
            {
                return defaultSite;
            }

            return _appSettings.FrontendSites[0];
        }

        return new FrontendSiteSettings
        {
            Code = _appSettings.DefaultFrontendSiteCode,
            BaseUrl = _appSettings.FrontendBaseUrl,
            EmailConfirmationPath = _appSettings.EmailConfirmationPath,
            PasswordResetPath = _appSettings.PasswordResetPath
        };
    }

    private async Task QueueEmailConfirmationAsync(string email, string token, int userId, int? customerId, string? siteCode)
    {
        var frontendSite = ResolveFrontendSite(siteCode);
        var confirmationUrl = $"{frontendSite.BaseUrl}{frontendSite.EmailConfirmationPath}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        // Create template model
        var model = new EmailConfirmationModel
        {
            ConfirmationUrl = confirmationUrl,
            ExpirationDays = "3",
            Email = email
        };

        // Render both HTML and plain text versions
        var emailBodyHtml = _messagingService.RenderMessage("EmailConfirmation", MessageChannel.EmailHtml, model);
        var emailBodyText = _messagingService.RenderMessage("EmailConfirmation", MessageChannel.EmailText, model);

        await _emailQueueService.QueueEmailAsync(new QueueEmailDto
        {
            To = email,
            Subject = "Confirm Your Email Address",
            BodyHtml = emailBodyHtml,
            BodyText = emailBodyText,
            UserId = userId,
            CustomerId = customerId,
            RelatedEntityType = "User",
            RelatedEntityId = userId
        });

        _log.Information("Email confirmation queued for {Email}", email);
    }

    private async Task QueuePasswordResetEmailAsync(string email, string token, int userId, int? customerId, string? siteCode)
    {
        var frontendSite = ResolveFrontendSite(siteCode);
        var resetUrl = $"{frontendSite.BaseUrl}{frontendSite.PasswordResetPath}?token={Uri.EscapeDataString(token)}";

        // Create template model
        var model = new PasswordResetModel
        {
            ResetUrl = resetUrl,
            ExpirationHours = "24",
            Email = email
        };

        // Render both HTML and plain text versions
        var emailBodyHtml = _messagingService.RenderMessage("PasswordReset", MessageChannel.EmailHtml, model);
        var emailBodyText = _messagingService.RenderMessage("PasswordReset", MessageChannel.EmailText, model);

        await _emailQueueService.QueueEmailAsync(new QueueEmailDto
        {
            To = email,
            Subject = "Password Reset Request",
            BodyHtml = emailBodyHtml,
            BodyText = emailBodyText,
            UserId = userId,
            CustomerId = customerId,
            RelatedEntityType = "User",
            RelatedEntityId = userId
        });

        _log.Information("Password reset email queued for {Email}", email);
    }

    #endregion

    public async Task<bool> RequestPasswordResetAsync(string email, string? siteCode = null)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // For security reasons, always return success even if user not found
            // This prevents email enumeration attacks
            if (user == null)
            {
                _log.Warning("Password reset requested for non-existent email: {Email}", email);
                return true; // Return true but don't send email
            }

            // Revoke any existing password reset tokens for this user
            var existingTokens = await _context.Tokens
                .Where(t => t.UserId == user.Id && t.TokenType == "PasswordReset" && t.RevokedAt == null)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            // Generate new password reset token
            var resetToken = await GeneratePasswordResetTokenAsync(user.Id);

            // Queue password reset email
            await QueuePasswordResetEmailAsync(email, resetToken, user.Id, user.CustomerId, siteCode);

            _log.Information("Password reset token generated for {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during password reset request for: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ResetPasswordWithTokenAsync(string token, string newPassword)
    {
        try
        {
            // Find the token
            var passwordResetToken = await _context.Tokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenType == "PasswordReset" 
                    && t.TokenValue == token 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (passwordResetToken == null)
            {
                _log.Warning("Reset password failed: Invalid or expired token");
                return false;
            }

            var user = passwordResetToken.User;
            if (user == null)
            {
                _log.Warning("Reset password failed: User not found for token");
                return false;
            }

            // Hash the new password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.PasswordHash = passwordHash;
            user.UpdatedAt = DateTime.UtcNow;

            // Revoke the token
            passwordResetToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Password reset successfully for user: {UserId}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during password reset with token");
            return false;
        }
    }
}
