using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages sales agents including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SalesAgentsController : ControllerBase
{
    private readonly ISalesAgentService _salesAgentService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SalesAgentsController>();

    public SalesAgentsController(ISalesAgentService salesAgentService)
    {
        _salesAgentService = salesAgentService;
    }

    /// <summary>
    /// Retrieves all sales agents in the system
    /// </summary>
    /// <returns>List of all sales agents</returns>
    /// <response code="200">Returns the list of sales agents</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "Admin.Only")]
    [ProducesResponseType(typeof(IEnumerable<SalesAgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SalesAgentDto>>> GetAllSalesAgents()
    {
        try
        {
            _log.Information("API: GetAllSalesAgents called by user {User}", User.Identity?.Name);
            
            var agents = await _salesAgentService.GetAllSalesAgentsAsync();
            return Ok(agents);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllSalesAgents");
            return StatusCode(500, "An error occurred while retrieving sales agents");
        }
    }

    /// <summary>
    /// Retrieves only active sales agents
    /// </summary>
    /// <returns>List of active sales agents</returns>
    /// <response code="200">Returns the list of active sales agents</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "SalesAgent.Read")]
    [ProducesResponseType(typeof(IEnumerable<SalesAgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SalesAgentDto>>> GetActiveSalesAgents()
    {
        try
        {
            _log.Information("API: GetActiveSalesAgents called by user {User}", User.Identity?.Name);
            
            var agents = await _salesAgentService.GetActiveSalesAgentsAsync();
            return Ok(agents);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveSalesAgents");
            return StatusCode(500, "An error occurred while retrieving active sales agents");
        }
    }

    /// <summary>
    /// Retrieves sales agents by reseller company
    /// </summary>
    /// <param name="resellerCompanyId">The reseller company ID</param>
    /// <returns>List of sales agents for the specified reseller company</returns>
    /// <response code="200">Returns the list of sales agents</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("by-company/{resellerCompanyId}")]
    [Authorize(Policy = "SalesAgent.Read")]
    [ProducesResponseType(typeof(IEnumerable<SalesAgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SalesAgentDto>>> GetSalesAgentsByCompany(int resellerCompanyId)
    {
        try
        {
            _log.Information("API: GetSalesAgentsByCompany called for company {ResellerCompanyId} by user {User}", 
                resellerCompanyId, User.Identity?.Name);
            
            var agents = await _salesAgentService.GetSalesAgentsByResellerCompanyAsync(resellerCompanyId);
            return Ok(agents);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSalesAgentsByCompany for company {ResellerCompanyId}", resellerCompanyId);
            return StatusCode(500, "An error occurred while retrieving sales agents");
        }
    }

    /// <summary>
    /// Retrieves a specific sales agent by ID
    /// </summary>
    /// <param name="id">The sales agent ID</param>
    /// <returns>The sales agent details</returns>
    /// <response code="200">Returns the sales agent</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sales agent is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "Admin.Only")]
    [ProducesResponseType(typeof(SalesAgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SalesAgentDto>> GetSalesAgentById(int id)
    {
        try
        {
            _log.Information("API: GetSalesAgentById called with ID {SalesAgentId} by user {User}", 
                id, User.Identity?.Name);
            
            var agent = await _salesAgentService.GetSalesAgentByIdAsync(id);
            
            if (agent == null)
            {
                return NotFound($"Sales agent with ID {id} not found");
            }
            
            return Ok(agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSalesAgentById for ID {SalesAgentId}", id);
            return StatusCode(500, "An error occurred while retrieving the sales agent");
        }
    }

    /// <summary>
    /// Creates a new sales agent
    /// </summary>
    /// <param name="createDto">The sales agent creation data</param>
    /// <returns>The created sales agent</returns>
    /// <response code="201">Returns the newly created sales agent</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "SalesAgent.Write")]
    [ProducesResponseType(typeof(SalesAgentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SalesAgentDto>> CreateSalesAgent([FromBody] CreateSalesAgentDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateSalesAgent called by user {User}", User.Identity?.Name);
            
            var agent = await _salesAgentService.CreateSalesAgentAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetSalesAgentById),
                new { id = agent.Id },
                agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateSalesAgent");
            return StatusCode(500, "An error occurred while creating the sales agent");
        }
    }

    /// <summary>
    /// Updates an existing sales agent
    /// </summary>
    /// <param name="id">The sales agent ID to update</param>
    /// <param name="updateDto">The updated sales agent data</param>
    /// <returns>The updated sales agent</returns>
    /// <response code="200">Returns the updated sales agent</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sales agent is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SalesAgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SalesAgentDto>> UpdateSalesAgent(int id, [FromBody] UpdateSalesAgentDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateSalesAgent called for ID {SalesAgentId} by user {User}", 
                id, User.Identity?.Name);
            
            var agent = await _salesAgentService.UpdateSalesAgentAsync(id, updateDto);
            
            if (agent == null)
            {
                return NotFound($"Sales agent with ID {id} not found");
            }
            
            return Ok(agent);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateSalesAgent for ID {SalesAgentId}", id);
            return StatusCode(500, "An error occurred while updating the sales agent");
        }
    }

    /// <summary>
    /// Deletes a sales agent
    /// </summary>
    /// <param name="id">The sales agent ID to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the sales agent was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If sales agent is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSalesAgent(int id)
    {
        try
        {
            _log.Information("API: DeleteSalesAgent called for ID {SalesAgentId} by user {User}", 
                id, User.Identity?.Name);
            
            var deleted = await _salesAgentService.DeleteSalesAgentAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Sales agent with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteSalesAgent for ID {SalesAgentId}", id);
            return StatusCode(500, "An error occurred while deleting the sales agent");
        }
    }
}
