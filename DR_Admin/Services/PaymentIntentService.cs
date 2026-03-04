using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayLib.Implementations;
using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing payment intents
/// </summary>
public class PaymentIntentService : IPaymentIntentService
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentIntentService>();

    public PaymentIntentService(ApplicationDbContext context, StripeSettings stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings;
    }

    public async Task<IEnumerable<PaymentIntentDto>> GetAllPaymentIntentsAsync()
    {
        var intents = await _context.PaymentIntents
            .AsNoTracking()
            .Where(i => i.DeletedAt == null)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return intents.Select(intent => MapToDto(intent));
    }

    public async Task<PaymentIntentDto?> GetPaymentIntentByIdAsync(int id)
    {
        var entity = await _context.PaymentIntents
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<PaymentIntentDto>> GetPaymentIntentsByCustomerIdAsync(int customerId)
    {
        var intents = await _context.PaymentIntents
            .AsNoTracking()
            .Where(i => i.CustomerId == customerId && i.DeletedAt == null)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return intents.Select(intent => MapToDto(intent));
    }

    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(CreatePaymentIntentDto createDto, int customerId)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        var resolvedCustomerId = customerId > 0 ? customerId : await ResolveCustomerIdFromRequestAsync(createDto);
        if (resolvedCustomerId <= 0)
        {
            throw new InvalidOperationException("Customer could not be resolved for payment intent creation.");
        }

        var gateway = await ResolveGatewayAsync(createDto, resolvedCustomerId);

        var entity = new Data.Entities.PaymentIntent
        {
            InvoiceId = createDto.InvoiceId,
            OrderId = createDto.OrderId,
            CustomerId = resolvedCustomerId,
            Amount = createDto.Amount,
            Currency = createDto.Currency,
            Status = PaymentIntentStatus.Created,
            PaymentGatewayId = gateway.Id,
            ReturnUrl = createDto.ReturnUrl,
            CancelUrl = createDto.CancelUrl,
            Description = createDto.Description,
            MetadataJson = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PaymentIntents.Add(entity);
        await _context.SaveChangesAsync();

        try
        {
            var gatewayClient = CreateGatewayClient(gateway);
            var preferredMethodType = MapPreferredMethodType(createDto.PaymentInstrument, gateway.ProviderCode);

            var result = await gatewayClient.CreatePaymentIntentAsync(new PaymentGatewayLib.Models.PaymentIntentRequest
            {
                Amount = createDto.Amount,
                Currency = createDto.Currency,
                CustomerId = resolvedCustomerId.ToString(),
                PreferredPaymentMethodType = preferredMethodType,
                PaymentMethodTypes = string.IsNullOrWhiteSpace(preferredMethodType)
                    ? new List<string>()
                    : new List<string> { preferredMethodType },
                Description = string.IsNullOrWhiteSpace(createDto.Description)
                    ? $"Payment intent {entity.Id}"
                    : createDto.Description,
                ReturnUrl = createDto.ReturnUrl,
                CancelUrl = createDto.CancelUrl,
                AutomaticCapture = true
            });

            if (!result.Success)
            {
                entity.Status = PaymentIntentStatus.Failed;
                entity.FailedAt = DateTime.UtcNow;
                entity.FailureReason = result.ErrorMessage;
                entity.GatewayResponse = result.ErrorCode;
                await _context.SaveChangesAsync();

                throw new InvalidOperationException($"Gateway failed to create payment intent: {result.ErrorMessage}");
            }

            entity.GatewayIntentId = result.IntentId;
            entity.ClientSecret = result.ClientSecret;
            entity.Status = MapGatewayIntentStatus(result.Status);
            entity.GatewayResponse = result.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(entity, gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating payment intent for customer {CustomerId}", resolvedCustomerId);
            throw;
        }
    }

    public async Task<bool> ConfirmPaymentIntentAsync(int id, string paymentMethodToken)
    {
        var entity = await _context.PaymentIntents
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (entity == null)
        {
            return false;
        }

        if (entity.Status is PaymentIntentStatus.Cancelled or PaymentIntentStatus.Failed)
        {
            return false;
        }

        var gateway = await _context.PaymentGateways
            .FirstOrDefaultAsync(g => g.Id == entity.PaymentGatewayId && g.IsActive && g.DeletedAt == null);

        if (gateway == null)
        {
            entity.Status = PaymentIntentStatus.Failed;
            entity.FailedAt = DateTime.UtcNow;
            entity.FailureReason = "Payment gateway not found or inactive.";
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return false;
        }

        var gatewayClient = CreateGatewayClient(gateway);
        var status = await gatewayClient.GetTransactionStatusAsync(entity.GatewayIntentId);

        var mappedStatus = MapGatewayIntentStatus(status.Status);
        entity.Status = mappedStatus;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.GatewayResponse = string.IsNullOrWhiteSpace(status.Status)
            ? entity.GatewayResponse
            : status.Status;

        if (mappedStatus == PaymentIntentStatus.Captured)
        {
            entity.CapturedAt ??= DateTime.UtcNow;
            await TryFinalizeCapturedPaymentAsync(entity, gateway, status.TransactionId, paymentMethodToken);
        }
        else if (mappedStatus == PaymentIntentStatus.Authorized)
        {
            entity.AuthorizedAt ??= DateTime.UtcNow;
        }
        else if (mappedStatus == PaymentIntentStatus.Failed)
        {
            entity.FailedAt = DateTime.UtcNow;
            entity.FailureReason = string.IsNullOrWhiteSpace(status.Status) ? "Payment failed" : status.Status;
        }

        await _context.SaveChangesAsync();
        return mappedStatus is PaymentIntentStatus.Authorized or PaymentIntentStatus.Captured or PaymentIntentStatus.RequiresAction;
    }

    public async Task<bool> CancelPaymentIntentAsync(int id)
    {
        var entity = await _context.PaymentIntents
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (entity == null)
        {
            return false;
        }

        if (entity.Status == PaymentIntentStatus.Captured)
        {
            return false;
        }

        entity.Status = PaymentIntentStatus.Cancelled;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessWebhookAsync(int gatewayId, string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return false;
        }

        var intent = await _context.PaymentIntents
            .Where(i => i.PaymentGatewayId == gatewayId && i.DeletedAt == null)
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefaultAsync();

        if (intent == null)
        {
            return false;
        }

        var lowered = payload.ToLowerInvariant();
        intent.GatewayResponse = payload;
        intent.UpdatedAt = DateTime.UtcNow;

        if (lowered.Contains("succeeded") || lowered.Contains("completed") || lowered.Contains("captured"))
        {
            intent.Status = PaymentIntentStatus.Captured;
            intent.CapturedAt = DateTime.UtcNow;
        }
        else if (lowered.Contains("requires_action") || lowered.Contains("requiresaction"))
        {
            intent.Status = PaymentIntentStatus.RequiresAction;
        }
        else if (lowered.Contains("authorized"))
        {
            intent.Status = PaymentIntentStatus.Authorized;
            intent.AuthorizedAt ??= DateTime.UtcNow;
        }
        else if (lowered.Contains("cancel") || lowered.Contains("void"))
        {
            intent.Status = PaymentIntentStatus.Cancelled;
        }
        else if (lowered.Contains("fail") || lowered.Contains("declin") || lowered.Contains("error"))
        {
            intent.Status = PaymentIntentStatus.Failed;
            intent.FailedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<int> ResolveCustomerIdFromRequestAsync(CreatePaymentIntentDto createDto)
    {
        if (createDto.InvoiceId.HasValue)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == createDto.InvoiceId.Value);
            if (invoice != null)
            {
                return invoice.CustomerId;
            }
        }

        if (createDto.OrderId.HasValue)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == createDto.OrderId.Value);
            if (order != null)
            {
                return order.CustomerId;
            }
        }

        return 0;
    }

    private async Task<Data.Entities.PaymentGateway> ResolveGatewayAsync(CreatePaymentIntentDto createDto, int customerId)
    {
        if (!string.IsNullOrWhiteSpace(createDto.PaymentInstrument))
        {
            var normalized = NormalizeInstrumentKey(createDto.PaymentInstrument);
            var instruments = await _context.PaymentInstruments
                .AsNoTracking()
                .Where(i => i.DeletedAt == null && i.IsActive)
                .ToListAsync();

            var instrument = instruments.FirstOrDefault(i =>
                NormalizeInstrumentKey(i.Code) == normalized ||
                NormalizeInstrumentKey(i.Name) == normalized);

            if (instrument != null)
            {
                var mappedGateway = await _context.PaymentInstrumentGateways
                    .AsNoTracking()
                    .Where(m => m.PaymentInstrumentId == instrument.Id && m.IsActive && m.DeletedAt == null)
                    .Join(
                        _context.PaymentGateways.AsNoTracking().Where(g => g.IsActive && g.DeletedAt == null),
                        m => m.PaymentGatewayId,
                        g => g.Id,
                        (m, g) => new { Mapping = m, Gateway = g })
                    .OrderByDescending(x => x.Mapping.IsDefault)
                    .ThenBy(x => x.Mapping.Priority)
                    .ThenBy(x => x.Gateway.DisplayOrder)
                    .Select(x => x.Gateway)
                    .FirstOrDefaultAsync();

                if (mappedGateway != null)
                {
                    return mappedGateway;
                }
            }

            if (instrument?.DefaultGatewayId is int defaultGatewayId && defaultGatewayId > 0)
            {
                var byInstrumentDefault = await _context.PaymentGateways
                    .FirstOrDefaultAsync(g => g.Id == defaultGatewayId && g.IsActive && g.DeletedAt == null);

                if (byInstrumentDefault != null)
                {
                    return byInstrumentDefault;
                }
            }
        }

        if (createDto.PaymentGatewayId > 0)
        {
            var byId = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == createDto.PaymentGatewayId && g.IsActive && g.DeletedAt == null);
            if (byId != null)
            {
                return byId;
            }
        }

        var defaultMethod = await _context.CustomerPaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CustomerId == customerId && m.IsDefault && m.IsActive && m.DeletedAt == null);

        if (defaultMethod != null)
        {
            var byDefaultMethod = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == defaultMethod.PaymentGatewayId && g.IsActive && g.DeletedAt == null);

            if (byDefaultMethod != null)
            {
                return byDefaultMethod;
            }
        }

        var fallback = await _context.PaymentGateways
            .FirstOrDefaultAsync(g => g.IsActive && g.IsDefault && g.DeletedAt == null);

        if (fallback != null)
        {
            return fallback;
        }

        throw new InvalidOperationException("No active payment gateway available for payment intent creation.");
    }

    private IPaymentGateway CreateGatewayClient(Data.Entities.PaymentGateway gateway)
    {
        var provider = (gateway.ProviderCode ?? string.Empty).Trim().ToLowerInvariant();

        return provider switch
        {
            "stripe" => new StripePaymentGateway(
                string.IsNullOrWhiteSpace(_stripeSettings.SecretKey) ? gateway.ApiSecret : _stripeSettings.SecretKey,
                string.IsNullOrWhiteSpace(_stripeSettings.PublishableKey) ? gateway.ApiKey : _stripeSettings.PublishableKey,
                string.IsNullOrWhiteSpace(_stripeSettings.ApiBaseUrl) ? "https://api.stripe.com" : _stripeSettings.ApiBaseUrl),
            "paypal" => new PayPalPaymentGateway(gateway.ApiKey, gateway.ApiSecret, gateway.UseSandbox),
            "square" => new SquarePaymentGateway(gateway.ApiKey, gateway.ApiSecret, gateway.UseSandbox),
            _ => throw new NotSupportedException($"Payment provider '{gateway.ProviderCode}' is not currently supported by PaymentIntentService")
        };
    }

    private static PaymentIntentStatus MapGatewayIntentStatus(string status)
    {
        return (status ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "requires_action" or "requiresaction" => PaymentIntentStatus.RequiresAction,
            "requires_capture" or "requirescapture" => PaymentIntentStatus.Authorized,
            "processing" or "pending" or "approved" => PaymentIntentStatus.Authorized,
            "authorized" => PaymentIntentStatus.Authorized,
            "captured" or "succeeded" or "completed" => PaymentIntentStatus.Captured,
            "cancelled" or "canceled" => PaymentIntentStatus.Cancelled,
            "failed" => PaymentIntentStatus.Failed,
            _ => PaymentIntentStatus.Created
        };
    }

    private static string MapPreferredMethodType(string paymentInstrument, string providerCode)
    {
        if (string.IsNullOrWhiteSpace(paymentInstrument))
        {
            return string.Empty;
        }

        var normalizedInstrument = NormalizeInstrumentKey(paymentInstrument);
        var normalizedProvider = (providerCode ?? string.Empty).Trim().ToLowerInvariant();

        if (normalizedProvider == "stripe")
        {
            return normalizedInstrument switch
            {
                "creditcard" or "card" => "card",
                "bankaccount" => "us_bank_account",
                "paypal" => "paypal",
                _ => normalizedInstrument
            };
        }

        if (normalizedProvider == "paypal")
        {
            return normalizedInstrument switch
            {
                "paypal" => "paypal",
                "creditcard" or "card" => "card",
                _ => normalizedInstrument
            };
        }

        return normalizedInstrument;
    }

    private static string NormalizeInstrumentKey(string value)
    {
        return (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("_", string.Empty);
    }

    private async Task TryFinalizeCapturedPaymentAsync(Data.Entities.PaymentIntent intent, Data.Entities.PaymentGateway gateway, string gatewayTransactionId, string paymentMethodToken)
    {
        if (!intent.InvoiceId.HasValue || intent.InvoiceId.Value <= 0)
        {
            return;
        }

        var existingTransaction = await _context.PaymentTransactions
            .FirstOrDefaultAsync(t => t.PaymentIntentId == intent.Id);

        if (existingTransaction != null)
        {
            existingTransaction.Status = PaymentTransactionStatus.Completed;
            existingTransaction.ProcessedAt ??= DateTime.UtcNow;
            existingTransaction.UpdatedAt = DateTime.UtcNow;
            return;
        }

        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == intent.InvoiceId.Value);

        if (invoice == null)
        {
            return;
        }

        var transaction = new PaymentTransaction
        {
            InvoiceId = invoice.Id,
            PaymentIntentId = intent.Id,
            PaymentGatewayId = gateway.Id,
            PaymentMethod = string.IsNullOrWhiteSpace(paymentMethodToken) ? "PaymentIntent" : "Tokenized",
            Status = PaymentTransactionStatus.Completed,
            TransactionId = string.IsNullOrWhiteSpace(gatewayTransactionId) ? intent.GatewayIntentId : gatewayTransactionId,
            Amount = intent.Amount,
            CurrencyCode = intent.Currency,
            ProcessedAt = DateTime.UtcNow,
            GatewayResponse = intent.GatewayResponse,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PaymentTransactions.Add(transaction);

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var invoicePayment = new InvoicePayment
        {
            InvoiceId = invoice.Id,
            PaymentTransactionId = transaction.Id,
            AmountApplied = intent.Amount,
            Currency = intent.Currency,
            InvoiceBalance = Math.Max(0, invoice.TotalAmount - intent.Amount),
            InvoiceTotalAmount = invoice.TotalAmount,
            IsFullPayment = intent.Amount >= invoice.TotalAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.InvoicePayments.Add(invoicePayment);
    }

    private PaymentIntentDto MapToDto(Data.Entities.PaymentIntent entity, Data.Entities.PaymentGateway? gateway = null)
    {
        var providerCode = gateway?.ProviderCode ?? string.Empty;
        var publicKey = gateway?.ApiKey ?? string.Empty;

        if (providerCode.Equals("stripe", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(_stripeSettings.PublishableKey))
        {
            publicKey = _stripeSettings.PublishableKey;
        }

        return new PaymentIntentDto
        {
            Id = entity.Id,
            InvoiceId = entity.InvoiceId,
            OrderId = entity.OrderId,
            CustomerId = entity.CustomerId,
            Amount = entity.Amount,
            Currency = entity.Currency,
            Status = entity.Status,
            PaymentGatewayId = entity.PaymentGatewayId,
            PaymentGatewayProviderCode = providerCode,
            PaymentGatewayPublicKey = publicKey,
            GatewayIntentId = entity.GatewayIntentId,
            ClientSecret = entity.ClientSecret,
            Description = entity.Description,
            AuthorizedAt = entity.AuthorizedAt,
            CapturedAt = entity.CapturedAt,
            FailedAt = entity.FailedAt,
            FailureReason = entity.FailureReason,
            CreatedAt = entity.CreatedAt
        };
    }
}

