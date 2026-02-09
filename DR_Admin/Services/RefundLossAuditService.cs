using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing refund loss audit operations
/// </summary>
public class RefundLossAuditService : IRefundLossAuditService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RefundLossAuditService>();

    public RefundLossAuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RefundLossAuditDto>> GetAllRefundLossAuditsAsync()
    {
        try
        {
            _log.Information("Fetching all refund loss audits");
            
            var audits = await _context.RefundLossAudits
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} refund loss audits", audits.Count);
            return audits.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all refund loss audits");
            throw;
        }
    }

    public async Task<PagedResult<RefundLossAuditDto>> GetAllRefundLossAuditsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated refund loss audits - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.RefundLossAudits.AsNoTracking().CountAsync();

            var audits = await _context.RefundLossAudits
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = audits.Select(MapToDto).ToList();
            
            return new PagedResult<RefundLossAuditDto>(dtos, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated refund loss audits");
            throw;
        }
    }

    public async Task<RefundLossAuditDto?> GetRefundLossAuditByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching refund loss audit with ID: {Id}", id);

            var audit = await _context.RefundLossAudits
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (audit == null)
            {
                _log.Warning("Refund loss audit with ID {Id} not found", id);
                return null;
            }

            return MapToDto(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching refund loss audit with ID: {Id}", id);
            throw;
        }
    }

    public async Task<RefundLossAuditDto?> GetRefundLossAuditByRefundIdAsync(int refundId)
    {
        try
        {
            _log.Information("Fetching refund loss audit for refund ID: {RefundId}", refundId);

            var audit = await _context.RefundLossAudits
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.RefundId == refundId);

            if (audit == null)
            {
                _log.Warning("Refund loss audit for refund ID {RefundId} not found", refundId);
                return null;
            }

            return MapToDto(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching refund loss audit for refund ID: {RefundId}", refundId);
            throw;
        }
    }

    public async Task<IEnumerable<RefundLossAuditDto>> GetRefundLossAuditsByInvoiceIdAsync(int invoiceId)
    {
        try
        {
            _log.Information("Fetching refund loss audits for invoice ID: {InvoiceId}", invoiceId);

            var audits = await _context.RefundLossAudits
                .AsNoTracking()
                .Where(a => a.InvoiceId == invoiceId)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} refund loss audits for invoice ID: {InvoiceId}", 
                audits.Count, invoiceId);
            return audits.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching refund loss audits for invoice ID: {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<RefundLossAuditDto> CreateRefundLossAuditAsync(CreateRefundLossAuditDto dto)
    {
        try
        {
            _log.Information("Creating new refund loss audit for refund ID: {RefundId}", dto.RefundId);

            var audit = new RefundLossAudit
            {
                RefundId = dto.RefundId,
                InvoiceId = dto.InvoiceId,
                OriginalInvoiceAmount = dto.OriginalInvoiceAmount,
                RefundedAmount = dto.RefundedAmount,
                VendorCostUnrecoverable = dto.VendorCostUnrecoverable,
                NetLoss = dto.NetLoss,
                Currency = dto.Currency,
                Reason = dto.Reason,
                InternalNotes = dto.InternalNotes,
                ApprovalStatus = Data.Enums.ApprovalStatus.PendingApproval,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RefundLossAudits.Add(audit);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created refund loss audit with ID: {Id}", audit.Id);
            return MapToDto(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating refund loss audit");
            throw;
        }
    }

    public async Task<RefundLossAuditDto?> ApproveRefundLossAsync(ApproveRefundLossDto dto)
    {
        try
        {
            _log.Information("Processing approval for refund loss audit ID: {Id}", dto.RefundLossAuditId);

            var audit = await _context.RefundLossAudits.FindAsync(dto.RefundLossAuditId);

            if (audit == null)
            {
                _log.Warning("Refund loss audit with ID {Id} not found for approval", dto.RefundLossAuditId);
                return null;
            }

            audit.ApprovalStatus = dto.IsApproved 
                ? Data.Enums.ApprovalStatus.Approved 
                : Data.Enums.ApprovalStatus.Denied;
            audit.ApprovedByUserId = dto.ApprovedByUserId;
            audit.ApprovedAt = DateTime.UtcNow;
            audit.DenialReason = dto.IsApproved ? null : dto.DenialReason;
            
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                audit.InternalNotes += $"\n[{DateTime.UtcNow}] {dto.Notes}";
            }

            audit.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _log.Information("Successfully processed approval for refund loss audit ID: {Id}, Approved: {IsApproved}", 
                dto.RefundLossAuditId, dto.IsApproved);

            return MapToDto(audit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while processing approval for refund loss audit ID: {Id}", dto.RefundLossAuditId);
            throw;
        }
    }

    public async Task<bool> DeleteRefundLossAuditAsync(int id)
    {
        try
        {
            _log.Information("Deleting refund loss audit with ID: {Id}", id);

            var audit = await _context.RefundLossAudits.FindAsync(id);

            if (audit == null)
            {
                _log.Warning("Refund loss audit with ID {Id} not found for deletion", id);
                return false;
            }

            _context.RefundLossAudits.Remove(audit);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted refund loss audit with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting refund loss audit with ID: {Id}", id);
            throw;
        }
    }

    private static RefundLossAuditDto MapToDto(RefundLossAudit audit)
    {
        return new RefundLossAuditDto
        {
            Id = audit.Id,
            RefundId = audit.RefundId,
            InvoiceId = audit.InvoiceId,
            OriginalInvoiceAmount = audit.OriginalInvoiceAmount,
            RefundedAmount = audit.RefundedAmount,
            VendorCostUnrecoverable = audit.VendorCostUnrecoverable,
            NetLoss = audit.NetLoss,
            Currency = audit.Currency,
            Reason = audit.Reason,
            ApprovalStatus = audit.ApprovalStatus,
            ApprovedByUserId = audit.ApprovedByUserId,
            ApprovedAt = audit.ApprovedAt,
            DenialReason = audit.DenialReason,
            InternalNotes = audit.InternalNotes,
            CreatedAt = audit.CreatedAt,
            UpdatedAt = audit.UpdatedAt
        };
    }
}
