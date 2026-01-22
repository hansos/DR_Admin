using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private static readonly Serilog.ILogger _log = Log.ForContext<OrdersController>();

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        try
        {
            _log.Information("API: GetAllOrders called by user {User}", User.Identity?.Name);
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllOrders");
            return StatusCode(500, "An error occurred while retrieving orders");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        try
        {
            _log.Information("API: GetOrderById called for ID {OrderId} by user {User}", id, User.Identity?.Name);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                _log.Information("API: Order with ID {OrderId} not found", id);
                return NotFound($"Order with ID {id} not found");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetOrderById for ID {OrderId}", id);
            return StatusCode(500, "An error occurred while retrieving the order");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createDto)
    {
        try
        {
            _log.Information("API: CreateOrder called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateOrder");
                return BadRequest(ModelState);
            }

            var order = await _orderService.CreateOrderAsync(createDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateOrder");
            return StatusCode(500, "An error occurred while creating the order");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateOrder called for ID {OrderId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateOrder");
                return BadRequest(ModelState);
            }

            var order = await _orderService.UpdateOrderAsync(id, updateDto);

            if (order == null)
            {
                _log.Information("API: Order with ID {OrderId} not found for update", id);
                return NotFound($"Order with ID {id} not found");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateOrder for ID {OrderId}", id);
            return StatusCode(500, "An error occurred while updating the order");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        try
        {
            _log.Information("API: DeleteOrder called for ID {OrderId} by user {User}", id, User.Identity?.Name);
            var result = await _orderService.DeleteOrderAsync(id);

            if (!result)
            {
                _log.Information("API: Order with ID {OrderId} not found for deletion", id);
                return NotFound($"Order with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteOrder for ID {OrderId}", id);
            return StatusCode(500, "An error occurred while deleting the order");
        }
    }
}
