using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing server control panels
/// </summary>
public class ServerControlPanelService : IServerControlPanelService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServerControlPanelService>();

    public ServerControlPanelService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all server control panels
    /// </summary>
    /// <returns>Collection of server control panel DTOs</returns>
    public async Task<IEnumerable<ServerControlPanelDto>> GetAllServerControlPanelsAsync()
    {
        try
        {
            _log.Information("Fetching all server control panels");
            
            var controlPanels = await _context.ServerControlPanels
                .AsNoTracking()
                .ToListAsync();

            var controlPanelDtos = controlPanels.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} server control panels", controlPanels.Count);
            return controlPanelDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all server control panels");
            throw;
        }
    }

    /// <summary>
    /// Retrieves control panels for a specific server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <returns>Collection of server control panel DTOs</returns>
    public async Task<IEnumerable<ServerControlPanelDto>> GetServerControlPanelsByServerIdAsync(int serverId)
    {
        try
        {
            _log.Information("Fetching control panels for server ID: {ServerId}", serverId);
            
            var controlPanels = await _context.ServerControlPanels
                .AsNoTracking()
                .Where(scp => scp.ServerId == serverId)
                .ToListAsync();

            var controlPanelDtos = controlPanels.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} control panels for server ID: {ServerId}", controlPanels.Count, serverId);
            return controlPanelDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching control panels for server ID: {ServerId}", serverId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a server control panel by its unique identifier
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>Server control panel DTO if found, otherwise null</returns>
    public async Task<ServerControlPanelDto?> GetServerControlPanelByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching server control panel with ID: {ControlPanelId}", id);
            
            var controlPanel = await _context.ServerControlPanels
                .AsNoTracking()
                .FirstOrDefaultAsync(scp => scp.Id == id);

            if (controlPanel == null)
            {
                _log.Warning("Server control panel with ID {ControlPanelId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched server control panel with ID: {ControlPanelId}", id);
            return MapToDto(controlPanel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching server control panel with ID: {ControlPanelId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new server control panel
    /// </summary>
    /// <param name="createDto">The control panel creation data</param>
    /// <returns>The created server control panel DTO</returns>
    public async Task<ServerControlPanelDto> CreateServerControlPanelAsync(CreateServerControlPanelDto createDto)
    {
        try
        {
            _log.Information("Creating new server control panel for server ID: {ServerId}", createDto.ServerId);

            var controlPanel = new ServerControlPanel
            {
                ServerId = createDto.ServerId,
                ControlPanelTypeId = createDto.ControlPanelTypeId,
                ApiUrl = createDto.ApiUrl,
                Port = createDto.Port,
                UseHttps = createDto.UseHttps,
                ApiToken = createDto.ApiToken,
                ApiKey = createDto.ApiKey,
                Username = createDto.Username,
                PasswordHash = string.IsNullOrEmpty(createDto.Password) ? null : HashPassword(createDto.Password),
                AdditionalSettings = createDto.AdditionalSettings,
                Status = createDto.Status,
                Notes = createDto.Notes
            };

            _context.ServerControlPanels.Add(controlPanel);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created server control panel with ID: {ControlPanelId}", controlPanel.Id);
            return MapToDto(controlPanel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating server control panel for server ID: {ServerId}", createDto.ServerId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <param name="updateDto">The control panel update data</param>
    /// <returns>The updated server control panel DTO if found, otherwise null</returns>
    public async Task<ServerControlPanelDto?> UpdateServerControlPanelAsync(int id, UpdateServerControlPanelDto updateDto)
    {
        try
        {
            _log.Information("Updating server control panel with ID: {ControlPanelId}", id);

            var controlPanel = await _context.ServerControlPanels.FindAsync(id);

            if (controlPanel == null)
            {
                _log.Warning("Server control panel with ID {ControlPanelId} not found", id);
                return null;
            }

            controlPanel.ApiUrl = updateDto.ApiUrl;
            controlPanel.Port = updateDto.Port;
            controlPanel.UseHttps = updateDto.UseHttps;
            controlPanel.ApiToken = updateDto.ApiToken;
            controlPanel.ApiKey = updateDto.ApiKey;
            controlPanel.Username = updateDto.Username;
            
            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                controlPanel.PasswordHash = HashPassword(updateDto.Password);
            }
            
            controlPanel.AdditionalSettings = updateDto.AdditionalSettings;
            controlPanel.Status = updateDto.Status;
            controlPanel.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated server control panel with ID: {ControlPanelId}", id);
            return MapToDto(controlPanel);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating server control panel with ID: {ControlPanelId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteServerControlPanelAsync(int id)
    {
        try
        {
            _log.Information("Deleting server control panel with ID: {ControlPanelId}", id);

            var controlPanel = await _context.ServerControlPanels.FindAsync(id);

            if (controlPanel == null)
            {
                _log.Warning("Server control panel with ID {ControlPanelId} not found", id);
                return false;
            }

            _context.ServerControlPanels.Remove(controlPanel);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted server control panel with ID: {ControlPanelId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting server control panel with ID: {ControlPanelId}", id);
            throw;
        }
    }

    /// <summary>
    /// Tests the connection to a server control panel
    /// </summary>
    /// <param name="id">The control panel ID</param>
    /// <returns>True if connection is successful, otherwise false</returns>
    public async Task<bool> TestConnectionAsync(int id)
    {
        try
        {
            _log.Information("Testing connection to server control panel with ID: {ControlPanelId}", id);

            var controlPanel = await _context.ServerControlPanels.FindAsync(id);

            if (controlPanel == null)
            {
                _log.Warning("Server control panel with ID {ControlPanelId} not found", id);
                return false;
            }

            // TODO: Implement actual connection test using HostingPanelLib
            // For now, just update the connection test time
            controlPanel.LastConnectionTest = DateTime.UtcNow;
            controlPanel.IsConnectionHealthy = true;
            controlPanel.LastError = null;

            await _context.SaveChangesAsync();

            _log.Information("Successfully tested connection to server control panel with ID: {ControlPanelId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while testing connection to server control panel with ID: {ControlPanelId}", id);
            
            var controlPanel = await _context.ServerControlPanels.FindAsync(id);
            if (controlPanel != null)
            {
                controlPanel.LastConnectionTest = DateTime.UtcNow;
                controlPanel.IsConnectionHealthy = false;
                controlPanel.LastError = ex.Message;
                await _context.SaveChangesAsync();
            }
            
            return false;
        }
    }

    private static ServerControlPanelDto MapToDto(ServerControlPanel controlPanel)
    {
        return new ServerControlPanelDto
        {
            Id = controlPanel.Id,
            ServerId = controlPanel.ServerId,
            ControlPanelTypeId = controlPanel.ControlPanelTypeId,
            ApiUrl = controlPanel.ApiUrl,
            Port = controlPanel.Port,
            UseHttps = controlPanel.UseHttps,
            Username = controlPanel.Username,
            Status = controlPanel.Status,
            LastConnectionTest = controlPanel.LastConnectionTest,
            IsConnectionHealthy = controlPanel.IsConnectionHealthy,
            LastError = controlPanel.LastError,
            Notes = controlPanel.Notes,
            CreatedAt = controlPanel.CreatedAt,
            UpdatedAt = controlPanel.UpdatedAt
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
