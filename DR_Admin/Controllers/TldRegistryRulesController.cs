using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages TLD registry policy rules.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TldRegistryRulesController : ControllerBase
{
    private readonly ITldRegistryRuleService _service;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldRegistryRulesController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="TldRegistryRulesController"/> class.
    /// </summary>
    /// <param name="service">TLD registry rule service.</param>
    public TldRegistryRulesController(ITldRegistryRuleService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all TLD registry rules.
    /// </summary>
    /// <returns>List of TLD registry rules.</returns>
    [HttpGet]
    [Authorize(Policy = "Tld.Read")]
    [ProducesResponseType(typeof(IEnumerable<TldRegistryRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TldRegistryRuleDto>>> GetAll()
    {
        try
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.GetAll");
            return StatusCode(500, "An error occurred while retrieving TLD registry rules");
        }
    }

    /// <summary>
    /// Retrieves active TLD registry rules.
    /// </summary>
    /// <returns>List of active TLD registry rules.</returns>
    [HttpGet("active")]
    [Authorize(Policy = "Tld.Read")]
    [ProducesResponseType(typeof(IEnumerable<TldRegistryRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TldRegistryRuleDto>>> GetActive()
    {
        try
        {
            var items = await _service.GetActiveAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.GetActive");
            return StatusCode(500, "An error occurred while retrieving active TLD registry rules");
        }
    }

    /// <summary>
    /// Retrieves rules for a specific TLD.
    /// </summary>
    /// <param name="tldId">The TLD identifier.</param>
    /// <returns>List of rules for the selected TLD.</returns>
    [HttpGet("tld/{tldId:int}")]
    [Authorize(Policy = "Tld.Read")]
    [ProducesResponseType(typeof(IEnumerable<TldRegistryRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TldRegistryRuleDto>>> GetByTldId(int tldId)
    {
        try
        {
            var items = await _service.GetByTldIdAsync(tldId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.GetByTldId for TldId {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving TLD registry rules for the selected TLD");
        }
    }

    /// <summary>
    /// Retrieves a TLD registry rule by identifier.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <returns>The selected TLD registry rule.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "Tld.Read")]
    [ProducesResponseType(typeof(TldRegistryRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldRegistryRuleDto>> GetById(int id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"TLD registry rule with ID {id} not found");
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.GetById for RuleId {RuleId}", id);
            return StatusCode(500, "An error occurred while retrieving the TLD registry rule");
        }
    }

    /// <summary>
    /// Creates a new TLD registry rule.
    /// </summary>
    /// <param name="createDto">The create payload.</param>
    /// <returns>The created TLD registry rule.</returns>
    [HttpPost]
    [Authorize(Policy = "Tld.Write")]
    [ProducesResponseType(typeof(TldRegistryRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldRegistryRuleDto>> Create([FromBody] CreateTldRegistryRuleDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in TldRegistryRules.Create");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.Create");
            return StatusCode(500, "An error occurred while creating the TLD registry rule");
        }
    }

    /// <summary>
    /// Updates an existing TLD registry rule.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <param name="updateDto">The update payload.</param>
    /// <returns>The updated TLD registry rule.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "Tld.Write")]
    [ProducesResponseType(typeof(TldRegistryRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldRegistryRuleDto>> Update(int id, [FromBody] UpdateTldRegistryRuleDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _service.UpdateAsync(id, updateDto);
            if (item == null)
            {
                return NotFound($"TLD registry rule with ID {id} not found");
            }

            return Ok(item);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in TldRegistryRules.Update for RuleId {RuleId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.Update for RuleId {RuleId}", id);
            return StatusCode(500, "An error occurred while updating the TLD registry rule");
        }
    }

    /// <summary>
    /// Deletes a TLD registry rule.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Tld.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"TLD registry rule with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in TldRegistryRules.Delete for RuleId {RuleId}", id);
            return StatusCode(500, "An error occurred while deleting the TLD registry rule");
        }
    }
}
