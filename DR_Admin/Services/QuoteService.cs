using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing quotes (stub implementation)
/// </summary>
public class QuoteService : IQuoteService
{
    public Task<IEnumerable<QuoteDto>> GetAllQuotesAsync()
    {
        return Task.FromResult(Enumerable.Empty<QuoteDto>());
    }

    public Task<IEnumerable<QuoteDto>> GetQuotesByCustomerIdAsync(int customerId)
    {
        return Task.FromResult(Enumerable.Empty<QuoteDto>());
    }

    public Task<IEnumerable<QuoteDto>> GetQuotesByStatusAsync(QuoteStatus status)
    {
        return Task.FromResult(Enumerable.Empty<QuoteDto>());
    }

    public Task<QuoteDto?> GetQuoteByIdAsync(int id)
    {
        return Task.FromResult<QuoteDto?>(null);
    }

    public Task<QuoteDto> CreateQuoteAsync(CreateQuoteDto createDto, int userId)
    {
        throw new NotImplementedException("QuoteService.CreateQuoteAsync not yet implemented");
    }

    public Task<QuoteDto?> UpdateQuoteAsync(int id, UpdateQuoteDto updateDto)
    {
        throw new NotImplementedException("QuoteService.UpdateQuoteAsync not yet implemented");
    }

    public Task<bool> DeleteQuoteAsync(int id)
    {
        throw new NotImplementedException("QuoteService.DeleteQuoteAsync not yet implemented");
    }

    public Task<bool> SendQuoteAsync(int id)
    {
        throw new NotImplementedException("QuoteService.SendQuoteAsync not yet implemented");
    }

    public Task<bool> AcceptQuoteAsync(string token)
    {
        throw new NotImplementedException("QuoteService.AcceptQuoteAsync not yet implemented");
    }

    public Task<bool> RejectQuoteAsync(int id, string reason)
    {
        throw new NotImplementedException("QuoteService.RejectQuoteAsync not yet implemented");
    }

    public Task<int?> ConvertQuoteToOrderAsync(int id)
    {
        throw new NotImplementedException("QuoteService.ConvertQuoteToOrderAsync not yet implemented");
    }

    public Task<byte[]> GenerateQuotePdfAsync(int id)
    {
        throw new NotImplementedException("QuoteService.GenerateQuotePdfAsync not yet implemented");
    }
}

