using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

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
    /// Get all countries
    /// </summary>
    [HttpGet]
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
    /// Get active countries only
    /// </summary>
    [HttpGet("active")]
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
    /// Get country by ID
    /// </summary>
    [HttpGet("{id}")]
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
    /// Get country by code
    /// </summary>
    [HttpGet("code/{code}")]
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
    /// Create a new country
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
