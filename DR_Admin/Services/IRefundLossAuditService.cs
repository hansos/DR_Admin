using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing refund loss audits
/// </summary>
public interface IRefundLossAuditService
{
    Task<IEnumerable<RefundLossAuditDto>> GetAllRefundLossAuditsAsync();
    Task<PagedResult<RefundLossAuditDto>> GetAllRefundLossAuditsPagedAsync(PaginationParameters parameters);
    Task<RefundLossAuditDto?> GetRefundLossAuditByIdAsync(int id);
    Task<RefundLossAuditDto?> GetRefundLossAuditByRefundIdAsync(int refundId);
    Task<IEnumerable<RefundLossAuditDto>> GetRefundLossAuditsByInvoiceIdAsync(int invoiceId);
    Task<RefundLossAuditDto> CreateRefundLossAuditAsync(CreateRefundLossAuditDto dto);
    Task<RefundLossAuditDto?> ApproveRefundLossAsync(ApproveRefundLossDto dto);
    Task<bool> DeleteRefundLossAuditAsync(int id);
}
