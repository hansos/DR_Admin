using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer credits (stub implementation)
/// </summary>
public class CreditService : ICreditService
{
    public Task<CustomerCreditDto?> GetCustomerCreditAsync(int customerId)
    {
        return Task.FromResult<CustomerCreditDto?>(null);
    }

    public Task<IEnumerable<CreditTransactionDto>> GetCreditTransactionsAsync(int customerId)
    {
        return Task.FromResult(Enumerable.Empty<CreditTransactionDto>());
    }

    public Task<CreditTransactionDto> CreateCreditTransactionAsync(CreateCreditTransactionDto createDto, int userId)
    {
        throw new NotImplementedException("CreditService.CreateCreditTransactionAsync not yet implemented");
    }

    public Task<decimal> AddCreditAsync(int customerId, decimal amount, string description, int userId)
    {
        throw new NotImplementedException("CreditService.AddCreditAsync not yet implemented");
    }

    public Task<decimal> DeductCreditAsync(int customerId, decimal amount, int? invoiceId, string description, int userId)
    {
        throw new NotImplementedException("CreditService.DeductCreditAsync not yet implemented");
    }

    public Task<bool> HasSufficientCreditAsync(int customerId, decimal amount)
    {
        return Task.FromResult(false);
    }
}

