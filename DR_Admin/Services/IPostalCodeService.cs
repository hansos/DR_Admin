using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IPostalCodeService
{
    Task<IEnumerable<PostalCodeDto>> GetAllPostalCodesAsync();
    Task<PagedResult<PostalCodeDto>> GetAllPostalCodesPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<PostalCodeDto>> GetPostalCodesByCountryAsync(string countryCode);
    Task<IEnumerable<PostalCodeDto>> GetPostalCodesByCityAsync(string city);
    Task<PostalCodeDto?> GetPostalCodeByIdAsync(int id);
    Task<PostalCodeDto?> GetPostalCodeByCodeAndCountryAsync(string code, string countryCode);
    Task<PostalCodeDto> CreatePostalCodeAsync(CreatePostalCodeDto createDto);
    Task<PostalCodeDto?> UpdatePostalCodeAsync(int id, UpdatePostalCodeDto updateDto);
    Task<bool> DeletePostalCodeAsync(int id);
    Task<ImportPostalCodesResultDto> ImportPostalCodesAsync(ImportPostalCodesDto importDto);
    Task<ImportPostalCodesResultDto> ImportPostalCodesFromCsvAsync(Stream csvStream, string countryCode);
}
