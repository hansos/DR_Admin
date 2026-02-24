using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Controller for viewing login history entries.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LoginHistoriesController : ControllerBase
{
    private readonly ILoginHistoryService _loginHistoryService;
    private static readonly Serilog.ILogger _log = Log.ForContext<LoginHistoriesController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginHistoriesController"/> class.
    /// </summary>
    /// <param name="loginHistoryService">Login history service.</param>
    public LoginHistoriesController(ILoginHistoryService loginHistoryService)
    {
        _loginHistoryService = loginHistoryService;
    }

    /// <summary>
    /// Retrieves all login history entries.
    /// </summary>
    /// <remarks>
    /// For large datasets, use pagination via <c>pageNumber</c> and <c>pageSize</c>.
    /// </remarks>
    /// <param name="pageNumber">Optional page number (1-based). If provided, returns a paginated response.</param>
    /// <param name="pageSize">Optional page size. If provided, returns a paginated response.</param>
    /// <param name="userId">Optional filter by user identifier.</param>
    /// <param name="isSuccessful">Optional filter by success flag.</param>
    /// <param name="from">Optional filter start timestamp (inclusive, UTC).</param>
    /// <param name="to">Optional filter end timestamp (inclusive, UTC).</param>
    /// <returns>All login history entries, or a paginated result.</returns>
    /// <response code="200">Returns the login history entries.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "User.Read")]
    [ProducesResponseType(typeof(IEnumerable<LoginHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<LoginHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAll(
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        [FromQuery] int? userId = null,
        [FromQuery] bool? isSuccessful = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 50
                };

                _log.Information(
                    "API: Get login histories (paginated) - Page: {PageNumber}, PageSize: {PageSize} by user {User}",
                    paginationParams.PageNumber,
                    paginationParams.PageSize,
                    User.Identity?.Name);

                var result = await _loginHistoryService.GetLoginHistoriesPagedAsync(
                    paginationParams,
                    userId,
                    isSuccessful,
                    from,
                    to);

                return Ok(result);
            }

            _log.Information("API: Get login histories (all) called by user {User}", User.Identity?.Name);
            var entries = await _loginHistoryService.GetAllLoginHistoriesAsync();
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAll login histories");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving login history entries");
        }
    }

    /// <summary>
    /// Retrieves a specific login history entry by identifier.
    /// </summary>
    /// <param name="id">Login history entry identifier.</param>
    /// <returns>The login history entry.</returns>
    /// <response code="200">Returns the login history entry.</response>
    /// <response code="404">If the entry is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "User.Read")]
    [ProducesResponseType(typeof(LoginHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginHistoryDto>> GetById(int id)
    {
        try
        {
            _log.Information("API: GetLoginHistoryById called for ID {Id} by user {User}", id, User.Identity?.Name);

            var entry = await _loginHistoryService.GetLoginHistoryByIdAsync(id);
            if (entry == null)
                return NotFound($"Login history entry with ID {id} not found");

            return Ok(entry);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetLoginHistoryById for ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the login history entry");
        }

    }
}
