using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages document templates for invoices, orders, emails, and other documents
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentTemplatesController : ControllerBase
{
    private readonly IDocumentTemplateService _documentTemplateService;
    private static readonly Serilog.ILogger _log = Log.ForContext<DocumentTemplatesController>();

    public DocumentTemplatesController(IDocumentTemplateService documentTemplateService)
    {
        _documentTemplateService = documentTemplateService;
    }

    /// <summary>
    /// Retrieves all document templates in the system
    /// </summary>
    /// <returns>List of all document templates</returns>
    /// <response code="200">Returns the list of document templates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "DocumentTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<DocumentTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentTemplateDto>>> GetAllTemplates()
    {
        try
        {
            _log.Information("API: GetAllTemplates called by user {User}", User.Identity?.Name);
            var templates = await _documentTemplateService.GetAllTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTemplates");
            return StatusCode(500, "An error occurred while retrieving document templates");
        }
    }

    /// <summary>
    /// Retrieves all active document templates
    /// </summary>
    /// <returns>List of active document templates</returns>
    /// <response code="200">Returns the list of active document templates</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [Authorize(Policy = "DocumentTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<DocumentTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentTemplateDto>>> GetActiveTemplates()
    {
        try
        {
            _log.Information("API: GetActiveTemplates called by user {User}", User.Identity?.Name);
            var templates = await _documentTemplateService.GetActiveTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveTemplates");
            return StatusCode(500, "An error occurred while retrieving active document templates");
        }
    }

    /// <summary>
    /// Retrieves document templates filtered by type
    /// </summary>
    /// <param name="type">The template type to filter by</param>
    /// <returns>List of document templates matching the specified type</returns>
    /// <response code="200">Returns the list of document templates matching the type</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("type/{type}")]
    [Authorize(Policy = "DocumentTemplate.Read")]
    [ProducesResponseType(typeof(IEnumerable<DocumentTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentTemplateDto>>> GetTemplatesByType(DocumentTemplateType type)
    {
        try
        {
            _log.Information("API: GetTemplatesByType called for type {TemplateType} by user {User}", type, User.Identity?.Name);
            var templates = await _documentTemplateService.GetTemplatesByTypeAsync(type);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTemplatesByType for type {TemplateType}", type);
            return StatusCode(500, "An error occurred while retrieving document templates");
        }
    }

    /// <summary>
    /// Retrieves a specific document template by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the document template</param>
    /// <returns>The document template information</returns>
    /// <response code="200">Returns the document template data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If document template is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Support")]
    [ProducesResponseType(typeof(DocumentTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentTemplateDto>> GetTemplateById(int id)
    {
        try
        {
            _log.Information("API: GetTemplateById called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var template = await _documentTemplateService.GetTemplateByIdAsync(id);

            if (template == null)
            {
                _log.Information("API: Document template with ID {TemplateId} not found", id);
                return NotFound($"Document template with ID {id} not found");
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTemplateById for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while retrieving the document template");
        }
    }

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    /// <returns>The default document template for the specified type</returns>
    /// <response code="200">Returns the default document template</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If no default template exists for the type</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default/{type}")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(DocumentTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentTemplateDto>> GetDefaultTemplate(DocumentTemplateType type)
    {
        try
        {
            _log.Information("API: GetDefaultTemplate called for type {TemplateType} by user {User}", type, User.Identity?.Name);
            var template = await _documentTemplateService.GetDefaultTemplateAsync(type);

            if (template == null)
            {
                _log.Information("API: No default template found for type {TemplateType}", type);
                return NotFound($"No default template found for type {type}");
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultTemplate for type {TemplateType}", type);
            return StatusCode(500, "An error occurred while retrieving the default template");
        }
    }

    /// <summary>
    /// Downloads the file content of a specific document template
    /// </summary>
    /// <param name="id">The unique identifier of the document template</param>
    /// <returns>The template file</returns>
    /// <response code="200">Returns the file content</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If document template is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}/download")]
    [Authorize(Roles = "Admin,Support,Sales")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadTemplate(int id)
    {
        try
        {
            _log.Information("API: DownloadTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);
            var result = await _documentTemplateService.DownloadTemplateAsync(id);

            if (result == null)
            {
                _log.Information("API: Document template with ID {TemplateId} not found for download", id);
                return NotFound($"Document template with ID {id} not found");
            }

            return File(result.Value.content, result.Value.mimeType, result.Value.fileName);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DownloadTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while downloading the document template");
        }
    }

    /// <summary>
    /// Uploads a new document template
    /// </summary>
    /// <param name="createDto">Document template information including the file</param>
    /// <returns>The newly created document template</returns>
    /// <response code="201">Returns the newly created document template</response>
    /// <response code="400">If the template data or file is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DocumentTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentTemplateDto>> UploadTemplate([FromForm] CreateDocumentTemplateDto createDto)
    {
        try
        {
            _log.Information("API: UploadTemplate called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UploadTemplate");
                return BadRequest(ModelState);
            }

            var template = await _documentTemplateService.CreateTemplateAsync(createDto);

            return CreatedAtAction(
                nameof(GetTemplateById),
                new { id = template.Id },
                template);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UploadTemplate");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UploadTemplate");
            return StatusCode(500, "An error occurred while uploading the document template");
        }
    }

    /// <summary>
    /// Updates an existing document template's information and optionally replaces the file
    /// </summary>
    /// <param name="id">The unique identifier of the document template to update</param>
    /// <param name="updateDto">Updated document template information</param>
    /// <returns>The updated document template</returns>
    /// <response code="200">Returns the updated document template</response>
    /// <response code="400">If the template data or file is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If document template is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DocumentTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentTemplateDto>> UpdateTemplate(int id, [FromForm] UpdateDocumentTemplateDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateTemplate");
                return BadRequest(ModelState);
            }

            var template = await _documentTemplateService.UpdateTemplateAsync(id, updateDto);

            if (template == null)
            {
                _log.Information("API: Document template with ID {TemplateId} not found for update", id);
                return NotFound($"Document template with ID {id} not found");
            }

            return Ok(template);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateTemplate");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while updating the document template");
        }
    }

    /// <summary>
    /// Sets a document template as the default for its type
    /// </summary>
    /// <param name="id">The unique identifier of the document template to set as default</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the template was successfully set as default</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If document template is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}/set-default")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetDefaultTemplate(int id)
    {
        try
        {
            _log.Information("API: SetDefaultTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);

            var result = await _documentTemplateService.SetDefaultTemplateAsync(id);

            if (!result)
            {
                _log.Information("API: Document template with ID {TemplateId} not found for setting as default", id);
                return NotFound($"Document template with ID {id} not found");
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
    /// Deletes a document template (soft delete)
    /// </summary>
    /// <param name="id">The unique identifier of the document template to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the template was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="404">If document template is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTemplate(int id)
    {
        try
        {
            _log.Information("API: DeleteTemplate called for ID {TemplateId} by user {User}", id, User.Identity?.Name);

            var result = await _documentTemplateService.SoftDeleteTemplateAsync(id);

            if (!result)
            {
                _log.Information("API: Document template with ID {TemplateId} not found for deletion", id);
                return NotFound($"Document template with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteTemplate for ID {TemplateId}", id);
            return StatusCode(500, "An error occurred while deleting the document template");
        }
    }
}
