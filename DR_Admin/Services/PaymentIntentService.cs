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
using System.Text.Json;
using System.Linq;

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
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == resolvedCustomerId);

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
                CustomerEmail = customer?.Email ?? string.Empty,
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
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                entity.CapturedAt ??= DateTime.UtcNow;
                await TryFinalizeCapturedPaymentAsync(entity, gateway, status.TransactionId, paymentMethodToken);
                await EnsureRecurringSubscriptionForCapturedOrderAsync(entity);
                await EnsureRecurringSubscriptionsFromIntentDescriptionAsync(entity);
                await EnsurePaidOrderArtifactsAsync(entity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
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

    private async Task EnsureRecurringSubscriptionForCapturedOrderAsync(Data.Entities.PaymentIntent intent)
    {
        if (!intent.OrderId.HasValue || intent.OrderId.Value <= 0)
        {
            return;
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == intent.OrderId.Value);

        if (order == null)
        {
            return;
        }

        await EnsureRecurringSubscriptionForOrderAsync(intent, order);
    }

    private async Task EnsureRecurringSubscriptionsFromIntentDescriptionAsync(Data.Entities.PaymentIntent intent)
    {
        var orderNumbers = GetCheckoutOrderNumbersFromDescription(intent.Description);
        if (orderNumbers.Count == 0)
        {
            return;
        }

        foreach (var orderNumber in orderNumbers)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.CustomerId == intent.CustomerId);

            if (order == null)
            {
                continue;
            }

            if (intent.OrderId.HasValue && order.Id == intent.OrderId.Value)
            {
                continue;
            }

            await EnsureRecurringSubscriptionForOrderAsync(intent, order);
        }
    }

    private async Task EnsurePaidOrderArtifactsAsync(Data.Entities.PaymentIntent intent)
    {
        var orders = await ResolveOrdersFromPaymentIntentAsync(intent);
        if (orders.Count == 0)
        {
            return;
        }

        foreach (var order in orders)
        {
            await EnsureOrderActivatedAndProvisionedArtifactsAsync(order);
        }
    }

    private async Task<List<Order>> ResolveOrdersFromPaymentIntentAsync(Data.Entities.PaymentIntent intent)
    {
        var orderIds = new HashSet<int>();
        if (intent.OrderId.HasValue && intent.OrderId.Value > 0)
        {
            orderIds.Add(intent.OrderId.Value);
        }

        var orderNumbers = GetCheckoutOrderNumbersFromDescription(intent.Description);

        IQueryable<Order> query = _context.Orders
            .Include(o => o.OrderLines)
            .Where(o => o.CustomerId == intent.CustomerId);

        if (orderIds.Count == 0 && orderNumbers.Count == 0)
        {
            return new List<Order>();
        }

        query = query.Where(o => orderIds.Contains(o.Id) || orderNumbers.Contains(o.OrderNumber));

        return await query.ToListAsync();
    }

    private async Task EnsureOrderActivatedAndProvisionedArtifactsAsync(Order order)
    {
        if (order.Status != OrderStatus.Cancelled)
        {
            order.Status = OrderStatus.Active;
            order.UpdatedAt = DateTime.UtcNow;
        }

        var hasPrivacyProtection = order.OrderLines.Any(line =>
            line.Description.StartsWith("WHOIS Privacy", StringComparison.OrdinalIgnoreCase));

        foreach (var orderLine in order.OrderLines)
        {
            if (orderLine.Description.StartsWith("Domain:", StringComparison.OrdinalIgnoreCase))
            {
                await EnsureRegisteredDomainForPaidOrderLineAsync(order, orderLine, hasPrivacyProtection);
                continue;
            }

            if (orderLine.Description.StartsWith("Hosting:", StringComparison.OrdinalIgnoreCase))
            {
                await EnsureHostingAccountForPaidOrderLineAsync(order, orderLine);
            }
        }
    }

    private async Task EnsureRegisteredDomainForPaidOrderLineAsync(Order order, OrderLine orderLine, bool hasPrivacyProtection)
    {
        var domainName = ExtractDomainName(orderLine.Description);
        if (string.IsNullOrWhiteSpace(domainName))
        {
            return;
        }

        var normalizedDomainName = domainName.Trim().ToLowerInvariant();
        var exists = await _context.RegisteredDomains
            .AnyAsync(d => d.CustomerId == order.CustomerId && d.NormalizedName == normalizedDomainName);

        if (exists)
        {
            return;
        }

        var registrarId = await ResolveRegistrarIdForOrderLineAsync(orderLine);
        if (registrarId <= 0)
        {
            _log.Warning("No active registrar found while creating paid domain {DomainName} for order {OrderId}", domainName, order.Id);
            return;
        }

        var calculatedPrice = orderLine.TotalPrice > 0
            ? orderLine.TotalPrice
            : Math.Max(0, orderLine.UnitPrice * Math.Max(1, orderLine.Quantity));

        var domain = new RegisteredDomain
        {
            CustomerId = order.CustomerId,
            ServiceId = orderLine.ServiceId ?? order.ServiceId,
            Name = domainName,
            NormalizedName = normalizedDomainName,
            RegistrarId = registrarId,
            Status = DomainStatus.PendingRegistration.ToString(),
            RegistrationStatus = DomainRegistrationStatus.PaidPendingRegistration,
            RegistrationDate = null,
            RegistrationAttemptCount = 0,
            LastRegistrationAttemptUtc = null,
            NextRegistrationAttemptUtc = DateTime.UtcNow,
            RegistrationError = null,
            AutoRenew = order.AutoRenew || orderLine.IsRecurring,
            PrivacyProtection = hasPrivacyProtection,
            RegistrationPrice = calculatedPrice,
            RenewalPrice = orderLine.IsRecurring ? calculatedPrice : null,
            Notes = $"Auto-created from paid checkout order {order.OrderNumber}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.RegisteredDomains.Add(domain);
    }

    private async Task EnsureHostingAccountForPaidOrderLineAsync(Order order, OrderLine orderLine)
    {
        var serviceId = await ResolveHostingServiceIdAsync(order, orderLine);
        if (!serviceId.HasValue || serviceId.Value <= 0)
        {
            _log.Warning("Could not resolve hosting service for paid order line {OrderLineId} in order {OrderId}", orderLine.Id, order.Id);
            return;
        }

        var externalAccountId = $"checkout-order-{order.Id}-line-{orderLine.Id}";
        var exists = await _context.HostingAccounts
            .AnyAsync(a => a.ExternalAccountId == externalAccountId);

        if (exists)
        {
            return;
        }

        var hostingAccount = new HostingAccount
        {
            CustomerId = order.CustomerId,
            ServiceId = serviceId.Value,
            ServerId = null,
            ServerControlPanelId = null,
            Provider = "checkout",
            Username = BuildHostingUsername(order, orderLine),
            PasswordHash = string.Empty,
            Status = "Active",
            ExpirationDate = order.EndDate > DateTime.UtcNow ? order.EndDate : DateTime.UtcNow.AddYears(1),
            ExternalAccountId = externalAccountId,
            SyncStatus = "NotSynced",
            PlanName = ExtractHostingPlanName(orderLine.Description),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.HostingAccounts.Add(hostingAccount);
    }

    private async Task<int> ResolveRegistrarIdForOrderLineAsync(OrderLine orderLine)
    {
        var registrarCode = TryGetOrderLineMetadataValue(orderLine.Notes, "registrarCode");
        if (!string.IsNullOrWhiteSpace(registrarCode))
        {
            var byCode = await _context.Registrars
                .AsNoTracking()
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync(r => r.Code == registrarCode);

            if (byCode != null)
            {
                return byCode.Id;
            }
        }

        var defaultRegistrar = await _context.Registrars
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.IsDefault)
            .ThenBy(r => r.Id)
            .FirstOrDefaultAsync();

        return defaultRegistrar?.Id ?? 0;
    }

    private async Task<int?> ResolveHostingServiceIdAsync(Order order, OrderLine orderLine)
    {
        if (orderLine.ServiceId.HasValue && orderLine.ServiceId.Value > 0)
        {
            return orderLine.ServiceId.Value;
        }

        if (order.ServiceId.HasValue && order.ServiceId.Value > 0)
        {
            return order.ServiceId.Value;
        }

        var planName = ExtractHostingPlanName(orderLine.Description);
        if (string.IsNullOrWhiteSpace(planName))
        {
            return null;
        }

        return await _context.Services
            .AsNoTracking()
            .Where(s => s.Name == planName)
            .OrderByDescending(s => s.IsActive)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync();
    }

    private static string BuildHostingUsername(Order order, OrderLine orderLine)
    {
        var seed = $"cust{order.CustomerId}ord{order.Id}ln{orderLine.LineNumber}".ToLowerInvariant();
        return seed.Length <= 32 ? seed : seed[..32];
    }

    private static string ExtractHostingPlanName(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || !description.StartsWith("Hosting:", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        var value = description["Hosting:".Length..].Trim();
        var bracketIndex = value.IndexOf('(');
        if (bracketIndex > 0)
        {
            value = value[..bracketIndex].Trim();
        }

        return value;
    }

    private static string ExtractDomainName(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || !description.StartsWith("Domain:", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        var value = description["Domain:".Length..].Trim();

        if (value.EndsWith(" recurring", StringComparison.OrdinalIgnoreCase))
        {
            value = value[..^" recurring".Length].Trim();
        }

        var bracketIndex = value.IndexOf('(');
        if (bracketIndex > 0)
        {
            value = value[..bracketIndex].Trim();
        }

        return value;
    }

    private static string? TryGetOrderLineMetadataValue(string notes, string key)
    {
        if (string.IsNullOrWhiteSpace(notes) || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        try
        {
            using var json = JsonDocument.Parse(notes);
            if (json.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            return json.RootElement.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static List<string> GetCheckoutOrderNumbersFromDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new List<string>();
        }

        var marker = "Checkout payment for order";
        var markerIndex = description.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return new List<string>();
        }

        var tail = description[(markerIndex + marker.Length)..];
        return tail
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task EnsureRecurringSubscriptionForOrderAsync(Data.Entities.PaymentIntent intent, Order order)
    {
        if (order.RecurringAmount <= 0 || !order.AutoRenew)
        {
            return;
        }

        var gateway = await _context.PaymentGateways
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == intent.PaymentGatewayId && g.IsActive && g.DeletedAt == null);

        order.Status = OrderStatus.Active;
        order.UpdatedAt = DateTime.UtcNow;

        var billingCycleId = await ResolveBillingCycleIdForOrderAsync(order);
        var (billingPeriodCount, billingPeriodUnit) = ResolveBillingPeriod(order.StartDate, order.NextBillingDate);

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s =>
                s.CustomerId == order.CustomerId &&
                s.ServiceId == order.ServiceId &&
                s.BillingCycleId == billingCycleId &&
                s.StartDate == order.StartDate &&
                s.NextBillingDate == order.NextBillingDate &&
                s.Amount == order.RecurringAmount &&
                s.Status != SubscriptionStatus.Cancelled);

        if (existingSubscription != null)
        {
            await EnsureStripeSubscriptionMirrorAsync(existingSubscription, order, gateway, intent.GatewayIntentId);
            return;
        }

        var defaultPaymentMethodId = await _context.CustomerPaymentMethods
            .AsNoTracking()
            .Where(m => m.CustomerId == order.CustomerId && m.IsActive && m.IsDefault && m.DeletedAt == null)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync();

        var subscription = new Subscription
        {
            CustomerId = order.CustomerId,
            ServiceId = order.ServiceId,
            BillingCycleId = billingCycleId,
            CustomerPaymentMethodId = defaultPaymentMethodId,
            Status = SubscriptionStatus.Active,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            NextBillingDate = order.NextBillingDate,
            CurrentPeriodStart = order.StartDate,
            CurrentPeriodEnd = order.NextBillingDate,
            Amount = order.RecurringAmount,
            CurrencyCode = string.IsNullOrWhiteSpace(order.CurrencyCode) ? intent.Currency : order.CurrencyCode,
            BillingPeriodCount = billingPeriodCount,
            BillingPeriodUnit = billingPeriodUnit,
            TrialEndDate = null,
            IsInTrial = false,
            RetryCount = 0,
            MaxRetryAttempts = 3,
            Metadata = string.Empty,
            Notes = $"Auto-created from paid order {order.OrderNumber}",
            Quantity = 1,
            SendEmailNotifications = true,
            AutoRetryFailedPayments = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Subscriptions.Add(subscription);
        await EnsureStripeSubscriptionMirrorAsync(subscription, order, gateway, intent.GatewayIntentId);
    }

    private async Task EnsureStripeSubscriptionMirrorAsync(Subscription subscription, Order order, Data.Entities.PaymentGateway? gateway, string gatewayIntentId)
    {
        if (gateway == null || !string.Equals(gateway.ProviderCode, "stripe", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(subscription.Metadata) && subscription.Metadata.Contains("stripeSubscriptionId", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

        if (customer == null)
        {
            return;
        }

        var stripeGateway = CreateGatewayClient(gateway) as StripePaymentGateway;
        if (stripeGateway == null)
        {
            return;
        }

        var stripeRequest = new StripeSubscriptionCreateRequest
        {
            CustomerEmail = customer.Email,
            CustomerName = string.IsNullOrWhiteSpace(customer.CustomerName) ? customer.Name : customer.CustomerName,
            ProductName = string.IsNullOrWhiteSpace(order.OrderNumber)
                ? "Recurring subscription"
                : $"Recurring subscription {order.OrderNumber}",
            Amount = subscription.Amount,
            Currency = string.IsNullOrWhiteSpace(subscription.CurrencyCode) ? "EUR" : subscription.CurrencyCode,
            Interval = MapStripeInterval(subscription.BillingPeriodUnit),
            IntervalCount = Math.Max(1, subscription.BillingPeriodCount),
            TrialEndUtc = null,
            Metadata = new Dictionary<string, string>
            {
                ["order_id"] = order.Id.ToString(),
                ["order_number"] = order.OrderNumber,
                ["subscription_id"] = subscription.Id.ToString()
            }
        };

        var ensuredPaymentMethod = await stripeGateway.EnsureCustomerPaymentMethodForOffSessionAsync(gatewayIntentId);
        if (!ensuredPaymentMethod)
        {
            _log.Warning("Unable to set Stripe default payment method from payment intent for order {OrderId}", order.Id);
        }

        var stripeResult = await stripeGateway.CreateRecurringSubscriptionAsync(stripeRequest);
        if (!stripeResult.Success)
        {
            _log.Warning("Failed to mirror recurring subscription {SubscriptionId} to Stripe for order {OrderId}: {Error}",
                subscription.Id, order.Id, stripeResult.ErrorMessage);
            return;
        }

        subscription.Metadata = JsonSerializer.Serialize(new
        {
            stripeSubscriptionId = stripeResult.SubscriptionId,
            stripeCustomerId = stripeResult.CustomerId,
            stripeStatus = stripeResult.Status
        });
        subscription.UpdatedAt = DateTime.UtcNow;
    }

    private static string MapStripeInterval(SubscriptionPeriodUnit unit)
    {
        return unit switch
        {
            SubscriptionPeriodUnit.Years => "year",
            SubscriptionPeriodUnit.Days => "day",
            _ => "month"
        };
    }

    private async Task<int> ResolveBillingCycleIdForOrderAsync(Order order)
    {
        var durationDays = Math.Max(1, (int)Math.Round((order.NextBillingDate - order.StartDate).TotalDays));

        var exactMatch = await _context.BillingCycles
            .AsNoTracking()
            .Where(c => c.DurationInDays == durationDays)
            .OrderBy(c => c.SortOrder)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (exactMatch > 0)
        {
            return exactMatch;
        }

        var closestMatch = await _context.BillingCycles
            .AsNoTracking()
            .OrderBy(c => Math.Abs(c.DurationInDays - durationDays))
            .ThenBy(c => c.SortOrder)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (closestMatch > 0)
        {
            return closestMatch;
        }

        var fallbackCycle = new BillingCycle
        {
            Code = $"AUTO-{durationDays}D",
            Name = $"{durationDays} Days",
            DurationInDays = durationDays,
            Description = "Auto-generated billing cycle from paid recurring order",
            SortOrder = 999,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BillingCycles.Add(fallbackCycle);
        await _context.SaveChangesAsync();
        return fallbackCycle.Id;
    }

    private static (int BillingPeriodCount, SubscriptionPeriodUnit BillingPeriodUnit) ResolveBillingPeriod(DateTime startDate, DateTime nextBillingDate)
    {
        var days = Math.Max(1, (int)Math.Round((nextBillingDate - startDate).TotalDays));

        if (days % 365 == 0)
        {
            return (Math.Max(1, days / 365), SubscriptionPeriodUnit.Years);
        }

        if (days % 30 == 0)
        {
            return (Math.Max(1, days / 30), SubscriptionPeriodUnit.Months);
        }

        return (days, SubscriptionPeriodUnit.Days);
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

