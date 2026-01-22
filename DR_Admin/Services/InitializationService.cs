using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class InitializationService : IInitializationService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationService>();

    public InitializationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request)
    {
        try
        {
            // Check if any users exist
            var userCount = await _context.Users.CountAsync();
            
            if (userCount > 0)
            {
                _log.Warning("Initialization attempted but users already exist in the system");
                return null;
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.Email))
            {
                _log.Warning("Initialization attempted with invalid input");
                return null;
            }

            // Check for duplicate username
            var existingUsername = await _context.Users
                .AnyAsync(u => u.Username == request.Username);
            
            if (existingUsername)
            {
                _log.Warning("Initialization attempted with duplicate username: {Username}", request.Username);
                return null;
            }

            // Check for duplicate email
            var existingEmail = await _context.Users
                .AnyAsync(u => u.Email == request.Email);
            
            if (existingEmail)
            {
                _log.Warning("Initialization attempted with duplicate email: {Email}", request.Email);
                return null;
            }

            // Find or create Admin role
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin",
                    Description = "Administrator with full system access"
                };
                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync();
                _log.Information("Admin role created");
            }

            // Create the first admin user
            // Note: In production, use proper password hashing (e.g., BCrypt, PBKDF2)
            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password, // TODO: Hash this password properly
                Email = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign Admin role to the user
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            _log.Information("Initial admin user created: {Username}", user.Username);

            return new InitializationResponseDto
            {
                Success = true,
                Message = "System initialized successfully with admin user",
                Username = user.Username
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during system initialization");
            return null;
        }
    }
}
