using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages Top-Level Domains (TLDs) and their registrars
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TldsController : ControllerBase
{
    private readonly ITldService _tldService;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldsController>();

    public TldsController(ITldService tldService)
    {
        _tldService = tldService;
    }

    /// <summary>
    /// Retrieves all registrars supporting a specific TLD
    /// </summary>
    /// <param name="tldId">The unique identifier of the TLD</param>
    /// <returns>List of registrars supporting the TLD</returns>
    /// <response code="200">Returns the list of registrars</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{tldId}/registrars")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegistrarDto>>> GetRegistrarsByTld(int tldId)
    {
        try
        {
            _log.Information("API: GetRegistrarsByTld called for TLD {TldId} by user {User}", tldId, User.Identity?.Name);

            var registrars = await _tldService.GetRegistrarsByTldAsync(tldId);
            return Ok(registrars);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarsByTld for TLD {TldId}", tldId);
            return StatusCode(500, "An error occurred while retrieving registrars for the TLD");
        }
    }

    /// <summary>
    /// Retrieves all Top-Level Domains in the system
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of all TLDs or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of TLDs or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAllTlds([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            // If pagination parameters are provided, return paginated result
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetAllTlds (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _tldService.GetAllTldsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            // Otherwise, return all TLDs (backward compatible)
            _log.Information("API: GetAllTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetAllTldsAsync();
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllTlds");
            return StatusCode(500, "An error occurred while retrieving TLDs");
        }
    }

    /// <summary>
    /// Retrieves only active Top-Level Domains
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of active TLDs or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of active TLDs or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetActiveTlds([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            // If pagination parameters are provided, return paginated result
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetActiveTlds (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _tldService.GetActiveTldsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            // Otherwise, return all active TLDs (backward compatible)
            _log.Information("API: GetActiveTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetActiveTldsAsync();
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetActiveTlds");
            return StatusCode(500, "An error occurred while retrieving active TLDs");
        }
    }

    /// <summary>
    /// Retrieves only second-level Top-Level Domains
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of second-level TLDs or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of second-level TLDs or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("secondlevel")]
    [ProducesResponseType(typeof(IEnumerable<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSecondLevelTlds([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            // If pagination parameters are provided, return paginated result
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetSecondLevelTlds (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _tldService.GetSecondLevelTldsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            // Otherwise, return all second-level TLDs (backward compatible)
            _log.Information("API: GetSecondLevelTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetAllTldsAsync(isSecondLevel: true);
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSecondLevelTlds");
            return StatusCode(500, "An error occurred while retrieving second-level TLDs");
        }
    }

    /// <summary>
    /// Retrieves only top-level (non-second-level) Top-Level Domains
    /// </summary>
    /// <param name="pageNumber">Optional: Page number for pagination (default: returns all)</param>
    /// <param name="pageSize">Optional: Number of items per page (default: 10, max: 100)</param>
    /// <returns>List of top-level TLDs or paginated result if pagination parameters provided</returns>
    /// <response code="200">Returns the list of top-level TLDs or paginated result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("toplevel")]
    [ProducesResponseType(typeof(IEnumerable<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<TldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetTopLevelTlds([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            // If pagination parameters are provided, return paginated result
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10
                };

                _log.Information("API: GetTopLevelTlds (paginated) called with PageNumber: {PageNumber}, PageSize: {PageSize} by user {User}", 
                    paginationParams.PageNumber, paginationParams.PageSize, User.Identity?.Name);

                var pagedResult = await _tldService.GetTopLevelTldsPagedAsync(paginationParams);
                return Ok(pagedResult);
            }

            // Otherwise, return all top-level TLDs (backward compatible)
            _log.Information("API: GetTopLevelTlds called by user {User}", User.Identity?.Name);
            
            var tlds = await _tldService.GetAllTldsAsync(isSecondLevel: false);
            return Ok(tlds);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTopLevelTlds");
            return StatusCode(500, "An error occurred while retrieving top-level TLDs");
        }
    }

    /// <summary>
    /// Retrieves a specific TLD by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the TLD</param>
    /// <returns>The TLD information</returns>
    /// <response code="200">Returns the TLD data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If TLD is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldDto>> GetTldById(int id)
    {
        try
        {
            _log.Information("API: GetTldById called for ID {TldId} by user {User}", id, User.Identity?.Name);
            
            var tld = await _tldService.GetTldByIdAsync(id);

            if (tld == null)
            {
                _log.Information("API: TLD with ID {TldId} not found", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return Ok(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTldById for ID {TldId}", id);
            return StatusCode(500, "An error occurred while retrieving the TLD");
        }
    }

    /// <summary>
    /// Retrieves a specific TLD by its extension
    /// </summary>
    /// <param name="extension">The TLD extension (e.g., "com", "net", "org")</param>
    /// <returns>The TLD information</returns>
    /// <response code="200">Returns the TLD data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="404">If TLD is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("extension/{extension}")]
    [ProducesResponseType(typeof(TldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TldDto>> GetTldByExtension(string extension)
    {
        try
        {
            _log.Information("API: GetTldByExtension called for extension {Extension} by user {User}", extension, User.Identity?.Name);
            
            var tld = await _tldService.GetTldByExtensionAsync(extension);

            if (tld == null)
            {
                _log.Information("API: TLD with extension {Extension} not found", extension);
                return NotFound($"TLD with extension {extension} not found");
            }

            return Ok(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetTldByExtension for extension {Extension}", extension);
            return StatusCode(500, "An error occurred while retrieving the TLD");
        }
    }

    /// <summary>
    /// Create a new TLD
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Tld.Write")]
    public async Task<ActionResult<TldDto>> CreateTld([FromBody] CreateTldDto createDto)
    {
        try
        {
            _log.Information("API: CreateTld called by user {User}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateTld");
                return BadRequest(ModelState);
            }

            var tld = await _tldService.CreateTldAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetTldById),
                new { id = tld.Id },
                tld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in CreateTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateTld");
            return StatusCode(500, "An error occurred while creating the TLD");
        }
    }

    /// <summary>
    /// Update an existing TLD
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TldDto>> UpdateTld(int id, [FromBody] UpdateTldDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateTld called for ID {TldId} by user {User}", id, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateTld");
                return BadRequest(ModelState);
            }

            var tld = await _tldService.UpdateTldAsync(id, updateDto);

            if (tld == null)
            {
                _log.Information("API: TLD with ID {TldId} not found for update", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return Ok(tld);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in UpdateTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateTld for ID {TldId}", id);
            return StatusCode(500, "An error occurred while updating the TLD");
        }
    }

    /// <summary>
    /// Delete a TLD
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTld(int id)
    {
        try
        {
            _log.Information("API: DeleteTld called for ID {TldId} by user {User}", id, User.Identity?.Name);

            var result = await _tldService.DeleteTldAsync(id);

            if (!result)
            {
                _log.Information("API: TLD with ID {TldId} not found for deletion", id);
                return NotFound($"TLD with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Invalid operation in DeleteTld");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteTld for ID {TldId}", id);
            return StatusCode(500, "An error occurred while deleting the TLD");
        }
    }
}
