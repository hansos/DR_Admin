using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class PostalCodeService : IPostalCodeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<PostalCodeService>();

    public PostalCodeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PostalCodeDto>> GetAllPostalCodesAsync()
    {
        try
        {
            _log.Information("Fetching all postal codes");
            
            var postalCodes = await _context.PostalCodes
                .AsNoTracking()
                .OrderBy(p => p.CountryCode)
                .ThenBy(p => p.Code)
                .ToListAsync();

            var postalCodeDtos = postalCodes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} postal codes", postalCodes.Count);
            return postalCodeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all postal codes");
            throw;
        }
    }

    public async Task<PagedResult<PostalCodeDto>> GetAllPostalCodesPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated postal codes - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.PostalCodes
                .AsNoTracking()
                .CountAsync();

            var postalCodes = await _context.PostalCodes
                .AsNoTracking()
                .OrderBy(p => p.CountryCode)
                .ThenBy(p => p.Code)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var postalCodeDtos = postalCodes.Select(MapToDto).ToList();
            
            var result = new PagedResult<PostalCodeDto>(
                postalCodeDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of postal codes - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, postalCodeDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated postal codes");
            throw;
        }
    }

    public async Task<IEnumerable<PostalCodeDto>> GetPostalCodesByCountryAsync(string countryCode)
    {
        try
        {
            _log.Information("Fetching postal codes for country: {CountryCode}", countryCode);
            
            var postalCodes = await _context.PostalCodes
                .AsNoTracking()
                .Where(p => p.CountryCode == countryCode)
                .OrderBy(p => p.Code)
                .ToListAsync();

            var postalCodeDtos = postalCodes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} postal codes for country {CountryCode}", postalCodes.Count, countryCode);
            return postalCodeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching postal codes for country: {CountryCode}", countryCode);
            throw;
        }
    }

    public async Task<IEnumerable<PostalCodeDto>> GetPostalCodesByCityAsync(string city)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new NullReferenceException("City name is mandatory.");
            }

            _log.Information("Fetching postal codes for city: {City}", city);
            
            var postalCodes = await _context.PostalCodes
                .AsNoTracking()
                .Where(p => p.NormalizedCity.Contains(NormalizationHelper.Normalize(city)!))
                .OrderBy(p => p.CountryCode)
                .ThenBy(p => p.Code)
                .ToListAsync();

            var postalCodeDtos = postalCodes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} postal codes for city {City}", postalCodes.Count, city);
            return postalCodeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching postal codes for city: {City}", city);
            throw;
        }
    }

    public async Task<PostalCodeDto?> GetPostalCodeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching postal code with ID: {PostalCodeId}", id);
            
            var postalCode = await _context.PostalCodes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (postalCode == null)
            {
                _log.Warning("Postal code with ID {PostalCodeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched postal code with ID: {PostalCodeId}", id);
            return MapToDto(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching postal code with ID: {PostalCodeId}", id);
            throw;
        }
    }

    public async Task<PostalCodeDto?> GetPostalCodeByCodeAndCountryAsync(string code, string countryCode)
    {
        try
        {
            _log.Information("Fetching postal code: {Code} for country: {CountryCode}", code, countryCode);
            
            var postalCode = await _context.PostalCodes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code && p.CountryCode == countryCode);

            if (postalCode == null)
            {
                _log.Warning("Postal code {Code} for country {CountryCode} not found", code, countryCode);
                return null;
            }

            _log.Information("Successfully fetched postal code {Code} for country {CountryCode}", code, countryCode);
            return MapToDto(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching postal code {Code} for country {CountryCode}", code, countryCode);
            throw;
        }
    }

    public async Task<PostalCodeDto> CreatePostalCodeAsync(CreatePostalCodeDto createDto)
    {
        try
        {
            _log.Information("Creating new postal code: {Code} for country: {CountryCode}", createDto.Code, createDto.CountryCode);

            var countryExists = await _context.Countries
                .AnyAsync(c => c.Code == createDto.CountryCode);

            if (!countryExists)
            {
                _log.Warning("Country with code {CountryCode} does not exist", createDto.CountryCode);
                throw new InvalidOperationException($"Country with code {createDto.CountryCode} does not exist");
            }

            var existingPostalCode = await _context.PostalCodes
                .FirstOrDefaultAsync(p => p.Code == createDto.Code && p.CountryCode == createDto.CountryCode);

            if (existingPostalCode != null)
            {
                _log.Warning("Postal code {Code} for country {CountryCode} already exists", createDto.Code, createDto.CountryCode);
                throw new InvalidOperationException($"Postal code {createDto.Code} for country {createDto.CountryCode} already exists");
            }

            var postalCode = new PostalCode
            {
                Code = createDto.Code,
                CountryCode = createDto.CountryCode.ToUpper(),
                City = createDto.City,
                State = createDto.State,
                Region = createDto.Region,
                District = createDto.District,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PostalCodes.Add(postalCode);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created postal code with ID: {PostalCodeId}", postalCode.Id);
            return MapToDto(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating postal code: {Code} for country: {CountryCode}", createDto.Code, createDto.CountryCode);
            throw;
        }
    }

    public async Task<PostalCodeDto?> UpdatePostalCodeAsync(int id, UpdatePostalCodeDto updateDto)
    {
        try
        {
            _log.Information("Updating postal code with ID: {PostalCodeId}", id);

            var postalCode = await _context.PostalCodes.FindAsync(id);

            if (postalCode == null)
            {
                _log.Warning("Postal code with ID {PostalCodeId} not found for update", id);
                return null;
            }

            var countryExists = await _context.Countries
                .AnyAsync(c => c.Code == updateDto.CountryCode);

            if (!countryExists)
            {
                _log.Warning("Country with code {CountryCode} does not exist", updateDto.CountryCode);
                throw new InvalidOperationException($"Country with code {updateDto.CountryCode} does not exist");
            }

            var duplicateCode = await _context.PostalCodes
                .AnyAsync(p => p.Code == updateDto.Code && p.CountryCode == updateDto.CountryCode && p.Id != id);

            if (duplicateCode)
            {
                _log.Warning("Cannot update postal code {PostalCodeId}: {Code} for country {CountryCode} already exists", 
                    id, updateDto.Code, updateDto.CountryCode);
                throw new InvalidOperationException($"Postal code {updateDto.Code} for country {updateDto.CountryCode} already exists");
            }

            postalCode.Code = updateDto.Code;
            postalCode.CountryCode = updateDto.CountryCode.ToUpper();
            postalCode.City = updateDto.City;
            postalCode.State = updateDto.State;
            postalCode.Region = updateDto.Region;
            postalCode.District = updateDto.District;
            postalCode.Latitude = updateDto.Latitude;
            postalCode.Longitude = updateDto.Longitude;
            postalCode.IsActive = updateDto.IsActive;
            postalCode.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated postal code with ID: {PostalCodeId}", id);
            return MapToDto(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating postal code with ID: {PostalCodeId}", id);
            throw;
        }
    }

    public async Task<bool> DeletePostalCodeAsync(int id)
    {
        try
        {
            _log.Information("Deleting postal code with ID: {PostalCodeId}", id);

            var postalCode = await _context.PostalCodes.FindAsync(id);

            if (postalCode == null)
            {
                _log.Warning("Postal code with ID {PostalCodeId} not found for deletion", id);
                return false;
            }

            _context.PostalCodes.Remove(postalCode);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted postal code with ID: {PostalCodeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting postal code with ID: {PostalCodeId}", id);
            throw;
        }
    }

    private static PostalCodeDto MapToDto(PostalCode postalCode)
    {
        return new PostalCodeDto
        {
            Id = postalCode.Id,
            Code = postalCode.Code,
            CountryCode = postalCode.CountryCode,
            City = postalCode.City,
            State = postalCode.State,
            Region = postalCode.Region,
            District = postalCode.District,
            Latitude = postalCode.Latitude,
            Longitude = postalCode.Longitude,
            IsActive = postalCode.IsActive,
            CreatedAt = postalCode.CreatedAt,
            UpdatedAt = postalCode.UpdatedAt
        };
    }
}
