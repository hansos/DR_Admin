using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class UnitService : IUnitService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<UnitService>();

    public UnitService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UnitDto>> GetAllUnitsAsync()
    {
        try
        {
            _log.Information("Fetching all units");
            
            var units = await _context.Units
                .AsNoTracking()
                .Where(u => u.DeletedAt == null)
                .ToListAsync();

            var unitDtos = units.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} units", units.Count);
            return unitDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all units");
            throw;
        }
    }

    public async Task<UnitDto?> GetUnitByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching unit with ID: {UnitId}", id);
            
            var unit = await _context.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

            if (unit == null)
            {
                _log.Warning("Unit with ID {UnitId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched unit with ID: {UnitId}", id);
            return MapToDto(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching unit with ID: {UnitId}", id);
            throw;
        }
    }

    public async Task<UnitDto?> GetUnitByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching unit with code: {UnitCode}", code);
            
            var unit = await _context.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Code == code && u.DeletedAt == null);

            if (unit == null)
            {
                _log.Warning("Unit with code {UnitCode} not found", code);
                return null;
            }

            _log.Information("Successfully fetched unit with code: {UnitCode}", code);
            return MapToDto(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching unit with code: {UnitCode}", code);
            throw;
        }
    }

    public async Task<UnitDto> CreateUnitAsync(CreateUnitDto createDto)
    {
        try
        {
            _log.Information("Creating new unit with code: {UnitCode}", createDto.Code);

            // Check if unit with same code already exists
            var existingUnit = await _context.Units
                .FirstOrDefaultAsync(u => u.Code == createDto.Code && u.DeletedAt == null);
            
            if (existingUnit != null)
            {
                throw new InvalidOperationException($"Unit with code '{createDto.Code}' already exists");
            }

            var unit = new Unit
            {
                Code = createDto.Code,
                Name = createDto.Name,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created unit with ID: {UnitId}", unit.Id);
            return MapToDto(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating unit with code: {UnitCode}", createDto.Code);
            throw;
        }
    }

    public async Task<UnitDto?> UpdateUnitAsync(int id, UpdateUnitDto updateDto)
    {
        try
        {
            _log.Information("Updating unit with ID: {UnitId}", id);

            var unit = await _context.Units.FindAsync(id);

            if (unit == null || unit.DeletedAt != null)
            {
                _log.Warning("Unit with ID {UnitId} not found for update", id);
                return null;
            }

            // Check if another unit with the same code already exists
            var existingUnit = await _context.Units
                .FirstOrDefaultAsync(u => u.Code == updateDto.Code && u.Id != id && u.DeletedAt == null);
            
            if (existingUnit != null)
            {
                throw new InvalidOperationException($"Another unit with code '{updateDto.Code}' already exists");
            }

            unit.Code = updateDto.Code;
            unit.Name = updateDto.Name;
            unit.Description = updateDto.Description;
            unit.IsActive = updateDto.IsActive;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated unit with ID: {UnitId}", id);
            return MapToDto(unit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating unit with ID: {UnitId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUnitAsync(int id)
    {
        try
        {
            _log.Information("Deleting unit with ID: {UnitId}", id);

            var unit = await _context.Units.FindAsync(id);

            if (unit == null || unit.DeletedAt != null)
            {
                _log.Warning("Unit with ID {UnitId} not found for deletion", id);
                return false;
            }

            // Soft delete
            unit.DeletedAt = DateTime.UtcNow;
            unit.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted unit with ID: {UnitId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting unit with ID: {UnitId}", id);
            throw;
        }
    }

    private static UnitDto MapToDto(Unit unit)
    {
        return new UnitDto
        {
            Id = unit.Id,
            Code = unit.Code,
            Name = unit.Name,
            Description = unit.Description,
            IsActive = unit.IsActive,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt,
            DeletedAt = unit.DeletedAt
        };
    }
}
