using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages report templates for FastReport and other reporting engines
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportTemplatesController : ControllerBase
{
    private readonly IReportTemplateService _reportTemplateService;
    private static readonly Serilog.ILogger _log = Log.ForContext<ReportTemplatesController>();

    public ReportTemplatesController(IReportTemplateService reportTemplateService)
    {
        _reportTemplateService = reportTemplateService;
    }

    /// <summary>
    /// Retrieves all report templates in the system
    /// </summary>
    /// <returns>List of all report templates</returns>
    /// <response code="200">Returns the list of report templates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<ReportTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> GetAllTemplates()
    {
        try
        {
            _log.Information("API: GetAllTemplates called by user {User}", User.Identity?.Name);
            var templates = await _reportTemplateService.GetAllTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTemplates");
            return StatusCode(500, "An error occurred while retrieving report templates");
        }
    }

    /// <summary>
    /// Retrieves all active report templates
    /// </summary>
    /// <returns>List of active report templates</returns>
    /// <response code="200">Returns the list of active report templates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<ReportTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> GetActiveTemplates()
    {
        try
        {
            _log.Information("API: GetActiveTemplates called by user {User}", User.Identity?.Name);
            var templates = await _reportTemplateService.GetActiveTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveTemplates");
            return StatusCode(500, "An error occurred while retrieving active report templates");
        }
    }

    /// <summary>
    /// Retrieves report templates filtered by type
    /// </summary>
    /// <param name="type">The template type to filter by</param>
    /// <returns>List of report templates matching the specified type</returns>
    /// <response code="200">Returns the list of report templates matching the type</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("type/{type}")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<ReportTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> GetTemplatesByType(ReportTemplateType type)
    {
        try
        {
            _log.Information("API: GetTemplatesByType called with type {Type} by user {User}", type, User.Identity?.Name);
            var templates = await _reportTemplateService.GetTemplatesByTypeAsync(type);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTemplatesByType for type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving report templates by type");
        }
    }

    /// <summary>
    /// Retrieves report templates filtered by reporting engine
    /// </summary>
    /// <param name="engine">The reporting engine name (e.g., "FastReport")</param>
    /// <returns>List of report templates matching the specified engine</returns>
    /// <response code="200">Returns the list of report templates matching the engine</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("engine/{engine}")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<ReportTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> GetTemplatesByEngine(string engine)
    {
        try
        {
            _log.Information("API: GetTemplatesByEngine called with engine {Engine} by user {User}", engine, User.Identity?.Name);
            var templates = await _reportTemplateService.GetTemplatesByEngineAsync(engine);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTemplatesByEngine for engine {Engine}", engine);
            return StatusCode(500, "An error occurred while retrieving report templates by engine");
        }
    }

    /// <summary>
    /// Retrieves a specific report template by ID
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The report template if found</returns>
    /// <response code="200">Returns the report template</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReportTemplateDto>> GetTemplateById(int id)
    {
        try
        {
            _log.Information("API: GetTemplateById called with ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var template = await _reportTemplateService.GetTemplateByIdAsync(id);

            if (template == null)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found", id);
                return NotFound($"Report template with ID {id} not found");
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTemplateById for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while retrieving the report template");
        }
    }

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    /// <returns>The default report template for the specified type</returns>
    /// <response code="200">Returns the default report template</response>
    /// <response code="404">If no default template is found for the type</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default/{type}")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReportTemplateDto>> GetDefaultTemplate(ReportTemplateType type)
    {
        try
        {
            _log.Information("API: GetDefaultTemplate called for type {Type} by user {User}", type, User.Identity?.Name);
            var template = await _reportTemplateService.GetDefaultTemplateAsync(type);

            if (template == null)
            {
                _log.Warning("API: No default template found for type {Type}", type);
                return NotFound($"No default template found for type {type}");
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultTemplate for type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving the default template");
        }
    }

    /// <summary>
    /// Searches for report templates by name, description, or tags
    /// </summary>
    /// <param name="searchTerm">The search term to match</param>
    /// <returns>List of matching report templates</returns>
    /// <response code="200">Returns the list of matching report templates</response>
    /// <response code="400">If the search term is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("search")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<ReportTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> SearchTemplates([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term cannot be empty");
            }

            _log.Information("API: SearchTemplates called with term {SearchTerm} by user {User}", searchTerm, User.Identity?.Name);
            var templates = await _reportTemplateService.SearchTemplatesAsync(searchTerm);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SearchTemplates with term {SearchTerm}", searchTerm);
            return StatusCode(500, "An error occurred while searching report templates");
        }
    }

    /// <summary>
    /// Creates a new report template
    /// </summary>
    /// <param name="createDto">The report template data including file upload</param>
    /// <returns>The created report template</returns>
    /// <response code="201">Returns the newly created report template</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "ReportTemplate.Create")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReportTemplateDto>> CreateTemplate([FromForm] CreateReportTemplateDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: CreateTemplate called by user {User}", User.Identity?.Name);
            var createdTemplate = await _reportTemplateService.CreateTemplateAsync(createDto);

            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = createdTemplate.Id },
                createdTemplate);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateTemplate");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateTemplate");
            return StatusCode(500, "An error occurred while creating the report template");
        }
    }

    /// <summary>
    /// Updates an existing report template
    /// </summary>
    /// <param name="id">The unique identifier of the template to update</param>
    /// <param name="updateDto">The updated report template data</param>
    /// <returns>The updated report template</returns>
    /// <response code="200">Returns the updated report template</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "ReportTemplate.Update")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReportTemplateDto>> UpdateTemplate(int id, [FromForm] UpdateReportTemplateDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _log.Information("API: UpdateTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var updatedTemplate = await _reportTemplateService.UpdateTemplateAsync(id, updateDto);

            if (updatedTemplate == null)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found for update", id);
                return NotFound($"Report template with ID {id} not found");
            }

            return Ok(updatedTemplate);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateTemplate for ID {TemplateId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while updating the report template");
        }
    }

    /// <summary>
    /// Sets a template as the default for its type
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the template was successfully set as default</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}/set-default")]
    [Authorize(Policy = "ReportTemplate.Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetDefaultTemplate(int id)
    {
        try
        {
            _log.Information("API: SetDefaultTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var success = await _reportTemplateService.SetDefaultTemplateAsync(id);

            if (!success)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found for set default", id);
                return NotFound($"Report template with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetDefaultTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while setting the default template");
        }
    }

    /// <summary>
    /// Toggles the active status of a report template
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the status was successfully toggled</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}/toggle-active")]
    [Authorize(Policy = "ReportTemplate.Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleActiveStatus(int id)
    {
        try
        {
            _log.Information("API: ToggleActiveStatus called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var success = await _reportTemplateService.ToggleActiveStatusAsync(id);

            if (!success)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found for toggle active", id);
                return NotFound($"Report template with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ToggleActiveStatus for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while toggling the active status");
        }
    }

    /// <summary>
    /// Downloads the template file
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The template file</returns>
    /// <response code="200">Returns the template file</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/download")]
    [Authorize(Policy = "ReportTemplate.Read")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadTemplate(int id)
    {
        try
        {
            _log.Information("API: DownloadTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var fileData = await _reportTemplateService.DownloadTemplateAsync(id);

            if (fileData == null)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found for download", id);
                return NotFound($"Report template with ID {id} not found");
            }

            var (content, fileName, mimeType) = fileData.Value;
            return File(content, mimeType, fileName);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while downloading the template");
        }
    }

    /// <summary>
    /// Soft deletes a report template
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the template was successfully deleted</response>
    /// <response code="404">If the template is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permission</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "ReportTemplate.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        try
        {
            _log.Information("API: DeleteTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var success = await _reportTemplateService.SoftDeleteTemplateAsync(id);

            if (!success)
            {
                _log.Warning("API: Report template with ID {TemplateId} not found for delete", id);
                return NotFound($"Report template with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while deleting the report template");
        }
    }
}
