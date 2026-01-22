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
    private readonly Serilog.ILogger _logger;

    public MyAccountService(ApplicationDbContext context, IConfiguration configuration, Serilog.ILogger logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<MyAccountLoginResponseDto?> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.Warning("Login attempt failed: User not found - {Email}", email);
                return null;
            }

            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            if (user.PasswordHash != password)
            {
                _logger.Warning("Login attempt failed: Invalid password - {Email}", email);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.Warning("Login attempt failed: User is inactive - {Email}", email);
                return null;
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var accessToken = GenerateAccessToken(user.Email, user.Id, roles);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id);

            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

            _logger.Information("User logged in successfully: {Email}", email);

            return new MyAccountLoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.TokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                RefreshTokenExpiresAt = refreshToken.Expiry,
                User = new UserAccountDto
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
                        Address = user.Customer.Address
                    } : null
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during login for email: {Email}", email);
            return null;
        }
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
                Address = request.CustomerAddress,
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

            _logger.Information("User registered successfully: {Email}", request.Email);

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
            _logger.Error(ex, "Error during registration for email: {Email}", request.Email);
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
                _logger.Warning("Email confirmation failed: User not found - {Email}", email);
                return false;
            }

            var token = user.Tokens
                .FirstOrDefault(t => t.TokenType == "EmailConfirmation" 
                    && t.TokenValue == confirmationToken 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (token == null)
            {
                _logger.Warning("Email confirmation failed: Invalid or expired token - {Email}", email);
                return false;
            }

            user.EmailConfirmed = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            token.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Email confirmed successfully: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during email confirmation for: {Email}", email);
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
                _logger.Warning("Set password failed: User not found - {Email}", email);
                return false;
            }

            var passwordResetToken = user.Tokens
                .FirstOrDefault(t => t.TokenType == "PasswordReset" 
                    && t.TokenValue == token 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (passwordResetToken == null)
            {
                _logger.Warning("Set password failed: Invalid or expired token - {Email}", email);
                return false;
            }

            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            user.PasswordHash = newPassword;
            user.UpdatedAt = DateTime.UtcNow;
            passwordResetToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Password set successfully: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during set password for: {Email}", email);
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
                _logger.Warning("Change password failed: User not found - {UserId}", userId);
                return false;
            }

            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            if (user.PasswordHash != currentPassword)
            {
                _logger.Warning("Change password failed: Invalid current password - {UserId}", userId);
                return false;
            }

            user.PasswordHash = newPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Password changed successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during password change for user: {UserId}", userId);
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
                _logger.Warning("Patch email failed: User not found - {UserId}", userId);
                return false;
            }

            // Verify password
            // TODO: In production, use proper password hashing (BCrypt, PBKDF2)
            if (user.PasswordHash != password)
            {
                _logger.Warning("Patch email failed: Invalid password - {UserId}", userId);
                return false;
            }

            // Check if new email is already in use
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == newEmail && u.Id != userId);

            if (existingUser != null)
            {
                _logger.Warning("Patch email failed: Email already in use - {Email}", newEmail);
                return false;
            }

            user.Email = newEmail;
            user.EmailConfirmed = null; // Require re-confirmation
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Email updated successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during email update for user: {UserId}", userId);
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
                _logger.Warning("Patch customer info failed: User or customer not found - {UserId}", userId);
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

            if (request.Address != null)
            {
                customer.Address = request.Address;
                hasChanges = true;
            }

            if (hasChanges)
            {
                customer.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.Information("Customer info updated successfully for user: {UserId}", userId);
            }

            return new CustomerAccountDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during customer info update for user: {UserId}", userId);
            return null;
        }
    }

    public async Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _context.Tokens
                .Include(t => t.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(t => t.TokenValue == refreshToken 
                    && t.TokenType == "RefreshToken" 
                    && t.RevokedAt == null 
                    && t.Expiry > DateTime.UtcNow);

            if (token == null)
            {
                _logger.Warning("Refresh token failed: Invalid or expired token");
                return null;
            }

            var user = token.User;
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate new tokens
            var newAccessToken = GenerateAccessToken(user.Email, user.Id, roles);
            var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id);

            // Revoke old refresh token
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

            _logger.Information("Token refreshed successfully for user: {UserId}", user.Id);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.TokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                RefreshTokenExpiresAt = newRefreshToken.Expiry
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during token refresh");
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
                _logger.Warning("Get account failed: User not found - {UserId}", userId);
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
                    Address = user.Customer.Address
                } : null
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting account for user: {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.TokenValue == refreshToken 
                    && t.TokenType == "RefreshToken" 
                    && t.RevokedAt == null);

            if (token == null)
            {
                _logger.Warning("Revoke token failed: Token not found");
                return false;
            }

            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.Information("Refresh token revoked successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during token revocation");
            return false;
        }
    }

    #region Private Helper Methods

    private string GenerateAccessToken(string email, int userId, List<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<Token> GenerateAndSaveRefreshTokenAsync(int userId)
    {
        var refreshToken = new Token
        {
            UserId = userId,
            TokenType = "RefreshToken",
            TokenValue = GenerateSecureToken(),
            Expiry = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
            CreatedAt = DateTime.UtcNow
        };

        _context.Tokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

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
