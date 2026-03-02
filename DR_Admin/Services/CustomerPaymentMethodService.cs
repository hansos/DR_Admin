using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer payment methods
/// </summary>
public class CustomerPaymentMethodService : ICustomerPaymentMethodService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerPaymentMethodService>();

    public CustomerPaymentMethodService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerPaymentMethodDto>> GetPaymentMethodsByCustomerIdAsync(int customerId)
    {
        var items = await _context.CustomerPaymentMethods
            .AsNoTracking()
            .Include(p => p.PaymentGateway)
            .Where(p => p.CustomerId == customerId && p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return items;
    }

    public async Task<CustomerPaymentMethodDto?> GetPaymentMethodByIdAsync(int id)
    {
        var entity = await _context.CustomerPaymentMethods
            .AsNoTracking()
            .Include(p => p.PaymentGateway)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<CustomerPaymentMethodDto?> GetDefaultPaymentMethodAsync(int customerId)
    {
        var entity = await _context.CustomerPaymentMethods
            .AsNoTracking()
            .Include(p => p.PaymentGateway)
            .FirstOrDefaultAsync(p => p.CustomerId == customerId && p.IsDefault && p.DeletedAt == null);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<CustomerPaymentMethodDto> CreatePaymentMethodAsync(CreateCustomerPaymentMethodDto createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        var customerExists = await _context.Customers
            .AsNoTracking()
            .AnyAsync(c => c.Id == createDto.CustomerId);
        if (!customerExists)
        {
            throw new InvalidOperationException($"Customer with ID {createDto.CustomerId} was not found.");
        }

        var gateway = await ResolveGatewayAsync(createDto.PaymentGatewayId, createDto.PaymentInstrument);
        if (gateway == null)
        {
            throw new InvalidOperationException("No active payment gateway found for selected instrument.");
        }

        if (createDto.IsDefault)
        {
            await UnsetCustomerDefaultAsync(createDto.CustomerId);
        }

        var token = createDto.PaymentMethodToken.Trim();
        var entity = new CustomerPaymentMethod
        {
            CustomerId = createDto.CustomerId,
            PaymentGatewayId = gateway.Id,
            Type = ResolvePaymentMethodType(createDto.Type, createDto.PaymentInstrument),
            PaymentMethodToken = token,
            Last4Digits = GetLast4Digits(token),
            CardBrand = string.Empty,
            CardholderName = string.Empty,
            BillingAddressJson = string.Empty,
            IsDefault = createDto.IsDefault,
            IsActive = true,
            IsVerified = false
        };

        _context.CustomerPaymentMethods.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created payment method {PaymentMethodId} for customer {CustomerId}", entity.Id, entity.CustomerId);

        entity.PaymentGateway = gateway;
        return MapToDto(entity);
    }

    public async Task<bool> SetAsDefaultAsync(int id, int customerId)
    {
        var target = await _context.CustomerPaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id && p.CustomerId == customerId && p.DeletedAt == null);

        if (target == null)
        {
            return false;
        }

        await UnsetCustomerDefaultAsync(customerId);
        target.IsDefault = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CustomerPaymentMethodDto?> UpdatePaymentMethodAsync(int id, int customerId, UpdateCustomerPaymentMethodDto updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        var gateway = await ResolveGatewayAsync(updateDto.PaymentGatewayId, updateDto.PaymentInstrument);

        if (gateway == null)
        {
            throw new InvalidOperationException("No active payment gateway found for selected instrument.");
        }

        var entity = await _context.CustomerPaymentMethods
            .Include(p => p.PaymentGateway)
            .FirstOrDefaultAsync(p => p.Id == id && p.CustomerId == customerId && p.DeletedAt == null);

        if (entity == null)
        {
            return null;
        }

        entity.PaymentGatewayId = gateway.Id;
        entity.Type = ResolvePaymentMethodType(updateDto.Type, updateDto.PaymentInstrument);

        if (updateDto.IsDefault)
        {
            await UnsetCustomerDefaultAsync(customerId);
            entity.IsDefault = true;
        }
        else if (entity.IsDefault)
        {
            entity.IsDefault = false;
        }

        await _context.SaveChangesAsync();

        entity.PaymentGateway = gateway;
        return MapToDto(entity);
    }

    public async Task<bool> DeletePaymentMethodAsync(int id, int customerId)
    {
        var entity = await _context.CustomerPaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id && p.CustomerId == customerId && p.DeletedAt == null);

        if (entity == null)
        {
            return false;
        }

        var wasDefault = entity.IsDefault;
        entity.IsActive = false;
        entity.IsDefault = false;
        entity.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (wasDefault)
        {
            var replacement = await _context.CustomerPaymentMethods
                .Where(p => p.CustomerId == customerId && p.DeletedAt == null && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (replacement != null)
            {
                replacement.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        return true;
    }

    private async Task<PaymentGateway?> ResolveGatewayAsync(int explicitGatewayId, string paymentInstrument)
    {
        if (explicitGatewayId > 0)
        {
            return await _context.PaymentGateways
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == explicitGatewayId && g.DeletedAt == null && g.IsActive);
        }

        if (string.IsNullOrWhiteSpace(paymentInstrument))
        {
            return null;
        }

        var normalizedInput = NormalizeInstrumentKey(paymentInstrument);

        var instrument = await _context.PaymentInstruments
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.DeletedAt == null && i.IsActive &&
                (NormalizeInstrumentKey(i.Code) == normalizedInput || NormalizeInstrumentKey(i.Name) == normalizedInput));

        if (instrument?.DefaultGatewayId is int defaultGatewayId && defaultGatewayId > 0)
        {
            var defaultGateway = await _context.PaymentGateways
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == defaultGatewayId && g.DeletedAt == null && g.IsActive);

            if (defaultGateway != null)
            {
                return defaultGateway;
            }
        }

        var activeGateways = await _context.PaymentGateways
            .AsNoTracking()
            .Include(g => g.PaymentInstrumentEntity)
            .Where(g => g.IsActive && g.DeletedAt == null)
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Name)
            .ToListAsync();

        var matchingGateways = activeGateways
            .Where(g =>
            {
                var code = NormalizeInstrumentKey(g.PaymentInstrumentEntity?.Code ?? g.PaymentInstrument);
                var name = NormalizeInstrumentKey(g.PaymentInstrumentEntity?.Name ?? string.Empty);
                return code == normalizedInput || name == normalizedInput;
            })
            .ToList();

        if (matchingGateways.Count == 0)
        {
            return null;
        }

        return matchingGateways.FirstOrDefault(g => g.IsDefault) ?? matchingGateways[0];
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

    private static PaymentMethodType ResolvePaymentMethodType(PaymentMethodType fallbackType, string paymentInstrument)
    {
        if (string.IsNullOrWhiteSpace(paymentInstrument))
        {
            return fallbackType;
        }

        return paymentInstrument.Trim().ToLowerInvariant() switch
        {
            "creditcard" or "credit card" or "card" => PaymentMethodType.CreditCard,
            "bankaccount" or "bank account" => PaymentMethodType.BankAccount,
            "paypal" => PaymentMethodType.PayPal,
            "cash" => PaymentMethodType.Cash,
            _ => fallbackType
        };
    }

    private async Task UnsetCustomerDefaultAsync(int customerId)
    {
        var defaults = await _context.CustomerPaymentMethods
            .Where(p => p.CustomerId == customerId && p.IsDefault && p.DeletedAt == null)
            .ToListAsync();

        if (defaults.Count == 0)
        {
            return;
        }

        foreach (var paymentMethod in defaults)
        {
            paymentMethod.IsDefault = false;
        }

        await _context.SaveChangesAsync();
    }

    private static string GetLast4Digits(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }

        var digits = new string(token.Where(char.IsDigit).ToArray());
        if (digits.Length >= 4)
        {
            return digits[^4..];
        }

        return string.Empty;
    }

    private static CustomerPaymentMethodDto MapToDto(CustomerPaymentMethod entity)
    {
        return new CustomerPaymentMethodDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            PaymentGatewayId = entity.PaymentGatewayId,
            PaymentGatewayName = entity.PaymentGateway?.Name ?? string.Empty,
            Type = entity.Type,
            Last4Digits = entity.Last4Digits,
            ExpiryMonth = entity.ExpiryMonth,
            ExpiryYear = entity.ExpiryYear,
            CardBrand = entity.CardBrand,
            CardholderName = entity.CardholderName,
            IsDefault = entity.IsDefault,
            IsActive = entity.IsActive,
            IsVerified = entity.IsVerified,
            CreatedAt = entity.CreatedAt
        };
    }
}

