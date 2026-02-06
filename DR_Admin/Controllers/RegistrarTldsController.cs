using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages registrar-TLD relationships and pricing
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RegistrarTldsController : ControllerBase
{
    private readonly IRegistrarTldService _registrarTldService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldsController>();

    public RegistrarTldsController(IRegistrarTldService registrarTldService)
    {
        _registrarTldService = registrarTldService;
    }

    /// <summary>
    /// Retrieves all registrar-TLD offerings in the system
    /// </summary>
    /// <returns>List of all registrar-TLD relationships</returns>
    /// <response code="200">Returns the list of registrar-TLD offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "RegistrarTld.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetAllRegistrarTlds()
    {
        try
        {
            _log.Information("API: GetAllRegistrarTlds called by user {User}", User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetAllRegistrarTldsAsync();
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllRegistrarTlds");
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves only available registrar-TLD offerings for purchase
    /// </summary>
    /// <returns>List of available registrar-TLD relationships</returns>
    /// <response code="200">Returns the list of available offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetAvailableRegistrarTlds()
    {
        try
        {
            _log.Information("API: GetAvailableRegistrarTlds called by user {User}", User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetAvailableRegistrarTldsAsync();
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAvailableRegistrarTlds");
            return StatusCode(500, "An error occurred while retrieving available registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves all TLD offerings for a specific registrar
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <returns>List of TLD offerings for the registrar</returns>
    /// <response code="200">Returns the list of TLD offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarId}")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetRegistrarTldsByRegistrar(int registrarId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldsByRegistrar called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetRegistrarTldsByRegistrarAsync(registrarId);
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldsByRegistrar for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves all registrars offering a specific TLD
    /// </summary>
    /// <param name="tldId">The unique identifier of the TLD</param>
    /// <returns>List of registrars offering the TLD</returns>
    /// <response code="200">Returns the list of registrar offerings</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("tld/{tldId}")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarTldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarTldDto>>> GetRegistrarTldsByTld(int tldId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldsByTld called for TLD {TldId} by user {User}", 
                tldId, User.Identity?.Name);
            
            var registrarTlds = await _registrarTldService.GetRegistrarTldsByTldAsync(tldId);
            return Ok(registrarTlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldsByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving registrar TLDs");
        }
    }

    /// <summary>
    /// Retrieves a specific registrar-TLD offering by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the registrar-TLD relationship</param>
    /// <returns>The registrar-TLD offering information</returns>
    /// <response code="200">Returns the registrar-TLD offering data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If registrar-TLD offering is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> GetRegistrarTldById(int id)
    {
        try
        {
            _log.Information("API: GetRegistrarTldById called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);
            
            var registrarTld = await _registrarTldService.GetRegistrarTldByIdAsync(id);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return Ok(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldById for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while retrieving the registrar TLD");
        }
    }

    /// <summary>
    /// Retrieves a specific registrar-TLD offering by registrar and TLD combination
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="tldId">The unique identifier of the TLD</param>
    /// <returns>The registrar-TLD offering information</returns>
    /// <response code="200">Returns the registrar-TLD offering data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If registrar-TLD offering is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("registrar/{registrarId}/tld/{tldId}")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> GetRegistrarTldByRegistrarAndTld(int registrarId, int tldId)
    {
        try
        {
            _log.Information("API: GetRegistrarTldByRegistrarAndTld called for registrar {RegistrarId} and TLD {TldId} by user {User}", 
                registrarId, tldId, User.Identity?.Name);
            
            var registrarTld = await _registrarTldService.GetRegistrarTldByRegistrarAndTldAsync(registrarId, tldId);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD for registrar {RegistrarId} and TLD {TldId} not found", 
                    registrarId, tldId);
                return NotFound($"Registrar TLD for registrar {registrarId} and TLD {tldId} not found");
            }

            return Ok(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarTldByRegistrarAndTld for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            return StatusCode(500, "An error occurred while retrieving the registrar TLD");
        }
    }

    /// <summary>
    /// Creates a new registrar-TLD offering with pricing information
    /// </summary>
    /// <param name="createDto">The creation data containing registrar, TLD, and pricing information</param>
    /// <returns>The created registrar-TLD offering</returns>
    /// <response code="201">Returns the newly created registrar-TLD offering</response>
    /// <response code="400">If the request data is invalid or relationship already exists</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> CreateRegistrarTld([FromBody] CreateRegistrarTldDto createDto)
    {
        try
        {
            _log.Information("API: CreateRegistrarTld called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRegistrarTld");
                return BadRequest(ModelState);
            }

            var registrarTld = await _registrarTldService.CreateRegistrarTldAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetRegistrarTldById),
                new { id = registrarTld.Id },
                registrarTld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRegistrarTld");
            return StatusCode(500, "An error occurred while creating the registrar TLD");
        }
    }

    /// <summary>
    /// Updates an existing registrar-TLD offering with new pricing information
    /// </summary>
    /// <param name="id">The unique identifier of the registrar-TLD relationship to update</param>
    /// <param name="updateDto">The update data containing new pricing and configuration</param>
    /// <returns>The updated registrar-TLD offering</returns>
    /// <response code="200">Returns the updated registrar-TLD offering</response>
    /// <response code="400">If the request data is invalid or new combination already exists</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="404">If registrar-TLD is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RegistrarTldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarTldDto>> UpdateRegistrarTld(int id, [FromBody] UpdateRegistrarTldDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRegistrarTld called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRegistrarTld");
                return BadRequest(ModelState);
            }

            var registrarTld = await _registrarTldService.UpdateRegistrarTldAsync(id, updateDto);

            if (registrarTld == null)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found for update", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return Ok(registrarTld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRegistrarTld for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while updating the registrar TLD");
        }
    }

    /// <summary>
    /// Delete a registrar TLD offering
    /// </summary>
    /// <param name="id">The unique identifier of the registrar-TLD relationship to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If deletion was successful</response>
    /// <response code="400">If the registrar-TLD has associated domains</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="404">If registrar-TLD is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteRegistrarTld(int id)
    {
        try
        {
            _log.Information("API: DeleteRegistrarTld called for ID {RegistrarTldId} by user {User}", 
                id, User.Identity?.Name);

            var result = await _registrarTldService.DeleteRegistrarTldAsync(id);

            if (!result)
            {
                _log.Information("API: Registrar TLD with ID {RegistrarTldId} not found for deletion", id);
                return NotFound($"Registrar TLD with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteRegistrarTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRegistrarTld for ID {RegistrarTldId}", id);
            return StatusCode(500, "An error occurred while deleting the registrar TLD");
        }
    }

    /// <summary>
    /// Imports TLDs for a specific registrar from CSV format form data
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="importDto">The import data containing TLD extensions and pricing information in CSV format (Tld, Description)</param>
    /// <returns>Import result with statistics</returns>
    /// <remarks>
    /// Expected CSV format:
    /// Tld, Description
    /// ac,
    /// academy,
    /// airforce, US Airforce only
    /// apartments,
    /// </remarks>
    /// <response code="200">Returns the import result with statistics</response>
    /// <response code="400">If the request data is invalid or registrar is not found/inactive</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("registrar/{registrarId}/import")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ImportRegistrarTldsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportRegistrarTldsResponseDto>> ImportRegistrarTlds(
        int registrarId,
        [FromForm] ImportRegistrarTldsDto importDto)
    {
        try
        {
            _log.Information("API: ImportRegistrarTlds called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for ImportRegistrarTlds");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(importDto.Content))
            {
                _log.Warning("API: ImportRegistrarTlds called with empty content");
                return BadRequest("Content is required");
            }

            var result = await _registrarTldService.ImportRegistrarTldsAsync(registrarId, importDto);

            if (!result.Success)
            {
                _log.Warning("API: ImportRegistrarTlds failed for registrar {RegistrarId}. Message: {Message}", 
                    registrarId, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in ImportRegistrarTlds for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while importing TLDs");
        }
    }

    /// <summary>
    /// Uploads a CSV file with TLDs to merge into the Tlds table and add references in RegistrarTlds table
    /// </summary>
    /// <param name="registrarId">The unique identifier of the registrar</param>
    /// <param name="uploadDto">The upload data containing the CSV file and pricing defaults</param>
    /// <returns>Import result with statistics</returns>
    /// <remarks>
    /// Expected CSV format:
    /// Tld, Description
    /// ac,
    /// academy,
    /// airforce, US Airforce only
    /// apartments,
    /// </remarks>
    /// <response code="200">Returns the import result with statistics</response>
    /// <response code="400">If the file is invalid or registrar is not found/inactive</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("registrar/{registrarId}/upload-csv")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ImportRegistrarTldsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportRegistrarTldsResponseDto>> UploadRegistrarTldsCsv(
        int registrarId,
        [FromForm] UploadRegistrarTldsCsvDto uploadDto)
    {
        try
        {
            _log.Information("API: UploadRegistrarTldsCsv called for registrar {RegistrarId} by user {User}", 
                registrarId, User.Identity?.Name);

            var file = uploadDto?.File;

            if (file == null || file.Length == 0)
            {
                _log.Warning("API: UploadRegistrarTldsCsv called with no file");
                return BadRequest("File is required");
            }

            using var stream = file.OpenReadStream();
            var result = await _registrarTldService.ImportRegistrarTldsFromCsvAsync(
                registrarId,
                stream,
                uploadDto.DefaultRegistrationCost,
                uploadDto.DefaultRegistrationPrice,
                uploadDto.DefaultRenewalCost,
                uploadDto.DefaultRenewalPrice,
                uploadDto.DefaultTransferCost,
                uploadDto.DefaultTransferPrice,
                uploadDto.IsAvailable,
                uploadDto.ActivateNewTlds,
                uploadDto.Currency);

            if (!result.Success)
            {
                _log.Warning("API: UploadRegistrarTldsCsv failed for registrar {RegistrarId}. Message: {Message}", 
                    registrarId, result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UploadRegistrarTldsCsv");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UploadRegistrarTldsCsv for registrar {RegistrarId}", registrarId);
            return StatusCode(500, "An error occurred while processing the CSV file");
        }
    }

    /// <summary>
    /// Updates the active status of all registrar-TLD offerings in the system
    /// </summary>
    /// <param name="statusDto">The DTO containing the optional registrar ID and desired active status</param>
    /// <returns>Result containing the number of updated records</returns>
    /// <remarks>
    /// If RegistrarId is provided, only updates offerings for that registrar. Otherwise updates all registrars.
    /// </remarks>
    /// <response code="200">Returns the update result with count of affected records</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("bulk-update-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BulkUpdateResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkUpdateResultDto>> BulkUpdateAllRegistrarTldStatus(
        [FromBody] BulkUpdateRegistrarTldStatusDto statusDto)
    {
        try
        {
            _log.Information("API: BulkUpdateAllRegistrarTldStatus called for registrar {RegistrarId} to set IsActive={IsActive} by user {User}", 
                statusDto.RegistrarId?.ToString() ?? "ALL", statusDto.IsActive, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for BulkUpdateAllRegistrarTldStatus");
                return BadRequest(ModelState);
            }

            var result = await _registrarTldService.BulkUpdateAllRegistrarTldStatusAsync(statusDto.RegistrarId, statusDto.IsActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in BulkUpdateAllRegistrarTldStatus");
            return StatusCode(500, "An error occurred while updating registrar TLD statuses");
        }
    }

    /// <summary>
    /// Updates the active status of registrar-TLD offerings for specific TLD extensions
    /// </summary>
    /// <param name="statusDto">The DTO containing the comma-separated TLD extensions and desired active status</param>
    /// <returns>Result containing the number of updated records</returns>
    /// <remarks>
    /// Example TLD extensions: "com,net,org" or "com, net, org"
    /// </remarks>
    /// <response code="200">Returns the update result with count of affected records</response>
    /// <response code="400">If the request data is invalid or no valid extensions found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("bulk-update-status-by-tld")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BulkUpdateResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkUpdateResultDto>> BulkUpdateRegistrarTldStatusByTld(
        [FromBody] BulkUpdateRegistrarTldStatusByTldDto statusDto)
    {
        try
        {
            _log.Information("API: BulkUpdateRegistrarTldStatusByTld called for extensions '{TldExtensions}' to set IsActive={IsActive} by user {User}", 
                statusDto.TldExtensions, statusDto.IsActive, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for BulkUpdateRegistrarTldStatusByTld");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(statusDto.TldExtensions))
            {
                _log.Warning("API: BulkUpdateRegistrarTldStatusByTld called with empty TLD extensions");
                return BadRequest("TLD extensions are required");
            }

            var result = await _registrarTldService.BulkUpdateRegistrarTldStatusByTldAsync(
                statusDto.RegistrarId,
                statusDto.TldExtensions, 
                statusDto.IsActive);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in BulkUpdateRegistrarTldStatusByTld");
            return StatusCode(500, "An error occurred while updating registrar TLD statuses");
        }
    }
}



