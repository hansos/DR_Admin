using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing order tax snapshot operations.
/// </summary>
public class OrderTaxSnapshotService : IOrderTaxSnapshotService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<OrderTaxSnapshotService>();

    public OrderTaxSnapshotService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all order tax snapshots.
    /// </summary>
    /// <returns>Collection of order tax snapshot DTOs.</returns>
    public async Task<IEnumerable<OrderTaxSnapshotDto>> GetAllOrderTaxSnapshotsAsync()
    {
        var entities = await _context.OrderTaxSnapshots
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        _log.Information("Fetched {Count} order tax snapshots", entities.Count);
        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves order tax snapshots by order identifier.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>Collection of order tax snapshot DTOs for the order.</returns>
    public async Task<IEnumerable<OrderTaxSnapshotDto>> GetOrderTaxSnapshotsByOrderIdAsync(int orderId)
    {
        var entities = await _context.OrderTaxSnapshots
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves an order tax snapshot by identifier.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>Order tax snapshot DTO if found; otherwise null.</returns>
    public async Task<OrderTaxSnapshotDto?> GetOrderTaxSnapshotByIdAsync(int id)
    {
        var entity = await _context.OrderTaxSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Creates a new order tax snapshot.
    /// </summary>
    /// <param name="dto">Order tax snapshot create request.</param>
    /// <returns>Created order tax snapshot DTO.</returns>
    public async Task<OrderTaxSnapshotDto> CreateOrderTaxSnapshotAsync(CreateOrderTaxSnapshotDto dto)
    {
        var entity = new OrderTaxSnapshot
        {
            OrderId = dto.OrderId,
            TaxJurisdictionId = dto.TaxJurisdictionId,
            BuyerCountryCode = dto.BuyerCountryCode,
            BuyerStateCode = dto.BuyerStateCode,
            BuyerType = dto.BuyerType,
            BuyerTaxId = dto.BuyerTaxId,
            BuyerTaxIdValidated = dto.BuyerTaxIdValidated,
            TaxCurrencyCode = dto.TaxCurrencyCode,
            DisplayCurrencyCode = dto.DisplayCurrencyCode,
            ExchangeRate = dto.ExchangeRate,
            ExchangeRateDate = dto.ExchangeRateDate,
            ExchangeRateSource = dto.ExchangeRateSource,
            TaxDeterminationEvidenceId = dto.TaxDeterminationEvidenceId,
            NetAmount = dto.NetAmount,
            TaxAmount = dto.TaxAmount,
            GrossAmount = dto.GrossAmount,
            AppliedTaxRate = dto.AppliedTaxRate,
            AppliedTaxName = dto.AppliedTaxName,
            ReverseChargeApplied = dto.ReverseChargeApplied,
            RuleVersion = dto.RuleVersion,
            IdempotencyKey = dto.IdempotencyKey,
            CalculationInputsJson = dto.CalculationInputsJson,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.OrderTaxSnapshots.Add(entity);
        await _context.SaveChangesAsync();

        _log.Information("Created order tax snapshot with ID {Id}", entity.Id);
        return MapToDto(entity);
    }

    /// <summary>
    /// Updates an existing order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <param name="dto">Order tax snapshot update request.</param>
    /// <returns>Updated order tax snapshot DTO if found; otherwise null.</returns>
    public async Task<OrderTaxSnapshotDto?> UpdateOrderTaxSnapshotAsync(int id, UpdateOrderTaxSnapshotDto dto)
    {
        var entity = await _context.OrderTaxSnapshots.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        throw new InvalidOperationException("Order tax snapshots are immutable and cannot be updated once created.");
    }

    /// <summary>
    /// Deletes an order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    public async Task<bool> DeleteOrderTaxSnapshotAsync(int id)
    {
        var entity = await _context.OrderTaxSnapshots.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        var isReferencedByInvoice = await _context.Invoices
            .AsNoTracking()
            .AnyAsync(x => x.OrderTaxSnapshotId == id && x.DeletedAt == null);

        if (isReferencedByInvoice)
        {
            throw new InvalidOperationException("Order tax snapshot is referenced by one or more invoices and cannot be deleted.");
        }

        throw new InvalidOperationException("Order tax snapshots are immutable and cannot be deleted.");
    }

    private static OrderTaxSnapshotDto MapToDto(OrderTaxSnapshot entity)
    {
        return new OrderTaxSnapshotDto
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            TaxJurisdictionId = entity.TaxJurisdictionId,
            BuyerCountryCode = entity.BuyerCountryCode,
            BuyerStateCode = entity.BuyerStateCode,
            BuyerType = entity.BuyerType,
            BuyerTaxId = entity.BuyerTaxId,
            BuyerTaxIdValidated = entity.BuyerTaxIdValidated,
            TaxCurrencyCode = entity.TaxCurrencyCode,
            DisplayCurrencyCode = entity.DisplayCurrencyCode,
            ExchangeRate = entity.ExchangeRate,
            ExchangeRateDate = entity.ExchangeRateDate,
            ExchangeRateSource = entity.ExchangeRateSource,
            TaxDeterminationEvidenceId = entity.TaxDeterminationEvidenceId,
            NetAmount = entity.NetAmount,
            TaxAmount = entity.TaxAmount,
            GrossAmount = entity.GrossAmount,
            AppliedTaxRate = entity.AppliedTaxRate,
            AppliedTaxName = entity.AppliedTaxName,
            ReverseChargeApplied = entity.ReverseChargeApplied,
            RuleVersion = entity.RuleVersion,
            IdempotencyKey = entity.IdempotencyKey,
            CalculationInputsJson = entity.CalculationInputsJson,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
