using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing seller tax registration operations.
/// </summary>
public class TaxRegistrationService : ITaxRegistrationService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TaxRegistrationService>();

    public TaxRegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all seller tax registrations.
    /// </summary>
    /// <returns>Collection of tax registration DTOs.</returns>
    public async Task<IEnumerable<TaxRegistrationDto>> GetAllTaxRegistrationsAsync()
    {
        var entities = await _context.TaxRegistrations
            .AsNoTracking()
            .OrderBy(x => x.TaxJurisdictionId)
            .ThenBy(x => x.LegalEntityName)
            .ToListAsync();

        _log.Information("Fetched {Count} tax registrations", entities.Count);
        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a seller tax registration by identifier.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>Tax registration DTO if found; otherwise null.</returns>
    public async Task<TaxRegistrationDto?> GetTaxRegistrationByIdAsync(int id)
    {
        var entity = await _context.TaxRegistrations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Creates a new seller tax registration.
    /// </summary>
    /// <param name="dto">Tax registration create request.</param>
    /// <returns>Created tax registration DTO.</returns>
    public async Task<TaxRegistrationDto> CreateTaxRegistrationAsync(CreateTaxRegistrationDto dto)
    {
        var entity = new TaxRegistration
        {
            TaxJurisdictionId = dto.TaxJurisdictionId,
            LegalEntityName = dto.LegalEntityName,
            RegistrationNumber = dto.RegistrationNumber,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveUntil = dto.EffectiveUntil,
            IsActive = dto.IsActive,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaxRegistrations.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created tax registration with ID {Id}", entity.Id);
        return MapToDto(entity);
    }

    /// <summary>
    /// Updates an existing seller tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <param name="dto">Tax registration update request.</param>
    /// <returns>Updated tax registration DTO if found; otherwise null.</returns>
    public async Task<TaxRegistrationDto?> UpdateTaxRegistrationAsync(int id, UpdateTaxRegistrationDto dto)
    {
        var entity = await _context.TaxRegistrations.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        entity.TaxJurisdictionId = dto.TaxJurisdictionId;
        entity.LegalEntityName = dto.LegalEntityName;
        entity.RegistrationNumber = dto.RegistrationNumber;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveUntil = dto.EffectiveUntil;
        entity.IsActive = dto.IsActive;
        entity.Notes = dto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    /// <summary>
    /// Deletes a seller tax registration.
    /// </summary>
    /// <param name="id">Tax registration identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    public async Task<bool> DeleteTaxRegistrationAsync(int id)
    {
        var entity = await _context.TaxRegistrations.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.TaxRegistrations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TaxRegistrationDto MapToDto(TaxRegistration entity)
    {
        return new TaxRegistrationDto
        {
            Id = entity.Id,
            TaxJurisdictionId = entity.TaxJurisdictionId,
            LegalEntityName = entity.LegalEntityName,
            RegistrationNumber = entity.RegistrationNumber,
            EffectiveFrom = entity.EffectiveFrom,
            EffectiveUntil = entity.EffectiveUntil,
            IsActive = entity.IsActive,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
