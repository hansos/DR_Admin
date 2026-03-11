using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing tax categories.
/// </summary>
public class TaxCategoryService : ITaxCategoryService
{
    private readonly ApplicationDbContext _context;

    public TaxCategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all tax categories.
    /// </summary>
    /// <returns>Collection of tax category DTOs.</returns>
    public async Task<IEnumerable<TaxCategoryDto>> GetAllTaxCategoriesAsync()
    {
        var entities = await _context.TaxCategories
            .AsNoTracking()
            .OrderBy(x => x.CountryCode)
            .ThenBy(x => x.StateCode)
            .ThenBy(x => x.Code)
            .ToListAsync();

        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a tax category by identifier.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>Tax category DTO if found; otherwise null.</returns>
    public async Task<TaxCategoryDto?> GetTaxCategoryByIdAsync(int id)
    {
        var entity = await _context.TaxCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Creates a tax category.
    /// </summary>
    /// <param name="dto">Create tax category payload.</param>
    /// <returns>Created tax category DTO.</returns>
    public async Task<TaxCategoryDto> CreateTaxCategoryAsync(CreateTaxCategoryDto dto)
    {
        var entity = new TaxCategory
        {
            Code = Normalize(dto.Code),
            Name = dto.Name,
            CountryCode = Normalize(dto.CountryCode),
            StateCode = NormalizeNullable(dto.StateCode),
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaxCategories.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    /// <summary>
    /// Updates a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <param name="dto">Update tax category payload.</param>
    /// <returns>Updated tax category DTO if found; otherwise null.</returns>
    public async Task<TaxCategoryDto?> UpdateTaxCategoryAsync(int id, UpdateTaxCategoryDto dto)
    {
        var entity = await _context.TaxCategories.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        entity.Code = Normalize(dto.Code);
        entity.Name = dto.Name;
        entity.CountryCode = Normalize(dto.CountryCode);
        entity.StateCode = NormalizeNullable(dto.StateCode);
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    /// <summary>
    /// Deletes a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    public async Task<bool> DeleteTaxCategoryAsync(int id)
    {
        var entity = await _context.TaxCategories.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.TaxCategories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TaxCategoryDto MapToDto(TaxCategory entity)
    {
        return new TaxCategoryDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            CountryCode = entity.CountryCode,
            StateCode = entity.StateCode,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static string Normalize(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }
}
