using System.Text;
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

            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == createDto.CountryCode);

            if (country == null)
            {
                _log.Warning("Country with code {CountryCode} does not exist", createDto.CountryCode);
                throw new InvalidOperationException($"Country with code {createDto.CountryCode} does not exist");
            }

            if (!country.IsActive)
            {
                _log.Warning("Country with code {CountryCode} is not active", createDto.CountryCode);
                throw new InvalidOperationException($"Country with code {createDto.CountryCode} is not active");
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

    public async Task<ImportPostalCodesResultDto> ImportPostalCodesAsync(ImportPostalCodesDto importDto)
    {
        var result = new ImportPostalCodesResultDto();
        
        try
        {
            _log.Information("Starting import of {Count} postal codes for country: {CountryCode}", 
                importDto.PostalCodes.Count, importDto.CountryCode);

            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == importDto.CountryCode);

            if (country == null)
            {
                var error = $"Country with code {importDto.CountryCode} does not exist";
                _log.Warning(error);
                result.Errors.Add(error);
                return result;
            }

            if (!country.IsActive)
            {
                var error = $"Country with code {importDto.CountryCode} is not active";
                _log.Warning(error);
                result.Errors.Add(error);
                return result;
            }

            var countryCode = importDto.CountryCode.ToUpper();
            var postalCodesList = importDto.PostalCodes;

            var existingPostalCodes = await _context.PostalCodes
                .Where(p => p.CountryCode == countryCode)
                .ToDictionaryAsync(p => p.Code, p => p);

            foreach (var item in postalCodesList)
            {
                result.TotalProcessed++;

                try
                {
                    if (string.IsNullOrWhiteSpace(item.Code))
                    {
                        result.Errors.Add($"Row {result.TotalProcessed}: Postal code is required");
                        result.Skipped++;
                        continue;
                    }

                    if (existingPostalCodes.TryGetValue(item.Code, out var existingPostalCode))
                    {
                        existingPostalCode.City = item.City;
                        existingPostalCode.State = item.State;
                        existingPostalCode.Region = item.Region;
                        existingPostalCode.District = item.District;
                        existingPostalCode.Latitude = item.Latitude;
                        existingPostalCode.Longitude = item.Longitude;
                        existingPostalCode.IsActive = item.IsActive;
                        existingPostalCode.UpdatedAt = DateTime.UtcNow;
                        result.Updated++;
                    }
                    else
                    {
                        var newPostalCode = new PostalCode
                        {
                            Code = item.Code,
                            CountryCode = countryCode,
                            City = item.City,
                            State = item.State,
                            Region = item.Region,
                            District = item.District,
                            Latitude = item.Latitude,
                            Longitude = item.Longitude,
                            IsActive = item.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.PostalCodes.Add(newPostalCode);
                        result.Created++;
                    }
                }
                catch (Exception ex)
                {
                    var error = $"Row {result.TotalProcessed} (Code: {item.Code}): {ex.Message}";
                    _log.Warning(ex, "Error processing postal code import item: {Code}", item.Code);
                    result.Errors.Add(error);
                    result.Skipped++;
                }
            }

            await _context.SaveChangesAsync();

            _log.Information("Import completed for country {CountryCode}: {Created} created, {Updated} updated, {Skipped} skipped", 
                countryCode, result.Created, result.Updated, result.Skipped);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during postal codes import for country: {CountryCode}", importDto.CountryCode);
            result.Errors.Add($"Import failed: {ex.Message}");
            throw;
        }
    }

    public async Task<ImportPostalCodesResultDto> ImportPostalCodesFromCsvAsync(Stream csvStream, string countryCode)
    {
        var result = new ImportPostalCodesResultDto();

        try
        {
            _log.Information("Starting CSV import of postal codes for country: {CountryCode}", countryCode);

            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == countryCode);

            if (country == null)
            {
                var error = $"Country with code {countryCode} does not exist";
                _log.Warning(error);
                result.Errors.Add(error);
                return result;
            }

            if (!country.IsActive)
            {
                var error = $"Country with code {countryCode} is not active";
                _log.Warning(error);
                result.Errors.Add(error);
                return result;
            }

            var normalizedCountryCode = countryCode.ToUpper();
            var postalCodesList = new List<ImportPostalCodeItemDto>();

            using var reader = new StreamReader(csvStream);
            
            string? firstLine = await reader.ReadLineAsync();
            if (firstLine == null)
            {
                result.Errors.Add("CSV file is empty");
                return result;
            }

            char delimiter = DetectDelimiter(firstLine);
            _log.Information("Detected CSV delimiter: {Delimiter}", delimiter == '\t' ? "TAB" : delimiter.ToString());

            string? line;
            int lineNumber = 1;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var parts = ParseCsvLine(line, delimiter);
                    
                    if (parts.Length < 2)
                    {
                        result.Errors.Add($"Line {lineNumber}: Insufficient columns (expected at least PostalCode,City)");
                        continue;
                    }

                    var item = new ImportPostalCodeItemDto
                    {
                        Code = parts[0].Trim(),
                        City = parts[1].Trim(),
                        State = parts.Length > 2 ? parts[2].Trim() : null,
                        Region = parts.Length > 3 ? parts[3].Trim() : null,
                        District = parts.Length > 4 ? parts[4].Trim() : null,
                        Latitude = parts.Length > 5 && decimal.TryParse(parts[5].Trim(), out var lat) ? lat : null,
                        Longitude = parts.Length > 6 && decimal.TryParse(parts[6].Trim(), out var lon) ? lon : null,
                        IsActive = parts.Length > 7 && bool.TryParse(parts[7].Trim(), out var active) ? active : true
                    };

                    postalCodesList.Add(item);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Line {lineNumber}: Error parsing line - {ex.Message}");
                }
            }

            _log.Information("Parsed {Count} postal codes from CSV", postalCodesList.Count);

            var importDto = new ImportPostalCodesDto
            {
                CountryCode = normalizedCountryCode,
                PostalCodes = postalCodesList
            };

            return await ImportPostalCodesAsync(importDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during CSV import for country: {CountryCode}", countryCode);
            result.Errors.Add($"CSV import failed: {ex.Message}");
            throw;
        }
    }

    private static char DetectDelimiter(string line)
    {
        if (line.Contains('\t')) return '\t';
        if (line.Contains(';')) return ';';
        if (line.Contains(',')) return ',';
        return '\t';
    }

    private static string[] ParseCsvLine(string line, char delimiter)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        fields.Add(currentField.ToString());

        return fields.ToArray();
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
