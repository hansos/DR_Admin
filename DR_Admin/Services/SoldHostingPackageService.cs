using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class SoldHostingPackageService : ISoldHostingPackageService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SoldHostingPackageService>();

    public SoldHostingPackageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SoldHostingPackageDto>> GetAllAsync()
    {
        var entities = await _context.SoldHostingPackages
            .AsNoTracking()
            .Include(x => x.RegisteredDomain)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return entities.Select(entity => MapToDto(entity));
    }

    public async Task<IEnumerable<SoldHostingPackageDto>> GetByCustomerIdAsync(int customerId)
    {
        var entities = await _context.SoldHostingPackages
            .AsNoTracking()
            .Include(x => x.RegisteredDomain)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return entities.Select(entity => MapToDto(entity));
    }

    public async Task<SoldHostingPackageDto?> GetByIdAsync(int id)
    {
        var entity = await _context.SoldHostingPackages
            .AsNoTracking()
            .Include(x => x.RegisteredDomain)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<SoldHostingPackageDto> CreateAsync(CreateSoldHostingPackageDto createDto)
    {
        var entity = new SoldHostingPackage
        {
            CustomerId = createDto.CustomerId,
            HostingPackageId = createDto.HostingPackageId,
            RegisteredDomainId = createDto.RegisteredDomainId,
            OrderId = createDto.OrderId,
            OrderLineId = createDto.OrderLineId,
            Status = string.IsNullOrWhiteSpace(createDto.Status) ? "PendingProvisioning" : createDto.Status,
            BillingCycle = string.IsNullOrWhiteSpace(createDto.BillingCycle) ? "monthly" : createDto.BillingCycle,
            SetupFee = createDto.SetupFee,
            RecurringPrice = createDto.RecurringPrice,
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

        _context.SoldHostingPackages.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created SoldHostingPackage {Id}", entity.Id);
        var connectedDomainName = await ResolveRegisteredDomainNameAsync(entity.RegisteredDomainId);
        return MapToDto(entity, connectedDomainName);
    }

    public async Task<SoldHostingPackageDto?> UpdateAsync(int id, UpdateSoldHostingPackageDto updateDto)
    {
        var entity = await _context.SoldHostingPackages.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        if (updateDto.RegisteredDomainId.HasValue) entity.RegisteredDomainId = updateDto.RegisteredDomainId;
        if (!string.IsNullOrWhiteSpace(updateDto.Status)) entity.Status = updateDto.Status;
        if (!string.IsNullOrWhiteSpace(updateDto.BillingCycle)) entity.BillingCycle = updateDto.BillingCycle;
        if (updateDto.SetupFee.HasValue) entity.SetupFee = updateDto.SetupFee.Value;
        if (updateDto.RecurringPrice.HasValue) entity.RecurringPrice = updateDto.RecurringPrice.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.CurrencyCode)) entity.CurrencyCode = updateDto.CurrencyCode;
        if (updateDto.ActivatedAt.HasValue) entity.ActivatedAt = updateDto.ActivatedAt.Value;
        if (updateDto.NextBillingDate.HasValue) entity.NextBillingDate = updateDto.NextBillingDate.Value;
        if (updateDto.ExpiresAt.HasValue) entity.ExpiresAt = updateDto.ExpiresAt;
        if (updateDto.AutoRenew.HasValue) entity.AutoRenew = updateDto.AutoRenew.Value;
        if (updateDto.ConfigurationSnapshotJson != null) entity.ConfigurationSnapshotJson = updateDto.ConfigurationSnapshotJson;
        if (updateDto.Notes != null) entity.Notes = updateDto.Notes;

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var connectedDomainName = await ResolveRegisteredDomainNameAsync(entity.RegisteredDomainId);
        return MapToDto(entity, connectedDomainName);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.SoldHostingPackages.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.SoldHostingPackages.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
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

    private static SoldHostingPackageDto MapToDto(SoldHostingPackage entity, string? connectedDomainName = null)
    {
        return new SoldHostingPackageDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            HostingPackageId = entity.HostingPackageId,
            RegisteredDomainId = entity.RegisteredDomainId,
            ConnectedDomainName = connectedDomainName ?? entity.RegisteredDomain?.Name ?? string.Empty,
            OrderId = entity.OrderId,
            OrderLineId = entity.OrderLineId,
            Status = entity.Status,
            BillingCycle = entity.BillingCycle,
            SetupFee = entity.SetupFee,
            RecurringPrice = entity.RecurringPrice,
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
