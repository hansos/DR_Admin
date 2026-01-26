using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages discount coupons
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CouponsController>();

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    /// <summary>
    /// Retrieves all coupons in the system
    /// </summary>
    /// <returns>List of all coupons</returns>
    /// <response code="200">Returns the list of coupons</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
    {
        try
        {
            _log.Information("API: GetAllCoupons called by user {User}", User.Identity?.Name);
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllCoupons");
            return StatusCode(500, "An error occurred while retrieving coupons");
        }
    }

    /// <summary>
    /// Retrieves all active coupons
    /// </summary>
    /// <returns>List of active coupons</returns>
    /// <response code="200">Returns the list of active coupons</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CouponDto>>> GetActiveCoupons()
    {
        try
        {
            _log.Information("API: GetActiveCoupons called by user {User}", User.Identity?.Name);
            var coupons = await _couponService.GetActiveCouponsAsync();
            return Ok(coupons);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveCoupons");
            return StatusCode(500, "An error occurred while retrieving active coupons");
        }
    }

    /// <summary>
    /// Retrieves a specific coupon by ID
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <returns>The coupon details</returns>
    /// <response code="200">Returns the coupon</response>
    /// <response code="404">If the coupon is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CouponDto>> GetCouponById(int id)
    {
        try
        {
            _log.Information("API: GetCouponById called for ID: {CouponId} by user {User}", id, User.Identity?.Name);
            var coupon = await _couponService.GetCouponByIdAsync(id);

            if (coupon == null)
            {
                _log.Warning("API: Coupon with ID {CouponId} not found", id);
                return NotFound($"Coupon with ID {id} not found");
            }

            return Ok(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCouponById for ID: {CouponId}", id);
            return StatusCode(500, "An error occurred while retrieving the coupon");
        }
    }

    /// <summary>
    /// Retrieves a coupon by code
    /// </summary>
    /// <param name="code">The coupon code</param>
    /// <returns>The coupon details</returns>
    /// <response code="200">Returns the coupon</response>
    /// <response code="404">If the coupon is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("code/{code}")]
    [Authorize]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CouponDto>> GetCouponByCode(string code)
    {
        try
        {
            _log.Information("API: GetCouponByCode called for code: {Code} by user {User}", code, User.Identity?.Name);
            var coupon = await _couponService.GetCouponByCodeAsync(code);

            if (coupon == null)
            {
                _log.Warning("API: Coupon with code {Code} not found", code);
                return NotFound($"Coupon with code {code} not found");
            }

            return Ok(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCouponByCode for code: {Code}", code);
            return StatusCode(500, "An error occurred while retrieving the coupon");
        }
    }

    /// <summary>
    /// Creates a new coupon
    /// </summary>
    /// <param name="createDto">The coupon creation data</param>
    /// <returns>The created coupon</returns>
    /// <response code="201">Returns the newly created coupon</response>
    /// <response code="400">If the coupon data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CreateCouponDto createDto)
    {
        try
        {
            _log.Information("API: CreateCoupon called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var coupon = await _couponService.CreateCouponAsync(createDto);
            
            _log.Information("API: Coupon created with ID: {CouponId}", coupon.Id);
            return CreatedAtAction(nameof(GetCouponById), new { id = coupon.Id }, coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCoupon");
            return StatusCode(500, "An error occurred while creating the coupon");
        }
    }

    /// <summary>
    /// Updates an existing coupon
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <param name="updateDto">The coupon update data</param>
    /// <returns>The updated coupon</returns>
    /// <response code="200">Returns the updated coupon</response>
    /// <response code="400">If the coupon data is invalid</response>
    /// <response code="404">If the coupon is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(int id, [FromBody] UpdateCouponDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCoupon called for ID: {CouponId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var coupon = await _couponService.UpdateCouponAsync(id, updateDto);

            if (coupon == null)
            {
                _log.Warning("API: Coupon with ID {CouponId} not found for update", id);
                return NotFound($"Coupon with ID {id} not found");
            }

            _log.Information("API: Coupon updated with ID: {CouponId}", id);
            return Ok(coupon);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCoupon for ID: {CouponId}", id);
            return StatusCode(500, "An error occurred while updating the coupon");
        }
    }

    /// <summary>
    /// Deletes a coupon (soft delete)
    /// </summary>
    /// <param name="id">The coupon ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the coupon was successfully deleted</response>
    /// <response code="404">If the coupon is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCoupon(int id)
    {
        try
        {
            _log.Information("API: DeleteCoupon called for ID: {CouponId} by user {User}", id, User.Identity?.Name);

            var result = await _couponService.DeleteCouponAsync(id);

            if (!result)
            {
                _log.Warning("API: Coupon with ID {CouponId} not found for deletion", id);
                return NotFound($"Coupon with ID {id} not found");
            }

            _log.Information("API: Coupon deleted with ID: {CouponId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCoupon for ID: {CouponId}", id);
            return StatusCode(500, "An error occurred while deleting the coupon");
        }
    }

    /// <summary>
    /// Validates a coupon for an order
    /// </summary>
    /// <param name="validateDto">The validation data</param>
    /// <returns>The validation result</returns>
    /// <response code="200">Returns the validation result</response>
    /// <response code="400">If the validation data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("validate")]
    [Authorize]
    [ProducesResponseType(typeof(CouponValidationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CouponValidationResultDto>> ValidateCoupon([FromBody] ValidateCouponDto validateDto)
    {
        try
        {
            _log.Information("API: ValidateCoupon called for code: {Code} by user {User}", validateDto.Code, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _couponService.ValidateCouponAsync(validateDto);
            
            _log.Information("API: Coupon validation result: {IsValid}", result.IsValid);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ValidateCoupon");
            return StatusCode(500, "An error occurred while validating the coupon");
        }
    }
}
