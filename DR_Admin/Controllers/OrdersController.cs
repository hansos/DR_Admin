using ISPAdmin.DTOs;
using ISPAdmin.Data.Enums;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer orders including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMyAccountService _myAccountService;
    private static readonly Serilog.ILogger _log = Log.ForContext<OrdersController>();

    public OrdersController(IOrderService orderService, IMyAccountService myAccountService)
    {
        _orderService = orderService;
        _myAccountService = myAccountService;
    }

    /// <summary>
    /// Cancels a checkout order for the currently authenticated customer when not yet paid.
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Updated cancelled order</returns>
    [HttpPost("checkout/{id:int}/cancel")]
    [Authorize(Policy = "Order.Checkout")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> CancelCheckoutOrder(int id)
    {
        try
        {
            _log.Information("API: CancelCheckoutOrder called for {OrderId} by user {User}", id, User.Identity?.Name);

            var userId = GetCurrentUserId();
            var account = await _myAccountService.GetMyAccountAsync(userId);
            var customerId = account?.Customer?.Id ?? 0;
            if (customerId <= 0)
            {
                return BadRequest("Authenticated user does not have a linked customer account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.CustomerId != customerId)
            {
                return NotFound($"Order with ID {id} was not found for this customer");
            }

            var isPaid = order.Status == OrderStatus.Active || order.Status == OrderStatus.Expired;
            if (isPaid)
            {
                return Conflict("Order is already paid and cannot be cancelled from checkout.");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                return Ok(order);
            }

            var updateDto = new UpdateOrderDto
            {
                ServiceId = order.ServiceId,
                Status = OrderStatus.Cancelled,
                StartDate = order.StartDate,
                EndDate = order.EndDate,
                NextBillingDate = order.NextBillingDate,
                AutoRenew = false,
                Notes = "Cancelled by customer from checkout"
            };

            var updated = await _orderService.UpdateOrderAsync(order.Id, updateDto);
            if (updated == null)
            {
                return NotFound($"Order with ID {id} not found");
            }

            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            _log.Warning(ex, "API: Unauthorized CancelCheckoutOrder access");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CancelCheckoutOrder for ID {OrderId}", id);
            return StatusCode(500, "An error occurred while cancelling checkout order");
        }
    }

    /// <summary>
    /// Creates a new checkout order for the currently authenticated user.
    /// </summary>
    /// <param name="createDto">Order information for creation</param>
    /// <returns>The newly created order</returns>
    [HttpPost("checkout")]
    [Authorize(Policy = "Order.Checkout")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> CreateCheckoutOrder([FromBody] CreateOrderDto createDto)
    {
        try
        {
            _log.Information("API: CreateCheckoutOrder called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateCheckoutOrder");
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var account = await _myAccountService.GetMyAccountAsync(userId);
            var customerId = account?.Customer?.Id ?? 0;
            if (customerId <= 0)
            {
                return BadRequest("Authenticated user does not have a linked customer account");
            }

            createDto.CustomerId = customerId;

            var order = await _orderService.CreateOrderAsync(createDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (UnauthorizedAccessException ex)
        {
            _log.Warning(ex, "API: Unauthorized CreateCheckoutOrder access");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCheckoutOrder");
            return StatusCode(500, "An error occurred while creating checkout order");
        }
    }

    /// <summary>
    /// Retrieves all orders in the system
    /// </summary>
    /// <returns>List of all orders</returns>
    /// <response code="200">Returns the list of orders</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Order.Read")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
    

    /// <summary>
    /// Retrieves a specific order by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the order</param>
    /// <returns>The order information</returns>
    /// <response code="200">Returns the order data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If order is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Order.Read")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Creates a new order in the system
    /// </summary>
    /// <param name="createDto">Order information for creation</param>
    /// <returns>The newly created order</returns>
    /// <response code="201">Returns the newly created order</response>
    /// <response code="400">If the order data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "Order.Write")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Updates an existing order's information
    /// </summary>
    /// <param name="id">The unique identifier of the order to update</param>
    /// <param name="updateDto">Updated order information</param>
    /// <returns>The updated order</returns>
    /// <response code="200">Returns the updated order</response>
    /// <response code="400">If the order data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If order is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "Order.Write")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Deletes an order from the system
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If order was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If order is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Order.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
