using ISPAdmin.DTOs;
using System.IO;

namespace ISPAdmin.Services;

public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<PagedResult<CountryDto>> GetAllCountriesPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<CountryDto>> GetActiveCountriesAsync();
    Task<PagedResult<CountryDto>> GetActiveCountriesPagedAsync(PaginationParameters parameters);
    Task<CountryDto?> GetCountryByIdAsync(int id);
    Task<CountryDto?> GetCountryByCodeAsync(string code);
    Task<CountryDto> CreateCountryAsync(CreateCountryDto createDto);
    Task<CountryDto?> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
    Task<bool> DeleteCountryAsync(int id);
    Task<int> MergeCountriesFromCsvAsync(Stream csvStream);
    Task<int> MergeLocalizedNamesFromCsvAsync(Stream csvStream);
    Task<int> SetAllCountriesActiveAsync(bool isActive);
    Task<int> SetCountriesActiveByCodesAsync(IEnumerable<string> codes, bool isActive);
}
