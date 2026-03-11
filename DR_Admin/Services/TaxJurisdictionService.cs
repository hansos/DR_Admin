using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing tax jurisdiction operations.
/// </summary>
public class TaxJurisdictionService : ITaxJurisdictionService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TaxJurisdictionService>();

    public TaxJurisdictionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all tax jurisdictions.
    /// </summary>
    /// <returns>Collection of tax jurisdiction DTOs.</returns>
    public async Task<IEnumerable<TaxJurisdictionDto>> GetAllTaxJurisdictionsAsync()
    {
        var entities = await _context.TaxJurisdictions
            .AsNoTracking()
            .OrderBy(x => x.CountryCode)
            .ThenBy(x => x.StateCode)
            .ThenBy(x => x.Name)
            .ToListAsync();

        _log.Information("Fetched {Count} tax jurisdictions", entities.Count);
        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a tax jurisdiction by identifier.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>Tax jurisdiction DTO if found; otherwise null.</returns>
    public async Task<TaxJurisdictionDto?> GetTaxJurisdictionByIdAsync(int id)
    {
        var entity = await _context.TaxJurisdictions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Creates a new tax jurisdiction.
    /// </summary>
    /// <param name="dto">Tax jurisdiction create request.</param>
    /// <returns>Created tax jurisdiction DTO.</returns>
    public async Task<TaxJurisdictionDto> CreateTaxJurisdictionAsync(CreateTaxJurisdictionDto dto)
    {
        var entity = new TaxJurisdiction
        {
            Code = dto.Code,
            Name = dto.Name,
            CountryCode = dto.CountryCode,
            StateCode = dto.StateCode,
            TaxAuthority = dto.TaxAuthority,
            TaxCurrencyCode = dto.TaxCurrencyCode,
            IsActive = dto.IsActive,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaxJurisdictions.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created tax jurisdiction with ID {Id}", entity.Id);
        return MapToDto(entity);
    }

    /// <summary>
    /// Updates an existing tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <param name="dto">Tax jurisdiction update request.</param>
    /// <returns>Updated tax jurisdiction DTO if found; otherwise null.</returns>
    public async Task<TaxJurisdictionDto?> UpdateTaxJurisdictionAsync(int id, UpdateTaxJurisdictionDto dto)
    {
        var entity = await _context.TaxJurisdictions.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        entity.Code = dto.Code;
        entity.Name = dto.Name;
        entity.CountryCode = dto.CountryCode;
        entity.StateCode = dto.StateCode;
        entity.TaxAuthority = dto.TaxAuthority;
        entity.TaxCurrencyCode = dto.TaxCurrencyCode;
        entity.IsActive = dto.IsActive;
        entity.Notes = dto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    /// <summary>
    /// Deletes a tax jurisdiction.
    /// </summary>
    /// <param name="id">Tax jurisdiction identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    public async Task<bool> DeleteTaxJurisdictionAsync(int id)
    {
        var entity = await _context.TaxJurisdictions.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.TaxJurisdictions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TaxJurisdictionDto MapToDto(TaxJurisdiction entity)
    {
        return new TaxJurisdictionDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            CountryCode = entity.CountryCode,
            StateCode = entity.StateCode,
            TaxAuthority = entity.TaxAuthority,
            TaxCurrencyCode = entity.TaxCurrencyCode,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
