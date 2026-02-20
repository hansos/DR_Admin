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

            // Fetch service to get default prices if not provided
            var service = await _context.Services.FindAsync(createDto.ServiceId);
            if (service == null)
            {
                throw new InvalidOperationException($"Service with ID {createDto.ServiceId} not found");
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

    private async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrder = await _context.Orders
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync();

        var nextNumber = (lastOrder?.Id ?? 0) + 1;
        return $"ORD-{DateTime.UtcNow.Year}-{nextNumber:D5}";
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
            UpdatedAt = order.UpdatedAt
        };
    }
}
