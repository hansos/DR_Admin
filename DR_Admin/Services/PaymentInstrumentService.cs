using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class PaymentInstrumentService : IPaymentInstrumentService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentInstrumentService>();

    public PaymentInstrumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentInstrumentDto>> GetAllAsync()
    {
        var items = await _context.PaymentInstruments
            .AsNoTracking()
            .Where(i => i.DeletedAt == null)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Name)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<PaymentInstrumentDto>> GetActiveAsync()
    {
        var items = await _context.PaymentInstruments
            .AsNoTracking()
            .Where(i => i.DeletedAt == null && i.IsActive)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Name)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<PaymentInstrumentDto?> GetByIdAsync(int id)
    {
        var item = await _context.PaymentInstruments
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        return item == null ? null : MapToDto(item);
    }

    public async Task<PaymentInstrumentDto?> GetByCodeAsync(string code)
    {
        var normalized = (code ?? string.Empty).Trim().ToLower();
        var item = await _context.PaymentInstruments
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.DeletedAt == null && i.Code.ToLower() == normalized);

        return item == null ? null : MapToDto(item);
    }

    public async Task<PaymentInstrumentDto> CreateAsync(CreatePaymentInstrumentDto dto)
    {
        var code = dto.Code.Trim();
        var exists = await _context.PaymentInstruments
            .AnyAsync(i => i.DeletedAt == null && i.Code.ToLower() == code.ToLower());

        if (exists)
        {
            throw new InvalidOperationException($"Payment instrument with code '{code}' already exists");
        }

        var resolvedDefaultGatewayId = await ResolveDefaultGatewayIdAsync(dto.DefaultGatewayId, code);

        var entity = new PaymentInstrument
        {
            Code = code,
            Name = dto.Name.Trim(),
            Description = dto.Description,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            DefaultGatewayId = resolvedDefaultGatewayId
        };

        _context.PaymentInstruments.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created payment instrument {Code} ({Id})", entity.Code, entity.Id);
        return MapToDto(entity);
    }

    public async Task<PaymentInstrumentDto?> UpdateAsync(int id, UpdatePaymentInstrumentDto dto)
    {
        var entity = await _context.PaymentInstruments
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (entity == null)
        {
            return null;
        }

        var code = dto.Code.Trim();
        var codeExists = await _context.PaymentInstruments
            .AnyAsync(i => i.Id != id && i.DeletedAt == null && i.Code.ToLower() == code.ToLower());

        if (codeExists)
        {
            throw new InvalidOperationException($"Payment instrument with code '{code}' already exists");
        }

        var oldCode = entity.Code;
        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        entity.DisplayOrder = dto.DisplayOrder;

        var resolvedDefaultGatewayId = await ResolveDefaultGatewayIdAsync(dto.DefaultGatewayId, code);
        entity.DefaultGatewayId = resolvedDefaultGatewayId;

        if (!string.Equals(oldCode, entity.Code, StringComparison.OrdinalIgnoreCase))
        {
            var linkedGateways = await _context.PaymentGateways
                .Where(g => g.DeletedAt == null && g.PaymentInstrumentId == id)
                .ToListAsync();

            foreach (var gateway in linkedGateways)
            {
                gateway.PaymentInstrument = entity.Code;
            }
        }

        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.PaymentInstruments
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (entity == null)
        {
            return false;
        }

        var inUse = await _context.PaymentGateways
            .AnyAsync(g => g.DeletedAt == null && g.PaymentInstrumentId == id);

        if (inUse)
        {
            throw new InvalidOperationException("Cannot delete payment instrument because it is used by one or more payment gateways");
        }

        entity.DeletedAt = DateTime.UtcNow;
        entity.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }

    private static PaymentInstrumentDto MapToDto(PaymentInstrument entity)
    {
        return new PaymentInstrumentDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder,
            DefaultGatewayId = entity.DefaultGatewayId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private async Task<int?> ResolveDefaultGatewayIdAsync(int? requestedGatewayId, string instrumentCode)
    {
        if (!requestedGatewayId.HasValue || requestedGatewayId.Value <= 0)
        {
            return null;
        }

        var gateway = await _context.PaymentGateways
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == requestedGatewayId.Value && g.DeletedAt == null && g.IsActive);

        if (gateway == null)
        {
            throw new InvalidOperationException("Selected default gateway was not found or is inactive.");
        }

        var normalizedGatewayInstrument = NormalizeInstrumentKey(gateway.PaymentInstrument);
        var normalizedTargetInstrument = NormalizeInstrumentKey(instrumentCode);
        if (normalizedGatewayInstrument != normalizedTargetInstrument)
        {
            throw new InvalidOperationException("Selected default gateway does not belong to this payment instrument.");
        }

        return gateway.Id;
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
}
