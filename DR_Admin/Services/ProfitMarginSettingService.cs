using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing profit margin settings by product class.
/// </summary>
public class ProfitMarginSettingService : IProfitMarginSettingService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ProfitMarginSettingService>();

    public ProfitMarginSettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all profit margin settings.
    /// </summary>
    /// <returns>A collection of profit margin settings.</returns>
    public async Task<IEnumerable<ProfitMarginSettingDto>> GetAllAsync()
    {
        var rows = await _context.Set<ProfitMarginSetting>()
            .AsNoTracking()
            .OrderBy(x => x.ProductClass)
            .ToListAsync();

        return rows.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a profit margin setting by id.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>The profit margin setting if found; otherwise null.</returns>
    public async Task<ProfitMarginSettingDto?> GetByIdAsync(int id)
    {
        var row = await _context.Set<ProfitMarginSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return row == null ? null : MapToDto(row);
    }

    /// <summary>
    /// Retrieves a profit margin setting for a product class.
    /// </summary>
    /// <param name="productClass">Product class.</param>
    /// <returns>The profit margin setting if found; otherwise null.</returns>
    public async Task<ProfitMarginSettingDto?> GetByProductClassAsync(ProfitProductClass productClass)
    {
        var row = await _context.Set<ProfitMarginSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductClass == productClass && x.IsActive);

        return row == null ? null : MapToDto(row);
    }

    /// <summary>
    /// Creates a new profit margin setting.
    /// </summary>
    /// <param name="dto">Create payload.</param>
    /// <returns>The created profit margin setting.</returns>
    public async Task<ProfitMarginSettingDto> CreateAsync(CreateProfitMarginSettingDto dto)
    {
        var exists = await _context.Set<ProfitMarginSetting>()
            .AnyAsync(x => x.ProductClass == dto.ProductClass);

        if (exists)
        {
            throw new InvalidOperationException($"Profit margin for '{dto.ProductClass}' already exists.");
        }

        var row = new ProfitMarginSetting
        {
            ProductClass = dto.ProductClass,
            ProfitPercent = dto.ProfitPercent,
            IsActive = dto.IsActive,
            Notes = dto.Notes,
        };

        _context.Set<ProfitMarginSetting>().Add(row);
        await _context.SaveChangesAsync();

        _log.Information("Created profit margin setting {Id} for class {ProductClass}", row.Id, row.ProductClass);
        return MapToDto(row);
    }

    /// <summary>
    /// Updates an existing profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <param name="dto">Update payload.</param>
    /// <returns>The updated profit margin setting if found; otherwise null.</returns>
    public async Task<ProfitMarginSettingDto?> UpdateAsync(int id, UpdateProfitMarginSettingDto dto)
    {
        var row = await _context.Set<ProfitMarginSetting>().FindAsync(id);
        if (row == null)
        {
            return null;
        }

        row.ProfitPercent = dto.ProfitPercent;
        row.IsActive = dto.IsActive;
        row.Notes = dto.Notes;
        row.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _log.Information("Updated profit margin setting {Id}", id);
        return MapToDto(row);
    }

    /// <summary>
    /// Deletes a profit margin setting.
    /// </summary>
    /// <param name="id">Profit margin setting id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var row = await _context.Set<ProfitMarginSetting>().FindAsync(id);
        if (row == null)
        {
            return false;
        }

        _context.Set<ProfitMarginSetting>().Remove(row);
        await _context.SaveChangesAsync();

        _log.Information("Deleted profit margin setting {Id}", id);
        return true;
    }

    private static ProfitMarginSettingDto MapToDto(ProfitMarginSetting row)
    {
        return new ProfitMarginSettingDto
        {
            Id = row.Id,
            ProductClass = row.ProductClass,
            ProfitPercent = row.ProfitPercent,
            IsActive = row.IsActive,
            Notes = row.Notes,
            CreatedAt = row.CreatedAt,
            UpdatedAt = row.UpdatedAt,
        };
    }
}
