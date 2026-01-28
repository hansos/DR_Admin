using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing coupons
/// </summary>
public class CouponService : ICouponService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CouponService>();

    public CouponService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
    {
        try
        {
            _log.Information("Fetching all coupons");

            var coupons = await _context.Coupons
                .AsNoTracking()
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

            var couponDtos = coupons.Select(MapToDto);

            _log.Information("Successfully fetched {Count} coupons", coupons.Count);
            return couponDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all coupons");
            throw;
        }
    }

    public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
    {
        try
        {
            _log.Information("Fetching active coupons");

            var now = DateTime.UtcNow;
            var coupons = await _context.Coupons
                .AsNoTracking()
                .Where(c => c.DeletedAt == null 
                    && c.IsActive 
                    && c.ValidFrom <= now 
                    && c.ValidUntil >= now)
                .ToListAsync();

            var couponDtos = coupons.Select(MapToDto);

            _log.Information("Successfully fetched {Count} active coupons", coupons.Count);
            return couponDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active coupons");
            throw;
        }
    }

    public async Task<CouponDto?> GetCouponByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching coupon with ID: {CouponId}", id);

            var coupon = await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

            if (coupon == null)
            {
                _log.Warning("Coupon with ID {CouponId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched coupon with ID: {CouponId}", id);
            return MapToDto(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching coupon with ID: {CouponId}", id);
            throw;
        }
    }

    public async Task<CouponDto?> GetCouponByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching coupon with code: {Code}", code);

            var coupon = await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code && c.DeletedAt == null);

            if (coupon == null)
            {
                _log.Warning("Coupon with code {Code} not found", code);
                return null;
            }

            _log.Information("Successfully fetched coupon with code: {Code}", code);
            return MapToDto(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching coupon with code: {Code}", code);
            throw;
        }
    }

    public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
    {
        try
        {
            _log.Information("Creating new coupon with code: {Code}", createDto.Code);

            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == createDto.Code && c.DeletedAt == null);

            if (existingCoupon != null)
            {
                _log.Error("Coupon with code {Code} already exists", createDto.Code);
                throw new InvalidOperationException($"Coupon with code '{createDto.Code}' already exists");
            }

            var coupon = new Coupon
            {
                Code = createDto.Code,
                Name = createDto.Name,
                Description = createDto.Description,
                Type = createDto.Type,
                Value = createDto.Value,
                AppliesTo = createDto.AppliesTo,
                MinimumAmount = createDto.MinimumAmount,
                MaximumDiscount = createDto.MaximumDiscount,
                ValidFrom = createDto.ValidFrom,
                ValidUntil = createDto.ValidUntil,
                MaxUsages = createDto.MaxUsages,
                MaxUsagesPerCustomer = createDto.MaxUsagesPerCustomer,
                IsActive = createDto.IsActive,
                AllowedServiceTypeIdsJson = createDto.AllowedServiceTypeIds != null 
                    ? JsonSerializer.Serialize(createDto.AllowedServiceTypeIds) 
                    : null,
                UsageCount = 0,
                InternalNotes = createDto.InternalNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created coupon with ID: {CouponId}", coupon.Id);
            return MapToDto(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating coupon with code: {Code}", createDto.Code);
            throw;
        }
    }

    public async Task<CouponDto?> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
    {
        try
        {
            _log.Information("Updating coupon with ID: {CouponId}", id);

            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null || coupon.DeletedAt != null)
            {
                _log.Warning("Coupon with ID {CouponId} not found for update", id);
                return null;
            }

            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == updateDto.Code && c.Id != id && c.DeletedAt == null);

            if (existingCoupon != null)
            {
                _log.Error("Coupon with code {Code} already exists", updateDto.Code);
                throw new InvalidOperationException($"Coupon with code '{updateDto.Code}' already exists");
            }

            coupon.Code = updateDto.Code;
            coupon.Name = updateDto.Name;
            coupon.Description = updateDto.Description;
            coupon.Type = updateDto.Type;
            coupon.Value = updateDto.Value;
            coupon.AppliesTo = updateDto.AppliesTo;
            coupon.MinimumAmount = updateDto.MinimumAmount;
            coupon.MaximumDiscount = updateDto.MaximumDiscount;
            coupon.ValidFrom = updateDto.ValidFrom;
            coupon.ValidUntil = updateDto.ValidUntil;
            coupon.MaxUsages = updateDto.MaxUsages;
            coupon.MaxUsagesPerCustomer = updateDto.MaxUsagesPerCustomer;
            coupon.IsActive = updateDto.IsActive;
            coupon.AllowedServiceTypeIdsJson = updateDto.AllowedServiceTypeIds != null 
                ? JsonSerializer.Serialize(updateDto.AllowedServiceTypeIds) 
                : null;
            coupon.InternalNotes = updateDto.InternalNotes;
            coupon.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated coupon with ID: {CouponId}", id);
            return MapToDto(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating coupon with ID: {CouponId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCouponAsync(int id)
    {
        try
        {
            _log.Information("Deleting coupon with ID: {CouponId}", id);

            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null || coupon.DeletedAt != null)
            {
                _log.Warning("Coupon with ID {CouponId} not found for deletion", id);
                return false;
            }

            coupon.DeletedAt = DateTime.UtcNow;
            coupon.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted coupon with ID: {CouponId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting coupon with ID: {CouponId}", id);
            throw;
        }
    }

    public async Task<CouponValidationResultDto> ValidateCouponAsync(ValidateCouponDto validateDto)
    {
        try
        {
            _log.Information("Validating coupon with code: {Code} for customer: {CustomerId}", 
                validateDto.Code, validateDto.CustomerId);

            var coupon = await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == validateDto.Code && c.DeletedAt == null);

            if (coupon == null)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Coupon code not found",
                    DiscountAmount = 0
                };
            }

            if (!coupon.IsActive)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Coupon is not active",
                    DiscountAmount = 0
                };
            }

            var now = DateTime.UtcNow;
            if (now < coupon.ValidFrom)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = $"Coupon is not valid until {coupon.ValidFrom:yyyy-MM-dd}",
                    DiscountAmount = 0
                };
            }

            if (now > coupon.ValidUntil)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Coupon has expired",
                    DiscountAmount = 0
                };
            }

            if (coupon.MaxUsages.HasValue && coupon.UsageCount >= coupon.MaxUsages.Value)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Coupon has reached maximum usage limit",
                    DiscountAmount = 0
                };
            }

            if (coupon.MaxUsagesPerCustomer.HasValue)
            {
                var customerUsageCount = await _context.CouponUsages
                    .CountAsync(cu => cu.CouponId == coupon.Id && cu.CustomerId == validateDto.CustomerId);

                if (customerUsageCount >= coupon.MaxUsagesPerCustomer.Value)
                {
                    return new CouponValidationResultDto
                    {
                        IsValid = false,
                        Message = "You have reached the maximum usage limit for this coupon",
                        DiscountAmount = 0
                    };
                }
            }

            if (coupon.MinimumAmount.HasValue && validateDto.TotalAmount < coupon.MinimumAmount.Value)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = $"Minimum order amount of {coupon.MinimumAmount.Value:C} required",
                    DiscountAmount = 0
                };
            }

            if (!string.IsNullOrEmpty(coupon.AllowedServiceTypeIdsJson))
            {
                var allowedServiceTypeIds = JsonSerializer.Deserialize<List<int>>(coupon.AllowedServiceTypeIdsJson);
                if (allowedServiceTypeIds != null && allowedServiceTypeIds.Any())
                {
                    var hasMatchingService = validateDto.ServiceTypeIds.Any(id => allowedServiceTypeIds.Contains(id));
                    if (!hasMatchingService)
                    {
                        return new CouponValidationResultDto
                        {
                            IsValid = false,
                            Message = "Coupon is not valid for the selected services",
                            DiscountAmount = 0
                        };
                    }
                }
            }

            var discountAmount = CalculateDiscount(coupon, validateDto.TotalAmount);

            _log.Information("Coupon validation successful for code: {Code}, discount: {Discount}", 
                validateDto.Code, discountAmount);

            return new CouponValidationResultDto
            {
                IsValid = true,
                Message = "Coupon is valid",
                DiscountAmount = discountAmount,
                Coupon = MapToDto(coupon)
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while validating coupon with code: {Code}", validateDto.Code);
            throw;
        }
    }

    public async Task<bool> RecordUsageAsync(int couponId, int customerId, int? orderId, decimal discountAmount)
    {
        try
        {
            _log.Information("Recording usage for coupon: {CouponId}, customer: {CustomerId}", 
                couponId, customerId);

            var coupon = await _context.Coupons.FindAsync(couponId);

            if (coupon == null || coupon.DeletedAt != null)
            {
                _log.Warning("Coupon with ID {CouponId} not found for usage recording", couponId);
                return false;
            }

            var couponUsage = new CouponUsage
            {
                CouponId = couponId,
                CustomerId = customerId,
                OrderId = orderId,
                DiscountAmount = discountAmount,
                UsedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CouponUsages.Add(couponUsage);

            coupon.UsageCount++;
            coupon.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully recorded usage for coupon: {CouponId}", couponId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while recording usage for coupon: {CouponId}", couponId);
            throw;
        }
    }

    private static decimal CalculateDiscount(Coupon coupon, decimal totalAmount)
    {
        decimal discount = coupon.Type == CouponType.Percentage
            ? totalAmount * (coupon.Value / 100m)
            : coupon.Value;

        if (coupon.MaximumDiscount.HasValue && discount > coupon.MaximumDiscount.Value)
        {
            discount = coupon.MaximumDiscount.Value;
        }

        if (discount > totalAmount)
        {
            discount = totalAmount;
        }

        return Math.Round(discount, 2);
    }

    private static CouponDto MapToDto(Coupon coupon)
    {
        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Name = coupon.Name,
            Description = coupon.Description,
            Type = coupon.Type,
            Value = coupon.Value,
            AppliesTo = coupon.AppliesTo,
            MinimumAmount = coupon.MinimumAmount,
            MaximumDiscount = coupon.MaximumDiscount,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MaxUsages = coupon.MaxUsages,
            MaxUsagesPerCustomer = coupon.MaxUsagesPerCustomer,
            IsActive = coupon.IsActive,
            UsageCount = coupon.UsageCount,
            CreatedAt = coupon.CreatedAt,
            UpdatedAt = coupon.UpdatedAt
        };
    }
}

