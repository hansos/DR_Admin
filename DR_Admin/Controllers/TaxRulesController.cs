using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages tax rules for automatic tax calculation
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaxRulesController : ControllerBase
{
    private readonly ITaxService _taxService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TaxRulesController>();

    public TaxRulesController(ITaxService taxService)
    {
        _taxService = taxService;
    }

    /// <summary>
    /// Retrieves all tax rules in the system
    /// </summary>
    /// <returns>List of all tax rules</returns>
    /// <response code="200">Returns the list of tax rules</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<TaxRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TaxRuleDto>>> GetAllTaxRules()
    {
        try
        {
            _log.Information("API: GetAllTaxRules called by user {User}", User.Identity?.Name);
            var taxRules = await _taxService.GetAllTaxRulesAsync();
            return Ok(taxRules);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTaxRules");
            return StatusCode(500, "An error occurred while retrieving tax rules");
        }
    }

    /// <summary>
    /// Retrieves all active tax rules
    /// </summary>
    /// <returns>List of active tax rules</returns>
    /// <response code="200">Returns the list of active tax rules</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<TaxRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TaxRuleDto>>> GetActiveTaxRules()
    {
        try
        {
            _log.Information("API: GetActiveTaxRules called by user {User}", User.Identity?.Name);
            var taxRules = await _taxService.GetActiveTaxRulesAsync();
            return Ok(taxRules);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveTaxRules");
            return StatusCode(500, "An error occurred while retrieving active tax rules");
        }
    }

    /// <summary>
    /// Retrieves a specific tax rule by ID
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <returns>The tax rule details</returns>
    /// <response code="200">Returns the tax rule</response>
    /// <response code="404">If the tax rule is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TaxRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaxRuleDto>> GetTaxRuleById(int id)
    {
        try
        {
            _log.Information("API: GetTaxRuleById called for ID: {TaxRuleId} by user {User}", id, User.Identity?.Name);
            var taxRule = await _taxService.GetTaxRuleByIdAsync(id);

            if (taxRule == null)
            {
                _log.Warning("API: Tax rule with ID {TaxRuleId} not found", id);
                return NotFound($"Tax rule with ID {id} not found");
            }

            return Ok(taxRule);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTaxRuleById for ID: {TaxRuleId}", id);
            return StatusCode(500, "An error occurred while retrieving the tax rule");
        }
    }

    /// <summary>
    /// Retrieves tax rules by location
    /// </summary>
    /// <param name="countryCode">The country code</param>
    /// <param name="stateCode">The state code (optional)</param>
    /// <returns>List of applicable tax rules</returns>
    /// <response code="200">Returns the list of tax rules</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("location/{countryCode}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<TaxRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TaxRuleDto>>> GetTaxRulesByLocation(string countryCode, [FromQuery] string? stateCode = null)
    {
        try
        {
            _log.Information("API: GetTaxRulesByLocation called for country: {Country}, state: {State} by user {User}", 
                countryCode, stateCode, User.Identity?.Name);
            var taxRules = await _taxService.GetTaxRulesByLocationAsync(countryCode, stateCode);
            return Ok(taxRules);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTaxRulesByLocation for country: {Country}", countryCode);
            return StatusCode(500, "An error occurred while retrieving tax rules");
        }
    }

    /// <summary>
    /// Creates a new tax rule
    /// </summary>
    /// <param name="createDto">The tax rule creation data</param>
    /// <returns>The created tax rule</returns>
    /// <response code="201">Returns the newly created tax rule</response>
    /// <response code="400">If the tax rule data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TaxRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaxRuleDto>> CreateTaxRule([FromBody] CreateTaxRuleDto createDto)
    {
        try
        {
            _log.Information("API: CreateTaxRule called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxRule = await _taxService.CreateTaxRuleAsync(createDto);
            
            _log.Information("API: Tax rule created with ID: {TaxRuleId}", taxRule.Id);
            return CreatedAtAction(nameof(GetTaxRuleById), new { id = taxRule.Id }, taxRule);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateTaxRule");
            return StatusCode(500, "An error occurred while creating the tax rule");
        }
    }

    /// <summary>
    /// Updates an existing tax rule
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <param name="updateDto">The tax rule update data</param>
    /// <returns>The updated tax rule</returns>
    /// <response code="200">Returns the updated tax rule</response>
    /// <response code="400">If the tax rule data is invalid</response>
    /// <response code="404">If the tax rule is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TaxRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaxRuleDto>> UpdateTaxRule(int id, [FromBody] UpdateTaxRuleDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateTaxRule called for ID: {TaxRuleId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxRule = await _taxService.UpdateTaxRuleAsync(id, updateDto);

            if (taxRule == null)
            {
                _log.Warning("API: Tax rule with ID {TaxRuleId} not found for update", id);
                return NotFound($"Tax rule with ID {id} not found");
            }

            _log.Information("API: Tax rule updated with ID: {TaxRuleId}", id);
            return Ok(taxRule);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateTaxRule for ID: {TaxRuleId}", id);
            return StatusCode(500, "An error occurred while updating the tax rule");
        }
    }

    /// <summary>
    /// Deletes a tax rule (soft delete)
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the tax rule was successfully deleted</response>
    /// <response code="404">If the tax rule is not found</response>
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
    public async Task<ActionResult> DeleteTaxRule(int id)
    {
        try
        {
            _log.Information("API: DeleteTaxRule called for ID: {TaxRuleId} by user {User}", id, User.Identity?.Name);

            var result = await _taxService.DeleteTaxRuleAsync(id);

            if (!result)
            {
                _log.Warning("API: Tax rule with ID {TaxRuleId} not found for deletion", id);
                return NotFound($"Tax rule with ID {id} not found");
            }

            _log.Information("API: Tax rule deleted with ID: {TaxRuleId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteTaxRule for ID: {TaxRuleId}", id);
            return StatusCode(500, "An error occurred while deleting the tax rule");
        }
    }

    /// <summary>
    /// Calculates tax for a customer and amount
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The taxable amount</param>
    /// <param name="isSetupFee">Whether this is a setup fee</param>
    /// <returns>The calculated tax amount, rate, and name</returns>
    /// <response code="200">Returns the tax calculation</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("calculate")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CalculateTax([FromQuery] int customerId, [FromQuery] decimal amount, [FromQuery] bool isSetupFee = false)
    {
        try
        {
            _log.Information("API: CalculateTax called for customer: {CustomerId}, amount: {Amount} by user {User}", 
                customerId, amount, User.Identity?.Name);

            var (taxAmount, taxRate, taxName) = await _taxService.CalculateTaxAsync(customerId, amount, isSetupFee);
            
            _log.Information("API: Tax calculated: {TaxAmount} ({TaxRate})", taxAmount, taxRate);
            return Ok(new { taxAmount, taxRate, taxName });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CalculateTax for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while calculating tax");
        }
    }

    /// <summary>
    /// Validates a VAT number (EU VIES check)
    /// </summary>
    /// <param name="vatNumber">The VAT number to validate</param>
    /// <param name="countryCode">The country code</param>
    /// <returns>Validation result</returns>
    /// <response code="200">Returns the validation result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("validate-vat")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ValidateVatNumber([FromQuery] string vatNumber, [FromQuery] string countryCode)
    {
        try
        {
            _log.Information("API: ValidateVatNumber called for VAT: {VatNumber}, country: {Country} by user {User}", 
                vatNumber, countryCode, User.Identity?.Name);

            var isValid = await _taxService.ValidateVatNumberAsync(vatNumber, countryCode);
            
            _log.Information("API: VAT validation result: {IsValid}", isValid);
            return Ok(new { isValid, vatNumber, countryCode });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ValidateVatNumber");
            return StatusCode(500, "An error occurred while validating the VAT number");
        }
    }
}
