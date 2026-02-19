using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing system settings
/// </summary>
public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemSettingService>();

    public SystemSettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all system settings
    /// </summary>
    /// <returns>A collection of all system setting DTOs</returns>
    public async Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync()
    {
        try
        {
            _log.Information("Fetching all system settings");

            var settings = await _context.SystemSettings
                .AsNoTracking()
                .OrderBy(s => s.Key)
                .ToListAsync();

            var dtos = settings.Select(MapToDto);

            _log.Information("Successfully fetched {Count} system settings", settings.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all system settings");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific system setting by its unique identifier
    /// </summary>
    /// <param name="id">The system setting ID</param>
    /// <returns>The system setting DTO if found, otherwise null</returns>
    public async Task<SystemSettingDto?> GetSystemSettingByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching system setting with ID: {SystemSettingId}", id);

            var setting = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (setting == null)
            {
                _log.Warning("System setting with ID {SystemSettingId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched system setting with ID: {SystemSettingId}", id);
            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching system setting with ID: {SystemSettingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific system setting by its unique key
    /// </summary>
    /// <param name="key">The system setting key (e.g., "CustomerNumber.NextValue")</param>
    /// <returns>The system setting DTO if found, otherwise null</returns>
    public async Task<SystemSettingDto?> GetSystemSettingByKeyAsync(string key)
    {
        try
        {
            _log.Information("Fetching system setting with key: {Key}", key);

            var setting = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null)
            {
                _log.Warning("System setting with key '{Key}' not found", key);
                return null;
            }

            _log.Information("Successfully fetched system setting with key: {Key}", key);
            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching system setting with key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Creates a new system setting
    /// </summary>
    /// <param name="createDto">The system setting data for creation</param>
    /// <returns>The newly created system setting DTO</returns>
    public async Task<SystemSettingDto> CreateSystemSettingAsync(CreateSystemSettingDto createDto)
    {
        try
        {
            _log.Information("Creating new system setting with key: {Key}", createDto.Key);

            var existing = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == createDto.Key);

            if (existing != null)
            {
                _log.Warning("System setting with key '{Key}' already exists", createDto.Key);
                throw new InvalidOperationException($"System setting with key '{createDto.Key}' already exists");
            }

            var setting = new SystemSetting
            {
                Key = createDto.Key,
                Value = createDto.Value,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created system setting with ID: {SystemSettingId}, Key: {Key}", setting.Id, setting.Key);
            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating system setting with key: {Key}", createDto.Key);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing system setting
    /// </summary>
    /// <param name="id">The system setting ID to update</param>
    /// <param name="updateDto">The updated system setting data</param>
    /// <returns>The updated system setting DTO if successful, otherwise null</returns>
    public async Task<SystemSettingDto?> UpdateSystemSettingAsync(int id, UpdateSystemSettingDto updateDto)
    {
        try
        {
            _log.Information("Updating system setting with ID: {SystemSettingId}", id);

            var setting = await _context.SystemSettings.FindAsync(id);

            if (setting == null)
            {
                _log.Warning("System setting with ID {SystemSettingId} not found for update", id);
                return null;
            }

            setting.Value = updateDto.Value;
            setting.Description = updateDto.Description;
            setting.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated system setting with ID: {SystemSettingId}, Key: {Key}", id, setting.Key);
            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating system setting with ID: {SystemSettingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Updates a system setting by its key, or creates it if it does not exist
    /// </summary>
    /// <param name="key">The system setting key</param>
    /// <param name="value">The new value</param>
    /// <param name="description">Optional description (used only when creating)</param>
    /// <returns>The upserted system setting DTO</returns>
    public async Task<SystemSettingDto> UpsertSystemSettingAsync(string key, string value, string? description = null)
    {
        try
        {
            _log.Information("Upserting system setting with key: {Key}", key);

            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = key,
                    Value = value,
                    Description = description ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
                if (description != null)
                    setting.Description = description;
                setting.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully upserted system setting with key: {Key}", key);
            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while upserting system setting with key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Deletes a system setting
    /// </summary>
    /// <param name="id">The system setting ID to delete</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteSystemSettingAsync(int id)
    {
        try
        {
            _log.Information("Deleting system setting with ID: {SystemSettingId}", id);

            var setting = await _context.SystemSettings.FindAsync(id);

            if (setting == null)
            {
                _log.Warning("System setting with ID {SystemSettingId} not found for deletion", id);
                return false;
            }

            _context.SystemSettings.Remove(setting);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted system setting with ID: {SystemSettingId}, Key: {Key}", id, setting.Key);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting system setting with ID: {SystemSettingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Maps a SystemSetting entity to a SystemSettingDto
    /// </summary>
    /// <param name="setting">The system setting entity</param>
    /// <returns>A mapped SystemSettingDto</returns>
    private static SystemSettingDto MapToDto(SystemSetting setting)
    {
        return new SystemSettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}
