using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<UserService>();

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            _log.Information("Fetching all users");
            
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            var userDtos = users.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} users", users.Count);
            return userDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all users");
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching user with ID: {UserId}", id);
            
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _log.Warning("User with ID {UserId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched user with ID: {UserId}", id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
    {
        try
        {
            _log.Information("Creating new user with username: {Username}", createDto.Username);

            // Check for duplicate username
            var existingUsername = await _context.Users
                .AnyAsync(u => u.Username == createDto.Username);
            
            if (existingUsername)
            {
                _log.Warning("Attempt to create user with duplicate username: {Username}", createDto.Username);
                throw new InvalidOperationException($"Username '{createDto.Username}' is already taken");
            }

            // Check for duplicate email
            var existingEmail = await _context.Users
                .AnyAsync(u => u.Email == createDto.Email);
            
            if (existingEmail)
            {
                _log.Warning("Attempt to create user with duplicate email: {Email}", createDto.Email);
                throw new InvalidOperationException($"Email '{createDto.Email}' is already registered");
            }

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

            _log.Information("Successfully created user with ID: {UserId}", user.Id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating user with username: {Username}", createDto.Username);
            throw;
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto)
    {
        try
        {
            _log.Information("Updating user with ID: {UserId}", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _log.Warning("User with ID {UserId} not found for update", id);
                return null;
            }

            // Check for duplicate username (excluding current user)
            var existingUsername = await _context.Users
                .AnyAsync(u => u.Username == updateDto.Username && u.Id != id);
            
            if (existingUsername)
            {
                _log.Warning("Attempt to update user {UserId} with duplicate username: {Username}", id, updateDto.Username);
                throw new InvalidOperationException($"Username '{updateDto.Username}' is already taken");
            }

            // Check for duplicate email (excluding current user)
            var existingEmail = await _context.Users
                .AnyAsync(u => u.Email == updateDto.Email && u.Id != id);
            
            if (existingEmail)
            {
                _log.Warning("Attempt to update user {UserId} with duplicate email: {Email}", id, updateDto.Email);
                throw new InvalidOperationException($"Email '{updateDto.Email}' is already registered");
            }

            user.CustomerId = updateDto.CustomerId;
            user.Username = updateDto.Username;
            user.Email = updateDto.Email;
            user.IsActive = updateDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated user with ID: {UserId}", id);
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            _log.Information("Deleting user with ID: {UserId}", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _log.Warning("User with ID {UserId} not found for deletion", id);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted user with ID: {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting user with ID: {UserId}", id);
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
        // Using BCrypt with work factor of 12 (configurable, higher = more secure but slower)
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
}
