using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages immutable order tax snapshots.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrderTaxSnapshotsController : ControllerBase
{
    private readonly IOrderTaxSnapshotService _orderTaxSnapshotService;

    public OrderTaxSnapshotsController(IOrderTaxSnapshotService orderTaxSnapshotService)
    {
        _orderTaxSnapshotService = orderTaxSnapshotService;
    }

    /// <summary>
    /// Retrieves all order tax snapshots.
    /// </summary>
    /// <returns>List of order tax snapshots.</returns>
    [HttpGet]
    [Authorize(Policy = "OrderTaxSnapshot.Read")]
    [ProducesResponseType(typeof(IEnumerable<OrderTaxSnapshotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<OrderTaxSnapshotDto>>> GetAllOrderTaxSnapshots()
    {
        var result = await _orderTaxSnapshotService.GetAllOrderTaxSnapshotsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves tax snapshots for a specific order.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>List of tax snapshots for the order.</returns>
    [HttpGet("order/{orderId}")]
    [Authorize(Policy = "OrderTaxSnapshot.Read")]
    [ProducesResponseType(typeof(IEnumerable<OrderTaxSnapshotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<OrderTaxSnapshotDto>>> GetOrderTaxSnapshotsByOrderId(int orderId)
    {
        var result = await _orderTaxSnapshotService.GetOrderTaxSnapshotsByOrderIdAsync(orderId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves an order tax snapshot by identifier.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>Order tax snapshot details.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "OrderTaxSnapshot.Read")]
    [ProducesResponseType(typeof(OrderTaxSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderTaxSnapshotDto>> GetOrderTaxSnapshotById(int id)
    {
        var result = await _orderTaxSnapshotService.GetOrderTaxSnapshotByIdAsync(id);
        if (result == null)
        {
            return NotFound($"Order tax snapshot with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates an order tax snapshot.
    /// </summary>
    /// <param name="dto">Order tax snapshot create payload.</param>
    /// <returns>Created order tax snapshot.</returns>
    [HttpPost]
    [Authorize(Policy = "OrderTaxSnapshot.Write")]
    [ProducesResponseType(typeof(OrderTaxSnapshotDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderTaxSnapshotDto>> CreateOrderTaxSnapshot([FromBody] CreateOrderTaxSnapshotDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _orderTaxSnapshotService.CreateOrderTaxSnapshotAsync(dto);
        return CreatedAtAction(nameof(GetOrderTaxSnapshotById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <param name="dto">Order tax snapshot update payload.</param>
    /// <returns>Updated order tax snapshot.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "OrderTaxSnapshot.Write")]
    [ProducesResponseType(typeof(OrderTaxSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderTaxSnapshotDto>> UpdateOrderTaxSnapshot(int id, [FromBody] UpdateOrderTaxSnapshotDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _orderTaxSnapshotService.UpdateOrderTaxSnapshotAsync(id, dto);
        if (result == null)
        {
            return NotFound($"Order tax snapshot with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes an order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "OrderTaxSnapshot.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteOrderTaxSnapshot(int id)
    {
        var deleted = await _orderTaxSnapshotService.DeleteOrderTaxSnapshotAsync(id);
        if (!deleted)
        {
            return NotFound($"Order tax snapshot with ID {id} not found");
        }

        return NoContent();
    }
}
