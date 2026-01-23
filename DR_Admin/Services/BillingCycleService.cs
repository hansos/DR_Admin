using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class BillingCycleService : IBillingCycleService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<BillingCycleService>();

    public BillingCycleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BillingCycleDto>> GetAllBillingCyclesAsync()
    {
        try
        {
            _log.Information("Fetching all billing cycles");
            
            var billingCycles = await _context.BillingCycles
                .AsNoTracking()
                .ToListAsync();

            var billingCycleDtos = billingCycles.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} billing cycles", billingCycles.Count);
            return billingCycleDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all billing cycles");
            throw;
        }
    }

    public async Task<BillingCycleDto?> GetBillingCycleByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching billing cycle with ID: {BillingCycleId}", id);
            
            var billingCycle = await _context.BillingCycles
                .AsNoTracking()
                .FirstOrDefaultAsync(bc => bc.Id == id);

            if (billingCycle == null)
            {
                _log.Warning("Billing cycle with ID {BillingCycleId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched billing cycle with ID: {BillingCycleId}", id);
            return MapToDto(billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching billing cycle with ID: {BillingCycleId}", id);
            throw;
        }
    }

    public async Task<BillingCycleDto> CreateBillingCycleAsync(CreateBillingCycleDto createDto)
    {
        try
        {
            _log.Information("Creating new billing cycle with name: {BillingCycleName}", createDto.Name);

            var billingCycle = new BillingCycle
            {
                Name = createDto.Name,
                DurationInDays = createDto.DurationInDays,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BillingCycles.Add(billingCycle);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created billing cycle with ID: {BillingCycleId}", billingCycle.Id);
            return MapToDto(billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating billing cycle with name: {BillingCycleName}", createDto.Name);
            throw;
        }
    }

    public async Task<BillingCycleDto?> UpdateBillingCycleAsync(int id, UpdateBillingCycleDto updateDto)
    {
        try
        {
            _log.Information("Updating billing cycle with ID: {BillingCycleId}", id);

            var billingCycle = await _context.BillingCycles.FindAsync(id);

            if (billingCycle == null)
            {
                _log.Warning("Billing cycle with ID {BillingCycleId} not found for update", id);
                return null;
            }

            billingCycle.Name = updateDto.Name;
            billingCycle.DurationInDays = updateDto.DurationInDays;
            billingCycle.Description = updateDto.Description;
            billingCycle.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated billing cycle with ID: {BillingCycleId}", id);
            return MapToDto(billingCycle);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating billing cycle with ID: {BillingCycleId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteBillingCycleAsync(int id)
    {
        try
        {
            _log.Information("Deleting billing cycle with ID: {BillingCycleId}", id);

            var billingCycle = await _context.BillingCycles.FindAsync(id);

            if (billingCycle == null)
            {
                _log.Warning("Billing cycle with ID {BillingCycleId} not found for deletion", id);
                return false;
            }

            _context.BillingCycles.Remove(billingCycle);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted billing cycle with ID: {BillingCycleId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting billing cycle with ID: {BillingCycleId}", id);
            throw;
        }
    }

    private static BillingCycleDto MapToDto(BillingCycle billingCycle)
    {
        return new BillingCycleDto
        {
            Id = billingCycle.Id,
            Name = billingCycle.Name,
            DurationInDays = billingCycle.DurationInDays,
            Description = billingCycle.Description,
            CreatedAt = billingCycle.CreatedAt,
            UpdatedAt = billingCycle.UpdatedAt
        };
    }
}
