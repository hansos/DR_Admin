using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.PaymentGateways;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for payment processing operations
/// </summary>
public class PaymentProcessingService : IPaymentProcessingService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentGatewayService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentProcessingService>();

    public PaymentProcessingService(
        ApplicationDbContext context,
        IPaymentGatewayService paymentGatewayService)
    {
        _context = context;
        _paymentGatewayService = paymentGatewayService;
    }

    public async Task<PaymentResultDto> ProcessInvoicePaymentAsync(ProcessInvoicePaymentDto dto)
    {
        try
        {
            _log.Information("Processing invoice payment for invoice ID: {InvoiceId}", dto.InvoiceId);

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);

            if (invoice == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Invoice not found"
                };
            }

            var paymentMethod = await _context.CustomerPaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == dto.CustomerPaymentMethodId);

            if (paymentMethod == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment method not found"
                };
            }

            // Create payment attempt
            var attempt = new PaymentAttempt
            {
                InvoiceId = dto.InvoiceId,
                CustomerPaymentMethodId = dto.CustomerPaymentMethodId,
                AttemptedAmount = invoice.TotalAmount,
                Currency = invoice.CurrencyCode,
                Status = PaymentAttemptStatus.Processing,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PaymentAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            // Get payment gateway adapter
            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == paymentMethod.PaymentGatewayId);

            if (gateway == null)
            {
                attempt.Status = PaymentAttemptStatus.Failed;
                attempt.ErrorMessage = "Payment gateway not found";
                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment gateway not configured"
                };
            }

            // Get payment token
            var token = await _context.PaymentMethodTokens
                .FirstOrDefaultAsync(t => t.CustomerPaymentMethodId == dto.CustomerPaymentMethodId);

            if (token == null)
            {
                attempt.Status = PaymentAttemptStatus.Failed;
                attempt.ErrorMessage = "Payment method token not found";
                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment method not properly configured"
                };
            }

            // TODO: Get actual gateway adapter based on gateway name
            // For now, we'll create a placeholder response
            
            // Simulate payment processing
            var isSuccess = true; // This would come from actual gateway

            if (isSuccess)
            {
                // Create payment transaction
                var transaction = new PaymentTransaction
                {
                    InvoiceId = invoice.Id,
                    PaymentGatewayId = gateway.Id,
                    Amount = invoice.TotalAmount,
                    CurrencyCode = invoice.CurrencyCode,
                    TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Status = PaymentTransactionStatus.Completed,
                    ProcessedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PaymentTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Update payment attempt
                attempt.Status = PaymentAttemptStatus.Succeeded;
                attempt.PaymentTransactionId = transaction.Id;
                attempt.GatewayTransactionId = transaction.TransactionId;
                attempt.UpdatedAt = DateTime.UtcNow;

                // Update invoice status
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidAt = DateTime.UtcNow;
                invoice.UpdatedAt = DateTime.UtcNow;

                // Create invoice payment record
                var invoicePayment = new InvoicePayment
                {
                    InvoiceId = invoice.Id,
                    PaymentTransactionId = transaction.Id,
                    AmountApplied = invoice.TotalAmount,
                    Currency = invoice.CurrencyCode,
                    InvoiceBalance = 0,
                    InvoiceTotalAmount = invoice.TotalAmount,
                    IsFullPayment = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.InvoicePayments.Add(invoicePayment);
                await _context.SaveChangesAsync();

                _log.Information("Successfully processed payment for invoice ID: {InvoiceId}", dto.InvoiceId);

                return new PaymentResultDto
                {
                    IsSuccess = true,
                    PaymentAttemptId = attempt.Id,
                    PaymentTransactionId = transaction.Id,
                    TransactionId = transaction.TransactionId
                };
            }
            else
            {
                attempt.Status = PaymentAttemptStatus.Failed;
                attempt.ErrorMessage = "Payment declined";
                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = false,
                    PaymentAttemptId = attempt.Id,
                    ErrorMessage = "Payment was declined"
                };
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error processing payment for invoice ID: {InvoiceId}", dto.InvoiceId);
            throw;
        }
    }

    public async Task<bool> HandlePaymentWebhookAsync(string gatewayName, string payload, string signature)
    {
        try
        {
            _log.Information("Handling payment webhook from gateway: {GatewayName}", gatewayName);
            
            // TODO: Implement webhook verification and processing
            // This would:
            // 1. Verify webhook signature
            // 2. Parse webhook payload
            // 3. Update payment attempt/transaction status
            // 4. Update invoice status
            // 5. Send notifications
            
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error handling payment webhook from gateway: {GatewayName}", gatewayName);
            return false;
        }
    }

    public async Task<PaymentResultDto> RetryFailedPaymentAsync(int paymentAttemptId)
    {
        try
        {
            _log.Information("Retrying failed payment for attempt ID: {PaymentAttemptId}", paymentAttemptId);

            var attempt = await _context.PaymentAttempts
                .Include(a => a.Invoice)
                .Include(a => a.CustomerPaymentMethod)
                .FirstOrDefaultAsync(a => a.Id == paymentAttemptId);

            if (attempt == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment attempt not found"
                };
            }

            if (attempt.Status != PaymentAttemptStatus.Failed)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Only failed payments can be retried"
                };
            }

            // Create new payment attempt
            var dto = new ProcessInvoicePaymentDto
            {
                InvoiceId = attempt.InvoiceId,
                CustomerPaymentMethodId = attempt.CustomerPaymentMethodId,
                IpAddress = attempt.IpAddress,
                UserAgent = attempt.UserAgent
            };

            return await ProcessInvoicePaymentAsync(dto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrying payment for attempt ID: {PaymentAttemptId}", paymentAttemptId);
            throw;
        }
    }

    public async Task<PaymentResultDto> ApplyCustomerCreditAsync(ApplyCustomerCreditDto dto)
    {
        try
        {
            _log.Information("Applying customer credit to invoice ID: {InvoiceId}", dto.InvoiceId);

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);

            if (invoice == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Invoice not found"
                };
            }

            var customerCredit = await _context.CustomerCredits
                .FirstOrDefaultAsync(c => c.CustomerId == invoice.CustomerId);

            if (customerCredit == null || customerCredit.Balance < dto.Amount)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Insufficient customer credit"
                };
            }

            // Deduct from credit balance
            customerCredit.Balance -= dto.Amount;
            customerCredit.UpdatedAt = DateTime.UtcNow;

            // Create credit transaction
            var creditTransaction = new CreditTransaction
            {
                CustomerCreditId = customerCredit.Id,
                Amount = -dto.Amount,
                Type = CreditTransactionType.Deduction,
                Description = $"Applied to invoice {invoice.InvoiceNumber}",
                InvoiceId = invoice.Id,
                BalanceAfter = customerCredit.Balance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CreditTransactions.Add(creditTransaction);

            // Update invoice
            if (dto.Amount >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidAt = DateTime.UtcNow;
            }
            // Note: InvoiceStatus doesn't have PartiallyPaid, keeping as Issued

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully applied {Amount} credit to invoice ID: {InvoiceId}", dto.Amount, dto.InvoiceId);

            return new PaymentResultDto
            {
                IsSuccess = true,
                TransactionId = $"CREDIT-{creditTransaction.Id}"
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error applying customer credit to invoice ID: {InvoiceId}", dto.InvoiceId);
            throw;
        }
    }

    public async Task<PaymentResultDto> ProcessPartialPaymentAsync(ProcessPartialPaymentDto dto)
    {
        try
        {
            _log.Information("Processing partial payment for invoice ID: {InvoiceId}", dto.InvoiceId);

            // Similar to ProcessInvoicePaymentAsync but with partial amount
            // TODO: Implement partial payment logic
            
            return new PaymentResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Partial payments not yet implemented"
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error processing partial payment for invoice ID: {InvoiceId}", dto.InvoiceId);
            throw;
        }
    }

    public async Task<PaymentResultDto> ConfirmAuthenticationAsync(int paymentAttemptId)
    {
        try
        {
            _log.Information("Confirming authentication for payment attempt ID: {PaymentAttemptId}", paymentAttemptId);

            // TODO: Implement 3D Secure confirmation logic
            
            return new PaymentResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Authentication confirmation not yet implemented"
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error confirming authentication for payment attempt ID: {PaymentAttemptId}", paymentAttemptId);
            throw;
        }
    }

    public async Task<IEnumerable<PaymentAttemptDto>> GetPaymentAttemptsByInvoiceIdAsync(int invoiceId)
    {
        try
        {
            var attempts = await _context.PaymentAttempts
                .AsNoTracking()
                .Where(a => a.InvoiceId == invoiceId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return attempts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving payment attempts for invoice ID: {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<PaymentAttemptDto?> GetPaymentAttemptByIdAsync(int id)
    {
        try
        {
            var attempt = await _context.PaymentAttempts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return attempt == null ? null : MapToDto(attempt);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving payment attempt with ID: {Id}", id);
            throw;
        }
    }

    private static PaymentAttemptDto MapToDto(PaymentAttempt attempt)
    {
        return new PaymentAttemptDto
        {
            Id = attempt.Id,
            InvoiceId = attempt.InvoiceId,
            PaymentTransactionId = attempt.PaymentTransactionId,
            CustomerPaymentMethodId = attempt.CustomerPaymentMethodId,
            AttemptedAmount = attempt.AttemptedAmount,
            Currency = attempt.Currency,
            Status = attempt.Status,
            GatewayResponse = attempt.GatewayResponse,
            GatewayTransactionId = attempt.GatewayTransactionId,
            ErrorCode = attempt.ErrorCode,
            ErrorMessage = attempt.ErrorMessage,
            RetryCount = attempt.RetryCount,
            NextRetryAt = attempt.NextRetryAt,
            RequiresAuthentication = attempt.RequiresAuthentication,
            AuthenticationUrl = attempt.AuthenticationUrl,
            AuthenticationStatus = attempt.AuthenticationStatus,
            IpAddress = attempt.IpAddress,
            CreatedAt = attempt.CreatedAt,
            UpdatedAt = attempt.UpdatedAt
        };
    }
}
