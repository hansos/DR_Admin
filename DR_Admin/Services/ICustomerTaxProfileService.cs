using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer tax profiles
/// </summary>
public interface ICustomerTaxProfileService
{
    Task<IEnumerable<CustomerTaxProfileDto>> GetAllCustomerTaxProfilesAsync();
    Task<CustomerTaxProfileDto?> GetCustomerTaxProfileByIdAsync(int id);
    Task<CustomerTaxProfileDto?> GetCustomerTaxProfileByCustomerIdAsync(int customerId);
    Task<CustomerTaxProfileDto> CreateCustomerTaxProfileAsync(CreateCustomerTaxProfileDto dto);
    Task<CustomerTaxProfileDto?> UpdateCustomerTaxProfileAsync(int id, UpdateCustomerTaxProfileDto dto);
    Task<bool> DeleteCustomerTaxProfileAsync(int id);
    Task<TaxIdValidationResultDto?> ValidateTaxIdAsync(ValidateTaxIdDto dto);
}
