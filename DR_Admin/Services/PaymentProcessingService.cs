using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Implementations;
using PaymentGatewayLib.Interfaces;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ISPAdmin.Services;

/// <summary>
/// Service for payment processing operations
/// </summary>
public class PaymentProcessingService : IPaymentProcessingService
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentProcessingService>();

    public PaymentProcessingService(
        ApplicationDbContext context,
        StripeSettings stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings;
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
                .Include(pm => pm.PaymentGateway)
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

            var gateway = paymentMethod.PaymentGateway;

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

            var gatewayClient = CreateGatewayClient(gateway);
            var gatewayResult = await gatewayClient.ProcessPaymentAsync(new PaymentGatewayLib.Models.PaymentRequest
            {
                Amount = invoice.TotalAmount,
                Currency = invoice.CurrencyCode,
                PaymentMethodToken = token.EncryptedToken,
                PaymentInstrument = gateway.PaymentInstrument,
                Description = $"Invoice {invoice.InvoiceNumber}",
                CustomerEmail = invoice.Customer?.Email ?? string.Empty,
                ReferenceId = $"INV-{invoice.Id}",
                CaptureImmediately = true
            });

            attempt.GatewayResponse = gatewayResult.RawResponse;
            attempt.GatewayTransactionId = gatewayResult.TransactionId;

            if (gatewayResult.Success)
            {
                // Create payment transaction
                var transaction = new PaymentTransaction
                {
                    InvoiceId = invoice.Id,
                    PaymentGatewayId = gateway.Id,
                    Amount = invoice.TotalAmount,
                    CurrencyCode = invoice.CurrencyCode,
                    TransactionId = gatewayResult.TransactionId,
                    PaymentMethod = paymentMethod.Type.ToString(),
                    Status = PaymentTransactionStatus.Completed,
                    ProcessedAt = DateTime.UtcNow,
                    GatewayResponse = gatewayResult.RawResponse,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PaymentTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                // Update payment attempt
                attempt.Status = PaymentAttemptStatus.Succeeded;
                attempt.PaymentTransactionId = transaction.Id;
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
                attempt.ErrorCode = gatewayResult.ErrorCode;
                attempt.ErrorMessage = gatewayResult.ErrorMessage;
                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = false,
                    PaymentAttemptId = attempt.Id,
                    ErrorCode = gatewayResult.ErrorCode,
                    ErrorMessage = string.IsNullOrWhiteSpace(gatewayResult.ErrorMessage)
                        ? "Payment was declined"
                        : gatewayResult.ErrorMessage
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

            if (!string.Equals(gatewayName, "stripe", StringComparison.OrdinalIgnoreCase))
            {
                _log.Warning("Unsupported webhook gateway: {GatewayName}", gatewayName);
                return false;
            }

            if (!IsValidStripeSignature(payload, signature))
            {
                _log.Warning("Invalid Stripe webhook signature");
                return false;
            }

            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;

            if (!root.TryGetProperty("type", out var typeElement)
                || !root.TryGetProperty("data", out var dataElement)
                || !dataElement.TryGetProperty("object", out var objectElement))
            {
                _log.Warning("Invalid Stripe webhook payload format");
                return false;
            }

            var eventType = typeElement.GetString() ?? string.Empty;

            switch (eventType)
            {
                case "invoice.paid":
                case "invoice.payment_succeeded":
                    await HandleStripeInvoicePaidAsync(objectElement);
                    break;

                case "invoice.payment_failed":
                    await HandleStripeInvoicePaymentFailedAsync(objectElement);
                    break;

                case "customer.subscription.updated":
                    await HandleStripeSubscriptionUpdatedAsync(objectElement);
                    break;

                case "customer.subscription.deleted":
                    await HandleStripeSubscriptionDeletedAsync(objectElement);
                    break;

                default:
                    _log.Information("Ignoring unsupported Stripe event type: {EventType}", eventType);
                    break;
            }

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

    private bool IsValidStripeSignature(string payload, string signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_stripeSettings.WebhookSecret))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return false;
        }

        string? timestamp = null;
        string? v1 = null;

        foreach (var part in signatureHeader.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.StartsWith("t=", StringComparison.OrdinalIgnoreCase))
            {
                timestamp = part[2..];
            }
            else if (part.StartsWith("v1=", StringComparison.OrdinalIgnoreCase))
            {
                v1 = part[3..];
            }
        }

        if (string.IsNullOrWhiteSpace(timestamp) || string.IsNullOrWhiteSpace(v1))
        {
            return false;
        }

        var signedPayload = $"{timestamp}.{payload}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_stripeSettings.WebhookSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));
        var computedSignature = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(v1));
    }

    private async Task HandleStripeInvoicePaidAsync(JsonElement stripeInvoice)
    {
        var invoice = await ResolveInvoiceFromStripeObjectAsync(stripeInvoice);
        if (invoice == null)
        {
            _log.Warning("Stripe invoice.paid received but local invoice could not be resolved");
            return;
        }

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt ??= DateTime.UtcNow;
        invoice.AmountPaid = invoice.TotalAmount;
        invoice.AmountDue = 0;
        invoice.UpdatedAt = DateTime.UtcNow;

        var chargeId = GetStringProperty(stripeInvoice, "charge");
        var paymentIntentId = GetStringProperty(stripeInvoice, "payment_intent");
        var transactionId = !string.IsNullOrWhiteSpace(chargeId) ? chargeId : paymentIntentId;

        if (!string.IsNullOrWhiteSpace(transactionId))
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction != null)
            {
                transaction.Status = PaymentTransactionStatus.Completed;
                transaction.ProcessedAt ??= DateTime.UtcNow;
                transaction.UpdatedAt = DateTime.UtcNow;

                var hasInvoicePayment = await _context.InvoicePayments
                    .AnyAsync(ip => ip.InvoiceId == invoice.Id && ip.PaymentTransactionId == transaction.Id);

                if (!hasInvoicePayment)
                {
                    _context.InvoicePayments.Add(new InvoicePayment
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
                    });
                }
            }

            var attempt = await _context.PaymentAttempts
                .FirstOrDefaultAsync(a => a.GatewayTransactionId == transactionId);

            if (attempt != null)
            {
                attempt.Status = PaymentAttemptStatus.Succeeded;
                attempt.UpdatedAt = DateTime.UtcNow;
            }
        }

        var subscription = await ResolveSubscriptionFromStripeObjectAsync(stripeInvoice, invoice);
        if (subscription != null)
        {
            subscription.Status = SubscriptionStatus.Active;
            subscription.RetryCount = 0;
            subscription.LastSuccessfulBilling = DateTime.UtcNow;
            subscription.LastBillingAttempt = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private async Task HandleStripeInvoicePaymentFailedAsync(JsonElement stripeInvoice)
    {
        var invoice = await ResolveInvoiceFromStripeObjectAsync(stripeInvoice);
        if (invoice != null)
        {
            invoice.Status = DateTime.UtcNow > invoice.DueDate ? InvoiceStatus.Overdue : InvoiceStatus.Issued;
            invoice.UpdatedAt = DateTime.UtcNow;
        }

        var chargeId = GetStringProperty(stripeInvoice, "charge");
        var paymentIntentId = GetStringProperty(stripeInvoice, "payment_intent");
        var transactionId = !string.IsNullOrWhiteSpace(chargeId) ? chargeId : paymentIntentId;

        if (!string.IsNullOrWhiteSpace(transactionId))
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction != null)
            {
                transaction.Status = PaymentTransactionStatus.Failed;
                transaction.UpdatedAt = DateTime.UtcNow;
            }

            var attempt = await _context.PaymentAttempts
                .FirstOrDefaultAsync(a => a.GatewayTransactionId == transactionId);

            if (attempt != null)
            {
                attempt.Status = PaymentAttemptStatus.Failed;
                attempt.UpdatedAt = DateTime.UtcNow;
            }
        }

        var subscription = await ResolveSubscriptionFromStripeObjectAsync(stripeInvoice, invoice);
        if (subscription != null)
        {
            subscription.Status = SubscriptionStatus.PastDue;
            subscription.RetryCount += 1;
            subscription.LastBillingAttempt = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private async Task HandleStripeSubscriptionUpdatedAsync(JsonElement stripeSubscription)
    {
        var subscription = await ResolveSubscriptionFromStripeObjectAsync(stripeSubscription, null);
        if (subscription == null)
        {
            _log.Warning("Stripe customer.subscription.updated received but local subscription could not be resolved");
            return;
        }

        var stripeStatus = GetStringProperty(stripeSubscription, "status")?.ToLowerInvariant();
        subscription.Status = stripeStatus switch
        {
            "active" => SubscriptionStatus.Active,
            "trialing" => SubscriptionStatus.Trialing,
            "past_due" => SubscriptionStatus.PastDue,
            "canceled" => SubscriptionStatus.Cancelled,
            "incomplete" => SubscriptionStatus.Incomplete,
            _ => subscription.Status
        };

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            subscription.CancelledAt ??= DateTime.UtcNow;
        }

        subscription.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private async Task HandleStripeSubscriptionDeletedAsync(JsonElement stripeSubscription)
    {
        var subscription = await ResolveSubscriptionFromStripeObjectAsync(stripeSubscription, null);
        if (subscription == null)
        {
            _log.Warning("Stripe customer.subscription.deleted received but local subscription could not be resolved");
            return;
        }

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.CancelledAt ??= DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task<Invoice?> ResolveInvoiceFromStripeObjectAsync(JsonElement stripeObject)
    {
        var localInvoiceId = TryGetMetadataInt(stripeObject, "localInvoiceId")
            ?? TryGetMetadataInt(stripeObject, "invoiceId");

        if (localInvoiceId.HasValue)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == localInvoiceId.Value);
        }

        var chargeId = GetStringProperty(stripeObject, "charge");
        var paymentIntentId = GetStringProperty(stripeObject, "payment_intent");

        if (!string.IsNullOrWhiteSpace(chargeId) || !string.IsNullOrWhiteSpace(paymentIntentId))
        {
            var transactionId = !string.IsNullOrWhiteSpace(chargeId) ? chargeId : paymentIntentId;

            var invoiceIdFromTransaction = await _context.PaymentTransactions
                .Where(t => t.TransactionId == transactionId)
                .Select(t => (int?)t.InvoiceId)
                .FirstOrDefaultAsync();

            if (invoiceIdFromTransaction.HasValue)
            {
                return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceIdFromTransaction.Value);
            }

            var invoiceIdFromAttempt = await _context.PaymentAttempts
                .Where(a => a.GatewayTransactionId == transactionId)
                .Select(a => (int?)a.InvoiceId)
                .FirstOrDefaultAsync();

            if (invoiceIdFromAttempt.HasValue)
            {
                return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceIdFromAttempt.Value);
            }
        }

        return null;
    }

    private async Task<Subscription?> ResolveSubscriptionFromStripeObjectAsync(JsonElement stripeObject, Invoice? invoice)
    {
        var localSubscriptionId = TryGetMetadataInt(stripeObject, "localSubscriptionId")
            ?? TryGetMetadataInt(stripeObject, "subscriptionId");

        if (localSubscriptionId.HasValue)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == localSubscriptionId.Value);
        }

        if (invoice != null)
        {
            var id = ExtractSubscriptionIdFromInvoiceComment(invoice.InternalComment);
            if (id.HasValue)
            {
                return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id.Value);
            }
        }

        var stripeSubscriptionId = GetStringProperty(stripeObject, "id");
        if (!string.IsNullOrWhiteSpace(stripeSubscriptionId) && stripeSubscriptionId.StartsWith("sub_", StringComparison.OrdinalIgnoreCase))
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Metadata.Contains(stripeSubscriptionId));
        }

        var nestedSubscriptionId = GetStringProperty(stripeObject, "subscription");
        if (!string.IsNullOrWhiteSpace(nestedSubscriptionId))
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Metadata.Contains(nestedSubscriptionId));
        }

        return null;
    }

    private static int? TryGetMetadataInt(JsonElement element, string key)
    {
        if (!element.TryGetProperty("metadata", out var metadata) || metadata.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (!metadata.TryGetProperty(key, out var valueElement))
        {
            return null;
        }

        var value = valueElement.GetString();
        return int.TryParse(value, out var result) ? result : null;
    }

    private static string? GetStringProperty(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var propertyValue))
        {
            return null;
        }

        return propertyValue.ValueKind switch
        {
            JsonValueKind.String => propertyValue.GetString(),
            JsonValueKind.Number => propertyValue.GetRawText(),
            JsonValueKind.Null => null,
            _ => propertyValue.GetRawText()
        };
    }

    private static int? ExtractSubscriptionIdFromInvoiceComment(string comment)
    {
        const string marker = "Auto-generated from subscription ID:";
        if (string.IsNullOrWhiteSpace(comment))
        {
            return null;
        }

        var markerIndex = comment.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return null;
        }

        var value = comment[(markerIndex + marker.Length)..].Trim();
        return int.TryParse(value, out var subscriptionId) ? subscriptionId : null;
    }

    private static IPaymentGateway CreateGatewayClient(PaymentGateway gateway)
    {
        var provider = (gateway.ProviderCode ?? string.Empty).Trim().ToLowerInvariant();

        return provider switch
        {
            "stripe" => new StripePaymentGateway(gateway.ApiSecret, gateway.ApiKey),
            "paypal" => new PayPalPaymentGateway(gateway.ApiKey, gateway.ApiSecret, gateway.UseSandbox),
            "square" => new SquarePaymentGateway(gateway.ApiKey, gateway.ApiSecret, gateway.UseSandbox),
            _ => throw new NotSupportedException($"Payment provider '{gateway.ProviderCode}' is not currently supported by PaymentProcessingService")
        };
    }
}
