using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages currency exchange rates and currency conversions
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CurrenciesController>();

    public CurrenciesController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    /// <summary>
    /// Retrieves all currency exchange rates in the system
    /// </summary>
    /// <returns>List of all currency exchange rates</returns>
    /// <response code="200">Returns the list of currency exchange rates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<CurrencyExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetAllRates()
    {
        try
        {
            _log.Information("API: GetAllRates called by user {User}", User.Identity?.Name);
            
            var rates = await _currencyService.GetAllRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRates");
            return StatusCode(500, "An error occurred while retrieving currency exchange rates");
        }
    }

    /// <summary>
    /// Retrieves all active currency exchange rates
    /// </summary>
    /// <returns>List of active currency exchange rates</returns>
    /// <response code="200">Returns the list of active currency exchange rates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates/active")]
    [ProducesResponseType(typeof(IEnumerable<CurrencyExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetActiveRates()
    {
        try
        {
            _log.Information("API: GetActiveRates called by user {User}", User.Identity?.Name);
            
            var rates = await _currencyService.GetActiveRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveRates");
            return StatusCode(500, "An error occurred while retrieving active currency exchange rates");
        }
    }

    /// <summary>
    /// Retrieves a specific currency exchange rate by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate</param>
    /// <returns>The currency exchange rate information</returns>
    /// <response code="200">Returns the currency exchange rate data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="404">If exchange rate is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates/{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(CurrencyExchangeRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrencyExchangeRateDto>> GetRateById(int id)
    {
        try
        {
            _log.Information("API: GetRateById called for ID {RateId} by user {User}", id, User.Identity?.Name);
            
            var rate = await _currencyService.GetRateByIdAsync(id);

            if (rate == null)
            {
                _log.Information("API: Currency exchange rate with ID {RateId} not found", id);
                return NotFound($"Currency exchange rate with ID {id} not found");
            }

            return Ok(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRateById for ID {RateId}", id);
            return StatusCode(500, "An error occurred while retrieving the currency exchange rate");
        }
    }

    /// <summary>
    /// Retrieves the current exchange rate between two currencies
    /// </summary>
    /// <param name="from">The source currency code (ISO 4217, e.g., EUR)</param>
    /// <param name="to">The target currency code (ISO 4217, e.g., USD)</param>
    /// <param name="date">Optional date for the exchange rate (defaults to current date)</param>
    /// <returns>The exchange rate information</returns>
    /// <response code="200">Returns the exchange rate data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If exchange rate is not found for the currency pair</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates/exchange")]
    [ProducesResponseType(typeof(CurrencyExchangeRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrencyExchangeRateDto>> GetExchangeRate([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime? date = null)
    {
        try
        {
            _log.Information("API: GetExchangeRate called for {From} to {To} by user {User}", from, to, User.Identity?.Name);
            
            var rate = await _currencyService.GetExchangeRateAsync(from, to, date);

            if (rate == null)
            {
                _log.Information("API: Exchange rate not found from {From} to {To}", from, to);
                return NotFound($"Exchange rate not found from {from} to {to}");
            }

            return Ok(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetExchangeRate from {From} to {To}", from, to);
            return StatusCode(500, "An error occurred while retrieving the exchange rate");
        }
    }

    /// <summary>
    /// Retrieves all exchange rates for a specific currency pair
    /// </summary>
    /// <param name="from">The source currency code (ISO 4217, e.g., EUR)</param>
    /// <param name="to">The target currency code (ISO 4217, e.g., USD)</param>
    /// <returns>List of exchange rates for the currency pair</returns>
    /// <response code="200">Returns the list of exchange rates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates/pair")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<CurrencyExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetRatesForCurrencyPair([FromQuery] string from, [FromQuery] string to)
    {
        try
        {
            _log.Information("API: GetRatesForCurrencyPair called for {From} to {To} by user {User}", from, to, User.Identity?.Name);
            
            var rates = await _currencyService.GetRatesForCurrencyPairAsync(from, to);
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRatesForCurrencyPair from {From} to {To}", from, to);
            return StatusCode(500, "An error occurred while retrieving exchange rates for the currency pair");
        }
    }

    /// <summary>
    /// Creates a new currency exchange rate
    /// </summary>
    /// <param name="createDto">Currency exchange rate information for creation</param>
    /// <returns>The newly created currency exchange rate</returns>
    /// <response code="201">Returns the newly created currency exchange rate</response>
    /// <response code="400">If the exchange rate data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("rates")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(CurrencyExchangeRateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrencyExchangeRateDto>> CreateRate([FromBody] CreateCurrencyExchangeRateDto createDto)
    {
        try
        {
            _log.Information("API: CreateRate called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRate");
                return BadRequest(ModelState);
            }

            var rate = await _currencyService.CreateRateAsync(createDto);
            
            _log.Information("API: Currency exchange rate created successfully with ID {RateId}", rate.Id);
            return CreatedAtAction(nameof(GetRateById), new { id = rate.Id }, rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRate");
            return StatusCode(500, "An error occurred while creating the currency exchange rate");
        }
    }

    /// <summary>
    /// Updates an existing currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to update</param>
    /// <param name="updateDto">Updated exchange rate information</param>
    /// <returns>The updated currency exchange rate</returns>
    /// <response code="200">Returns the updated currency exchange rate</response>
    /// <response code="400">If the exchange rate data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="404">If exchange rate is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("rates/{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(CurrencyExchangeRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrencyExchangeRateDto>> UpdateRate(int id, [FromBody] UpdateCurrencyExchangeRateDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRate called for ID {RateId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRate");
                return BadRequest(ModelState);
            }

            var rate = await _currencyService.UpdateRateAsync(id, updateDto);

            if (rate == null)
            {
                _log.Information("API: Currency exchange rate with ID {RateId} not found for update", id);
                return NotFound($"Currency exchange rate with ID {id} not found");
            }

            _log.Information("API: Currency exchange rate with ID {RateId} updated successfully", id);
            return Ok(rate);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRate for ID {RateId}", id);
            return StatusCode(500, "An error occurred while updating the currency exchange rate");
        }
    }

    /// <summary>
    /// Deletes a currency exchange rate
    /// </summary>
    /// <param name="id">The unique identifier of the exchange rate to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the exchange rate was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin)</response>
    /// <response code="404">If exchange rate is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("rates/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRate(int id)
    {
        try
        {
            _log.Information("API: DeleteRate called for ID {RateId} by user {User}", id, User.Identity?.Name);

            var result = await _currencyService.DeleteRateAsync(id);

            if (!result)
            {
                _log.Information("API: Currency exchange rate with ID {RateId} not found for deletion", id);
                return NotFound($"Currency exchange rate with ID {id} not found");
            }

            _log.Information("API: Currency exchange rate with ID {RateId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRate for ID {RateId}", id);
            return StatusCode(500, "An error occurred while deleting the currency exchange rate");
        }
    }

    /// <summary>
    /// Converts an amount from one currency to another
    /// </summary>
    /// <param name="convertDto">Currency conversion request details</param>
    /// <returns>The conversion result including the converted amount and exchange rate used</returns>
    /// <response code="200">Returns the conversion result</response>
    /// <response code="400">If the conversion data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If exchange rate is not found for the currency pair</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(CurrencyConversionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrencyConversionResultDto>> ConvertCurrency([FromBody] ConvertCurrencyDto convertDto)
    {
        try
        {
            _log.Information("API: ConvertCurrency called for {Amount} {From} to {To} by user {User}", 
                convertDto.Amount, convertDto.FromCurrency, convertDto.ToCurrency, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for ConvertCurrency");
                return BadRequest(ModelState);
            }

            var result = await _currencyService.ConvertCurrencyAsync(
                convertDto.Amount, 
                convertDto.FromCurrency, 
                convertDto.ToCurrency, 
                convertDto.RateDate);

            _log.Information("API: Successfully converted {Amount} {From} to {ConvertedAmount} {To}", 
                convertDto.Amount, convertDto.FromCurrency, result.ConvertedAmount, convertDto.ToCurrency);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Exchange rate not found for conversion");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ConvertCurrency");
            return StatusCode(500, "An error occurred while converting currency");
        }
    }

    /// <summary>
    /// Deactivates all expired currency exchange rates
    /// </summary>
    /// <returns>The number of rates that were deactivated</returns>
    /// <response code="200">Returns the count of deactivated rates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role (Admin, Finance)</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("rates/deactivate-expired")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeactivateExpiredRates()
    {
        try
        {
            _log.Information("API: DeactivateExpiredRates called by user {User}", User.Identity?.Name);
            
            var count = await _currencyService.DeactivateExpiredRatesAsync();
            
            _log.Information("API: Deactivated {Count} expired currency exchange rates", count);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeactivateExpiredRates");
            return StatusCode(500, "An error occurred while deactivating expired currency exchange rates");
        }
    }
}
