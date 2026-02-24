using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing login history entries.
/// </summary>
public class LoginHistoryService : ILoginHistoryService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<LoginHistoryService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginHistoryService"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public LoginHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all login history entries.
    /// </summary>
    /// <returns>List of login history entries.</returns>
    public async Task<IEnumerable<LoginHistoryDto>> GetAllLoginHistoriesAsync()
    {
        try
        {
            _log.Information("Fetching all login history entries");

            var entries = await _context.LoginHistories
                .Include(l => l.User)
                .AsNoTracking()
                .OrderByDescending(l => l.AttemptedAt)
                .ToListAsync();

            return entries.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all login history entries");
            throw;
        }
    }

    /// <summary>
    /// Retrieves paginated login history entries.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="userId">Optional filter by user identifier.</param>
    /// <param name="isSuccessful">Optional filter by success flag.</param>
    /// <param name="from">Optional filter start timestamp (inclusive, UTC).</param>
    /// <param name="to">Optional filter end timestamp (inclusive, UTC).</param>
    /// <returns>Paginated result containing login history entries.</returns>
    public async Task<PagedResult<LoginHistoryDto>> GetLoginHistoriesPagedAsync(
        PaginationParameters parameters,
        int? userId = null,
        bool? isSuccessful = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        try
        {
            _log.Information(
                "Fetching paginated login history entries - Page: {PageNumber}, PageSize: {PageSize}",
                parameters.PageNumber,
                parameters.PageSize);

            var query = _context.LoginHistories
                .Include(l => l.User)
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            if (isSuccessful.HasValue)
                query = query.Where(l => l.IsSuccessful == isSuccessful.Value);

            if (from.HasValue)
                query = query.Where(l => l.AttemptedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(l => l.AttemptedAt <= to.Value);

            var totalCount = await query.CountAsync();

            var entries = await query
                .OrderByDescending(l => l.AttemptedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = entries.Select(MapToDto).ToList();

            return new PagedResult<LoginHistoryDto>(dtos, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated login history entries");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific login history entry by identifier.
    /// </summary>
    /// <param name="id">Login history identifier.</param>
    /// <returns>The login history entry, or null if not found.</returns>
    public async Task<LoginHistoryDto?> GetLoginHistoryByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching login history entry with ID: {LoginHistoryId}", id);

            var entry = await _context.LoginHistories
                .Include(l => l.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            return entry == null ? null : MapToDto(entry);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching login history entry with ID: {LoginHistoryId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new login history entry.
    /// </summary>
    /// <param name="dto">Login history creation data.</param>
    /// <returns>The created login history entry.</returns>
    public async Task<LoginHistoryDto> CreateLoginHistoryAsync(CreateLoginHistoryDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Identifier))
                throw new ArgumentException("Identifier is required", nameof(dto));

            var entry = new LoginHistory
            {
                UserId = dto.UserId,
                Identifier = dto.Identifier.Trim(),
                IsSuccessful = dto.IsSuccessful,
                AttemptedAt = dto.AttemptedAt,
                IPAddress = dto.IPAddress ?? string.Empty,
                UserAgent = dto.UserAgent ?? string.Empty,
                FailureReason = string.IsNullOrWhiteSpace(dto.FailureReason) ? null : dto.FailureReason.Trim()
            };

            _context.LoginHistories.Add(entry);
            await _context.SaveChangesAsync();

            var created = await _context.LoginHistories
                .Include(l => l.User)
                .AsNoTracking()
                .FirstAsync(l => l.Id == entry.Id);

            _log.Information("Successfully created login history entry with ID: {LoginHistoryId}", entry.Id);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating login history entry");
            throw;
        }
    }

    private static LoginHistoryDto MapToDto(LoginHistory entity)
    {
        return new LoginHistoryDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Username = entity.User?.Username,
            Identifier = entity.Identifier,
            IsSuccessful = entity.IsSuccessful,
            AttemptedAt = entity.AttemptedAt,
            IPAddress = entity.IPAddress,
            UserAgent = entity.UserAgent,
            FailureReason = entity.FailureReason
        };
    }
}
