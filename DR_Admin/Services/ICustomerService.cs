using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<PagedResult<CustomerDto>> GetAllCustomersPagedAsync(PaginationParameters parameters);
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto?> GetCustomerByEmailAsync(string email);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto);
    Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto);
    Task<bool> DeleteCustomerAsync(int id);
}
