using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing coupons
/// </summary>
public interface ICouponService
{
    /// <summary>
    /// Retrieves all coupons
    /// </summary>
    /// <returns>A collection of coupon DTOs</returns>
    Task<IEnumerable<CouponDto>> GetAllCouponsAsync();

    /// <summary>
    /// Retrieves active coupons
    /// </summary>
    /// <returns>A collection of active coupon DTOs</returns>
    Task<IEnumerable<CouponDto>> GetActiveCouponsAsync();

    /// <summary>
    /// Retrieves a coupon by ID
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <returns>The coupon DTO if found, otherwise null</returns>
    Task<CouponDto?> GetCouponByIdAsync(int id);

    /// <summary>
    /// Retrieves a coupon by code
    /// </summary>
    /// <param name="code">The coupon code</param>
    /// <returns>The coupon DTO if found, otherwise null</returns>
    Task<CouponDto?> GetCouponByCodeAsync(string code);

    /// <summary>
    /// Creates a new coupon
    /// </summary>
    /// <param name="createDto">The coupon creation data</param>
    /// <returns>The created coupon DTO</returns>
    Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto);

    /// <summary>
    /// Updates an existing coupon
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <param name="updateDto">The coupon update data</param>
    /// <returns>The updated coupon DTO if successful, otherwise null</returns>
    Task<CouponDto?> UpdateCouponAsync(int id, UpdateCouponDto updateDto);

    /// <summary>
    /// Deletes a coupon (soft delete)
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> DeleteCouponAsync(int id);

    /// <summary>
    /// Validates a coupon for an order
    /// </summary>
    /// <param name="validateDto">The validation data</param>
    /// <returns>The validation result</returns>
    Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto validateDto);

    /// <summary>
    /// Records a coupon usage
    /// </summary>
    /// <param name="couponId">The coupon ID</param>
    /// <param name="customerId">The customer ID</param>
    /// <param name="orderId">The order ID</param>
    /// <param name="discountAmount">The discount amount applied</param>
    /// <returns>True if successful</returns>
    Task<bool> RecordUsageAsync(int couponId, int customerId, int? orderId, decimal discountAmount);
}
