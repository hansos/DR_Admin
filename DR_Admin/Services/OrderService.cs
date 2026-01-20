using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public OrderService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        try
        {
            _logger.Information("Fetching all orders");
            
            var orders = await _context.Orders
                .AsNoTracking()
                .ToListAsync();

            var orderDtos = orders.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} orders", orders.Count);
            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all orders");
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching order with ID: {OrderId}", id);
            
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                _logger.Warning("Order with ID {OrderId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched order with ID: {OrderId}", id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        try
        {
            _logger.Information("Creating new order for customer: {CustomerId}", createDto.CustomerId);

            var order = new Order
            {
                CustomerId = createDto.CustomerId,
                ServiceId = createDto.ServiceId,
                Status = createDto.Status,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                NextBillingDate = createDto.NextBillingDate
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created order with ID: {OrderId}", order.Id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating order for customer: {CustomerId}", createDto.CustomerId);
            throw;
        }
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto)
    {
        try
        {
            _logger.Information("Updating order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                _logger.Warning("Order with ID {OrderId} not found for update", id);
                return null;
            }

            order.CustomerId = updateDto.CustomerId;
            order.ServiceId = updateDto.ServiceId;
            order.Status = updateDto.Status;
            order.StartDate = updateDto.StartDate;
            order.EndDate = updateDto.EndDate;
            order.NextBillingDate = updateDto.NextBillingDate;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated order with ID: {OrderId}", id);
            return MapToDto(order);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating order with ID: {OrderId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            _logger.Information("Deleting order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                _logger.Warning("Order with ID {OrderId} not found for deletion", id);
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted order with ID: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting order with ID: {OrderId}", id);
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
            NextBillingDate = order.NextBillingDate
        };
    }
}
