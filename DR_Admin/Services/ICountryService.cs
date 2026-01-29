using ISPAdmin.DTOs;
using System.IO;

namespace ISPAdmin.Services;

public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<IEnumerable<CountryDto>> GetActiveCountriesAsync();
    Task<CountryDto?> GetCountryByIdAsync(int id);
    Task<CountryDto?> GetCountryByCodeAsync(string code);
    Task<CountryDto> CreateCountryAsync(CreateCountryDto createDto);
    Task<CountryDto?> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
    Task<bool> DeleteCountryAsync(int id);
    Task<int> MergeCountriesFromCsvAsync(Stream csvStream);
}
