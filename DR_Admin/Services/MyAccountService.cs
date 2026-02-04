using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace ISPAdmin.Services;

public class MyAccountService : IMyAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private static readonly Serilog.ILogger _log = Log.ForContext<MyAccountService>();

    public MyAccountService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUsername != null)
            {
                throw new InvalidOperationException("Username already taken");
            }

            // Create customer
            var customer = new Customer
            {
                Name = request.CustomerName,
                Email = request.CustomerEmail,
                Phone = request.CustomerPhone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Create user
            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            var user = new User
            {
                CustomerId = customer.Id,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password, // TODO: Hash this properly
                EmailConfirmed = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate email confirmation token
            var confirmationToken = await GenerateEmailConfirmationTokenAsync(user.Id);

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

    public async Task<bool> ConfirmEmailAsync(string email, string confirmationToken)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _log.Warning("Email confirmation failed: User not found - {Email}", email);
                return false;
            }

            var token = user.Tokens
                .FirstOrDefault(t => t.TokenType == "EmailConfirmation" 
                    && t.TokenValue == confirmationToken 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (token == null)
            {
                _log.Warning("Email confirmation failed: Invalid or expired token - {Email}", email);
                return false;
            }

            user.EmailConfirmed = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            token.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Email confirmed successfully: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during email confirmation for: {Email}", email);
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

            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            user.PasswordHash = newPassword;
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

            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            if (user.PasswordHash != currentPassword)
            {
                _log.Warning("Change password failed: Invalid current password - {UserId}", userId);
                return false;
            }

            user.PasswordHash = newPassword;
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

    public async Task<bool> PatchEmailAsync(int userId, string newEmail, string password)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                _log.Warning("Patch email failed: User not found - {UserId}", userId);
                return false;
            }

            // Verify password
            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            if (user.PasswordHash != password)
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

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}
