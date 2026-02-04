using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages postal codes and their geographic information
/// </summary>
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
    /// Retrieves all postal codes in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all postal codes or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of postal codes or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "GeographicalRead")]
    [ProducesResponseType(typeof(IEnumerable<PostalCodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<PostalCodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllPostalCodes([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllPostalCodes (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _postalCodeService.GetAllPostalCodesPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

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
    /// Retrieves all postal codes for a specific country
    /// </summary>
    /// <param name="countryCode">The country code (e.g., "US", "GB")</param>
    /// <returns>List of postal codes for the country</returns>
    /// <response code="200">Returns the list of postal codes</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("country/{countryCode}")]
    [ProducesResponseType(typeof(IEnumerable<PostalCodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves all postal codes for a specific city
    /// </summary>
    /// <param name="city">The city name</param>
    /// <returns>List of postal codes for the city</returns>
    /// <response code="200">Returns the list of postal codes</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("city/{city}")]
    [ProducesResponseType(typeof(IEnumerable<PostalCodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves a specific postal code by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the postal code</param>
    /// <returns>The postal code information</returns>
    /// <response code="200">Returns the postal code data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If postal code is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PostalCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// Retrieves a specific postal code by code and country
    /// </summary>
    /// <param name="code">The postal code</param>
    /// <param name="countryCode">The country code (e.g., "US", "GB")</param>
    /// <returns>The postal code information</returns>
    /// <response code="200">Returns the postal code data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If postal code is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{code}/country/{countryCode}")]
    [ProducesResponseType(typeof(PostalCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [Authorize(Policy = "GeographicalWrite")]
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
