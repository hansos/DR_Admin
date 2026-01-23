using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<OrderService>();

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
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

            var order = new Order
            {
                CustomerId = createDto.CustomerId,
                ServiceId = createDto.ServiceId,
                Status = createDto.Status,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                NextBillingDate = createDto.NextBillingDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created order with ID: {OrderId}", order.Id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating order for customer: {CustomerId}", createDto.CustomerId);
            throw;
        }
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

            order.CustomerId = updateDto.CustomerId;
            order.ServiceId = updateDto.ServiceId;
            order.Status = updateDto.Status;
            order.StartDate = updateDto.StartDate;
            order.EndDate = updateDto.EndDate;
            order.NextBillingDate = updateDto.NextBillingDate;
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
            CustomerId = order.CustomerId,
            ServiceId = order.ServiceId,
            Status = order.Status,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            NextBillingDate = order.NextBillingDate,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
