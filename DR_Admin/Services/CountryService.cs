using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class CountryService : ICountryService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CountryService>();

    public CountryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        try
        {
            _log.Information("Fetching all countries");
            
            var countries = await _context.Countries
                .AsNoTracking()
                .OrderBy(c => c.EnglishName)
                .ToListAsync();

            var countryDtos = countries.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} countries", countries.Count);
            return countryDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all countries");
            throw;
        }
    }

    public async Task<IEnumerable<CountryDto>> GetActiveCountriesAsync()
    {
        try
        {
            _log.Information("Fetching active countries");
            
            var countries = await _context.Countries
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.EnglishName)
                .ToListAsync();

            var countryDtos = countries.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active countries", countries.Count);
            return countryDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active countries");
            throw;
        }
    }

    public async Task<CountryDto?> GetCountryByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching country with ID: {CountryId}", id);
            
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                _log.Warning("Country with ID {CountryId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched country with ID: {CountryId}", id);
            return MapToDto(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching country with ID: {CountryId}", id);
            throw;
        }
    }

    public async Task<CountryDto?> GetCountryByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching country with code: {CountryCode}", code);
            
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);

            if (country == null)
            {
                _log.Warning("Country with code {CountryCode} not found", code);
                return null;
            }

            _log.Information("Successfully fetched country with code: {CountryCode}", code);
            return MapToDto(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching country with code: {CountryCode}", code);
            throw;
        }
    }

    public async Task<CountryDto> CreateCountryAsync(CreateCountryDto createDto)
    {
        try
        {
            _log.Information("Creating new country with code: {CountryCode}", createDto.Code);

            var existingCountry = await _context.Countries
                .FirstOrDefaultAsync(c => c.Code == createDto.Code);

            if (existingCountry != null)
            {
                _log.Warning("Country with code {CountryCode} already exists", createDto.Code);
                throw new InvalidOperationException($"Country with code {createDto.Code} already exists");
            }

            var country = new Country
            {
                Code = createDto.Code.ToUpper(),
                Tld = createDto.Tld?.TrimStart('.') ?? string.Empty,
                EnglishName = createDto.EnglishName,
                LocalName = createDto.LocalName,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created country with ID: {CountryId} and code: {CountryCode}", country.Id, country.Code);
            return MapToDto(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating country with code: {CountryCode}", createDto.Code);
            throw;
        }
    }

    public async Task<CountryDto?> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
    {
        try
        {
            _log.Information("Updating country with ID: {CountryId}", id);

            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                _log.Warning("Country with ID {CountryId} not found for update", id);
                return null;
            }

            var duplicateCode = await _context.Countries
                .AnyAsync(c => c.Code == updateDto.Code && c.Id != id);

            if (duplicateCode)
            {
                _log.Warning("Cannot update country {CountryId}: code {CountryCode} already exists", id, updateDto.Code);
                throw new InvalidOperationException($"Country with code {updateDto.Code} already exists");
            }

            country.Code = updateDto.Code.ToUpper();
            country.Tld = updateDto.Tld?.TrimStart('.') ?? string.Empty;
            country.EnglishName = updateDto.EnglishName;
            country.LocalName = updateDto.LocalName;
            country.IsActive = updateDto.IsActive;
            country.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated country with ID: {CountryId}", id);
            return MapToDto(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating country with ID: {CountryId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCountryAsync(int id)
    {
        try
        {
            _log.Information("Deleting country with ID: {CountryId}", id);

            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                _log.Warning("Country with ID {CountryId} not found for deletion", id);
                return false;
            }

            var hasCustomers = await _context.Customers.AnyAsync(c => c.CountryCode == country.Code);
            if (hasCustomers)
            {
                _log.Warning("Cannot delete country {CountryId}: has associated customers", id);
                throw new InvalidOperationException("Cannot delete country with associated customers");
            }

            var hasPostalCodes = await _context.PostalCodes.AnyAsync(p => p.CountryCode == country.Code);
            if (hasPostalCodes)
            {
                _log.Warning("Cannot delete country {CountryId}: has associated postal codes", id);
                throw new InvalidOperationException("Cannot delete country with associated postal codes");
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted country with ID: {CountryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting country with ID: {CountryId}", id);
            throw;
        }
    }

    private static CountryDto MapToDto(Country country)
    {
        return new CountryDto
        {
            Id = country.Id,
            Code = country.Code,
            Tld = country.Tld,
            EnglishName = country.EnglishName,
            LocalName = country.LocalName,
            IsActive = country.IsActive,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }
}
