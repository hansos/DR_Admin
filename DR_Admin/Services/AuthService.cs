using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;

namespace ISPAdmin.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly Serilog.ILogger _logger;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, Serilog.ILogger logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Find user by username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.Warning("Login attempt failed: User not found - {Username}", username);
                return null;
            }

            // Note: In production, you should use proper password hashing (e.g., BCrypt, PBKDF2)
            // For now, this is a simple comparison - REPLACE THIS WITH PROPER PASSWORD VERIFICATION
            if (user.PasswordHash != password)
            {
                _logger.Warning("Login attempt failed: Invalid password - {Username}", username);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.Warning("Login attempt failed: User is inactive - {Username}", username);
                return null;
            }

            // Get user roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate JWT token
            var token = GenerateJwtToken(user.Username, user.Id, roles);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

            _logger.Information("User authenticated successfully: {Username}", username);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Roles = roles
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during authentication for user: {Username}", username);
            return null;
        }
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
}
