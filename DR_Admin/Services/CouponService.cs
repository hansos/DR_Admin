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

    /// <summary>
    /// Initializes a new instance of the <see cref="CouponService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public CouponService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all non-deleted coupons.
    /// </summary>
    /// <returns>A collection of coupons.</returns>
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

    /// <summary>
    /// Retrieves all currently active coupons within validity dates.
    /// </summary>
    /// <returns>A collection of active coupons.</returns>
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

    /// <summary>
    /// Retrieves a coupon by its identifier.
    /// </summary>
    /// <param name="id">Coupon identifier.</param>
    /// <returns>The coupon if found; otherwise <c>null</c>.</returns>
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

    /// <summary>
    /// Retrieves a coupon by its code.
    /// </summary>
    /// <param name="code">Coupon code.</param>
    /// <returns>The coupon if found; otherwise <c>null</c>.</returns>
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

    /// <summary>
    /// Creates a new coupon.
    /// </summary>
    /// <param name="createDto">Coupon creation payload.</param>
    /// <returns>The created coupon.</returns>
    public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
    {
        try
        {
            _log.Information("Creating new coupon with code: {Code}", createDto.Code);

            if (createDto.RecurrenceType == CouponRecurrenceType.RecurringYears
                && (!createDto.RecurringYears.HasValue || createDto.RecurringYears.Value <= 0))
            {
                throw new InvalidOperationException("Recurring years must be greater than zero for recurring coupons");
            }

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
                RecurrenceType = createDto.RecurrenceType,
                RecurringYears = createDto.RecurrenceType == CouponRecurrenceType.RecurringYears
                    ? createDto.RecurringYears
                    : null,
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

    /// <summary>
    /// Updates an existing coupon.
    /// </summary>
    /// <param name="id">Coupon identifier.</param>
    /// <param name="updateDto">Coupon update payload.</param>
    /// <returns>The updated coupon if found; otherwise <c>null</c>.</returns>
    public async Task<CouponDto?> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
    {
        try
        {
            _log.Information("Updating coupon with ID: {CouponId}", id);

            if (updateDto.RecurrenceType == CouponRecurrenceType.RecurringYears
                && (!updateDto.RecurringYears.HasValue || updateDto.RecurringYears.Value <= 0))
            {
                throw new InvalidOperationException("Recurring years must be greater than zero for recurring coupons");
            }

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
            coupon.RecurrenceType = updateDto.RecurrenceType;
            coupon.RecurringYears = updateDto.RecurrenceType == CouponRecurrenceType.RecurringYears
                ? updateDto.RecurringYears
                : null;
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

    /// <summary>
    /// Soft deletes a coupon.
    /// </summary>
    /// <param name="id">Coupon identifier.</param>
    /// <returns><c>true</c> when deleted; otherwise <c>false</c>.</returns>
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

    /// <summary>
    /// Validates coupon applicability for a customer and amount.
    /// </summary>
    /// <param name="validateDto">Coupon validation payload.</param>
    /// <returns>A validation result including discount amount when valid.</returns>
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

            var customerUsageDates = await _context.CouponUsages
                .Where(cu => cu.CouponId == coupon.Id && cu.CustomerId == validateDto.CustomerId)
                .OrderBy(cu => cu.UsedAt)
                .Select(cu => cu.UsedAt)
                .ToListAsync();

            if (coupon.RecurrenceType == CouponRecurrenceType.OneTime && customerUsageDates.Count > 0)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Coupon can only be used once per customer",
                    DiscountAmount = 0
                };
            }

            if (coupon.RecurrenceType == CouponRecurrenceType.RecurringYears)
            {
                if (!coupon.RecurringYears.HasValue || coupon.RecurringYears.Value <= 0)
                {
                    return new CouponValidationResultDto
                    {
                        IsValid = false,
                        Message = "Coupon recurrence configuration is invalid",
                        DiscountAmount = 0
                    };
                }

                if (customerUsageDates.Count > 0)
                {
                    var firstUsageDate = customerUsageDates[0];
                    var recurrenceEnd = firstUsageDate.AddYears(coupon.RecurringYears.Value);

                    if (now >= recurrenceEnd)
                    {
                        return new CouponValidationResultDto
                        {
                            IsValid = false,
                            Message = "Coupon recurrence period has ended",
                            DiscountAmount = 0
                        };
                    }

                    var cycleStart = firstUsageDate;
                    while (cycleStart.AddYears(1) <= now)
                    {
                        cycleStart = cycleStart.AddYears(1);
                    }

                    var cycleEnd = cycleStart.AddYears(1);
                    var alreadyUsedInCurrentCycle = customerUsageDates.Any(usedAt =>
                        usedAt >= cycleStart && usedAt < cycleEnd);

                    if (alreadyUsedInCurrentCycle)
                    {
                        return new CouponValidationResultDto
                        {
                            IsValid = false,
                            Message = "Coupon has already been used for the current yearly cycle",
                            DiscountAmount = 0
                        };
                    }
                }
            }

            if (coupon.MaxUsagesPerCustomer.HasValue)
            {
                var customerUsageCount = customerUsageDates.Count;

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
                    Message = $"Minimum invoice amount of {coupon.MinimumAmount.Value:C} required",
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

    /// <summary>
    /// Records coupon usage for a customer.
    /// </summary>
    /// <param name="couponId">Coupon identifier.</param>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="discountAmount">Applied discount amount.</param>
    /// <returns><c>true</c> when recorded; otherwise <c>false</c>.</returns>
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
            RecurrenceType = coupon.RecurrenceType,
            RecurringYears = coupon.RecurringYears,
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

