using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
    Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto);
    Task<bool> DeleteOrderAsync(int id);
}
