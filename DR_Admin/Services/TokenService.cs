using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TokenService>();

    public TokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TokenDto>> GetAllTokensAsync()
    {
        try
        {
            _log.Information("Fetching all tokens");
            
            var tokens = await _context.Tokens
                .AsNoTracking()
                .ToListAsync();

            var tokenDtos = tokens.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} tokens", tokens.Count);
            return tokenDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all tokens");
            throw;
        }
    }

    public async Task<TokenDto?> GetTokenByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching token with ID: {TokenId}", id);
            
            var token = await _context.Tokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (token == null)
            {
                _log.Warning("Token with ID {TokenId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched token with ID: {TokenId}", id);
            return MapToDto(token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching token with ID: {TokenId}", id);
            throw;
        }
    }

    public async Task<TokenDto> CreateTokenAsync(CreateTokenDto createDto)
    {
        try
        {
            _log.Information("Creating new token for user: {UserId}", createDto.UserId);

            var token = new Token
            {
                UserId = createDto.UserId,
                TokenType = createDto.TokenType,
                TokenValue = createDto.TokenValue,
                Expiry = createDto.Expiry,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created token with ID: {TokenId}", token.Id);
            return MapToDto(token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating token for user: {UserId}", createDto.UserId);
            throw;
        }
    }

    public async Task<TokenDto?> UpdateTokenAsync(int id, UpdateTokenDto updateDto)
    {
        try
        {
            _log.Information("Updating token with ID: {TokenId}", id);

            var token = await _context.Tokens.FindAsync(id);

            if (token == null)
            {
                _log.Warning("Token with ID {TokenId} not found for update", id);
                return null;
            }

            token.RevokedAt = updateDto.RevokedAt;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated token with ID: {TokenId}", id);
            return MapToDto(token);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating token with ID: {TokenId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTokenAsync(int id)
    {
        try
        {
            _log.Information("Deleting token with ID: {TokenId}", id);

            var token = await _context.Tokens.FindAsync(id);

            if (token == null)
            {
                _log.Warning("Token with ID {TokenId} not found for deletion", id);
                return false;
            }

            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted token with ID: {TokenId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting token with ID: {TokenId}", id);
            throw;
        }
    }

    private static TokenDto MapToDto(Token token)
    {
        return new TokenDto
        {
            Id = token.Id,
            UserId = token.UserId,
            TokenType = token.TokenType,
            Expiry = token.Expiry,
            CreatedAt = token.CreatedAt,
            RevokedAt = token.RevokedAt
        };
    }
}
