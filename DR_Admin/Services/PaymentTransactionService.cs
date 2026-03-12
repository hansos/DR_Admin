using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for querying payment transaction list data.
/// </summary>
public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentTransactionService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentTransactionService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PaymentTransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all payment transactions with invoice and allocation details.
    /// </summary>
    /// <returns>A collection of payment transaction list items.</returns>
    public async Task<IEnumerable<PaymentTransactionListDto>> GetAllAsync()
    {
        _log.Information("Fetching all payment transactions for list view");

        var transactions = await _context.PaymentTransactions
            .AsNoTracking()
            .Include(t => t.Invoice)
            .Include(t => t.PaymentGateway)
            .Where(t => t.Invoice.DeletedAt == null)
            .OrderByDescending(t => t.ProcessedAt ?? t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Select(t => new PaymentTransactionListDto
            {
                Id = t.Id,
                InvoiceId = t.InvoiceId,
                InvoiceNumber = t.Invoice.InvoiceNumber,
                CustomerId = t.Invoice.CustomerId,
                CustomerName = t.Invoice.CustomerName,
                PaymentMethod = t.PaymentMethod,
                Status = t.Status.ToString(),
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                CurrencyCode = t.CurrencyCode,
                PaymentGatewayId = t.PaymentGatewayId,
                PaymentGatewayName = t.PaymentGateway != null ? t.PaymentGateway.Name : string.Empty,
                ProcessedAt = t.ProcessedAt,
                RefundedAmount = t.RefundedAmount,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        if (transactions.Count == 0)
        {
            _log.Information("Fetched 0 payment transactions for list view");
            return transactions;
        }

        var transactionIds = transactions.Select(t => t.Id).ToList();

        var allocations = await _context.InvoicePayments
            .AsNoTracking()
            .Where(p => transactionIds.Contains(p.PaymentTransactionId))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.PaymentTransactionId,
                Allocation = new PaymentTransactionAllocationDto
                {
                    Id = p.Id,
                    AmountApplied = p.AmountApplied,
                    Currency = p.Currency,
                    InvoiceBalance = p.InvoiceBalance,
                    InvoiceTotalAmount = p.InvoiceTotalAmount,
                    IsFullPayment = p.IsFullPayment,
                    CreatedAt = p.CreatedAt
                }
            })
            .ToListAsync();

        var allocationsByTransactionId = allocations
            .GroupBy(x => x.PaymentTransactionId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Allocation).ToList());

        foreach (var transaction in transactions)
        {
            if (allocationsByTransactionId.TryGetValue(transaction.Id, out var linkedAllocations))
            {
                transaction.Allocations = linkedAllocations;
            }
        }

        _log.Information("Fetched {Count} payment transactions for list view", transactions.Count);
        return transactions;
    }
}
