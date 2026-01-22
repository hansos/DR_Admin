using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PostalCodesController : ControllerBase
{
    private readonly IPostalCodeService _postalCodeService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PostalCodesController>();

    public PostalCodesController(IPostalCodeService postalCodeService)
    {
        _postalCodeService = postalCodeService;
    }

    /// <summary>
    /// Get all postal codes
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<IEnumerable<PostalCodeDto>>> GetAllPostalCodes()
    {
        try
        {
            _log.Information("API: GetAllPostalCodes called by user {User}", User.Identity?.Name);
            
            var postalCodes = await _postalCodeService.GetAllPostalCodesAsync();
            return Ok(postalCodes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllPostalCodes");
            return StatusCode(500, "An error occurred while retrieving postal codes");
        }
    }

    /// <summary>
    /// Get postal codes by country
    /// </summary>
    [HttpGet("country/{countryCode}")]
    public async Task<ActionResult<IEnumerable<PostalCodeDto>>> GetPostalCodesByCountry(string countryCode)
    {
        try
        {
            _log.Information("API: GetPostalCodesByCountry called for country {CountryCode} by user {User}", 
                countryCode, User.Identity?.Name);
            
            var postalCodes = await _postalCodeService.GetPostalCodesByCountryAsync(countryCode);
            return Ok(postalCodes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPostalCodesByCountry for country {CountryCode}", countryCode);
            return StatusCode(500, "An error occurred while retrieving postal codes");
        }
    }

    /// <summary>
    /// Get postal codes by city
    /// </summary>
    [HttpGet("city/{city}")]
    public async Task<ActionResult<IEnumerable<PostalCodeDto>>> GetPostalCodesByCity(string city)
    {
        try
        {
            _log.Information("API: GetPostalCodesByCity called for city {City} by user {User}", 
                city, User.Identity?.Name);
            
            var postalCodes = await _postalCodeService.GetPostalCodesByCityAsync(city);
            return Ok(postalCodes);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPostalCodesByCity for city {City}", city);
            return StatusCode(500, "An error occurred while retrieving postal codes");
        }
    }

    /// <summary>
    /// Get postal code by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostalCodeDto>> GetPostalCodeById(int id)
    {
        try
        {
            _log.Information("API: GetPostalCodeById called for ID {PostalCodeId} by user {User}", 
                id, User.Identity?.Name);
            
            var postalCode = await _postalCodeService.GetPostalCodeByIdAsync(id);

            if (postalCode == null)
            {
                _log.Information("API: Postal code with ID {PostalCodeId} not found", id);
                return NotFound($"Postal code with ID {id} not found");
            }

            return Ok(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPostalCodeById for ID {PostalCodeId}", id);
            return StatusCode(500, "An error occurred while retrieving the postal code");
        }
    }

    /// <summary>
    /// Get postal code by code and country
    /// </summary>
    [HttpGet("{code}/country/{countryCode}")]
    public async Task<ActionResult<PostalCodeDto>> GetPostalCodeByCodeAndCountry(string code, string countryCode)
    {
        try
        {
            _log.Information("API: GetPostalCodeByCodeAndCountry called for code {Code} and country {CountryCode} by user {User}", 
                code, countryCode, User.Identity?.Name);
            
            var postalCode = await _postalCodeService.GetPostalCodeByCodeAndCountryAsync(code, countryCode);

            if (postalCode == null)
            {
                _log.Information("API: Postal code {Code} for country {CountryCode} not found", code, countryCode);
                return NotFound($"Postal code {code} for country {countryCode} not found");
            }

            return Ok(postalCode);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetPostalCodeByCodeAndCountry for code {Code} and country {CountryCode}", 
                code, countryCode);
            return StatusCode(500, "An error occurred while retrieving the postal code");
        }
    }

    /// <summary>
    /// Create a new postal code
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PostalCodeDto>> CreatePostalCode([FromBody] CreatePostalCodeDto createDto)
    {
        try
        {
            _log.Information("API: CreatePostalCode called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreatePostalCode");
                return BadRequest(ModelState);
            }

            var postalCode = await _postalCodeService.CreatePostalCodeAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetPostalCodeById),
                new { id = postalCode.Id },
                postalCode);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreatePostalCode");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreatePostalCode");
            return StatusCode(500, "An error occurred while creating the postal code");
        }
    }

    /// <summary>
    /// Update an existing postal code
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PostalCodeDto>> UpdatePostalCode(int id, [FromBody] UpdatePostalCodeDto updateDto)
    {
        try
        {
            _log.Information("API: UpdatePostalCode called for ID {PostalCodeId} by user {User}", 
                id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdatePostalCode");
                return BadRequest(ModelState);
            }

            var postalCode = await _postalCodeService.UpdatePostalCodeAsync(id, updateDto);

            if (postalCode == null)
            {
                _log.Information("API: Postal code with ID {PostalCodeId} not found for update", id);
                return NotFound($"Postal code with ID {id} not found");
            }

            return Ok(postalCode);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdatePostalCode");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdatePostalCode for ID {PostalCodeId}", id);
            return StatusCode(500, "An error occurred while updating the postal code");
        }
    }

    /// <summary>
    /// Delete a postal code
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePostalCode(int id)
    {
        try
        {
            _log.Information("API: DeletePostalCode called for ID {PostalCodeId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _postalCodeService.DeletePostalCodeAsync(id);

            if (!result)
            {
                _log.Information("API: Postal code with ID {PostalCodeId} not found for deletion", id);
                return NotFound($"Postal code with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeletePostalCode for ID {PostalCodeId}", id);
            return StatusCode(500, "An error occurred while deleting the postal code");
        }
    }
}
