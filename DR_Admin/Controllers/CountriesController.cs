using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages countries and their information
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CountriesController>();

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Retrieves all countries in the system
    /// </summary>
    /// <returns>List of all countries</returns>
    /// <response code="200">Returns the list of countries</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
    {
        try
        {
            _log.Information("API: GetAllCountries called by user {User}", User.Identity?.Name);
            
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllCountries");
            return StatusCode(500, "An error occurred while retrieving countries");
        }
    }

    /// <summary>
    /// Retrieves only active countries
    /// </summary>
    /// <returns>List of active countries</returns>
    /// <response code="200">Returns the list of active countries</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetActiveCountries()
    {
        try
        {
            _log.Information("API: GetActiveCountries called by user {User}", User.Identity?.Name);
            
            var countries = await _countryService.GetActiveCountriesAsync();
            return Ok(countries);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveCountries");
            return StatusCode(500, "An error occurred while retrieving active countries");
        }
    }

    /// <summary>
    /// Retrieves a specific country by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the country</param>
    /// <returns>The country information</returns>
    /// <response code="200">Returns the country data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If country is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CountryDto>> GetCountryById(int id)
    {
        try
        {
            _log.Information("API: GetCountryById called for ID {CountryId} by user {User}", id, User.Identity?.Name);
            
            var country = await _countryService.GetCountryByIdAsync(id);

            if (country == null)
            {
                _log.Information("API: Country with ID {CountryId} not found", id);
                return NotFound($"Country with ID {id} not found");
            }

            return Ok(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCountryById for ID {CountryId}", id);
            return StatusCode(500, "An error occurred while retrieving the country");
        }
    }

    /// <summary>
    /// Retrieves a specific country by its country code
    /// </summary>
    /// <param name="code">The country code (e.g., "US", "GB")</param>
    /// <returns>The country information</returns>
    /// <response code="200">Returns the country data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If country is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CountryDto>> GetCountryByCode(string code)
    {
        try
        {
            _log.Information("API: GetCountryByCode called for code {CountryCode} by user {User}", code, User.Identity?.Name);
            
            var country = await _countryService.GetCountryByCodeAsync(code);

            if (country == null)
            {
                _log.Information("API: Country with code {CountryCode} not found", code);
                return NotFound($"Country with code {code} not found");
            }

            return Ok(country);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCountryByCode for code {CountryCode}", code);
            return StatusCode(500, "An error occurred while retrieving the country");
        }
    }

    /// <summary>
    /// Creates a new country in the system
    /// </summary>
    /// <param name="createDto">Country information for creation</param>
    /// <returns>The newly created country</returns>
    /// <response code="201">Returns the newly created country</response>
    /// <response code="400">If the country data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] CreateCountryDto createDto)
    {
        try
        {
            _log.Information("API: CreateCountry called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateCountry");
                return BadRequest(ModelState);
            }

            var country = await _countryService.CreateCountryAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetCountryById),
                new { id = country.Id },
                country);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateCountry");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCountry");
            return StatusCode(500, "An error occurred while creating the country");
        }
    }

    /// <summary>
    /// Update an existing country
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CountryDto>> UpdateCountry(int id, [FromBody] UpdateCountryDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateCountry called for ID {CountryId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateCountry");
                return BadRequest(ModelState);
            }

            var country = await _countryService.UpdateCountryAsync(id, updateDto);

            if (country == null)
            {
                _log.Information("API: Country with ID {CountryId} not found for update", id);
                return NotFound($"Country with ID {id} not found");
            }

            return Ok(country);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateCountry");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateCountry for ID {CountryId}", id);
            return StatusCode(500, "An error occurred while updating the country");
        }
    }

    /// <summary>
    /// Delete a country
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCountry(int id)
    {
        try
        {
            _log.Information("API: DeleteCountry called for ID {CountryId} by user {User}", id, User.Identity?.Name);

            var result = await _countryService.DeleteCountryAsync(id);

            if (!result)
            {
                _log.Information("API: Country with ID {CountryId} not found for deletion", id);
                return NotFound($"Country with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteCountry");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteCountry for ID {CountryId}", id);
            return StatusCode(500, "An error occurred while deleting the country");
        }
    }
}
