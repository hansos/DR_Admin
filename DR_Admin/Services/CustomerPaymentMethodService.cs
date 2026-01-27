using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer payment methods (stub implementation)
/// </summary>
public class CustomerPaymentMethodService : ICustomerPaymentMethodService
{
    public Task<IEnumerable<CustomerPaymentMethodDto>> GetPaymentMethodsByCustomerIdAsync(int customerId)
    {
        return Task.FromResult(Enumerable.Empty<CustomerPaymentMethodDto>());
    }

    public Task<CustomerPaymentMethodDto?> GetPaymentMethodByIdAsync(int id)
    {
        return Task.FromResult<CustomerPaymentMethodDto?>(null);
    }

    public Task<CustomerPaymentMethodDto?> GetDefaultPaymentMethodAsync(int customerId)
    {
        return Task.FromResult<CustomerPaymentMethodDto?>(null);
    }

    public Task<CustomerPaymentMethodDto> CreatePaymentMethodAsync(CreateCustomerPaymentMethodDto createDto)
    {
        throw new NotImplementedException("CustomerPaymentMethodService.CreatePaymentMethodAsync not yet implemented");
    }

    public Task<bool> SetAsDefaultAsync(int id, int customerId)
    {
        throw new NotImplementedException("CustomerPaymentMethodService.SetAsDefaultAsync not yet implemented");
    }

    public Task<bool> DeletePaymentMethodAsync(int id, int customerId)
    {
        throw new NotImplementedException("CustomerPaymentMethodService.DeletePaymentMethodAsync not yet implemented");
    }
}

