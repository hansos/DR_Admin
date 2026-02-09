using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing vendor costs
/// </summary>
public interface IVendorCostService
{
    Task<IEnumerable<VendorCostDto>> GetAllVendorCostsAsync();
    Task<PagedResult<VendorCostDto>> GetAllVendorCostsPagedAsync(PaginationParameters parameters);
    Task<VendorCostDto?> GetVendorCostByIdAsync(int id);
    Task<IEnumerable<VendorCostDto>> GetVendorCostsByInvoiceLineIdAsync(int invoiceLineId);
    Task<IEnumerable<VendorCostDto>> GetVendorCostsByPayoutIdAsync(int payoutId);
    Task<VendorCostDto> CreateVendorCostAsync(CreateVendorCostDto dto);
    Task<VendorCostDto?> UpdateVendorCostAsync(int id, UpdateVendorCostDto dto);
    Task<bool> DeleteVendorCostAsync(int id);
    Task<VendorCostSummaryDto?> GetVendorCostSummaryByInvoiceIdAsync(int invoiceId);
}
