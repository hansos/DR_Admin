using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class TldService : ITldService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldService>();

    public TldService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TldDto>> GetAllTldsAsync()
    {
        try
        {
            _log.Information("Fetching all TLDs");
            
            var tlds = await _context.Tlds
                .AsNoTracking()
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} TLDs", tlds.Count);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<TldDto>> GetActiveTldsAsync()
    {
        try
        {
            _log.Information("Fetching active TLDs");
            
            var tlds = await _context.Tlds
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active TLDs", tlds.Count);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active TLDs");
            throw;
        }
    }

    public async Task<TldDto?> GetTldByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching TLD with ID: {TldId}", id);
            
            var tld = await _context.Tlds
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched TLD with ID: {TldId}", id);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLD with ID: {TldId}", id);
            throw;
        }
    }

    public async Task<TldDto?> GetTldByExtensionAsync(string extension)
    {
        try
        {
            // Normalize extension: remove leading dot and surrounding whitespace
            var ext = extension?.Trim().TrimStart('.') ?? string.Empty;
            _log.Information("Fetching TLD with extension: {Extension}", ext);

            var tld = await _context.Tlds
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Extension == ext);

            if (tld == null)
            {
                _log.Warning("TLD with extension {Extension} not found", extension);
                return null;
            }

            _log.Information("Successfully fetched TLD with extension: {Extension}", ext);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLD with extension: {Extension}", extension);
            throw;
        }
    }

    public async Task<TldDto> CreateTldAsync(CreateTldDto createDto)
    {
        try
        {
            // Normalize extension: remove leading dot and surrounding whitespace
            var extension = createDto.Extension?.Trim().TrimStart('.') ?? string.Empty;

            _log.Information("Creating new TLD with extension: {Extension}", extension);

            if (string.IsNullOrEmpty(extension))
            {
                _log.Warning("TLD extension is empty or invalid");
                throw new InvalidOperationException("TLD extension cannot be empty");
            }

            var existingTld = await _context.Tlds
                .FirstOrDefaultAsync(t => t.Extension == extension);

            if (existingTld != null)
            {
                _log.Warning("TLD with extension {Extension} already exists", extension);
                throw new InvalidOperationException($"TLD with extension {extension} already exists");
            }

            var tld = new Tld
            {
                Extension = extension,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                DefaultRegistrationYears = createDto.DefaultRegistrationYears,
                MaxRegistrationYears = createDto.MaxRegistrationYears,
                RequiresPrivacy = createDto.RequiresPrivacy,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tlds.Add(tld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created TLD with ID: {TldId} and extension: {Extension}", tld.Id, tld.Extension);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating TLD with extension: {Extension}", createDto.Extension);
            throw;
        }
    }

    public async Task<TldDto?> UpdateTldAsync(int id, UpdateTldDto updateDto)
    {
        try
        {
            _log.Information("Updating TLD with ID: {TldId}", id);

            var tld = await _context.Tlds.FindAsync(id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found for update", id);
                return null;
            }

            // Normalize extension: remove leading dot and surrounding whitespace
            var extension = updateDto.Extension?.Trim().TrimStart('.') ?? string.Empty;

            var duplicateExtension = await _context.Tlds
                .AnyAsync(t => t.Extension == extension && t.Id != id);

            if (duplicateExtension)
            {
                _log.Warning("Cannot update TLD {TldId}: extension {Extension} already exists", id, extension);
                throw new InvalidOperationException($"TLD with extension {extension} already exists");
            }

            tld.Extension = extension;
            tld.Description = updateDto.Description;
            tld.IsActive = updateDto.IsActive;
            tld.DefaultRegistrationYears = updateDto.DefaultRegistrationYears;
            tld.MaxRegistrationYears = updateDto.MaxRegistrationYears;
            tld.RequiresPrivacy = updateDto.RequiresPrivacy;
            tld.Notes = updateDto.Notes;
            tld.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated TLD with ID: {TldId}", id);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating TLD with ID: {TldId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTldAsync(int id)
    {
        try
        {
            _log.Information("Deleting TLD with ID: {TldId}", id);

            var tld = await _context.Tlds.FindAsync(id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found for deletion", id);
                return false;
            }

            var hasRegistrarTlds = await _context.RegistrarTlds.AnyAsync(rt => rt.TldId == id);
            if (hasRegistrarTlds)
            {
                _log.Warning("Cannot delete TLD {TldId}: has associated registrar TLDs", id);
                throw new InvalidOperationException("Cannot delete TLD with associated registrar offerings");
            }

            _context.Tlds.Remove(tld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted TLD with ID: {TldId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting TLD with ID: {TldId}", id);
            throw;
        }
    }

    private static TldDto MapToDto(Tld tld)
    {
        return new TldDto
        {
            Id = tld.Id,
            Extension = tld.Extension,
            Description = tld.Description,
            IsActive = tld.IsActive,
            DefaultRegistrationYears = tld.DefaultRegistrationYears,
            MaxRegistrationYears = tld.MaxRegistrationYears,
            RequiresPrivacy = tld.RequiresPrivacy,
            Notes = tld.Notes,
            CreatedAt = tld.CreatedAt,
            UpdatedAt = tld.UpdatedAt
        };
    }

    public async Task<IEnumerable<RegistrarDto>> GetRegistrarsByTldAsync(int tldId)
    {
        try
        {
            _log.Information("Fetching registrars for TLD {TldId}", tldId);

            var registrars = await _context.RegistrarTlds
                .AsNoTracking()
                .Where(rt => rt.TldId == tldId)
                .Include(rt => rt.Registrar)
                .Select(rt => rt.Registrar)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(r => new RegistrarDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                ApiUrl = r.ApiUrl,
                IsActive = r.IsActive,
                ContactEmail = r.ContactEmail,
                ContactPhone = r.ContactPhone,
                Website = r.Website,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            _log.Information("Successfully fetched {Count} registrars for TLD {TldId}", registrars.Count, tldId);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrars for TLD {TldId}", tldId);
            throw;
        }
    }
}
