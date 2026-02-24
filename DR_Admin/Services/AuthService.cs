using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;
using ISPAdmin.Data.Entities;

namespace ISPAdmin.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private static readonly Serilog.ILogger _log = Log.ForContext<AuthService>();

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Find user by username or email
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

            if (user == null)
            {
                _log.Warning("Login attempt failed: User not found - {UsernameOrEmail}", username);
                return null;
            }

            // Verify password using BCrypt
            if (!VerifyPassword(password, user.PasswordHash))
            {
                _log.Warning("Login attempt failed: Invalid password - {Username}", username);
                return null;
            }

            if (!user.IsActive)
            {
                _log.Warning("Login attempt failed: User is inactive - {Username}", username);
                return null;
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate ACCESS token (short-lived)
            var accessToken = GenerateJwtToken(user.Username, user.Id, roles);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

            // Generate REFRESH token (long-lived)
            var refreshToken = GenerateRefreshToken();
            var refreshExpirationDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationInDays", 30);
            
            // Store refresh token in database
            var tokenEntity = new Token
            {
                UserId = user.Id,
                TokenType = "RefreshToken",
                TokenValue = refreshToken,
                Expiry = DateTime.UtcNow.AddDays(refreshExpirationDays),
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Tokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            _log.Information("User authenticated successfully: {Username}", username);

            return new LoginResponseDto
            {
                UserId = user.Id,
                AccessToken = accessToken,        // ← Short-lived JWT
                RefreshToken = refreshToken,      // ← Long-lived, stored in DB
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Roles = roles
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during authentication for user: {Username}", username);
            return null;
        }
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        // Find the refresh token in database
        var tokenEntity = await _context.Tokens
            .Include(t => t.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(t => 
                t.TokenValue == refreshToken && 
                t.TokenType == "RefreshToken" &&
                t.RevokedAt == null &&
                t.Expiry > DateTime.UtcNow);

        if (tokenEntity == null || !tokenEntity.User.IsActive)
        {
            _log.Warning("Refresh token invalid or expired");
            return null;
        }

        var user = tokenEntity.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Generate NEW access token
        var newAccessToken = GenerateJwtToken(user.Username, user.Id, roles);
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

        // Optionally: Rotate refresh token (generate new one)
        // This is a security best practice
        var newRefreshToken = GenerateRefreshToken();
        
        // Revoke old refresh token
        tokenEntity.RevokedAt = DateTime.UtcNow;
        
        // Create new refresh token
        var newTokenEntity = new Token
        {
            UserId = user.Id,
            TokenType = "RefreshToken",
            TokenValue = newRefreshToken,
            Expiry = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationInDays", 30)),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Tokens.Add(newTokenEntity);
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.Id,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Roles = roles
        };
    }

    private string GenerateJwtToken(string username, int userId, List<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
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

    private string GenerateRefreshToken()
    {
        // Generate a cryptographically secure random token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var tokenEntity = await _context.Tokens
                .FirstOrDefaultAsync(t => 
                    t.TokenValue == refreshToken && 
                    t.TokenType == "RefreshToken" &&
                    t.RevokedAt == null);

            if (tokenEntity == null)
            {
                _log.Warning("Attempted to revoke non-existent or already revoked refresh token");
                return false;
            }

            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _log.Information("Refresh token revoked for user ID: {UserId}", tokenEntity.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while revoking refresh token");
            return false;
        }
    }

    private static string HashPassword(string password)
    {
        // Using BCrypt with work factor of 12 (configurable, higher = more secure but slower)
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        // BCrypt verification - compares plain password with hashed password
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
