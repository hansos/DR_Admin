using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class RegistrarService : IRegistrarService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarService>();

    public RegistrarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistrarDto>> GetAllRegistrarsAsync()
    {
        try
        {
            _log.Information("Fetching all registrars");
            
            var registrars = await _context.Registrars
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrars", registrars.Count);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all registrars");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarDto>> GetActiveRegistrarsAsync()
    {
        try
        {
            _log.Information("Fetching active registrars");
            
            var registrars = await _context.Registrars
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active registrars", registrars.Count);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active registrars");
            throw;
        }
    }

    public async Task<RegistrarDto?> GetRegistrarByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching registrar with ID: {RegistrarId}", id);
            
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched registrar with ID: {RegistrarId}", id);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    public async Task<RegistrarDto?> GetRegistrarByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching registrar with code: {RegistrarCode}", code);
            
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == code);

            if (registrar == null)
            {
                _log.Warning("Registrar with code {RegistrarCode} not found", code);
                return null;
            }

            _log.Information("Successfully fetched registrar with code: {RegistrarCode}", code);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar with code: {RegistrarCode}", code);
            throw;
        }
    }

    public async Task<RegistrarDto> CreateRegistrarAsync(CreateRegistrarDto createDto)
    {
        try
        {
            _log.Information("Creating new registrar with code: {RegistrarCode}", createDto.Code);

            var existingRegistrar = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Code == createDto.Code);

            if (existingRegistrar != null)
            {
                _log.Warning("Registrar with code {RegistrarCode} already exists", createDto.Code);
                throw new InvalidOperationException($"Registrar with code {createDto.Code} already exists");
            }

            var registrar = new Registrar
            {
                Name = createDto.Name,
                Code = createDto.Code.ToUpper(),
                ApiUrl = createDto.ApiUrl,
                ApiKey = createDto.ApiKey,
                ApiSecret = createDto.ApiSecret,
                ApiUsername = createDto.ApiUsername,
                ApiPassword = createDto.ApiPassword,
                IsActive = createDto.IsActive,
                ContactEmail = createDto.ContactEmail,
                ContactPhone = createDto.ContactPhone,
                Website = createDto.Website,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Registrars.Add(registrar);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created registrar with ID: {RegistrarId} and code: {RegistrarCode}", 
                registrar.Id, registrar.Code);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating registrar with code: {RegistrarCode}", createDto.Code);
            throw;
        }
    }

    public async Task<RegistrarDto?> UpdateRegistrarAsync(int id, UpdateRegistrarDto updateDto)
    {
        try
        {
            _log.Information("Updating registrar with ID: {RegistrarId}", id);

            var registrar = await _context.Registrars.FindAsync(id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found for update", id);
                return null;
            }

            var duplicateCode = await _context.Registrars
                .AnyAsync(r => r.Code == updateDto.Code && r.Id != id);

            if (duplicateCode)
            {
                _log.Warning("Cannot update registrar {RegistrarId}: code {RegistrarCode} already exists", 
                    id, updateDto.Code);
                throw new InvalidOperationException($"Registrar with code {updateDto.Code} already exists");
            }

            registrar.Name = updateDto.Name;
            registrar.Code = updateDto.Code.ToUpper();
            registrar.ApiUrl = updateDto.ApiUrl;
            registrar.ApiKey = updateDto.ApiKey;
            registrar.ApiSecret = updateDto.ApiSecret;
            registrar.ApiUsername = updateDto.ApiUsername;
            registrar.ApiPassword = updateDto.ApiPassword;
            registrar.IsActive = updateDto.IsActive;
            registrar.ContactEmail = updateDto.ContactEmail;
            registrar.ContactPhone = updateDto.ContactPhone;
            registrar.Website = updateDto.Website;
            registrar.Notes = updateDto.Notes;
            registrar.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar with ID: {RegistrarId}", id);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRegistrarAsync(int id)
    {
        try
        {
            _log.Information("Deleting registrar with ID: {RegistrarId}", id);

            var registrar = await _context.Registrars.FindAsync(id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found for deletion", id);
                return false;
            }

            var hasDomains = await _context.Domains.AnyAsync(d => d.RegistrarId == id);
            if (hasDomains)
            {
                _log.Warning("Cannot delete registrar {RegistrarId}: has associated domains", id);
                throw new InvalidOperationException("Cannot delete registrar with associated domains");
            }

            var hasRegistrarTlds = await _context.RegistrarTlds.AnyAsync(rt => rt.RegistrarId == id);
            if (hasRegistrarTlds)
            {
                _log.Warning("Cannot delete registrar {RegistrarId}: has associated TLD offerings", id);
                throw new InvalidOperationException("Cannot delete registrar with associated TLD offerings");
            }

            _context.Registrars.Remove(registrar);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted registrar with ID: {RegistrarId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    private static RegistrarDto MapToDto(Registrar registrar)
    {
        return new RegistrarDto
        {
            Id = registrar.Id,
            Name = registrar.Name,
            Code = registrar.Code,
            ApiUrl = registrar.ApiUrl,
            IsActive = registrar.IsActive,
            ContactEmail = registrar.ContactEmail,
            ContactPhone = registrar.ContactPhone,
            Website = registrar.Website,
            Notes = registrar.Notes,
            CreatedAt = registrar.CreatedAt,
            UpdatedAt = registrar.UpdatedAt
        };
    }
}
