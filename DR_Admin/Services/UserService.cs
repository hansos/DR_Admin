using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public UserService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            _logger.Information("Fetching all users");
            
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            var userDtos = users.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} users", users.Count);
            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all users");
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching user with ID: {UserId}", id);
            
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.Warning("User with ID {UserId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched user with ID: {UserId}", id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
    {
        try
        {
            _logger.Information("Creating new user with username: {Username}", createDto.Username);

            var user = new User
            {
                CustomerId = createDto.CustomerId,
                Username = createDto.Username,
                PasswordHash = HashPassword(createDto.Password),
                Email = createDto.Email,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created user with ID: {UserId}", user.Id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating user with username: {Username}", createDto.Username);
            throw;
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto)
    {
        try
        {
            _logger.Information("Updating user with ID: {UserId}", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.Warning("User with ID {UserId} not found for update", id);
                return null;
            }

            user.CustomerId = updateDto.CustomerId;
            user.Username = updateDto.Username;
            user.Email = updateDto.Email;
            user.IsActive = updateDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated user with ID: {UserId}", id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            _logger.Information("Deleting user with ID: {UserId}", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.Warning("User with ID {UserId} not found for deletion", id);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted user with ID: {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting user with ID: {UserId}", id);
            throw;
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            CustomerId = user.CustomerId,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private static string HashPassword(string password)
    {
        // Simple hash for now - in production use BCrypt.Net or ASP.NET Core Identity
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
