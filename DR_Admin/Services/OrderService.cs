using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ICustomerService _customerService;
    private static readonly Serilog.ILogger _log = Log.ForContext<OrderService>();

    public OrderService(ApplicationDbContext context, ICustomerService customerService)
    {
        _context = context;
        _customerService = customerService;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        try
        {
            _log.Information("Fetching all orders");
            
            var orders = await _context.Orders
                .Include(o => o.OrderLines)
                .AsNoTracking()
                .ToListAsync();

            var orderDtos = orders.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} orders", orders.Count);
            return orderDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all orders");
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching order with ID: {OrderId}", id);
            
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                _log.Warning("Order with ID {OrderId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched order with ID: {OrderId}", id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching order with ID: {OrderId}", id);
            throw;
        }
    }


    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        try
        {
            _log.Information("Creating new order for customer: {CustomerId}", createDto.CustomerId);

            var recentOrders = await _context.Orders
                .Include(o => o.OrderLines)
                .Where(o => o.CustomerId == createDto.CustomerId && o.CreatedAt >= DateTime.UtcNow.AddMinutes(-2))
                .AsNoTracking()
                .ToListAsync();

            var duplicate = recentOrders.FirstOrDefault(existing => IsDuplicateOrder(existing, createDto));
            if (duplicate != null)
            {
                _log.Warning("Duplicate order request detected for customer {CustomerId}, returning existing order {OrderId}", createDto.CustomerId, duplicate.Id);
                return MapToDto(duplicate);
            }

            if (createDto.ServiceId.HasValue)
            {
                var service = await _context.Services.FindAsync(createDto.ServiceId.Value);
                if (service == null)
                {
                    throw new InvalidOperationException($"Service with ID {createDto.ServiceId.Value} not found");
                }
            }

            if (createDto.OrderLines.Count > 0)
            {
                var distinctServiceIds = createDto.OrderLines
                    .Where(line => line.ServiceId.HasValue)
                    .Select(line => line.ServiceId!.Value)
                    .Distinct()
                    .ToList();

                if (distinctServiceIds.Count > 0)
                {
                    var validServiceIds = await _context.Services
                        .Where(service => distinctServiceIds.Contains(service.Id))
                        .Select(service => service.Id)
                        .ToListAsync();

                    var missingServiceId = distinctServiceIds.FirstOrDefault(id => !validServiceIds.Contains(id));
                    if (missingServiceId > 0)
                    {
                        throw new InvalidOperationException($"Service with ID {missingServiceId} not found");
                    }
                }
            }

            var order = new Order
            {
                OrderNumber = await GenerateOrderNumberAsync(),
                CustomerId = createDto.CustomerId,
                ServiceId = createDto.ServiceId,
                QuoteId = createDto.QuoteId,
                OrderType = createDto.OrderType,
                Status = Data.Enums.OrderStatus.Pending,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                NextBillingDate = createDto.NextBillingDate,
                SetupFee = createDto.SetupFee ?? 0,
                RecurringAmount = createDto.RecurringAmount ?? 0,
                AutoRenew = createDto.AutoRenew,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (createDto.OrderLines.Count > 0)
            {
                var lineNumber = 1;
                var lines = createDto.OrderLines.Select(line => new OrderLine
                {
                    OrderId = order.Id,
                    ServiceId = line.ServiceId,
                    LineNumber = lineNumber++,
                    Description = line.Description,
                    Quantity = line.Quantity <= 0 ? 1 : line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = (line.Quantity <= 0 ? 1 : line.Quantity) * line.UnitPrice,
                    IsRecurring = line.IsRecurring || (line.ServiceId.HasValue && createDto.RecurringAmount.GetValueOrDefault() > 0),
                    Notes = line.Notes
                }).ToList();

                _context.OrderLines.AddRange(lines);
                await _context.SaveChangesAsync();
                order.OrderLines = lines;
            }

            // Assign a CustomerNumber on first sale if the customer doesn't have one yet
            await _customerService.EnsureCustomerNumberAsync(createDto.CustomerId);

            _log.Information("Successfully created order with ID: {OrderId}", order.Id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating order for customer: {CustomerId}", createDto.CustomerId);
            throw;
        }
    }

    private static bool IsDuplicateOrder(Order existing, CreateOrderDto request)
    {
        if (existing.ServiceId != request.ServiceId ||
            existing.QuoteId != request.QuoteId ||
            existing.OrderType != request.OrderType ||
            existing.StartDate != request.StartDate ||
            existing.EndDate != request.EndDate ||
            existing.NextBillingDate != request.NextBillingDate ||
            existing.SetupFee != request.SetupFee.GetValueOrDefault() ||
            existing.RecurringAmount != request.RecurringAmount.GetValueOrDefault() ||
            existing.AutoRenew != request.AutoRenew)
        {
            return false;
        }

        var requestLines = request.OrderLines ?? new List<CreateOrderLineDto>();
        var existingLines = existing.OrderLines.OrderBy(line => line.LineNumber).ToList();
        if (existingLines.Count != requestLines.Count)
        {
            return false;
        }

        for (var i = 0; i < existingLines.Count; i++)
        {
            var existingLine = existingLines[i];
            var requestLine = requestLines[i];
            var requestQuantity = requestLine.Quantity <= 0 ? 1 : requestLine.Quantity;

            if (existingLine.ServiceId != requestLine.ServiceId ||
                existingLine.Description != requestLine.Description ||
                existingLine.Quantity != requestQuantity ||
                existingLine.UnitPrice != requestLine.UnitPrice ||
                existingLine.TotalPrice != requestQuantity * requestLine.UnitPrice ||
                existingLine.IsRecurring != requestLine.IsRecurring)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var nextNumber = await GetNextOrderNumberAsync();
        var prefix = await GetOrderNumberPrefixAsync();

        return string.IsNullOrWhiteSpace(prefix)
            ? nextNumber.ToString()
            : $"{prefix}{nextNumber}";
    }

    private async Task<long> GetNextOrderNumberAsync()
    {
        const string key = "ONR";
        const long defaultStartValue = 1001;

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Value = (defaultStartValue + 1).ToString(),
                Description = "The next order number (ONR) to assign. Auto-incremented on each new order creation.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsSystemKey = true
            };
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            _log.Information("Initialized {Key} system setting with starting value {Value}", key, defaultStartValue);
            return defaultStartValue;
        }

        if (!long.TryParse(setting.Value, out var currentValue) || currentValue <= 0)
        {
            _log.Warning("Invalid {Key} value '{Value}', resetting to default {Default}", key, setting.Value, defaultStartValue);
            currentValue = defaultStartValue;
        }

        setting.Value = (currentValue + 1).ToString();
        setting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return currentValue;
    }

    private async Task<string?> GetOrderNumberPrefixAsync()
    {
        const string key = "OSX";

        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting?.Value;
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto)
    {
        try
        {
            _log.Information("Updating order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                _log.Warning("Order with ID {OrderId} not found for update", id);
                return null;
            }

            order.ServiceId = updateDto.ServiceId;
            order.Status = updateDto.Status;
            order.StartDate = updateDto.StartDate;
            order.EndDate = updateDto.EndDate;
            order.NextBillingDate = updateDto.NextBillingDate;
            order.AutoRenew = updateDto.AutoRenew;
            order.Notes = updateDto.Notes;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated order with ID: {OrderId}", id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            _log.Information("Deleting order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                _log.Warning("Order with ID {OrderId} not found for deletion", id);
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted order with ID: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting order with ID: {OrderId}", id);
            throw;
        }
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            ServiceId = order.ServiceId,
            QuoteId = order.QuoteId,
            OrderType = order.OrderType,
            Status = order.Status,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            NextBillingDate = order.NextBillingDate,
            SetupFee = order.SetupFee,
            RecurringAmount = order.RecurringAmount,
            DiscountAmount = order.DiscountAmount,
            CurrencyCode = order.CurrencyCode,
            BaseCurrencyCode = order.BaseCurrencyCode,
            ExchangeRate = order.ExchangeRate,
            ExchangeRateDate = order.ExchangeRateDate,
            TrialEndsAt = order.TrialEndsAt,
            AutoRenew = order.AutoRenew,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            OrderLines = order.OrderLines
                .OrderBy(line => line.LineNumber)
                .Select(line => new OrderLineDto
                {
                    Id = line.Id,
                    ServiceId = line.ServiceId,
                    LineNumber = line.LineNumber,
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = line.TotalPrice,
                    IsRecurring = line.IsRecurring,
                    Notes = line.Notes
                })
                .ToList()
        };
    }
}
