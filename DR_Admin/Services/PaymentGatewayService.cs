using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing payment gateways
/// </summary>
public class PaymentGatewayService : IPaymentGatewayService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentGatewayService>();

    public PaymentGatewayService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all payment gateways
    /// </summary>
    public async Task<IEnumerable<PaymentGatewayDto>> GetAllPaymentGatewaysAsync()
    {
        try
        {
            _log.Information("Fetching all payment gateways");

            var gateways = await _context.PaymentGateways
                .AsNoTracking()
                .Where(g => g.DeletedAt == null)
                .OrderBy(g => g.DisplayOrder)
                .ThenBy(g => g.Name)
                .ToListAsync();

            var gatewayDtos = gateways.Select(MapToDto);

            _log.Information("Successfully fetched {Count} payment gateways", gateways.Count);
            return gatewayDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all payment gateways");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active payment gateways
    /// </summary>
    public async Task<IEnumerable<PaymentGatewayDto>> GetActivePaymentGatewaysAsync()
    {
        try
        {
            _log.Information("Fetching active payment gateways");

            var gateways = await _context.PaymentGateways
                .AsNoTracking()
                .Where(g => g.DeletedAt == null && g.IsActive)
                .OrderBy(g => g.DisplayOrder)
                .ThenBy(g => g.Name)
                .ToListAsync();

            var gatewayDtos = gateways.Select(MapToDto);

            _log.Information("Successfully fetched {Count} active payment gateways", gateways.Count);
            return gatewayDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active payment gateways");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific payment gateway by ID
    /// </summary>
    public async Task<PaymentGatewayDto?> GetPaymentGatewayByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching payment gateway with ID: {GatewayId}", id);

            var gateway = await _context.PaymentGateways
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with ID {GatewayId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched payment gateway with ID: {GatewayId}", id);
            return MapToDto(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching payment gateway with ID: {GatewayId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default payment gateway
    /// </summary>
    public async Task<PaymentGatewayDto?> GetDefaultPaymentGatewayAsync()
    {
        try
        {
            _log.Information("Fetching default payment gateway");

            var gateway = await _context.PaymentGateways
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.IsDefault && g.IsActive && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("No default payment gateway found");
                return null;
            }

            _log.Information("Successfully fetched default payment gateway: {GatewayName}", gateway.Name);
            return MapToDto(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default payment gateway");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a payment gateway by provider code
    /// </summary>
    public async Task<PaymentGatewayDto?> GetPaymentGatewayByProviderAsync(string providerCode)
    {
        try
        {
            _log.Information("Fetching payment gateway with provider code: {ProviderCode}", providerCode);

            var gateway = await _context.PaymentGateways
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.ProviderCode == providerCode && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with provider code {ProviderCode} not found", providerCode);
                return null;
            }

            _log.Information("Successfully fetched payment gateway with provider code: {ProviderCode}", providerCode);
            return MapToDto(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching payment gateway with provider code: {ProviderCode}", providerCode);
            throw;
        }
    }

    /// <summary>
    /// Creates a new payment gateway
    /// </summary>
    public async Task<PaymentGatewayDto> CreatePaymentGatewayAsync(CreatePaymentGatewayDto dto)
    {
        try
        {
            _log.Information("Creating new payment gateway: {GatewayName}", dto.Name);

            var gateway = new PaymentGateway
            {
                Name = dto.Name,
                ProviderCode = dto.ProviderCode,
                IsActive = dto.IsActive,
                IsDefault = dto.IsDefault,
                ApiKey = dto.ApiKey,
                ApiSecret = dto.ApiSecret,
                ConfigurationJson = dto.ConfigurationJson,
                UseSandbox = dto.UseSandbox,
                WebhookUrl = dto.WebhookUrl,
                WebhookSecret = dto.WebhookSecret,
                DisplayOrder = dto.DisplayOrder,
                Description = dto.Description,
                LogoUrl = dto.LogoUrl,
                SupportedCurrencies = dto.SupportedCurrencies,
                FeePercentage = dto.FeePercentage,
                FixedFee = dto.FixedFee,
                Notes = dto.Notes
            };

            // If this is set as default, unset other defaults
            if (dto.IsDefault)
            {
                await UnsetAllDefaultsAsync();
            }

            _context.PaymentGateways.Add(gateway);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created payment gateway with ID: {GatewayId}", gateway.Id);
            return MapToDto(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating payment gateway: {GatewayName}", dto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing payment gateway
    /// </summary>
    public async Task<PaymentGatewayDto?> UpdatePaymentGatewayAsync(int id, UpdatePaymentGatewayDto dto)
    {
        try
        {
            _log.Information("Updating payment gateway with ID: {GatewayId}", id);

            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with ID {GatewayId} not found", id);
                return null;
            }

            gateway.Name = dto.Name;
            gateway.ProviderCode = dto.ProviderCode;
            gateway.IsActive = dto.IsActive;
            gateway.IsDefault = dto.IsDefault;
            gateway.ConfigurationJson = dto.ConfigurationJson;
            gateway.UseSandbox = dto.UseSandbox;
            gateway.WebhookUrl = dto.WebhookUrl;
            gateway.DisplayOrder = dto.DisplayOrder;
            gateway.Description = dto.Description;
            gateway.LogoUrl = dto.LogoUrl;
            gateway.SupportedCurrencies = dto.SupportedCurrencies;
            gateway.FeePercentage = dto.FeePercentage;
            gateway.FixedFee = dto.FixedFee;
            gateway.Notes = dto.Notes;

            // Only update sensitive fields if provided
            if (!string.IsNullOrWhiteSpace(dto.ApiKey))
            {
                gateway.ApiKey = dto.ApiKey;
            }

            if (!string.IsNullOrWhiteSpace(dto.ApiSecret))
            {
                gateway.ApiSecret = dto.ApiSecret;
            }

            if (!string.IsNullOrWhiteSpace(dto.WebhookSecret))
            {
                gateway.WebhookSecret = dto.WebhookSecret;
            }

            // If this is set as default, unset other defaults
            if (dto.IsDefault)
            {
                await UnsetAllDefaultsAsync(id);
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated payment gateway with ID: {GatewayId}", id);
            return MapToDto(gateway);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating payment gateway with ID: {GatewayId}", id);
            throw;
        }
    }

    /// <summary>
    /// Sets a payment gateway as the default
    /// </summary>
    public async Task<bool> SetDefaultPaymentGatewayAsync(int id)
    {
        try
        {
            _log.Information("Setting payment gateway {GatewayId} as default", id);

            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with ID {GatewayId} not found", id);
                return false;
            }

            // Unset all other defaults
            await UnsetAllDefaultsAsync(id);

            gateway.IsDefault = true;
            gateway.IsActive = true; // Ensure default gateway is active

            await _context.SaveChangesAsync();

            _log.Information("Successfully set payment gateway {GatewayId} as default", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting payment gateway {GatewayId} as default", id);
            throw;
        }
    }

    /// <summary>
    /// Activates or deactivates a payment gateway
    /// </summary>
    public async Task<bool> SetPaymentGatewayActiveStatusAsync(int id, bool isActive)
    {
        try
        {
            _log.Information("Setting payment gateway {GatewayId} active status to {IsActive}", id, isActive);

            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with ID {GatewayId} not found", id);
                return false;
            }

            // If deactivating the default gateway, find a new default
            if (!isActive && gateway.IsDefault)
            {
                var alternateGateway = await _context.PaymentGateways
                    .FirstOrDefaultAsync(g => g.Id != id && g.IsActive && g.DeletedAt == null);

                if (alternateGateway != null)
                {
                    alternateGateway.IsDefault = true;
                }

                gateway.IsDefault = false;
            }

            gateway.IsActive = isActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully set payment gateway {GatewayId} active status to {IsActive}", id, isActive);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting payment gateway {GatewayId} active status", id);
            throw;
        }
    }

    /// <summary>
    /// Soft deletes a payment gateway
    /// </summary>
    public async Task<bool> DeletePaymentGatewayAsync(int id)
    {
        try
        {
            _log.Information("Deleting payment gateway with ID: {GatewayId}", id);

            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);

            if (gateway == null)
            {
                _log.Information("Payment gateway with ID {GatewayId} not found", id);
                return false;
            }

            // If deleting the default gateway, find a new default
            if (gateway.IsDefault)
            {
                var alternateGateway = await _context.PaymentGateways
                    .FirstOrDefaultAsync(g => g.Id != id && g.IsActive && g.DeletedAt == null);

                if (alternateGateway != null)
                {
                    alternateGateway.IsDefault = true;
                }
            }

            gateway.DeletedAt = DateTime.UtcNow;
            gateway.IsActive = false;
            gateway.IsDefault = false;

            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted payment gateway with ID: {GatewayId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting payment gateway with ID: {GatewayId}", id);
            throw;
        }
    }

    private async Task UnsetAllDefaultsAsync(int? exceptId = null)
    {
        var defaultGateways = await _context.PaymentGateways
            .Where(g => g.IsDefault && g.DeletedAt == null && (exceptId == null || g.Id != exceptId))
            .ToListAsync();

        foreach (var gateway in defaultGateways)
        {
            gateway.IsDefault = false;
        }

        if (defaultGateways.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    private PaymentGatewayDto MapToDto(PaymentGateway gateway)
    {
        return new PaymentGatewayDto
        {
            Id = gateway.Id,
            Name = gateway.Name,
            ProviderCode = gateway.ProviderCode,
            IsActive = gateway.IsActive,
            IsDefault = gateway.IsDefault,
            ApiKey = MaskSensitiveData(gateway.ApiKey),
            UseSandbox = gateway.UseSandbox,
            WebhookUrl = gateway.WebhookUrl,
            DisplayOrder = gateway.DisplayOrder,
            Description = gateway.Description,
            LogoUrl = gateway.LogoUrl,
            SupportedCurrencies = gateway.SupportedCurrencies,
            FeePercentage = gateway.FeePercentage,
            FixedFee = gateway.FixedFee,
            Notes = gateway.Notes,
            CreatedAt = gateway.CreatedAt,
            UpdatedAt = gateway.UpdatedAt,
            DeletedAt = gateway.DeletedAt
        };
    }

    private string MaskSensitiveData(string data)
    {
        if (string.IsNullOrWhiteSpace(data) || data.Length <= 8)
        {
            return "****";
        }

        return data.Substring(0, 4) + new string('*', data.Length - 8) + data.Substring(data.Length - 4);
    }
}
