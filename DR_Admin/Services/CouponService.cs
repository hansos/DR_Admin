using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing coupons (stub implementation)
/// </summary>
public class CouponService : ICouponService
{
    public Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
    {
        return Task.FromResult(Enumerable.Empty<CouponDto>());
    }

    public Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
    {
        return Task.FromResult(Enumerable.Empty<CouponDto>());
    }

    public Task<CouponDto?> GetCouponByIdAsync(int id)
    {
        return Task.FromResult<CouponDto?>(null);
    }

    public Task<CouponDto?> GetCouponByCodeAsync(string code)
    {
        return Task.FromResult<CouponDto?>(null);
    }

    public Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
    {
        throw new NotImplementedException("CouponService.CreateCouponAsync not yet implemented");
    }

    public Task<CouponDto?> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
    {
        throw new NotImplementedException("CouponService.UpdateCouponAsync not yet implemented");
    }

    public Task<bool> DeleteCouponAsync(int id)
    {
        throw new NotImplementedException("CouponService.DeleteCouponAsync not yet implemented");
    }

    public Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto validateDto)
    {
        throw new NotImplementedException("CouponService.ValidateCouponAsync not yet implemented");
    }

    public Task<bool> RecordUsageAsync(int couponId, int customerId, int? orderId, decimal discountAmount)
    {
        throw new NotImplementedException("CouponService.RecordUsageAsync not yet implemented");
    }
}

