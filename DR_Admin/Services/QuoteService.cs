using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing quotes (stub implementation)
/// </summary>
public class QuoteService : IQuoteService
{
    private readonly ApplicationDbContext _context;

    public QuoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuoteDto>> GetAllQuotesAsync()
    {
        var quotes = await _context.Quotes
            .AsNoTracking()
            .Where(q => q.DeletedAt == null)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return quotes.Select(MapToDto);
    }

    public async Task<IEnumerable<QuoteDto>> GetQuotesByCustomerIdAsync(int customerId)
    {
        var quotes = await _context.Quotes
            .AsNoTracking()
            .Where(q => q.DeletedAt == null && q.CustomerId == customerId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return quotes.Select(MapToDto);
    }

    public async Task<IEnumerable<QuoteDto>> GetQuotesByStatusAsync(QuoteStatus status)
    {
        var quotes = await _context.Quotes
            .AsNoTracking()
            .Where(q => q.DeletedAt == null && q.Status == status)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return quotes.Select(MapToDto);
    }

    public async Task<QuoteDto?> GetQuoteByIdAsync(int id)
    {
        var quote = await _context.Quotes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.DeletedAt == null && q.Id == id);

        return quote == null ? null : MapToDto(quote);
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
        return ConvertQuoteToOrderInternalAsync(id);
    }

    private async Task<int?> ConvertQuoteToOrderInternalAsync(int id)
    {
        var quote = await _context.Quotes
            .Include(q => q.QuoteLines)
            .FirstOrDefaultAsync(q => q.DeletedAt == null && q.Id == id);

        if (quote == null)
        {
            return null;
        }

        var existingOrder = await _context.Orders
            .FirstOrDefaultAsync(o => o.QuoteId == quote.Id);

        if (existingOrder != null)
        {
            if (quote.Status != QuoteStatus.Converted)
            {
                quote.Status = QuoteStatus.Converted;
                quote.AcceptedAt ??= DateTime.UtcNow;
                quote.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return existingOrder.Id;
        }

        var now = DateTime.UtcNow;
        var serviceId = quote.QuoteLines
            .Where(line => line.DeletedAt == null && line.ServiceId.HasValue)
            .Select(line => line.ServiceId)
            .FirstOrDefault();

        var fallbackServiceId = await _context.Services
            .AsNoTracking()
            .OrderBy(service => service.Id)
            .Select(service => (int?)service.Id)
            .FirstOrDefaultAsync();

        var order = new Order
        {
            OrderNumber = await GenerateOrderNumberAsync(),
            CustomerId = quote.CustomerId,
            ServiceId = serviceId ?? fallbackServiceId,
            QuoteId = quote.Id,
            CouponId = quote.CouponId,
            OrderType = OrderType.New,
            Status = OrderStatus.Pending,
            StartDate = now,
            EndDate = now.AddYears(1),
            NextBillingDate = now.AddMonths(1),
            SetupFee = quote.TotalSetupFee,
            RecurringAmount = quote.TotalRecurring,
            DiscountAmount = quote.DiscountAmount,
            CurrencyCode = quote.CurrencyCode,
            BaseCurrencyCode = quote.CurrencyCode,
            AutoRenew = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.Orders.Add(order);

        quote.Status = QuoteStatus.Converted;
        quote.AcceptedAt ??= now;
        quote.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return order.Id;
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrderId = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.Id)
            .Select(order => order.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastOrderId + 1;
        return $"ORD-{DateTime.UtcNow.Year}-{nextNumber:D5}";
    }

    public Task<byte[]> GenerateQuotePdfAsync(int id)
    {
        throw new NotImplementedException("QuoteService.GenerateQuotePdfAsync not yet implemented");
    }

    private static QuoteDto MapToDto(Quote quote)
    {
        return new QuoteDto
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CustomerId = quote.CustomerId,
            CustomerName = quote.CustomerName,
            Status = quote.Status,
            ValidUntil = quote.ValidUntil,
            SubTotal = quote.SubTotal,
            TotalSetupFee = quote.TotalSetupFee,
            TotalRecurring = quote.TotalRecurring,
            TaxAmount = quote.TaxAmount,
            TotalAmount = quote.TotalAmount,
            CurrencyCode = quote.CurrencyCode,
            TaxRate = quote.TaxRate,
            TaxName = quote.TaxName,
            CustomerAddress = quote.CustomerAddress,
            CustomerTaxId = quote.CustomerTaxId,
            Notes = quote.Notes,
            TermsAndConditions = quote.TermsAndConditions,
            InternalComment = quote.InternalComment,
            SentAt = quote.SentAt,
            AcceptedAt = quote.AcceptedAt,
            RejectedAt = quote.RejectedAt,
            RejectionReason = quote.RejectionReason,
            PreparedByUserId = quote.PreparedByUserId,
            CouponId = quote.CouponId,
            DiscountAmount = quote.DiscountAmount,
            CreatedAt = quote.CreatedAt,
            UpdatedAt = quote.UpdatedAt
        };
    }
}

