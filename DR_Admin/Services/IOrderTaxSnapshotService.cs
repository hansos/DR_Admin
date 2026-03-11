using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing order tax snapshot operations.
/// </summary>
public interface IOrderTaxSnapshotService
{
    /// <summary>
    /// Retrieves all order tax snapshots.
    /// </summary>
    /// <returns>Collection of order tax snapshot DTOs.</returns>
    Task<IEnumerable<OrderTaxSnapshotDto>> GetAllOrderTaxSnapshotsAsync();

    /// <summary>
    /// Retrieves order tax snapshots by order identifier.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>Collection of order tax snapshot DTOs for the order.</returns>
    Task<IEnumerable<OrderTaxSnapshotDto>> GetOrderTaxSnapshotsByOrderIdAsync(int orderId);

    /// <summary>
    /// Retrieves an order tax snapshot by identifier.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>Order tax snapshot DTO if found; otherwise null.</returns>
    Task<OrderTaxSnapshotDto?> GetOrderTaxSnapshotByIdAsync(int id);

    /// <summary>
    /// Creates a new order tax snapshot.
    /// </summary>
    /// <param name="dto">Order tax snapshot create request.</param>
    /// <returns>Created order tax snapshot DTO.</returns>
    Task<OrderTaxSnapshotDto> CreateOrderTaxSnapshotAsync(CreateOrderTaxSnapshotDto dto);

    /// <summary>
    /// Updates an existing order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <param name="dto">Order tax snapshot update request.</param>
    /// <returns>Updated order tax snapshot DTO if found; otherwise null.</returns>
    Task<OrderTaxSnapshotDto?> UpdateOrderTaxSnapshotAsync(int id, UpdateOrderTaxSnapshotDto dto);

    /// <summary>
    /// Deletes an order tax snapshot.
    /// </summary>
    /// <param name="id">Order tax snapshot identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    Task<bool> DeleteOrderTaxSnapshotAsync(int id);
}
