using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class SoldOptionalServiceService : ISoldOptionalServiceService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SoldOptionalServiceService>();

    public SoldOptionalServiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SoldOptionalServiceDto>> GetAllAsync()
    {
        var entities = await _context.SoldOptionalServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.RegisteredDomain)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return entities.Select<SoldOptionalService, SoldOptionalServiceDto>(entity => MapToDto(entity));
    }

    public async Task<IEnumerable<SoldOptionalServiceDto>> GetByCustomerIdAsync(int customerId)
    {
        var entities = await _context.SoldOptionalServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.RegisteredDomain)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return entities.Select<SoldOptionalService, SoldOptionalServiceDto>(entity => MapToDto(entity));
    }

    public async Task<SoldOptionalServiceDto?> GetByIdAsync(int id)
    {
        var entity = await _context.SoldOptionalServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.RegisteredDomain)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<SoldOptionalServiceDto> CreateAsync(CreateSoldOptionalServiceDto createDto)
    {
        var quantity = createDto.Quantity <= 0 ? 1 : createDto.Quantity;
        var totalPrice = createDto.TotalPrice <= 0 ? quantity * createDto.UnitPrice : createDto.TotalPrice;

        var entity = new SoldOptionalService
        {
            CustomerId = createDto.CustomerId,
            ServiceId = createDto.ServiceId,
            RegisteredDomainId = createDto.RegisteredDomainId,
            OrderId = createDto.OrderId,
            OrderLineId = createDto.OrderLineId,
            Quantity = quantity,
            UnitPrice = createDto.UnitPrice,
            TotalPrice = totalPrice,
            Status = string.IsNullOrWhiteSpace(createDto.Status) ? "Active" : createDto.Status,
            BillingCycle = string.IsNullOrWhiteSpace(createDto.BillingCycle) ? "monthly" : createDto.BillingCycle,
            CurrencyCode = string.IsNullOrWhiteSpace(createDto.CurrencyCode) ? "EUR" : createDto.CurrencyCode,
            ActivatedAt = createDto.ActivatedAt,
            NextBillingDate = createDto.NextBillingDate,
            ExpiresAt = createDto.ExpiresAt,
            AutoRenew = createDto.AutoRenew,
            ConfigurationSnapshotJson = createDto.ConfigurationSnapshotJson ?? string.Empty,
            Notes = createDto.Notes ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.SoldOptionalServices.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created SoldOptionalService {Id}", entity.Id);
        var serviceName = await ResolveServiceNameAsync(entity.ServiceId);
        var connectedDomainName = await ResolveRegisteredDomainNameAsync(entity.RegisteredDomainId);
        return MapToDto(entity, serviceName, connectedDomainName);
    }

    public async Task<SoldOptionalServiceDto?> UpdateAsync(int id, UpdateSoldOptionalServiceDto updateDto)
    {
        var entity = await _context.SoldOptionalServices.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        if (updateDto.RegisteredDomainId.HasValue) entity.RegisteredDomainId = updateDto.RegisteredDomainId;
        if (updateDto.Quantity.HasValue && updateDto.Quantity.Value > 0) entity.Quantity = updateDto.Quantity.Value;
        if (updateDto.UnitPrice.HasValue) entity.UnitPrice = updateDto.UnitPrice.Value;
        if (updateDto.TotalPrice.HasValue) entity.TotalPrice = updateDto.TotalPrice.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.Status)) entity.Status = updateDto.Status;
        if (!string.IsNullOrWhiteSpace(updateDto.BillingCycle)) entity.BillingCycle = updateDto.BillingCycle;
        if (!string.IsNullOrWhiteSpace(updateDto.CurrencyCode)) entity.CurrencyCode = updateDto.CurrencyCode;
        if (updateDto.ActivatedAt.HasValue) entity.ActivatedAt = updateDto.ActivatedAt.Value;
        if (updateDto.NextBillingDate.HasValue) entity.NextBillingDate = updateDto.NextBillingDate.Value;
        if (updateDto.ExpiresAt.HasValue) entity.ExpiresAt = updateDto.ExpiresAt;
        if (updateDto.AutoRenew.HasValue) entity.AutoRenew = updateDto.AutoRenew.Value;
        if (updateDto.ConfigurationSnapshotJson != null) entity.ConfigurationSnapshotJson = updateDto.ConfigurationSnapshotJson;
        if (updateDto.Notes != null) entity.Notes = updateDto.Notes;

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var serviceName = await ResolveServiceNameAsync(entity.ServiceId);
        var connectedDomainName = await ResolveRegisteredDomainNameAsync(entity.RegisteredDomainId);
        return MapToDto(entity, serviceName, connectedDomainName);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.SoldOptionalServices.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.SoldOptionalServices.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> ResolveServiceNameAsync(int serviceId)
    {
        return await _context.Services
            .AsNoTracking()
            .Where(x => x.Id == serviceId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync() ?? string.Empty;
    }

    private async Task<string> ResolveRegisteredDomainNameAsync(int? registeredDomainId)
    {
        if (!registeredDomainId.HasValue)
        {
            return string.Empty;
        }

        return await _context.RegisteredDomains
            .AsNoTracking()
            .Where(x => x.Id == registeredDomainId.Value)
            .Select(x => x.Name)
            .FirstOrDefaultAsync() ?? string.Empty;
    }

    private static SoldOptionalServiceDto MapToDto(SoldOptionalService entity, string? serviceName = null, string? connectedDomainName = null)
    {
        return new SoldOptionalServiceDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            ServiceId = entity.ServiceId,
            RegisteredDomainId = entity.RegisteredDomainId,
            ServiceName = serviceName ?? entity.Service?.Name ?? string.Empty,
            ConnectedDomainName = connectedDomainName ?? entity.RegisteredDomain?.Name ?? string.Empty,
            OrderId = entity.OrderId,
            OrderLineId = entity.OrderLineId,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            TotalPrice = entity.TotalPrice,
            Status = entity.Status,
            BillingCycle = entity.BillingCycle,
            CurrencyCode = entity.CurrencyCode,
            ActivatedAt = entity.ActivatedAt,
            NextBillingDate = entity.NextBillingDate,
            ExpiresAt = entity.ExpiresAt,
            AutoRenew = entity.AutoRenew,
            ConfigurationSnapshotJson = entity.ConfigurationSnapshotJson,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
