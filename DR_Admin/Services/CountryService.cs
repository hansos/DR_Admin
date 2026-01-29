using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;
using System.Collections.Generic;

namespace ISPAdmin.Services;

public class CountryService : ICountryService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CountryService>();

    public CountryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> MergeLocalizedNamesFromCsvAsync(System.IO.Stream csvStream)
    {
        try
        {
            _log.Information("Merging localized names from CSV with batching");

            using var reader = new System.IO.StreamReader(csvStream);

            string? headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new InvalidOperationException("CSV file is empty");
            }

            const int batchSize = 200;
            var merged = 0;
            var processedInBatch = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = ParseCsvLine(line);
                if (parts.Count < 2) continue;

                var iso2 = parts[0].Trim().ToUpper();
                var localName = parts[1].Trim();

                if (string.IsNullOrEmpty(iso2)) continue;

                var country = _context.Countries.Local.FirstOrDefault(c => c.Code == iso2);
                if (country == null)
                {
                    country = await _context.Countries.FirstOrDefaultAsync(c => c.Code == iso2);
                }

                if (country == null)
                {
                    // skip unknown countries
                    continue;
                }

                if (!string.Equals(country.LocalName, localName, StringComparison.Ordinal))
                {
                    country.LocalName = localName;
                    country.NormalizedLocalName = localName.ToUpperInvariant();
                    country.UpdatedAt = DateTime.UtcNow;
                    merged++;
                }

                processedInBatch++;

                if (processedInBatch >= batchSize)
                {
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    processedInBatch = 0;
                }
            }

            if (processedInBatch > 0)
            {
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            _log.Information("Successfully merged {Count} localized names from CSV", merged);
            return merged;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while merging localized names from CSV");
            throw;
        }
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        if (line == null) return result;

        var current = new System.Text.StringBuilder();
        var inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // escaped quote
                    current.Append('"');
                    i++; // skip next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result;
    }

    public async Task<int> MergeCountriesFromCsvAsync(System.IO.Stream csvStream)
    {
        try
        {
            _log.Information("Merging countries from CSV with batching");

            using var reader = new System.IO.StreamReader(csvStream);

            string? headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new InvalidOperationException("CSV file is empty");
            }

            const int batchSize = 50;
            var merged = 0;
            var processedInBatch = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = ParseCsvLine(line);
                if (parts.Count < 5) continue;

                var name = parts[0].Trim();
                var iso2 = parts[1].Trim().ToUpper();
                var iso3 = parts[2].Trim();
                var tld = parts[3].Trim();
                var numericStr = parts[4].Trim();

                int? numeric = null;
                if (int.TryParse(numericStr, out var n)) numeric = n;

                if (string.IsNullOrEmpty(iso2)) continue;

                // First check the DbContext local tracker for entities already added in this session
                var country = _context.Countries.Local.FirstOrDefault(c => c.Code == iso2);
                if (country == null)
                {
                    country = await _context.Countries.FirstOrDefaultAsync(c => c.Code == iso2);
                }

                if (country == null)
                {
                    country = new Country
                    {
                        Code = iso2,
                        Iso3 = string.IsNullOrWhiteSpace(iso3) ? null : iso3,
                        Tld = tld.StartsWith('.') ? tld.Substring(1) : tld,
                        Numeric = numeric,
                        EnglishName = name,
                        LocalName = name,
                        NormalizedEnglishName = name.ToUpperInvariant(),
                        NormalizedLocalName = name.ToUpperInvariant(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Countries.Add(country);
                    merged++;
                }
                else
                {
                    // update fields if different
                    var changed = false;
                    if (!string.Equals(country.EnglishName, name, StringComparison.Ordinal))
                    {
                        country.EnglishName = name;
                        country.NormalizedEnglishName = name.ToUpperInvariant();
                        changed = true;
                    }

                    if (!string.Equals(country.Code, iso2, StringComparison.OrdinalIgnoreCase))
                    {
                        country.Code = iso2;
                        changed = true;
                    }

                    if (country.Iso3 != iso3)
                    {
                        country.Iso3 = string.IsNullOrWhiteSpace(iso3) ? null : iso3;
                        changed = true;
                    }

                    if (country.Numeric != numeric)
                    {
                        country.Numeric = numeric;
                        changed = true;
                    }

                    if (!string.Equals(country.Tld, tld.StartsWith('.') ? tld.Substring(1) : tld, StringComparison.Ordinal))
                    {
                        country.Tld = tld.StartsWith('.') ? tld.Substring(1) : tld;
                        changed = true;
                    }

                    if (changed)
                    {
                        country.UpdatedAt = DateTime.UtcNow;
                        merged++;
                    }
                }

                processedInBatch++;

                if (processedInBatch >= batchSize)
                {
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    processedInBatch = 0;
                }
            }

            if (processedInBatch > 0)
            {
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            _log.Information("Successfully merged {Count} countries from CSV", merged);
            return merged;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while merging countries from CSV");
            throw;
        }
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
                Iso3 = createDto.Iso3,
                Numeric = createDto.Numeric,
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
            country.Iso3 = updateDto.Iso3;
            country.Numeric = updateDto.Numeric;
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
                Iso3 = country.Iso3,
                Numeric = country.Numeric,
            EnglishName = country.EnglishName,
            LocalName = country.LocalName,
            IsActive = country.IsActive,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }
}
